namespace DeviceOfHermes;

/// <summary>A base class of ChainBattle</summary>
public class ChainBattleStageManager : EnemyTeamStageManager
{
    /// <summary>A flag that enable chain battle</summary>
    public virtual bool IsChainBattle => true;

    /// <summary>A flag that enable supplies enemy</summary>
    public virtual bool IsSuppliesEnemy => true;

    /// <summary>A flag that remove sephirah</summary>
    public virtual bool IsSephirahRemove => true;

    /// <summary>A number of maximum rear floor</summary>
    public virtual int MaximumRearFloorNum => 1;

    /// <summary>Runs on unit arrived</summary>
    public virtual void OnArrived(BattleUnitModel unit)
    {
    }

    /// <summary>Runs on unit retreated</summary>
    public virtual void OnRetreat(BattleUnitModel unit)
    {
    }

    /// <summary>Runs on unit dead</summary>
    public virtual void OnDead(BattleUnitModel unit)
    {
    }

    /// <summary>Returns user selected floors</summary>
    public static List<SephirahType> GetSelectedFloors()
    {
        return new(TmpSelectedFloors);
    }

    internal static List<SephirahType> TmpSelectedFloors = new();
}
