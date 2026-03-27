using System.Diagnostics.CodeAnalysis;

namespace DeviceOfHermes;

public static class BattleBufExtension
{
    public static T? GetBuf<T>(this BattleUnitModel? model)
        where T : BattleUnitBuf
    {
        return model?.bufListDetail?.GetActivatedBufList().Find(buf => buf is T) as T;
    }

    public static bool TryGetBuf<T>(this BattleUnitModel? model, [NotNullWhen(true)] out T? buf)
        where T : BattleUnitBuf
    {
        var res = model.GetBuf<T>();

        buf = res as T;

        return res is not null;
    }

    public static T GetBufAndInitIfNull<T>(this BattleUnitModel model, Func<T> bufMake)
        where T : BattleUnitBuf
    {
        if (!model.TryGetBuf<T>(out var buf))
        {
            var newBuf = bufMake();

            model.bufListDetail.AddBuf(newBuf);

            return newBuf;
        }

        return buf;
    }

    public static void RemoveBuf<T>(this BattleUnitModel? model)
    {
        model?.bufListDetail?.RemoveBufAll(typeof(T));
    }

    public static void RemoveBufIf(this BattleUnitModel? model, Func<BattleUnitBuf, bool> cond)
    {
        List<BattleUnitBuf> bin = new();

        foreach (var buf in model?.bufListDetail?.GetActivatedBufList() ?? new())
        {
            if (cond(buf))
            {
                bin.Add(buf);
            }
        }

        foreach (var trash in bin)
        {
            model?.bufListDetail?.RemoveBuf(trash);
        }
    }

    public static int? GetBufStack<T>(this BattleUnitModel? model)
        where T : BattleUnitBuf
    {
        return model?.GetBuf<T>()?.stack;
    }
}
