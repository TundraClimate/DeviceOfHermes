using HarmonyExtension;
using UI;
using UnityEngine;

namespace DeviceOfHermes;

/// <summary>A extension of BattleUnit</summary>
public static class BattleUnitExtension
{
    const int SUPPLIES = 10;

    static BattleUnitExtension()
    {
        AddUnitInfoUI();
    }

    extension(StageController controller)
    {
        /// <summary>Applies new enemy unit</summary>
        public bool TryAddNewEnemy(LorId id, int idx = -1, int height = -1)
        {
            if (idx <= -1)
            {
                idx = BattleObjectManager.instance.GetList(Faction.Enemy).Max(u => u.index) + 1;
            }

            var isInvalid = idx >= BattleManagerUI.Instance.ui_unitListInfoSummary.enemyProfileArray.Length
                && idx >= controller.GetCurrentWaveModel().GetFormation().PostionList.Count;

            if (isInvalid)
            {
                return false;
            }

            UICharacterRenderer.Instance.SetCharacter(controller.AddNewUnit(Faction.Enemy, id, idx, height).UnitData.unitData, idx);

            return true;
        }

        /// <summary>Applies new librarian unit</summary>
        public bool TryAddNewLibrarian(LorId id, int idx = -1, int height = -1)
        {
            if (idx <= -1)
            {
                idx = BattleObjectManager.instance.GetList(Faction.Player).Max(u => u.index) + 1;
            }

            var isInvalid = idx >= BattleManagerUI.Instance.ui_unitListInfoSummary.allyProfileArray.Length
                && idx >= controller.GetCurrentStageFloorModel().GetFormation().PostionList.Count;

            if (isInvalid)
            {
                return false;
            }

            UICharacterRenderer.Instance.SetCharacter(AddNewLibrarian(controller, id, idx, height).UnitData.unitData, idx);

            return true;
        }
    }

    private static BattleUnitModel AddNewLibrarian(StageController __instance, LorId id, int index, int height = -1)
    {
        var data = EnemyUnitClassInfoList.Instance.GetData(id);
        var unitModel = new UnitDataModel(new LorId(data.workshopID, data.bookId[0]), __instance.CurrentFloor, false);

        unitModel.SetByEnemyUnitClassInfo(data);

        var unit = new UnitBattleDataModel(__instance.GetStageModel(), unitModel).Also(u => u.Init());

        if (height > -1)
        {
            unit.unitData.customizeData.height = height;
        }

        BattleObjectManager.instance.UnregisterUnitByIndex(Faction.Player, index);

        return (BattleUnitModel)typeof(StageController)
            .Method("CreateLibrarianUnit", [typeof(SephirahType), typeof(UnitBattleDataModel), typeof(int)])
            .Invoke(__instance, [__instance.CurrentFloor, unit, index]);
    }

    static void AddUnitInfoUI()
    {
        var manager = BattleManagerUI.Instance.ui_unitListInfoSummary;
        var yAdder = 64;
        var originSize = manager.enemyProfileArray.Length;

        if (SUPPLIES > originSize)
        {
            var pref = manager.enemyProfileArray.Last();

            Array.Resize(ref manager.enemyProfileArray, SUPPLIES);

            for (var i = originSize; SUPPLIES > i; i++)
            {
                var cloned = UnityEngine.Object.Instantiate(pref, pref.gameObject.transform.parent);

                cloned.gameObject.name = $"[DoH] Enemy Added +{yAdder}";
                cloned.transform.localPosition += new Vector3(0, yAdder, 0);

                yAdder += 64;

                manager.enemyProfileArray[i] = cloned;
            }
        }

        yAdder = 64;
        originSize = manager.allyProfileArray.Length;

        if (SUPPLIES > originSize)
        {
            var pref = manager.allyProfileArray.Last();

            Array.Resize(ref manager.allyProfileArray, SUPPLIES);

            for (var i = originSize; SUPPLIES > i; i++)
            {
                var cloned = UnityEngine.Object.Instantiate(pref, pref.gameObject.transform.parent);

                cloned.gameObject.name = $"[DoH] Librarian Added +{yAdder}";
                cloned.transform.localPosition += new Vector3(0, yAdder, 0);

                yAdder += 64;

                manager.allyProfileArray[i] = cloned;
            }
        }

        var coin = BattleManagerUI.Instance.ui_battleEmotionCoinUI;

        originSize = coin.enermy.Length;

        if (SUPPLIES > originSize)
        {
            var pref = coin.enermy[4];

            Array.Resize(ref coin.enermy, SUPPLIES);

            for (var i = originSize; SUPPLIES > i; i++)
            {
                coin.enermy[i] = new() { target = pref.target };
            }
        }

        originSize = coin.librarian.Length;

        if (SUPPLIES > originSize)
        {
            var pref = coin.librarian[4];

            Array.Resize(ref coin.librarian, SUPPLIES);

            for (var i = originSize; SUPPLIES > i; i++)
            {
                coin.librarian[i] = new() { target = pref.target };
            }
        }
    }
}
