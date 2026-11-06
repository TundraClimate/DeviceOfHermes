using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>A set of Stage events</summary>
public static class StageEvents
{
    static StageEvents()
    {
        var harmony = new Harmony("DeviceOfHermes.StageEvents");

        harmony.CreateClassProcessor(typeof(PatchOnWaveStart)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnRoundStart)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnStopSpeedDice)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnStartBattle)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnRoundEnd)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnUseCard)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnClearResource)).Patch();
    }

    /// <summary>An event before wave start</summary>
    public static event Action BeforeWaveStart = () => { };

    /// <summary>An event on wave start</summary>
    public static event Action OnWaveStart = () => { };

    /// <summary>An event before round start</summary>
    public static event Action BeforeRoundStart = () => { };

    /// <summary>An event on round start</summary>
    public static event Action OnRoundStart = () => { };

    /// <summary>An event on stop speeddice</summary>
    public static event Action OnStopSpeedDice = () => { };

    /// <summary>An event before start battle</summary>
    public static event Action BeforeStartBattle = () => { };

    /// <summary>An event on start battle</summary>
    public static event Action OnStartBattle = () => { };

    /// <summary>An event before round end</summary>
    public static event Action BeforeRoundEnd = () => { };

    /// <summary>An event on round end</summary>
    public static event Action OnRoundEnd = () => { };

    /// <summary>An event on use card</summary>
    public static event Action<BattlePlayingCardDataInUnitModel> OnUseCard = _ => { };

    /// <summary>An event on cleanup</summary>
    public static event Action OnCleanup = () => { };

    [HarmonyPatch(typeof(StageController), "StartBattle")]
    class PatchOnWaveStart
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdarg(0),
                CodeMatch.IsLdfld(typeof(StageController).Field("_enemyStageManager")),
                CodeMatch.Calls(typeof(EnemyTeamStageManager).Method("OnWaveStart"))
            )
                .Insert(CodeInstruction.Call(typeof(PatchOnWaveStart).Method("InjectBefore")));

            matcher.MatchStartForward(
                CodeMatch.Calls(typeof(PlatformManager).Method("get_Instance")),
                CodeMatch.IsConst(AchievementEnum.ONCE_BINAH),
                CodeMatch.Calls(typeof(PlatformManager).Method("AchievementUnlocked"))
            )
                .Insert(CodeInstruction.Call(typeof(PatchOnWaveStart).Method("InjectAfter")));

            return matcher.Instructions();
        }

        static void InjectBefore()
        {
            BeforeWaveStart.Invoke();
        }

        static void InjectAfter()
        {
            OnWaveStart.Invoke();
        }
    }

    [HarmonyPatch(typeof(StageController), "RoundStartPhase_System")]
    class PatchOnRoundStart
    {
        static void Prefix(bool ____bCalledRoundStart_system)
        {
            if (!____bCalledRoundStart_system)
            {
                BeforeRoundStart.Invoke();
            }
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdarg(0),
                CodeMatch.Calls(typeof(StageController).Method("CheckEndBattle"))
            )
                .Insert(CodeInstruction.Call(typeof(PatchOnRoundStart).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod()
        {
            OnRoundStart.Invoke();
        }
    }

    [HarmonyPatch(typeof(StageController), "StopSpeedDiceRoll")]
    class PatchOnStopSpeedDice
    {
        static Exception Finalizer(Exception __exception, StageController.StagePhase ____phase)
        {
            if (____phase is StageController.StagePhase.SortUnitPhase)
            {
                OnStopSpeedDice.Invoke();
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(StageController), "ActivateStartBattleEffectPhase")]
    class PatchOnStartBattle
    {
        static void Prefix()
        {
            BeforeStartBattle.Invoke();
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdarg(0),
                CodeMatch.Calls(typeof(StageController).Method("CheckRoundEnd"))
            )
                .Insert(CodeInstruction.Call(typeof(PatchOnStartBattle).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod()
        {
            OnStartBattle.Invoke();
        }
    }

    [HarmonyPatch(typeof(StageController), "RoundEndPhase_ExpandMap")]
    static class PatchOnRoundEnd
    {
        static void Prefix()
        {
            if (!(BattleSceneRoot.Instance?.currentMapObject?.IsRunningEffect == true))
            {
                BeforeRoundEnd.Invoke();
            }
        }

        static Exception Finalizer(Exception __exception, bool __result)
        {
            if (__result)
            {
                OnRoundEnd.Invoke();
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattlePlayingCardDataInUnitModel), "OnUseCard")]
    class PatchOnUseCard
    {
        static Exception Finalizer(Exception __exception, BattlePlayingCardDataInUnitModel __instance)
        {
            OnUseCard.Invoke(__instance);

            return __exception;
        }
    }

    [HarmonyPatch(typeof(StageController), "ClearResources")]
    class PatchOnClearResource
    {
        static Exception Finalizer(Exception __exception)
        {
            OnCleanup.Invoke();

            return __exception;
        }
    }
}
