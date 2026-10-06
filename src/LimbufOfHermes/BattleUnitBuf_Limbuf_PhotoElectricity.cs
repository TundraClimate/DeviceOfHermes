namespace LimbufOfHermes;

/// <summary>A unit buf the PhotoElectricity</summary>
public class BattleUnitBuf_Limbuf_PhotoElectricity : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_PhotoElectricity";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.PhotoElectricity;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl OnTakeDamageByAttack</summary>
    public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
    {
        if (!atkDice.owner.bufListDetail.HasBuf<Check>())
        {
            var num = atkDice.owner.bufListDetail?.GetKewordBufStack(KeywordBuf.WarpCharge) > 5 ? this.stack : this.stack + 3;

            atkDice.owner.bufListDetail?.AddKeywordBufThisRoundByEtc(KeywordBuf.WarpCharge, num, base._owner);
            atkDice.owner.bufListDetail?.AddBuf(new Check());
        }
    }

    /// <summary>Impl OnAddBuf</summary>
    public override void OnAddBuf(int addedStack)
    {
        this.stack = 3.Min(this.stack);
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }

    class Check : BattleUnitBuf
    {
        public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
        {
            Destroy();
        }

        public override void OnRoundEnd()
        {
            Destroy();
        }
    }
}
