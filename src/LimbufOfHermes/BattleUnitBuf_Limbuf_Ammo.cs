namespace LimbufOfHermes;

/// <summary>A buf the Ammo</summary>
public class BattleUnitBuf_Limbuf_Ammo : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Ammo";

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.None;

    /// <summary>Impl paramInBufDesc</summary>
    public override int paramInBufDesc => MaxValue;

    /// <summary>Ammo can fires while empty</summary>
    public virtual bool CanFireWhileEmpty => false;

    /// <summary>Returns max value</summary>
    public int MaxValue { get; internal set; }

    /// <summary>Returns total consumed ammo</summary>
    public int TotalConsumed { get; internal set; }

    /// <summary>Impl OnAddBuf</summary>
    public override void OnAddBuf(int addedStack)
    {
        this.stack = MaxValue.Min(this.stack);
    }

    /// <summary>Consume ammo</summary>
    public bool Consume(BattleDiceBehavior dice, int num)
    {
        num += BeforeConsume(dice, this.stack, num);
        num = 0.Max(num);

        if (this.stack < num)
        {
            if (!CanFireWhileEmpty)
            {
                dice.ApplyDiceStatBonus(new AdvancedDiceStatBonus { noHit = true });
            }

            return false;
        }

        this.stack -= num;
        this.stack = 0.Max(this.stack);

        TotalConsumed += num;

        OnConsume(dice, num);

        base._owner.EachPassiveOf<ILimbuf.OnConsumeAmmo>(i => i.OnConsumeAmmo(this, dice, num));
        base._owner.EachUnitBufOf<ILimbuf.OnConsumeAmmo>(i => i.OnConsumeAmmo(this, dice, num));

        if (this.stack <= 0)
        {
            OnEmpty(dice);

            base._owner.EachPassiveOf<ILimbuf.OnAmmoEmpty>(i => i.OnAmmoEmpty(this, dice));
            base._owner.EachUnitBufOf<ILimbuf.OnAmmoEmpty>(i => i.OnAmmoEmpty(this, dice));
        }

        dice.ApplyDiceStatBonus(new AdvancedDiceStatBonus { customFields = { [$"LimbufOfHermes_Ammo_{this.GetType().FullName}"] = 1 } });

        return true;
    }

    /// <summary>Ammo before consume</summary>
    public virtual int BeforeConsume(BattleDiceBehavior dice, int current, int num)
    {
        return 0;
    }

    /// <summary>Ammo on consume</summary>
    public virtual void OnConsume(BattleDiceBehavior dice, int num)
    {
    }

    /// <summary>Ammo on empty</summary>
    public virtual void OnEmpty(BattleDiceBehavior dice)
    {
    }

    /// <summary>Reload ammo</summary>
    public void Reload()
    {
        this.stack = MaxValue;

        OnReload();

        base._owner.EachPassiveOf<ILimbuf.OnReloadAmmo>(i => i.OnReloadAmmo(this));
        base._owner.EachUnitBufOf<ILimbuf.OnReloadAmmo>(i => i.OnReloadAmmo(this));
    }

    /// <summary>On reload ammo</summary>
    public virtual void OnReload()
    {
    }
}
