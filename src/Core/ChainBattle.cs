using System.Collections;
using HarmonyLib;
using HarmonyExtension;
using UI;
using UnityEngine;
using TMPro;

namespace DeviceOfHermes;

internal static class ChainBattle
{
    public static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.ChainBattle");

        harmony.CreateClassProcessor(typeof(PatchOnUIPhaseEnter)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnSetSephirah)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnBattleStart)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnStartBattle)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnRoundEndLast)).Patch();
    }

    static void InitManager()
    {
        var manager = AssemblyManager.Instance
            .CreateInstance_EnemyTeamStageManager(StageController.Instance.GetStageModel().GetWave(1).ManagerScript);

        if (manager is ChainBattleStageManager cm)
        {
            Manager = cm;
        }
    }

    static bool IsActiveChainBattle()
    {
        return Manager is not null && Manager.IsChainBattle;
    }

    static string RemFloorTxt()
    {
        return $"- Available {Manager?.MaximumRearFloorNum - SelectOrdering.Count + 1} / {Manager?.MaximumRearFloorNum + 1} Floor -";
    }

    static void OnSelect(SephirahType sephirah)
    {
        if (SelectOrdering.Contains(sephirah))
        {
            if (SelectOrdering.Count > 1)
            {
                SelectOrdering.Remove(sephirah);
            }
        }
        else
        {
            if (Manager?.MaximumRearFloorNum >= SelectOrdering.Count)
            {
                SelectOrdering.Add(sephirah);
            }
        }

        RefreshSelection();
    }

    static void RefreshSelection()
    {
        foreach (var (_, slot) in OrderingNumers)
        {
            slot.text = "";
            slot.color = Color.cyan;
        }

        foreach (var (i, sephirah) in SelectOrdering.Enumerate())
        {
            if (i > OrderingNumers.Count)
            {
                continue;
            }

            var num = i + 1;
            var slot = OrderingNumers[sephirah];

            slot.text = $"{num}";

            if (num == 1)
            {
                slot.color = new(1f, 1f, 0.2f, 1f);
            }
        }

        TitleText?.text = RemFloorTxt();

        ChainBattleStageManager.TmpSelectedFloors = SelectOrdering;
    }

    [HarmonyPatch(typeof(UIBattleSettingPanel), "OnUIPhaseEnter")]
    class PatchOnUIPhaseEnter
    {
        static void Prefix(
            UIBattleSettingPanel __instance,
            UIPhase phase,
            List<UISephirahButton> ___SephirahButtons
        )
        {
            if (phase is UIPhase.BattleSetting)
            {
                CleanupUI(__instance);
                InitManager();

                if (IsActiveChainBattle())
                {
                    InitUI(__instance, ___SephirahButtons);
                    RefreshSelection();
                }
            }
        }

        static void CleanupUI(UIBattleSettingPanel panel)
        {
            UnityObject.Destroy(TitleText);

            foreach (var (_, txt) in OrderingNumers)
            {
                UnityObject.Destroy(txt.gameObject);
            }

            OrderingNumers.Clear();
            SelectOrdering.Clear();
            PlayerRearguard.Clear();
            EnemyRearguard.Clear();
            TitleText = null;
            Manager = null;
        }

        static void InitUI(UIBattleSettingPanel panel, List<UISephirahButton> buttons)
        {
            panel.ActiveControl.AddChildObject("[DoH]ChainBattleTitle").Also(go =>
            {
                var txt = go.AddComponent<TextMeshProUGUI>();

                go.transform.localPosition += new Vector3(0f, 320f, 0f);

                txt.text = RemFloorTxt();
                txt.alignment = TextAlignmentOptions.Center;

                txt.color = new(1f, 1f, 0.2f, 1f);
                txt.raycastTarget = false;
                txt.enableWordWrapping = false;

                LocalizedFontSetter.Instance.SetLocalizedFont(txt, FontType.FONT_HEAD);

                txt.fontMaterial = new(txt.fontMaterial);

                txt.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, new(1f, 1f, 0.2f, 1f));
                txt.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.1f);
                txt.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, 0.1f);
                txt.fontMaterial.EnableKeyword(ShaderUtilities.Keyword_Underlay);
                txt.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, new(1f, 0.2f, 0.2f, 1f));
                txt.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.5f);
                txt.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.7f);

                TitleText = txt;

                txt.StartCoroutine(BlinkRoutine());
            });

            OrderingNumers.Clear();

            foreach (var button in buttons)
            {
                button.gameObject.AddChildObject("[DoH]Text").Also(go =>
                {
                    var txt = go.AddComponent<TextMeshProUGUI>();

                    txt.text = "";
                    txt.alignment = TextAlignmentOptions.Center;
                    txt.color = new(1f, 1f, 0.2f, 1f);
                    txt.raycastTarget = false;

                    LocalizedFontSetter.Instance.SetLocalizedFont(txt, FontType.FONT_HEAD);

                    txt.fontMaterial = new(txt.fontMaterial);

                    txt.fontMaterial.EnableKeyword(ShaderUtilities.Keyword_Underlay);
                    txt.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, new(0.15f, 0.12f, 0.3f, 0.9f));
                    txt.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.5f);
                    txt.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.7f);

                    OrderingNumers[button.sephirahType] = txt;
                });
            }

            if (StageController.Instance.GetCurrentStageFloorModel()?.Sephirah is SephirahType sep)
            {
                OnSelect(sep);
            }
        }

        static IEnumerator BlinkRoutine()
        {
            while (true)
            {
                yield return CommonCoroutine.TMPFadeout(TitleText!, 0f, 2.5f);
                yield return CommonCoroutine.TMPFadein(TitleText!, 0f, 0.5f);
            }
        }
    }

    [HarmonyPatch(typeof(UISephirahButton), "OnPointerClick")]
    class PatchOnSetSephirah
    {
        static void Prefix(UISephirahButton __instance, bool ___isDisabled, UIBattleSettingPanel ___panel)
        {
            if (!IsActiveChainBattle() || ___isDisabled)
            {
                return;
            }

            OnSelect(__instance.sephirahType);
        }
    }

    [HarmonyPatch(typeof(UIBattleSettingPanel), "OnClickBattleStart")]
    class PatchOnBattleStart
    {
        static void Prefix(UIBattleSettingPanel __instance, List<UISephirahButton> ___SephirahButtons)
        {
            if (IsActiveChainBattle() && SelectOrdering.Count > 0)
            {
                var sephirah = SelectOrdering[0];
                var idx = (int)sephirah;

                if (sephirah is SephirahType.Keter or SephirahType.None)
                {
                    idx = 0;
                }

                if (___SephirahButtons.Count > idx)
                {
                    __instance.SetNextSephirah(___SephirahButtons[idx]);
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "StartBattle")]
    class PatchOnStartBattle
    {
        static Exception Finalizer(StageController __instance, Exception __exception)
        {
            if (1 > SelectOrdering.Count)
            {
                return __exception;
            }

            var stage = __instance.GetStageModel();

            if (IsActiveChainBattle())
            {
                AddUnusedUnitsIn1st(stage);
                AddSpecifiedUnits(stage);
            }

            if (Manager?.IsSuppliesEnemy is true)
            {
                AddEnemySupplies(stage);
            }

            return __exception;
        }

        static IEnumerable<BattleUnitModel> ConvertToBattleUnit(IEnumerable<UnitBattleDataModel> models, Faction faction = Faction.Player)
        {
            return models.Map(unit =>
            {
                var model = BattleObjectManager.CreateDefaultUnit(faction);

                model.SetUnitData(unit);
                model.OnCreated();

                return model;
            });
        }

        static void AddUnusedUnitsIn1st(StageModel stage)
        {
            var units = stage.GetFloor(SelectOrdering[0]).GetUnitBattleDataList().Filter(unit => !unit.IsAddedBattle);

            PlayerRearguard.AddRange(ConvertToBattleUnit(units));
        }

        static void AddSpecifiedUnits(StageModel stage)
        {
            for (var i = 1; SelectOrdering.Count > i; i++)
            {
                var units = stage.GetFloor(SelectOrdering[i]).GetUnitBattleDataList();

                if (Manager?.IsSephirahRemove is true)
                {
                    units = units.Filter(unit => !unit.unitData.isSephirah).ToList();
                }

                PlayerRearguard.AddRange(ConvertToBattleUnit(units));
            }
        }

        static void AddEnemySupplies(StageModel stage)
        {
            var units = stage.GetWave(StageController.Instance.CurrentWave).UnitList.Filter(unit => BattleObjectManager.instance.GetList(Faction.Enemy).All(model => model.UnitData != unit));

            EnemyRearguard.AddRange(ConvertToBattleUnit(units, Faction.Enemy));
        }
    }

    [HarmonyPatch(typeof(StageController), "RoundEndPhase_TheLast")]
    class PatchOnRoundEndLast
    {
        static Exception Finalizer(Exception __exception)
        {
            var librarians = BattleObjectManager.instance.GetList(Faction.Player)
                .Filter(unit => unit.IsDead())
                .Inspect(unit => unit.UnitData.isDead = true)
                .Collect();

            var enemies = BattleObjectManager.instance.GetList(Faction.Enemy)
                .Filter(unit => unit.IsDead())
                .Inspect(unit => unit.UnitData.isDead = true)
                .Collect();

            ReplaceUnit(Faction.Player, librarians);
            ReplaceUnit(Faction.Enemy, enemies);

            var retreatLibrarians = BattleObjectManager.instance.GetAliveList(Faction.Player)
                .Filter(unit => unit.bufListDetail.HasBuf<ChainBattleRetreatBuf>())
                .Collect();

            var retreatEnemies = BattleObjectManager.instance.GetAliveList(Faction.Enemy)
                .Filter(unit => unit.bufListDetail.HasBuf<ChainBattleRetreatBuf>())
                .Collect();

            RetreatUnit(Faction.Player, retreatLibrarians);
            RetreatUnit(Faction.Enemy, retreatEnemies);

            RefreshUnitList();

            return __exception;
        }

        static void InitUnit(Faction faction, BattleUnitModel unit, int idx)
        {
            unit.index = idx;
            unit.grade = unit.UnitData.unitData.grade;

            if (faction is Faction.Player)
            {
                unit.formation = StageController.Instance.GetCurrentStageFloorModel().GetFormationPosition(unit.index);

                _libraianTeamRef(StageController.Instance).AddUnit(unit);
            }
            else if (faction is Faction.Enemy)
            {
                unit.formation = StageController.Instance.GetCurrentWaveModel().GetFormationPosition(unit.index);

                _enemyTeamRef(StageController.Instance).AddUnit(unit);
            }

            BattleObjectManager.instance.RegisterUnit(unit);

            unit.isRegister = true;

            unit.passiveDetail.OnCreated();
            unit.cardSlotDetail.SetNextRoundPlayPoint(unit.MaxPlayPoint);
            unit.allyCardDetail.DrawCards(3);
            unit.OnWaveStart();

            Util.LoadPrefab("Battle/DiceAttackEffects/New/FX/PC/Angela/FX_PC_Angela_LibrarianCreate")?.Let(eff =>
            {
                eff.transform.position = unit.view.WorldPosition;
                eff.AddComponent<AutoDestruct>().time = 2f;
            });

            Manager?.OnArrived(unit);
        }

        static void RetreatUnit(Faction faction, List<BattleUnitModel> units)
        {
            if (1 > units.Count)
            {
                return;
            }

            foreach (var unit in units)
            {
                Manager?.OnRetreat(unit);
                unit.bufListDetail.GetActivatedBufList().OfType<ChainBattleRetreatBuf>().Foreach(buf => buf.OnRetreat());
                unit.bufListDetail.GetActivatedBufList().RemoveAll(buf => buf is ChainBattleRetreatBuf);

                BattleObjectManager.instance.UnregisterUnit(unit);

                unit.isRegister = false;
            }

            if (IsActiveChainBattle())
            {
                var rearguard = faction is Faction.Player ? PlayerRearguard : EnemyRearguard;

                for (var i = 0; units.Count > i && rearguard.Count > 0; i++)
                {
                    var rear = rearguard[0];
                    var unit = units[i];

                    if (!rear.IsDead())
                    {
                        InitUnit(faction, rear, unit.index);
                    }

                    rearguard.Remove(rear);
                }

                foreach (var unit in units)
                {
                    rearguard.Add(unit);
                }
            }
        }

        static void ReplaceUnit(Faction faction, List<BattleUnitModel> deads)
        {
            var rearguard = faction is Faction.Player ? PlayerRearguard : EnemyRearguard;

            if (1 > deads.Count || 1 > rearguard.Count || !IsActiveChainBattle())
            {
                return;
            }

            for (var i = 0; deads.Count > i && rearguard.Count > 0; i++)
            {
                var rear = rearguard[0];
                var dead = deads[i];

                if (!rear.IsDead())
                {
                    InitUnit(faction, rear, dead.index);

                    BattleObjectManager.instance.UnregisterUnit(dead);
                    dead.isRegister = false;
                }

                if (dead.IsDead())
                {
                    Manager?.OnDead(dead);
                }

                rearguard.Remove(rear);
            }
        }

        static void RefreshUnitList()
        {
            foreach (var (i, unit) in BattleObjectManager.instance.GetList().Enumerate())
            {
                UICharacterRenderer.Instance.SetCharacter(unit.UnitData.unitData, i, true, false);
            }

            BattleObjectManager.instance.InitUI();
        }

        static AccessTools.FieldRef<StageController, BattleTeamModel> _libraianTeamRef
            = typeof(StageController).FieldRefAccess<BattleTeamModel>("_librarianTeam");

        static AccessTools.FieldRef<StageController, BattleTeamModel> _enemyTeamRef
            = typeof(StageController).FieldRefAccess<BattleTeamModel>("_enemyTeam");
    }

    static ChainBattleStageManager? Manager;

    static List<SephirahType> SelectOrdering = new();

    static Dictionary<SephirahType, TextMeshProUGUI> OrderingNumers = new();

    static TextMeshProUGUI? TitleText;

    static List<BattleUnitModel> PlayerRearguard = new();

    static List<BattleUnitModel> EnemyRearguard = new();
}
