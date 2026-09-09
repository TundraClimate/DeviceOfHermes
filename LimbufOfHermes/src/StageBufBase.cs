namespace LimbufOfHermes;

/// <summary>A base of StageBuf</summary>
public class StageBufBase : BattleUnitBuf
{
    /// <summary>On consumed</summary>
    public virtual void OnConsume(int stack, BattleUnitModel? consume)
    {
    }

    internal virtual void AddBuf(int stack)
    {
    }
}
