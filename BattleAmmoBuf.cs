using LOR_DiceSystem;

namespace DeviceOfHermes.AdvancedBase;

public class BattleAmmoBuf : AdvancedUnitBuf
{
    public virtual int GainByReload => this.DefaultStack;

    public virtual bool DiceBlockWithNotConsumable => true;

    public int ConsumedStack => _consumedStack;

    public int LosedStack => _losedStack;

    private int _consumedStack = 0;

    private int _losedStack = 0;

    public void Consume(int num)
    {
        this.OnConsume(ref num);

        if (this.IsConsumable(num))
        {
            this._consumedStack += num;
            this._losedStack += num;
            base.stack = 0.Max(base.stack - num);
        }
        else if (this.DiceBlockWithNotConsumable)
        {
            var card = base._owner?.currentDiceAction;

            if (card is not null)
            {
                card.currentBehavior = CreateCancelAlternate(card);
            }

            this.OnCanceled();
        }
    }

    public virtual void OnConsume(ref int require)
    {
    }

    public virtual void OnCanceled()
    {
    }

    public virtual bool IsConsumable(int num = 1)
    {
        return base.stack != 0 && base.stack >= num;
    }

    public void Reload()
    {
        if (this.OnReload())
        {
            this._losedStack += base.stack;
            base.stack = this.GainByReload;
        }
    }

    public virtual bool OnReload()
    {
        return true;
    }

    private BattleDiceBehavior CreateCancelAlternate(BattlePlayingCardDataInUnitModel card)
    {
        var beh = new BattleDiceBehavior()
        {
            card = card,
            abilityList = [new ForceInvalid()],
            behaviourInCard = new DiceBehaviour()
            {
                Min = 0,
                Dice = 0,
                Detail = BehaviourDetail.None,
                Type = BehaviourType.Atk,
            },
        };

        beh.SetBlocked(true);

        return beh;
    }

    private class ForceInvalid : DiceCardAbilityBase
    {
        public override bool Invalidity => true;
    }
}

public class ReloadAmmoBuf<T> : AdvancedUnitBuf
    where T : BattleAmmoBuf
{
    public override bool IsInstant => true;

    public override void OnInstant()
    {
        base._owner?.ReloadAmmo<T>();
    }
}

public static class AmmoExtension
{
    public static bool ConsumeAmmo<T>(this BattleUnitModel? owner, int num)
        where T : BattleAmmoBuf
    {
        var ammo = owner?.GetBuf<T>();

        ammo?.Consume(num);

        return ammo is not null;
    }

    public static bool ReloadAmmo<T>(this BattleUnitModel? owner)
        where T : BattleAmmoBuf
    {
        var ammo = owner?.GetBuf<T>();

        ammo?.Reload();

        return ammo is not null;
    }
}
