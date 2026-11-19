namespace LimbufOfHermes;

/// <summary>A unit buf the Tremor</summary>
public class BattleUnitBuf_Limbuf_TremorBurstByCard : BattleUnitBuf_Limbuf_TremorBurst
{
    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.TremorBurstByCard;

    /// <summary>Impl OnInstant</summary>
    public override void OnInstant()
    {
        base.OnInstant();

        if (this.LastInflictBy is BattleUnitModel actor)
        {
            actor.EachPassiveOf<ILimbuf.OnExecTremorBurstByCard>(i => i.OnExecTremorBurstByCard(base._owner));
            actor.EachUnitBufOf<ILimbuf.OnExecTremorBurstByCard>(i => i.OnExecTremorBurstByCard(base._owner));
        }
    }

    /// <summary>Impl OnActivate</summary>
    public override void OnActivate(int stack)
    {
        base.OnActivate(stack);

        base._owner.EachPassiveOf<ILimbuf.OnApplyTremorBurstByCard>(i => i.OnApplyTremorBurstByCard());
        base._owner.EachUnitBufOf<ILimbuf.OnApplyTremorBurstByCard>(i => i.OnApplyTremorBurstByCard());
    }
}
