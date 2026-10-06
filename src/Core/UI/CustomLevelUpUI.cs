using System.Collections;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes.UI;

/// <summary>Customized levelup ui</summary>
public class CustomLevelUpUI
{
    static CustomLevelUpUI()
    {
        var harmony = new Harmony("DeviceOfHermes.UI.CustomLevelUpUI");

        harmony.CreateReversePatcher(typeof(LevelUpUI).Method("Init"), new HarmonyMethod(typeof(CustomLevelUpUI).Method("Init", [typeof(LevelUpUI), typeof(int), typeof(List<EmotionCardXmlInfo>)]))).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnCheckEmotion)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnSelecteEmotion)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnPickPassive)).Patch();
    }

    /// <summary>Init ui</summary>
    public virtual void Init(LevelUpUI ui)
    {
    }

    /// <summary>Drop ui</summary>
    public virtual void Drop(LevelUpUI ui)
    {
    }

    /// <summary>On select card</summary>
    public virtual bool OnSelect(EmotionCardXmlInfo info)
    {
        return true;
    }

    /// <summary>Queue levelui</summary>
    public static void QueueLevelUp(CustomLevelUpUI ui, int level, Func<List<EmotionCardXmlInfo>> candGen)
    {
        _levelUpQueue.Enqueue((ui, level, candGen));
    }

    /// <summary>Open levelui</summary>
    public static void OpenLevelUpUI(CustomLevelUpUI ui, int level, Func<List<EmotionCardXmlInfo>> candGen)
    {
        var origin = BattleManagerUI.Instance.ui_levelup;
        var cand = candGen();

        if (origin.IsEnabled || cand.Count <= 0)
        {
            return;
        }

        _currentUI = ui;

        origin.SetRootCanvas(true);

        Init(origin, level, cand);
    }

    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    private static void Init(LevelUpUI __instance, int count, List<EmotionCardXmlInfo> cardList)
    {
        static void InjectBefore(LevelUpUI __instance)
        {
            _currentUI?.Init(__instance);
        }

        IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdarg(0),
                CodeMatch.IsLdfld(typeof(LevelUpUI).Field("candidates")),
                CodeMatch.IsOpCode(OpCodes.Ldlen),
                CodeMatch.IsOpCode(OpCodes.Brfalse)
            )
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Call(((Action<LevelUpUI>)InjectBefore).Method)
                );

            return matcher.Instructions();
        }

        List<CodeInstruction> dummy = new();

        _ = Transpiler(dummy);

        throw new NotImplementedException();
    }

    private static Queue<(CustomLevelUpUI, int, Func<List<EmotionCardXmlInfo>>)> _levelUpQueue = new();

    private static CustomLevelUpUI? _currentUI;

    private static EmotionCardXmlInfo? _selectedCard;

    [HarmonyPatch(typeof(StageController), "RoundEndPhase_ChoiceEmotionCard")]
    class PatchOnCheckEmotion
    {
        static Exception Finalizer(Exception __exception, ref bool __result)
        {
            if (__result && _levelUpQueue.Count > 0)
            {
                var (ui, level, candGen) = _levelUpQueue.Dequeue();

                OpenLevelUpUI(ui, level, candGen);

                __result = false;
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(LevelUpUI), "OnSelectPassive")]
    class PatchOnSelecteEmotion
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            var label = matcher.MatchStartForward(
                CodeMatch.Calls(typeof(BattleSoundManager).Method("get_Instance")),
                CodeMatch.IsOpCode(OpCodes.Ldc_R4),
                CodeMatch.Calls(typeof(BattleSoundManager).Method("SetBgmVolumeRatio"))
            )
                .Instruction
                .labels[0];

            matcher.Start()
                .MatchEndForward(CodeMatch.IsLdarg(1), CodeMatch.Calls(typeof(EmotionPassiveCardUI).Method("get_Card")), CodeMatch.IsStloc())
                .Advance(1)
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Local(0),
                    CodeInstruction.Call(typeof(PatchOnSelecteEmotion).Method("InjectMethod")),
                    new CodeInstruction(OpCodes.Brfalse, label)
                );

            return matcher.Instructions();
        }

        static bool InjectMethod(LevelUpUI __instance, EmotionCardXmlInfo info)
        {
            if (_currentUI is not null)
            {
                _selectedCard = info;

                return _currentUI.OnSelect(info);
            }

            return true;
        }

        static Exception Finalizer(Exception __exception, LevelUpUI __instance)
        {
            __instance.StartCoroutine(DropRoutine(__instance));

            return __exception;
        }

        static IEnumerator DropRoutine(LevelUpUI __instance)
        {
            yield return new WaitForSeconds(0.2f);

            if (_currentUI is not null)
            {
                _currentUI.Drop(__instance);
                _currentUI = null;
            }
        }
    }

    [HarmonyPatch(typeof(StageLibraryFloorModel), "OnPickPassiveCard")]
    class PatchOnPickPassive
    {
        static bool Prefix(EmotionCardXmlInfo card, BattleUnitModel target)
        {
            if (Equals(card, _selectedCard))
            {
                if (target is null)
                {
                    var b = true;

                    foreach (var unit in Faction.Player.AliveUnits)
                    {
                        unit.emotionDetail.ApplyEmotionCard(card, b);

                        b = false;
                    }
                }
                else
                {
                    target.emotionDetail.ApplyEmotionCard(card, true);
                }

                _selectedCard = null;

                return false;
            }

            return true;
        }
    }
}
