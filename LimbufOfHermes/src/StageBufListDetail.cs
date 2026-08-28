using DeviceOfHermes.UI;

using LimbufOfHermes;

/// <summary>A detail of StageBuf</summary>
public static class StageBufListDetail
{
    static StageBufListDetail()
    {
        StageBufListUI.OnCleanup += () => stageBufs.Clear();
    }

    /// <summary>Activate buf</summary>
    public static void ActivateStageBuf(StageBuf bufType)
    {
        if (bufType is StageBuf.Bloodfeast && !stageBufs.Exists(buf => buf is BattleUnitBuf_Limbuf_Bloodfeast))
        {
            UpdateBuf<BattleUnitBuf_Limbuf_Bloodfeast>(_ => BattleUnitBuf_Limbuf_Bloodfeast.BleedDmgHistory.GetValue(StageController.Instance.waveHistory, _ => new(0)).value);
            UpdateBuf<BattleUnitBuf_Limbuf_BloodfeastConsumedAll>(_ => 0);
        }

        if (bufType is StageBuf.Scorchfield && !stageBufs.Exists(buf => buf is BattleUnitBuf_Limbuf_Scorchfield))
        {
            UpdateBuf<BattleUnitBuf_Limbuf_Scorchfield>(_ => BattleUnitBuf_Limbuf_Scorchfield.BurnDmgHistory.GetValue(StageController.Instance.waveHistory, _ => new(0)).value);
        }

        UpdateUI();
    }

    /// <summary>Try consume buf</summary>
    public static bool TryConsume(StageBuf bufType, int stack, BattleUnitModel? consume = null)
    {
        if (GetStageBuf(bufType) is StageBufBase buf && buf.stack >= stack)
        {
            buf.stack -= stack;

            buf.OnConsume(stack, consume);

            UpdateUI();

            BattleObjectManager.instance.GetAliveList().ForEach(unit =>
            {
                unit.EachPassiveOf<ILimbuf.OnConsumeStageBuf>(i => i.OnConsumeStageBuf(bufType, consume, stack));
                unit.EachUnitBufOf<ILimbuf.OnConsumeStageBuf>(i => i.OnConsumeStageBuf(bufType, consume, stack));
            });

            return true;
        }

        return false;
    }

    /// <summary>Consumes buf</summary>
    public static int Consume(StageBuf bufType, int max, BattleUnitModel? consume = null)
    {
        if (GetStageBuf(bufType) is StageBufBase buf && buf.stack > 0)
        {
            var res = max;

            if (max > buf.stack)
            {
                res = buf.stack;
            }

            buf.stack -= res;

            buf.OnConsume(res, consume);

            UpdateUI();

            BattleObjectManager.instance.GetAliveList().ForEach(unit =>
            {
                unit.EachPassiveOf<ILimbuf.OnConsumeStageBuf>(i => i.OnConsumeStageBuf(bufType, consume, res));
                unit.EachUnitBufOf<ILimbuf.OnConsumeStageBuf>(i => i.OnConsumeStageBuf(bufType, consume, res));
            });

            return res;
        }

        return 0;
    }

    /// <summary>Returns buf count</summary>
    public static int GetBufCount(StageBuf buf)
    {
        return GetStageBuf(buf)?.stack ?? 0;
    }

    internal static StageBufBase? GetStageBuf(StageBuf buf)
    {
        var ty = buf switch
        {
            StageBuf.Bloodfeast => typeof(BattleUnitBuf_Limbuf_Bloodfeast),
            StageBuf.Scorchfield => typeof(BattleUnitBuf_Limbuf_Scorchfield),
            _ => null,
        };

        if (ty is not null && stageBufs.Find(buf => buf.GetType() == ty) is StageBufBase res)
        {
            return res;
        }

        return null;
    }

    internal static void ChangeBufStack<T>(Func<int, int> f)
        where T : StageBufBase
    {
        if (stageBufs.Find(buf => buf is T) is T buf)
        {
            buf.stack = f(buf.stack);
        }
    }

    private static void UpdateBuf<T>(Func<int, int> f)
        where T : StageBufBase, new()
    {
        var buf = stageBufs.Find(buf => buf is T) ?? new T().Also(buf => stageBufs.Add(buf));

        buf.stack = f(buf.stack);
    }

    internal static void UpdateUI()
    {
        BattleManagerUI.Instance.GetBehaviour<StageBufListUI>("stageBufList")?.UpdateBufUI(stageBufs);
    }

    static List<StageBufBase> stageBufs = new();
}
