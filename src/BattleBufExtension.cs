using System.Diagnostics.CodeAnalysis;

namespace DeviceOfHermes;

/// <summary>Additional battleunitbuf extension</summary>
public static class BattleBufExtension
{
    /// <summary>Get unitbuf if found</summary>
    /// <param name="model">A target of retrieves</param>
    /// <typeparam name="T">A target unitBuf</typeparam>
    /// <returns>Returns T if found</returns>
    /// <example><code>
    /// var buf = owner.GetBuf&lt;MyUnitBuf&gt;();
    /// </code></example>
    public static T? GetBuf<T>(this BattleUnitModel? model)
        where T : BattleUnitBuf
    {
        return model?.bufListDetail?.GetActivatedBufList().Find(buf => buf is T && !buf.IsDestroyed()) as T;
    }

    /// <summary>Try get unitbuf</summary>
    /// <param name="model">A target of retrieves</param>
    /// <param name="buf">Get resut if found</param>
    /// <typeparam name="T">A target unitBuf</typeparam>
    /// <returns>Unitbuf is found</returns>
    /// <example><code>
    /// if(owner.TryGetBuf&lt;MyUnitBuf&gt;(var out buf))
    /// {
    /// }
    /// </code></example>
    public static bool TryGetBuf<T>(this BattleUnitModel? model, [NotNullWhen(true)] out T? buf)
        where T : BattleUnitBuf
    {
        var res = model.GetBuf<T>();

        buf = res as T;

        return res is not null;
    }

    /// <summary>Get unitBuf if found, otherwise initialize by Func</summary>
    /// <param name="model">A target of retrieves</param>
    /// <param name="bufMake">New instance constructor</param>
    /// <typeparam name="T">A target unitBuf</typeparam>
    /// <returns>Returns T</returns>
    /// <remarks>
    /// Try get the <typeparamref name="T"/> is failed, run <paramref name="bufMake"/> and initialize buf.
    /// </remarks>
    /// <example><code>
    /// var buf = owner.GetBufAndInitIfNull&lt;MyUnitBuf&gt;(() => new MyUnitBuf());
    /// </code></example>
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

    /// <summary>Remove specific buf if found</summary>
    /// <param name="model">A target of retrieves</param>
    /// <typeparam name="T">A target unitBuf</typeparam>
    /// <example><code>
    /// owner.RemoveBuf&lt;MyUnitBuf&gt;();
    /// </code></example>
    public static void RemoveBuf<T>(this BattleUnitModel? model)
    {
        model?.bufListDetail?.RemoveBufAll(typeof(T));
    }

    /// <summary>Remove cond matches buf</summary>
    /// <param name="model">A target of retrieves</param>
    /// <param name="cond">Iterate activated bufs, remove that if returns true</param>
    /// <example><code>
    /// owner.RemoveBufIf(buf => buf.bufType == KeywordBuf.None);
    /// </code></example>
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

    /// <summary>Get stack number of specified unitBuf</summary>
    /// <param name="model">A target of retrieves</param>
    /// <typeparam name="T">A target unitBuf</typeparam>
    /// <example><code>
    /// // Binds MyUnitBuf stack if found otherwise binds 0
    /// var stack = owner?.GetBufStack&lt;MyUnitBuf&gt;() ?? 0;
    /// </code></example>
    public static int? GetBufStack<T>(this BattleUnitModel? model)
        where T : BattleUnitBuf
    {
        return model?.GetBuf<T>()?.stack;
    }

    /// <summary>Add buf stacks if <typeparamref name="T"/> is null then initialize by <paramref name="bufMake"/></summary>
    /// <param name="model">A target of retrieves</param>
    /// <param name="stack">A number of addition stacks</param>
    /// <param name="bufMake">New instance constructor</param>
    /// <typeparam name="T">A target unitBuf</typeparam>
    public static void AddBufStack<T>(this BattleUnitModel model, int stack, Func<BattleUnitBuf> bufMake)
        where T : BattleUnitBuf
    {
        var buf = model.GetBufAndInitIfNull(bufMake);

        buf.stack += stack;
    }

    /// <summary>Add buf stacks if <typeparamref name="T"/> is null then initialize <c>new T()</c></summary>
    /// <param name="model">A target of retrieves</param>
    /// <param name="stack">A number of addition stacks</param>
    /// <typeparam name="T">A target unitBuf</typeparam>
    public static void AddBufStack<T>(this BattleUnitModel model, int stack)
        where T : BattleUnitBuf, new()
    {
        var buf = model.GetBufAndInitIfNull(() => new T());

        buf.stack += stack;
    }
}
