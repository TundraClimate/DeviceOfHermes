namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced <see cref="BattleUnitBuf"/></summary>
/// <remarks>
/// Can replaces <c>BattleUnitBuf</c> into this.
/// </remarks>
/// <example><code>
/// public class BattleUnitBuf_Advance : AdvancedUnitBuf
/// {
/// }
/// </code></example>
public class AdvancedUnitBuf : BattleUnitBuf
{
    static AdvancedUnitBuf()
    {
        AdvancedPatch.Init();
        BattleTickAction.OnTick += OnTick;
    }

    /// <summary>Initialize UnitBuf</summary>
    /// <remarks>
    /// AdvancedUnitBuf is implemented force initializer.<br/>
    /// Dont calls <c>base.Init(onwer)</c>.
    /// </remarks>
    public override void Init(BattleUnitModel owner)
    {
        base.Init(owner);

        this.lastStack = this.DefaultStack;
        this.stack = this.DefaultStack;
    }

    /// <summary>The stack of Unitbuf on inflicted</summary>
    /// <returns>Returns default stack</returns>
    public virtual int DefaultStack { get => 0; }

    /// <summary>Is Unitbuf is instant</summary>
    /// <returns>Is instant buf if true</returns>
    public virtual bool IsInstant { get => false; }

    /// <summary>The instant buf on inflicted</summary>
    /// <remarks>
    /// Only activates <see cref="IsInstant"/> is true.
    /// </remarks>
    public virtual void OnInstant()
    {
    }

    /// <summary>Other instants on inflicted</summary>
    /// <param name="instant">inflicted other instant</param>
    public virtual void OnOtherInstant(AdvancedUnitBuf instant)
    {
    }

    /// <summary>Unit on round start before <see cref="PassiveAbilityBase.OnRoundStart"/></summary>
    public virtual void OnRoundStartFirst()
    {
    }

    /// <summary>Unit on round start after <see cref="PassiveAbilityBase.OnRoundStartAfter"/></summary>
    public virtual void OnRoundStartLast()
    {
    }

    /// <summary>Unitbuf stack on changed</summary>
    public virtual void OnStackChange(int last)
    {
    }

    /// <summary>Unitbuf icon on clicked</summary>
    public virtual void OnClick(ClickType ty)
    {
    }

    /// <summary>The clicktype for OnClick</summary>
    public enum ClickType
    {
        /// <summary>Left</summary>
        Left,

        /// <summary>Right</summary>
        Right,

        /// <summary>Middle</summary>
        Middle,
    }

    internal static void OnTick()
    {
        var alives = BattleObjectManager.instance.GetAliveList();

        foreach (var unit in alives)
        {
            foreach (var buf in unit.bufListDetail?.GetActivatedBufList() ?? new())
            {
                if (buf is AdvancedUnitBuf advBuf && advBuf.stack != advBuf.lastStack)
                {
                    advBuf.OnStackChange(advBuf.lastStack);

                    advBuf.lastStack = advBuf.stack;
                }
            }
        }
    }

    internal int lastStack;
}
