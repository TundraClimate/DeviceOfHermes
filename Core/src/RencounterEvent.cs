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
            RencounterEvent.Rolldice => owner.battleCardResultLog.SetRolldiceEvent,
            RencounterEvent.AfterAction => owner.battleCardResultLog.SetAfterActionEvent,
            RencounterEvent.TakeDamaged => owner.battleCardResultLog.SetTakeDamagedEvent,
            RencounterEvent.UseCard => owner.battleCardResultLog.SetUseCardEvent,
            RencounterEvent.EndCardAction => owner.battleCardResultLog.SetEndCardActionEvent,
            RencounterEvent.PrintEffect => owner.battleCardResultLog.SetPrintEffectEvent,
            RencounterEvent.PrintDamagedEffect => owner.battleCardResultLog.SetPrintDamagedEffectEvent,
            _ => owner.battleCardResultLog.SetPrintEffectEvent,
        };

        f(new BattleCardBehaviourResult.BehaviourEvent(action));
    }
}
