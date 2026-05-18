using System.Runtime.CompilerServices;
using HarmonyLib;

namespace DeviceOfHermes;

///
public class SpeedDiceBufDetail
{
    static SpeedDiceBufDetail()
    {
        var harmony = new Harmony("DeviceOfHermes.SpeedDiceBuf");

        harmony.CreateClassProcessor(typeof(PatchOnStartBattle)).Patch();
        harmony.CreateClassProcessor(typeof(PatchBeforeRollDice)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnRollDice)).Patch();
        harmony.CreateClassProcessor(typeof(PatchBeforeGiveDamage)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnSucceedAttack)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnWinParrying)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnLoseParrying)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnDrawParrying)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnUseCard)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnStartParrying)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnStartOneSideAction)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnRoundEnd)).Patch();
    }

    ///
    public List<SpeedDiceBuf> ActivatedBufs => _activatedBufs.Map(bufs => bufs.Value).Flatten().ToList();

    ///
    public void AddBuf(int idx, SpeedDiceBuf buf)
    {
        if (owner is null || idx >= owner.view.speedDiceSetterUI.SpeedDicesCount)
        {
            return;
        }

        if (!_activatedBufs.TryAdd(idx, [buf]))
        {
            _activatedBufs[idx].Add(buf);
        }

        buf.Index = idx;

        UpdateActivatedBufs();
    }

    ///
    public void RemoveBuf(SpeedDiceBuf buf)
    {
        foreach (var (_, bufs) in _activatedBufs)
        {
            bufs.Remove(buf);
        }

        UpdateActivatedBufs();
    }

    ///
    public void RemoveAll(Predicate<SpeedDiceBuf> matcher)
    {
        foreach (var (_, bufs) in _activatedBufs)
        {
            bufs.RemoveAll(matcher);
        }

        UpdateActivatedBufs();
    }

    private void OnStartBattle()
    {
        for (var i = 0; owner?.view?.speedDiceSetterUI?.SpeedDicesCount > i; i++)
        {
            var equipped = owner?.view?.speedDiceSetterUI?.GetSpeedDiceByIndex(i)?.CardInDice;

            if (equipped is not null && _activatedBufs.TryGetValue(i, out var bufs))
            {
                foreach (var buf in bufs)
                {
                    buf.equipped = equipped;

                    buf.OnStartBattle();
                }
            }
        }

        UpdateActivatedBufs();
    }

    private void BeforeRollDice(BattleDiceBehavior behavior)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == behavior.card)
            {
                buf.BeforeRollDice(behavior);
            }
        }
    }

    private void OnRollDice(BattleDiceBehavior behavior)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == behavior.card)
            {
                buf.OnRollDice(behavior);
            }
        }
    }

    private void BeforeGiveDamage(BattleDiceBehavior behavior)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == behavior.card)
            {
                buf.BeforeGiveDamage(behavior);
            }
        }
    }

    private void OnSucceedAttack(BattleDiceBehavior behavior)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == behavior.card)
            {
                buf.OnSucceedAttack(behavior);
            }
        }
    }

    private void OnWinParrying(BattleDiceBehavior behavior)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == behavior.card)
            {
                buf.OnWinParrying(behavior);
            }
        }
    }

    private void OnLoseParrying(BattleDiceBehavior behavior)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == behavior.card)
            {
                buf.OnLoseParrying(behavior);
            }
        }
    }

    private void OnDrawParrying(BattleDiceBehavior behavior)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == behavior.card)
            {
                buf.OnDrawParrying(behavior);
            }
        }
    }

    private void OnUseCard(BattlePlayingCardDataInUnitModel card)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == card)
            {
                buf.OnUseCard();
            }
        }
    }

    private void OnStartParrying(BattlePlayingCardDataInUnitModel card)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == card)
            {
                buf.OnStartParrying();
            }
        }
    }

    private void OnStartOneSideAction(BattlePlayingCardDataInUnitModel card)
    {
        foreach (var buf in ActivatedBufs)
        {
            if (buf.equipped == card)
            {
                buf.OnStartOneSideAction();
            }
        }
    }

    private void OnRoundEnd()
    {
        foreach (var buf in ActivatedBufs)
        {
            buf.OnRoundEnd();
        }

        UpdateActivatedBufs();
    }

    private void UpdateActivatedBufs()
    {
        foreach (var (i, bufs) in _activatedBufs)
        {
            UpdateActivatedBuf(i, bufs);
        }

        CleanupInvalids();
    }

    private void UpdateActivatedBuf(int idx, List<SpeedDiceBuf> bufs)
    {
        var speedDice = owner?.view?.speedDiceSetterUI?.GetSpeedDiceByIndex(idx);
        var go = speedDice?.gameObject;

        if (go is null)
        {
            return;
        }

        var diceUIs = go.GetComponentsInChildren<SpeedDiceBufUI>(true).ToList();

        foreach (var buf in bufs)
        {
            if (diceUIs.Exists(ui => ui.buf == buf))
            {
                continue;
            }

            var ui = go.AddChildObject($"[SpeedDiceBuf] {buf.keywordId}").AddComponent<SpeedDiceBufUI>();

            ui.Init(idx, buf);
            buf.ui = ui;
        }
    }

    private void CleanupInvalids()
    {
        var diceCount = owner?.view?.speedDiceSetterUI?.SpeedDicesCount;

        if (diceCount is null)
        {
            return;
        }

        foreach (var (i, bufs) in _activatedBufs)
        {
            if (i >= diceCount)
            {
                foreach (var buf in bufs)
                {
                    buf.Destroy();
                    UnityEngine.Object.Destroy(buf.ui!.gameObject);
                }
            }
            else
            {
                foreach (var buf in bufs)
                {
                    if (buf.Destroyed)
                    {
                        UnityEngine.Object.Destroy(buf.ui!.gameObject);
                    }
                }
            }
        }

        foreach (var (_, bufs) in _activatedBufs)
        {
            bufs.RemoveAll(buf => buf.Destroyed);
        }
    }

    ///
    public BattleUnitModel? owner;

    private Dictionary<int, List<SpeedDiceBuf>> _activatedBufs = new();

    [HarmonyPatch(typeof(BattleUnitModel), "OnStartBattle")]
    class PatchOnStartBattle
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance)
        {
            try
            {
                __instance.speedDiceBufDetail.OnStartBattle();
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "BeforeRollDice")]
    class PatchBeforeRollDice
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattleDiceBehavior behavior)
        {
            try
            {
                __instance.speedDiceBufDetail.BeforeRollDice(behavior);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnRollDice")]
    class PatchOnRollDice
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattleDiceBehavior behavior)
        {
            try
            {
                __instance.speedDiceBufDetail.OnRollDice(behavior);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "BeforeGiveDamage")]
    class PatchBeforeGiveDamage
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattleDiceBehavior behavior)
        {
            try
            {
                __instance.speedDiceBufDetail.BeforeGiveDamage(behavior);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnSucceedAttack")]
    class PatchOnSucceedAttack
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattleDiceBehavior behavior)
        {
            try
            {
                __instance.speedDiceBufDetail.OnSucceedAttack(behavior);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnWinParrying")]
    class PatchOnWinParrying
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattleDiceBehavior behavior)
        {
            try
            {
                __instance.speedDiceBufDetail.OnWinParrying(behavior);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnLoseParrying")]
    class PatchOnLoseParrying
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattleDiceBehavior behavior)
        {
            try
            {
                __instance.speedDiceBufDetail.OnLoseParrying(behavior);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnDrawParrying")]
    class PatchOnDrawParrying
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattleDiceBehavior behavior)
        {
            try
            {
                __instance.speedDiceBufDetail.OnDrawParrying(behavior);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnUseCard")]
    class PatchOnUseCard
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattlePlayingCardDataInUnitModel card)
        {
            try
            {
                __instance.speedDiceBufDetail.OnUseCard(card);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnParryingStart")]
    class PatchOnStartParrying
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattlePlayingCardDataInUnitModel card)
        {
            try
            {
                __instance.speedDiceBufDetail.OnStartParrying(card);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnStartOneSideAction")]
    class PatchOnStartOneSideAction
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattlePlayingCardDataInUnitModel curCard)
        {
            try
            {
                __instance.speedDiceBufDetail.OnStartOneSideAction(curCard);
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnRoundEnd")]
    class PatchOnRoundEnd
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance)
        {
            try
            {
                __instance.speedDiceBufDetail.OnRoundEnd();
            }
            catch (Exception ex)
            {
                Hermes.Say(ex.ToString(), MessageLevel.Error);
            }

            return __exception;
        }
    }
}

///
public static class SpeedDiceBufDetailExt
{
    extension(BattleUnitModel owner)
    {
        ///
        public SpeedDiceBufDetail speedDiceBufDetail
        {
            get
            {
                return _details.GetValue(owner, _ => new SpeedDiceBufDetail()
                {
                    owner = owner,
                });
            }
        }

        ///
        public SpeedDiceBufDetail GetSpeedDiceBufDetail() => owner.speedDiceBufDetail;
    }

    private static ConditionalWeakTable<BattleUnitModel, SpeedDiceBufDetail> _details = new();
}
