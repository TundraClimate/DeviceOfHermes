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
        public bool TryAddNewEnemy(LorId id, int idx = -1)
        {
            if (idx <= -1)
            {
                idx = BattleObjectManager.instance.GetList(Faction.Enemy).Max(u => u.index) + 1;
            }

            var isValid = idx >= BattleManagerUI.Instance.ui_unitListInfoSummary.enemyProfileArray.Length
                && idx >= controller.GetCurrentWaveModel().GetFormation().PostionList.Count;

            if (isValid)
            {
                return false;
            }

            UICharacterRenderer.Instance.SetCharacter(controller.AddNewUnit(Faction.Enemy, id, idx).UnitData.unitData, idx);

            return true;
        }
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
