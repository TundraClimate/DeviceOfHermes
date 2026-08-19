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

        if (instant is BattleUnitBuf_Limbuf_TremorBurst && instant.Owner == base._owner && TremorStack > 0)
        {
            if (StageController.Instance.IsLogState())
            {
                base._owner.AddRencounterEvent(RencounterEvent.PrintEffect, () =>
                {
                    OnActivate(this.stack);

                    base._owner.view.unitBottomStatUI.UpdateStatUI(base._owner.hp, base._owner.breakDetail.breakGauge, null);
                });
            }
            else
            {
                OnActivate(this.stack);

                base._owner.view.unitBottomStatUI.UpdateStatUI(base._owner.hp, base._owner.breakDetail.breakGauge, null);
            }
        }
    }

    /// <summary>Impl OnActivate</summary>
    public override void OnActivate(int stack)
    {
        this.OnTremorBurst(TremorStack);
    }

    /// <summary>Unit on tremor burst</summary>
    public virtual void OnTremorBurst(int stack)
    {
        base._owner.breakDetail.TakeBreakDamage(stack, DamageType.Buf, keyword: this.bufType);
    }

    internal bool active = true;
}
