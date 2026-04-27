using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes;

/// <summary>A behaviour for BattleManagerUI</summary>
public class BattleUIBehaviour : MonoBehaviour
{
    internal static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.BattleUIBehaviour");

        harmony.CreateClassProcessor(typeof(PatchOnRoundStart)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnStartBattle)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnetimeInvoke)).Patch();
    }

    /// <summary>Runs on Round start</summary>
    public virtual void OnRoundStart()
    {
    }

    /// <summary>Runs on Start battle</summary>
    public virtual void OnStartBattle()
    {
    }

    internal static ConditionalWeakTable<BattleManagerUI, Dictionary<string, MonoBehaviour>> AddionalUI = new();

    internal static Dictionary<string, Type> Stored = new();

    [HarmonyPatch(typeof(StageController), "RoundStartPhase_UI")]
    class PatchOnRoundStart
    {
        static void Prefix(bool ____bRoundStarted)
        {
            if (SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.IsRunningEffect)
            {
                return;
            }

            if (!____bRoundStarted)
            {
                foreach (var beh in AddionalUI.GetValue(BattleManagerUI.Instance, _ => new()).Values.OfType<BattleUIBehaviour>())
                {
                    beh.OnRoundStart();
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "CompleteApplyingLibrarianCardPhase")]
    class PatchOnStartBattle
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = typeof(BattleUnitCardsInHandUI).Method("Deactivate");
            var inject = typeof(PatchOnStartBattle).Method("InjectMethod");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.Calls(target))
                .Insert(new CodeInstruction(OpCodes.Call, inject));

            return matcher.Instructions();
        }

        static void InjectMethod()
        {
            foreach (var beh in AddionalUI.GetValue(BattleManagerUI.Instance, _ => new()).Values.OfType<BattleUIBehaviour>())
            {
                beh.OnStartBattle();
            }
        }
    }

    [HarmonyPatch(typeof(GameSceneManager), "Start")]
    class PatchOnetimeInvoke
    {
        static void Postfix()
        {
            var val = BattleUIBehaviour.AddionalUI.GetValue(BattleManagerUI.Instance, _ => new());

            foreach (var (name, ty) in Stored)
            {
                if (!val.ContainsKey(name))
                {
                    var go = new GameObject(name);

                    go.transform.SetParent(BattleManagerUI.Instance.gameObject.transform);

                    var behaviour = (MonoBehaviour)go.AddComponent(ty);

                    val.Add(name, behaviour);
                }
            }
        }
    }
}

/// <summary>The extensions of BattleManagerUI</summary>
public static class BattleUIBehaviourExtension
{
    extension(BattleManagerUI battleManagerUI)
    {
        /// <summary>Add behaviour to BattleManagerUI</summary>
        public void AddBehaviour<T>(string name)
            where T : MonoBehaviour
        {
            if (battleManagerUI is null)
            {
                BattleUIBehaviour.Stored.Add(name, typeof(T));

                return;
            }

            var val = BattleUIBehaviour.AddionalUI.GetValue(battleManagerUI, _ => new());

            if (!val.ContainsKey(name))
            {
                var go = new GameObject(name);

                go.transform.SetParent(battleManagerUI.gameObject.transform);

                var behaviour = go.AddComponent<T>();

                val.Add(name, behaviour);
            }
        }

        /// <summary>Get behaviour from BattleManagerUI</summary>
        public T? GetBehaviour<T>(string name)
            where T : MonoBehaviour
        {
            if (battleManagerUI is null)
            {
                Hermes.Say("BattleManagerUI is not initialized");

                return null;
            }

            if (BattleUIBehaviour.AddionalUI.GetValue(BattleManagerUI.Instance, _ => new()).TryGetValue(name, out var res))
            {
                return res as T;
            }

            return null;
        }
    }
}
