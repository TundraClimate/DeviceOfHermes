using UnityEngine;

namespace DeviceOfHermes;

///
public class SpeedDiceBuf
{
    ///
    public virtual string keywordId => "";

    ///
    public virtual string keywordIconId => this.keywordId;

    ///
    public virtual void OnStartBattle()
    {
    }

    ///
    public virtual void BeforeRollDice(BattleDiceBehavior behavior)
    {
    }

    ///
    public virtual void OnRollDice(BattleDiceBehavior behavior)
    {
    }

    ///
    public virtual void BeforeGiveDamage(BattleDiceBehavior behavior)
    {
    }

    ///
    public virtual void OnSucceedAttack(BattleDiceBehavior behavior)
    {
    }

    ///
    public virtual void OnWinParrying(BattleDiceBehavior behavior)
    {
    }

    ///
    public virtual void OnLoseParrying(BattleDiceBehavior behavior)
    {
    }

    ///
    public virtual void OnDrawParrying(BattleDiceBehavior behavior)
    {
    }

    ///
    public virtual void OnUseCard()
    {
    }

    ///
    public virtual void OnStartParrying()
    {
    }

    ///
    public virtual void OnStartOneSideAction()
    {
    }

    ///
    public virtual void OnRoundEnd()
    {
    }

    ///
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

    ///
    public void Destroy()
    {
        Destroyed = true;
    }

    ///
    public int Index = -1;

    ///
    public BattlePlayingCardDataInUnitModel? equipped;

    ///
    public bool Destroyed { get; private set; }

    private bool _bInit;

    private Sprite? _bufIcon;

    internal SpeedDiceBufUI? ui;
}
