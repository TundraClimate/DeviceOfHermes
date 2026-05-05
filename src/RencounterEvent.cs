namespace DeviceOfHermes;

/// <summary>An enum of events on rencounter</summary>
public enum RencounterEvent
{
    /// <summary>SucceedAtk</summary>
    SucceedAtk,

    /// <summary>Rolldice</summary>
    Rolldice,

    /// <summary>AfterAction</summary>
    AfterAction,

    /// <summary>TakeDamaged</summary>
    TakeDamaged,

    /// <summary>UseCard</summary>
    UseCard,

    /// <summary>EndCardAction</summary>
    EndCardAction,

    /// <summary>PrintEffect</summary>
    PrintEffect,

    /// <summary>PrintDamagedEffect</summary>
    PrintDamagedEffect,
}

/// <summary>An extension of events on rencounter</summary>
public static class RencounterEventExtension
{
    /// <summary>Add an action to current rencounter result</summary>
    public static void AddRencounterEvent(this BattleUnitModel? owner, RencounterEvent e, Action action)
    {
        if (owner?.battleCardResultLog is null)
        {
            return;
        }

        Action<BattleCardBehaviourResult.BehaviourEvent> f = e switch
        {
            RencounterEvent.SucceedAtk => owner.battleCardResultLog.SetSucceedAtkEvent,
            RencounterEvent.Rolldice => owner.battleCardResultLog.SetSucceedAtkEvent,
            RencounterEvent.AfterAction => owner.battleCardResultLog.SetSucceedAtkEvent,
            RencounterEvent.TakeDamaged => owner.battleCardResultLog.SetSucceedAtkEvent,
            RencounterEvent.UseCard => owner.battleCardResultLog.SetSucceedAtkEvent,
            RencounterEvent.EndCardAction => owner.battleCardResultLog.SetSucceedAtkEvent,
            RencounterEvent.PrintEffect => owner.battleCardResultLog.SetSucceedAtkEvent,
            RencounterEvent.PrintDamagedEffect => owner.battleCardResultLog.SetSucceedAtkEvent,
            _ => owner.battleCardResultLog.SetSucceedDefEvent,
        };

        f(new BattleCardBehaviourResult.BehaviourEvent(action));
    }
}
