namespace LimbufOfHermes;

/// <summary>A unit buf the AssistDefense marker</summary>
public class BattleUnitBuf_Limbuf_AssistDefenseCard(LorId cardId) : LimbufBase
{
    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.None;

    /// <summary>An id of specify card</summary>
    public LorId cardId = cardId;
}
