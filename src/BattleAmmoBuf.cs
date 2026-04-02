using LOR_DiceSystem;

namespace DeviceOfHermes.AdvancedBase;

/// <summary>A ammo optimized <see cref="BattleUnitBuf"/></summary>
/// <remarks>
/// What is ammo:<br/>
/// Is consumable, and generally can reload.
/// </remarks>
/// <example><code>
/// public class BattleUnitBuf_Ammo : BattleAmmoBuf
/// {
/// }
/// </code></example>
public class BattleAmmoBuf : AdvancedUnitBuf
{
    /// <summary>A number of gain by reloaded</summary>
    /// <returns>Returns number by reload</returns>
    public virtual int GainByReload => this.DefaultStack;

    /// <summary>Is atk dice block when stack is zero</summary>
    /// <returns>Returns true if block</returns>
    public virtual bool DiceBlockWithNotConsumable => true;

    /// <summary>A number of Consumed ammo value excludes reload</summary>
    /// <returns>Returns number of consumed</returns>
    public int ConsumedStack => _consumedStack;

    /// <summary>A number of Consumed ammo value includes reload</summary>
    /// <returns>Returns number of consumed</returns>
    public int LosedStack => _losedStack;

    private int _consumedStack = 0;

    private int _losedStack = 0;

    /// <summary>Consume ammo</summary>
    /// <param name="num">A number of consume</param>
    /// <remarks>
    /// Ammo consume with <paramref name="num"/> stack. <br/>
    /// <see cref="IsConsumable(int)"/> if false, will consuming cancel.
    /// </remarks>
    /// <example><code>
    /// var ammo = owner.GetBuf&lt;MyAmmoBuf&gt;();
    ///
    /// ammo.Consume(1);
    /// </code></example>
    public void Consume(int num)
    {
        this.OnBeforeConsume(ref num);

        if (this.IsConsumable(num))
        {
            this.OnConsume(num);

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

            this.OnCancelled();
        }
    }

    /// <summary>Unitbuf before consume ammo</summary>
    /// <param name="require">A value of required number by consume</param>
    public virtual void OnBeforeConsume(ref int require)
    {
    }

    /// <summary>Unitbuf on consumed ammo</summary>
    /// <param name="consumed">A value of consumed ammo</param>
    public virtual void OnConsume(int consumed)
    {
    }

    /// <summary>Unitbuf on reload cancelled</summary>
    public virtual void OnCancelled()
    {
    }

    /// <summary>Unit buf is ammo consumable</summary>
    /// <returns>An ammo is consumable</returns>
    public virtual bool IsConsumable(int num = 1)
    {
        return base.stack != 0 && base.stack >= num;
    }

    /// <summary>Reload ammo</summary>
    /// <remarks>
    /// <see cref="OnReload"/> if false, will reloading cancel.
    /// </remarks>
    /// <example><code>
    /// var ammo = owner.GetBuf&lt;MyAmmoBuf&gt;();
    ///
    /// ammo.Reload();
    /// </code></example>
    public void Reload()
    {
        if (this.OnReload())
        {
            this._losedStack += base.stack;
            base.stack = this.GainByReload;
        }
    }

    /// <summary>Unitbuf on reload</summary>
    /// <returns>Reload be cancelled if false</returns>
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

/// <summary>A instant <see cref="BattleUnitBuf"/> for reload ammo</summary>
/// <typeparam name="T">A type of BattleAmmoBuf</typeparam>
/// <remarks>
/// Reload instantly applied when inflict.
/// </remarks>
/// <example><code>
/// public class BattleUnitBuf_Reload : ReloadAmmoBuf&lt;BattleUnitBuf_Ammo&gt;
/// {
/// }
///
/// // or Simply
///
/// owner.bufListDetail.AddBuf(new ReloadAmmoBuf&lt;BattleUnitBuf_Ammo&gt;());
/// </code></example>
public class ReloadAmmoBuf<T> : AdvancedUnitBuf
    where T : BattleAmmoBuf
{
    /// <summary>This unitbuf is instat</summary>
    public override bool IsInstant => true;

    /// <summary>Reload for T</summary>
    public override void OnInstant()
    {
        base._owner?.ReloadAmmo<T>();
    }
}

/// <summary>The functions related to BattleAmmoBuf</summary>
public static class AmmoExtension
{
    /// <summary>Consumes ammos if find</summary>
    /// <param name="owner">Consumes owner</param>
    /// <param name="num">Consumes number</param>
    /// <typeparam name="T">Ammo type</typeparam>
    /// <returns>Is find ammo buf</returns>
    /// <example><code>
    /// owner.ConsumeAmmo&lt;BattleUnitBuf_Ammo&gt;(6);
    /// </code></example>
    public static bool ConsumeAmmo<T>(this BattleUnitModel? owner, int num)
        where T : BattleAmmoBuf
    {
        var ammo = owner?.GetBuf<T>();

        ammo?.Consume(num);

        return ammo is not null;
    }

    /// <summary>Reload ammos if find</summary>
    /// <param name="owner">Ammo owner</param>
    /// <typeparam name="T">Ammo type</typeparam>
    /// <returns>Is find ammo buf</returns>
    /// <example><code>
    /// owner.ReloadAmmo&lt;BattleUnitBuf_Ammo&gt;(6);
    /// </code></example>
    public static bool ReloadAmmo<T>(this BattleUnitModel? owner)
        where T : BattleAmmoBuf
    {
        var ammo = owner?.GetBuf<T>();

        ammo?.Reload();

        return ammo is not null;
    }
}
