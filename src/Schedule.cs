using HarmonyLib;

namespace DeviceOfHermes;

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

public enum ScheduleTiming
{
    RoundStart,
    RollSpeedDice,
    StartBattle,
    UseCard,
    EndBattle,
    RoundEnd,
}
