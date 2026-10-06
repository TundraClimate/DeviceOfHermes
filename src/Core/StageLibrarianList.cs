using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>A util of stage librarian list</summary>
public class StageLibrarianList
{
    static StageLibrarianList()
    {
        var harmony = new Harmony("DeviceOfHermes.StageLibrarianList");

        harmony.CreateClassProcessor(typeof(PatchUnitList)).Patch();
    }

    /// <summary>Set unit in index</summary>
    public static void SetUnit(LorId stageId, int idx, LorId bookId, string? name = null)
    {
        if (!_unitList.ContainsKey(stageId))
        {
            _unitList.Add(stageId, new());
        }

        _unitList[stageId].Add(idx, (bookId, name));
    }

    /// <summary>Add unit to head</summary>
    public static void AddUnit(LorId stageId, LorId bookId, string? name = null)
    {
        if (!_insertUnitList.ContainsKey(stageId))
        {
            _insertUnitList.Add(stageId, new());
        }

        _insertUnitList[stageId].Add((bookId, name));
    }

    private static UnitDataModel CreateUnitData(LorId id, string? name = null)
    {
        var unitModel = new UnitDataModel(31, SephirahType.Keter, true);

        unitModel.SetTemporaryPlayerUnitByBook(id);
        unitModel.isSephirah = true;
        unitModel.SetTempName(string.IsNullOrEmpty(name) ? BookXmlList.Instance.GetData(id).Name : name);
        unitModel.CreateDeckByDeckInfo();
        unitModel.forceItemChangeLock = true;

        return unitModel;
    }

    private static Dictionary<LorId, Dictionary<int, (LorId, string?)>> _unitList = new();

    private static Dictionary<LorId, List<(LorId, string?)>> _insertUnitList = new();

    [HarmonyPatch(typeof(StageLibraryFloorModel), "InitUnitList")]
    class PatchUnitList
    {
        static Exception Finalizer(Exception __exception, StageModel stage, List<UnitBattleDataModel> ____unitList)
        {
            if (_unitList.TryGetValue(stage.ClassInfo.id, out var replaceUnits))
            {
                foreach (var (idx, (replaceUnit, name)) in replaceUnits)
                {
                    if (idx >= ____unitList.Count)
                    {
                        continue;
                    }

                    var unit = CreateUnitData(replaceUnit, name);

                    ____unitList[idx] = new UnitBattleDataModel(stage, unit).Also(u => u.Init());
                }
            }

            if (_insertUnitList.TryGetValue(stage.ClassInfo.id, out var insertUnits))
            {
                var units = insertUnits
                    .Map(unit => new UnitBattleDataModel(stage, CreateUnitData(unit.Item1, unit.Item2)).Also(u => u.Init()))
                    .ToList();

                ____unitList.InsertRange(0, units);

                if (____unitList.Count > 5)
                {
                    ____unitList.RemoveRange(5, ____unitList.Count - 5);
                }
            }

            return __exception;
        }
    }
}
