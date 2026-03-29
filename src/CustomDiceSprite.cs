using System.Reflection.Emit;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using LOR_DiceSystem;

namespace DeviceOfHermes;

/// <summary>Supplies dice sprite customizer</summary>
/// <remarks>
/// Applies exchange sequence, the all dice sprite that cond matched change to specified sprite.
/// </remarks>
/// <example><code>
/// CustomDiceSprite.AddSequence(() => true, yourSprite, new Color(255, 255, 255, 255));
/// </code></example>
public static class CustomDiceSprite
{
    static CustomDiceSprite()
    {
        _seqs = new();

        var harmony = new Harmony("DeviceOfHermes.CustomSprite");

        harmony.CreateClassProcessor(typeof(CustomDicePatch.PatchBattleDiceCardUI)).Patch();
        harmony.CreateClassProcessor(typeof(CustomDicePatch.PatchOriginCardSlot)).Patch();
        harmony.CreateClassProcessor(typeof(CustomDicePatch.PatchDetailCardSlot)).Patch();
    }

    /// <summary>Add customize sequence</summary>
    /// <param name="cond">Apply mapping if returns true</param>
    /// <param name="map">Sprite mapping</param>
    /// <param name="behColor">Text that beside dice sprite applies color</param>
    /// <example><code>
    /// CustomDiceSprite.AddSequence(() => true, yourSprite, new Color(255, 255, 255, 255));
    /// </code></example>
    public static void AddSequence(Func<DiceBehaviour, bool> cond, Sprite map, Color? behColor = null)
    {
        _seqs.Add((cond, map, behColor));
    }

    internal static bool TryGetSeq(DiceBehaviour beh, [NotNullWhen(true)] out (Sprite, Color?)? result)
    {
        foreach (var (cond, sprite, behColor) in _seqs)
        {
            if (cond(beh))
            {
                result = (sprite, behColor);

                return true;
            }
        }

        result = null;

        return false;
    }

    private static List<(Func<DiceBehaviour, bool>, Sprite, Color?)> _seqs;
}

internal static class CustomDicePatch
{
    [HarmonyPatch(typeof(BattleDiceCardUI), "SetCard", [typeof(BattleDiceCardModel), typeof(BattleDiceCardUI.Option[])])]
    public class PatchBattleDiceCardUI
    {
        static void Postfix(BattleDiceCardUI __instance, BattleDiceCardModel cardModel)
        {
            foreach (var (i, beh) in cardModel.GetBehaviourList().Enumerate())
            {
                if (CustomDiceSprite.TryGetSeq(beh, out var res))
                {
                    var sprite = res.Value.Item1;
                    var color = res.Value.Item2;

                    __instance.ui_behaviourDescList[i].img_detail.sprite = sprite;
                    __instance.img_behaviourDetatilList[i].sprite = sprite;

                    if (color is not null)
                    {
                        __instance.ui_behaviourDescList[i].txt_range.color = color.Value;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(UI.UIOriginCardSlot), "SetData")]
    public class PatchOriginCardSlot
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var ldfld = AccessTools.Field(typeof(UI.UIOriginCardSlot), "img_BehaviourIcons");
            var target = AccessTools.Method(typeof(Image), "set_sprite", [typeof(Sprite)]);
            var inject = AccessTools.Method(typeof(PatchOriginCardSlot), "InjectMethod");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldfld, ldfld),
                    new CodeMatch(OpCodes.Ldloc_3),
                    new CodeMatch(OpCodes.Ldelem_Ref),
                    CodeMatch.IsLdloc(),
                    CodeMatch.Calls(target)
                )
                .ThrowIfNotMatch("Not match CustomDiceSprite: IL not valid")
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Call, inject)
                );

            return matcher.Instructions();
        }

        static Sprite InjectMethod(Sprite grab, DiceCardItemModel cardmodel, int i)
        {
            var beh = cardmodel.GetBehaviourList()[i];

            if (CustomDiceSprite.TryGetSeq(beh, out var res))
            {
                var sprite = res.Value.Item1;

                return sprite;
            }

            return grab;
        }
    }

    [HarmonyPatch(typeof(UI.UIDetailCardSlot), "SetData")]
    public class PatchDetailCardSlot
    {
        static void Postfix(DiceCardItemModel cardmodel, List<UI.UIDetailCardDescSlot> ___rightDescSlotList)
        {
            foreach (var (i, beh) in cardmodel.GetBehaviourList().Enumerate())
            {
                if (CustomDiceSprite.TryGetSeq(beh, out var res))
                {
                    var sprite = res.Value.Item1;
                    var color = res.Value.Item2;

                    ___rightDescSlotList[i].img_detail.sprite = sprite;

                    if (color is not null)
                    {
                        ___rightDescSlotList[i].txt_range.color = color.Value;
                    }
                }
            }
        }
    }
}
