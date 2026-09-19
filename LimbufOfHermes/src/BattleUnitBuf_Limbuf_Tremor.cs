using System.Runtime.CompilerServices;

namespace LimbufOfHermes;

/// <summary>A unit buf the Tremor</summary>
public class BattleUnitBuf_Limbuf_Tremor : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Tremor";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.Tremor;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl Hide</summary>
    public override bool Hide
    {
        get
        {
            if (this is BattleUnitBuf_Limbuf_TremorSuperposition)
            {
                return false;
            }

            if (IsEntangled())
            {
                return true;
            }

            return !active;
        }
    }

    /// <summary>Impl ChangeStack</summary>
    public override void ChangeStack(Func<int, int> f)
    {
        base.ChangeStack(f);

        if (base._owner?.bufListDetail?.GetActivatedBuf(LimKeywordBuf.Tremor)?.IsDestroyed() is true)
        {
            base._owner.bufListDetail.GetActivatedBufList()?.OfType<BattleUnitBuf_Limbuf_Tremor>()?.Foreach(buf =>
            {
                buf.Destroy();
            });
        }
    }

    /// <summary>Get tremor stack</summary>
    public int TremorStack => base._owner?.bufListDetail?.GetActivatedBuf(LimKeywordBuf.Tremor)?.stack ?? 0;

    /// <summary>Consume tremor</summary>
    public void Consume(int num)
        => (base._owner?.bufListDetail?.GetActivatedBuf(LimKeywordBuf.Tremor) as BattleUnitBuf_Limbuf_Tremor)?.ChangeStack(s => s - num);

    /// <summary>Tremor is entangled</summary>
    public bool IsEntangled()
    {
        return base._owner.bufListDetail.HasBuf<BattleUnitBuf_Limbuf_TremorSuperposition>();
    }

    /// <summary>Returns conversion tremor</summary>
    public List<BattleUnitBuf> GetConversionTremor()
    {
        return base._owner.bufListDetail.GetActivatedBufList()
                .FindAll(buf => buf is BattleUnitBuf_Limbuf_Tremor && buf.GetType() != typeof(BattleUnitBuf_Limbuf_Tremor) && buf.GetType() != typeof(BattleUnitBuf_Limbuf_TremorSuperposition));
    }

    /// <summary>Returns current tremor</summary>
    public BattleUnitBuf GetCurrentTremor()
    {
        if (base._owner.bufListDetail.GetActivatedBuf(LimKeywordBuf.TremorSuperposition) is BattleUnitBuf sup)
        {
            return sup;
        }

        if (base._owner.bufListDetail.GetActivatedBufList().Find(buf => buf is BattleUnitBuf_Limbuf_Tremor && !buf.IsDestroyed() && !buf.Hide) is BattleUnitBuf tremor)
        {
            return tremor;
        }

        return this;
    }

    /// <summary>Impl OnStackChange</summary>
    public override void OnStackChangeAll(BattleUnitBuf buf, int last)
    {
        base._owner?.bufListDetail?.GetActivatedBufList()?.OfType<BattleUnitBuf_Limbuf_Tremor>()?
            .Foreach(buf =>
            {
                buf.ChangeStack(_ => this.stack);

                if (!StageController.Instance.IsLogState())
                {
                    base._owner.view.unitBottomStatUI.UpdateStatUI(base._owner.hp, base._owner.breakDetail.breakGauge, null);
                }
            });
    }

    /// <summary>Impl OnAddBuf</summary>
    public override void OnAddBuf(int addedStack)
    {
        if (this.GetType() != typeof(BattleUnitBuf_Limbuf_Tremor) && 0 >= this.TremorStack)
        {
            Destroy();
        }
    }

    /// <summary>Impl OnOtherInstant</summary>
    public override void OnOtherInstant(AdvancedUnitBuf instant)
    {
        if (base._owner.IsImmune(this.bufType))
        {
            return;
        }

        if (!active || this.GetType() == typeof(BattleUnitBuf_Limbuf_Tremor) && IsEntangled())
        {
            return;
        }

        if (instant is BattleUnitBuf_Limbuf_TremorBurst burst && instant.Owner == base._owner && TremorStack > 0)
        {
            var dmg = TremorStack * (1 + burst.GetTremorBreakRateAdder());

            OnActivate((int)dmg);

            if (StageController.Instance.IsLogState())
            {
                base._owner.AddRencounterEvent(RencounterEvent.TakeDamaged, () =>
                {
                    base._owner.view.unitBottomStatUI.UpdateStatUI(base._owner.hp, base._owner.breakDetail.breakGauge, null);
                });
            }
            else
            {
                base._owner.view.unitBottomStatUI.UpdateStatUI(base._owner.hp, base._owner.breakDetail.breakGauge, null);
            }
        }
    }

    /// <summary>Impl OnActivate</summary>
    public override void OnActivate(int stack)
    {
        this.OnTremorBurst(stack);

        base._owner.EachPassiveOf<ILimbuf.OnTremorBurst>(i => i.OnTremorBurst(stack));
        base._owner.EachUnitBufOf<ILimbuf.OnTremorBurst>(i => i.OnTremorBurst(stack));
    }

    /// <summary>Unit on tremor burst</summary>
    public virtual void OnTremorBurst(int stack)
    {
        if (!IsEntangled() || this.GetType() == typeof(BattleUnitBuf_Limbuf_TremorSuperposition))
        {
            base._owner.StyledBreakDamage(stack, this.GetBufIcon(), DamageType.Buf, keyword: this.bufType);
        }
    }

    internal bool active = true;

    internal static ConditionalWeakTable<BattleDiceBehavior, Box<int>> BreakRateAdder = new();
}
