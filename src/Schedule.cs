using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>In battle scheduler</summary>
/// <remarks>
/// Scheduler ovserves some events then hooks.<br/>
/// Event handlers can adds to scheduler with <see cref="AddSchedule"/>.
/// Scheduler lives on game running.
/// </remarks>
/// <example><code>
/// ScheduleRunner.AddSchedule(ScheduleTiming.RoundStart, () => { Hermes.Say("Round started") });
/// </code></example>
public static class ScheduleRunner
{
    static ScheduleRunner()
    {
        Harmony harmony = new Harmony("DeviceOfHermes.Schedule");

        harmony.CreateClassProcessor(typeof(SchedulePatch_OnRoundStart)).Patch();
        harmony.CreateClassProcessor(typeof(SchedulePatch_OnRollSpeedDice)).Patch();
        harmony.CreateClassProcessor(typeof(SchedulePatch_OnStartBattle)).Patch();
        harmony.CreateClassProcessor(typeof(SchedulePatch_OnUseCard)).Patch();
        harmony.CreateClassProcessor(typeof(SchedulePatch_OnRoundEnd)).Patch();
    }

    /// <summary>Add schedule handler for scheduler</summary>
    /// <param name="time">The handling timing</param>
    /// <param name="schedule">A handler</param>
    /// <example><code>
    /// ScheduleRunner.AddSchedule(ScheduleTiming.RoundStart, () => { Hermes.Say("Round started") });
    /// </code></example>
    public static void AddSchedule(ScheduleTiming time, Action schedule)
    {
        if (_schedule.ContainsKey(time))
        {
            _schedule[time] += schedule;
        }
        else
        {
            _schedule.Add(time, schedule);
        }
    }

    private static void InvokeSchedule(ScheduleTiming time)
    {
        if (_schedule.TryGetValue(time, out var action))
        {
            action.Invoke();
        }
    }

    private readonly static Dictionary<ScheduleTiming, Action> _schedule = new();

    [HarmonyPatch(typeof(StageController), "RoundStartPhase_System")]
    private static class SchedulePatch_OnRoundStart
    {
        private static void Prefix(bool ____bCalledRoundStart_system)
        {
            if (!____bCalledRoundStart_system)
            {
                ScheduleRunner.InvokeSchedule(ScheduleTiming.RoundStart);
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "SortUnitPhase")]
    private static class SchedulePatch_OnRollSpeedDice
    {
        private static void Prefix()
        {
            ScheduleRunner.InvokeSchedule(ScheduleTiming.RollSpeedDice);
        }
    }

    [HarmonyPatch(typeof(StageController), "ArrangeCardsPhase")]
    private static class SchedulePatch_OnStartBattle
    {
        private static void Prefix()
        {
            ScheduleRunner.InvokeSchedule(ScheduleTiming.StartBattle);
        }
    }

    [HarmonyPatch(typeof(StageController), "SetCurrentDiceActionPhase")]
    private static class SchedulePatch_OnUseCard
    {
        private static void Prefix(List<BattlePlayingCardDataInUnitModel> ____allCardList)
        {
            if (____allCardList.Count > 0)
            {
                ScheduleRunner.InvokeSchedule(ScheduleTiming.UseCard);
            }
        }

        private static void Postfix(StageController.StagePhase ____phase)
        {
            if (____phase == StageController.StagePhase.RoundEndPhase)
            {
                ScheduleRunner.InvokeSchedule(ScheduleTiming.EndBattle);
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "RoundEndPhase_ExpandMap")]
    private static class SchedulePatch_OnRoundEnd
    {
        private static void Prefix()
        {
            ScheduleRunner.InvokeSchedule(ScheduleTiming.RoundEnd);
        }
    }
}

/// <summary>A timing of scheduled</summary>
public enum ScheduleTiming
{
    /// <summary>RoundStart</summary>
    RoundStart,

    /// <summary>RollSpeedDice</summary>
    RollSpeedDice,

    /// <summary>StartBattle</summary>
    StartBattle,

    /// <summary>UseCard</summary>
    UseCard,

    /// <summary>EndBattle</summary>
    EndBattle,

    /// <summary>RoundEnd</summary>
    RoundEnd,
}
