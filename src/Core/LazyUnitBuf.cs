namespace DeviceOfHermes;

/// <summary>Activates on lazy</summary>
public class LazyUnitBuf(int remain) : BattleUnitBuf
{
    /// <summary>On activated</summary>
    public Action<BattleUnitModel> onActivate = _ => { };

    /// <summary>Impl OnRoundStart</summary>
    public override void OnRoundStart()
    {
        remainder.Lose();

        if (!remainder.Remains)
        {
            onActivate(this._owner);

            Destroy();
        }
    }

    Remainder remainder = new(remain);
}
