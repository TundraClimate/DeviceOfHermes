using UnityEngine;

namespace DeviceOfHermes;

/// <summary>A buf of equips by speed dice</summary>
public class SpeedDiceBuf
{
    /// <summary>A keywordId</summary>
    public virtual string keywordId => "";

    /// <summary>A keywordIconId</summary>
    public virtual string keywordIconId => this.keywordId;

    /// <summary>Runs on start battle</summary>
    public virtual void OnStartBattle()
    {
    }

    /// <summary>Runs on roll dice before</summary>
    public virtual void BeforeRollDice(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Runs on roll dice</summary>
    public virtual void OnRollDice(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Runs on give damage before</summary>
    public virtual void BeforeGiveDamage(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Runs on succeed attack</summary>
    public virtual void OnSucceedAttack(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Runs on win parrying</summary>
    public virtual void OnWinParrying(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Runs on lose parrying</summary>
    public virtual void OnLoseParrying(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Runs on draw parrying</summary>
    public virtual void OnDrawParrying(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Runs on uses card</summary>
    public virtual void OnUseCard()
    {
    }

    /// <summary>Runs on start parrying</summary>
    public virtual void OnStartParrying()
    {
    }

    /// <summary>Runs on start oneside action</summary>
    public virtual void OnStartOneSideAction()
    {
    }

    /// <summary>Runs on round end</summary>
    public virtual void OnRoundEnd()
    {
    }

    /// <summary>Returns buf icon</summary>
    public Sprite? GetBufIcon()
    {
        if (this._bInit)
        {
            return _bufIcon;
        }

        this._bInit = true;

        if (BattleUnitBuf._bufIconDictionary.TryGetValue(keywordIconId, out var icon))
        {
            this._bufIcon = icon;

            return icon;
        }

        this._bufIcon = null;

        return null;
    }

    /// <summary>Destroys self</summary>
    public void Destroy()
    {
        Destroyed = true;
    }

    /// <summary>The index of speed dice</summary>
    public int Index = -1;

    /// <summary>A card of equips dice</summary>
    public BattlePlayingCardDataInUnitModel? equipped;

    /// <summary>Card is destroyed</summary>
    public bool Destroyed { get; private set; }

    private bool _bInit;

    private Sprite? _bufIcon;

    internal SpeedDiceBufUI? ui;
}
