namespace LimbufOfHermes;

/// <summary>A unit buf the Rupture</summary>
public class BattleUnitBuf_Limbuf_Rupture : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Rupture";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.Rupture;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl OnTakeDamageByAttack</summary>
    public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
    {
        if (!base._owner.IsImmune(this.bufType))
        {
            base._owner.TakeDamage(this.stack, DamageType.Buf, null, this.bufType);
            this.OnActivate(this.stack);
            this.ChangeStack(s => s * 2 / 3);
        }
    }
}
