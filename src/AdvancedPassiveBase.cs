using System.Runtime.CompilerServices;

namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced <see cref="PassiveAbilityBase"/></summary>
/// <remarks>
/// Can replaces <c>PassiveAbilityBase</c> into this.
/// </remarks>
/// <example><code>
/// public class PassiveAbility_Advance : AdvancedPassiveBase
/// {
/// }
/// </code></example>
public class AdvancedPassiveBase : PassiveAbilityBase
{
    static AdvancedPassiveBase()
    {
        AdvancedPatch.Init();

        BattleTickAction.OnTick += OnTick;
    }

    /// <summary>Unit on round start before <see cref="PassiveAbilityBase.OnRoundStart"/></summary>
    public virtual void OnRoundStartFirst()
    {
    }

    /// <summary>Unit on round start after <see cref="PassiveAbilityBase.OnRoundStartAfter"/></summary>
    public virtual void OnRoundStartLast()
    {
    }

    /// <summary>Unit on activate bufs</summary>
    public virtual void OnActivatedBuf(BattleUnitBuf activate)
    {
    }

    /// <summary>Unit on change buf stack</summary>
    public virtual void OnChangeBufStack(BattleUnitBuf changed, int last)
    {
    }

    /// <summary>Is <paramref name="card"/> is clashable</summary>
    /// <param name="card">A card of set slot</param>
    /// <returns>Is <paramref name="card"/> is clashable if true</returns>
    public virtual bool IsClashable(BattlePlayingCardDataInUnitModel card)
    {
        return true;
    }

    /// <summary>Is <paramref name="self"/> is clashable with <paramref name="target"/></summary>
    /// <param name="self">A card of set slot</param>
    /// <param name="target">A card of targeted</param>
    /// <returns>Is <paramref name="self"/> is clashable with <paramref name="target"/> if true</returns>
    public virtual bool IsClashable(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
    {
        return true;
    }

    /// <summary>Is <paramref name="self"/> is ignore speed by match with <paramref name="target"/></summary>
    /// <param name="self">A card of set slot</param>
    /// <param name="target">A card of targeted</param>
    /// <returns>Is <paramref name="self"/> is ignore speed by match with <paramref name="target"/> if true</returns>
    public virtual bool IsIgnoreSpeedByMatch(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
    {
        return false;
    }

    /// <summary>Is allows round end</summary>
    /// <returns>Is allows if true</returns>
    public virtual bool IsAllowRoundEnd()
    {
        return true;
    }

    /// <summary>Can discard this <paramref name="card"/> by ability</summary>
    /// <param name="card">A card of discard</param>
    /// <returns>Is card can discard</returns>
    public virtual bool CanDiscardByAbility(BattleDiceCardModel card)
    {
        return true;
    }

    /// <summary>On dropped card</summary>
    /// <param name="playcard">A card of dropped</param>
    public virtual void OnDropCard(BattlePlayingCardDataInUnitModel playcard)
    {
    }

    static void OnTick()
    {
        var alives = BattleObjectManager.instance.GetAliveList();

        foreach (var unit in alives)
        {
            var passives = unit.passiveDetail.PassiveList.FilterMap(p => p is AdvancedPassiveBase ap ? ap : null).ToList();

            if (passives.Count == 0)
            {
                continue;
            }

            List<BattleUnitBuf> activatedBufs = new();

            var lastBufs = _lastBufList.GetValue(unit, _ => new());
            var actives = unit.bufListDetail.GetActivatedBufList();

            foreach (var buf in actives)
            {
                if (!lastBufs.Contains(buf))
                {
                    activatedBufs.Add(buf);
                }

                var lastStack = _lastBufStack.GetValue(buf, _ => new());

                if (buf.stack != lastStack.value)
                {
                    foreach (var p in passives)
                    {
                        p?.OnChangeBufStack(buf, lastStack.value);
                    }

                    lastStack.value = buf.stack;
                }
            }

            foreach (var p in passives)
            {
                foreach (var buf in activatedBufs)
                {
                    p?.OnActivatedBuf(buf);
                }
            }

            if (activatedBufs.Count != 0)
            {
                lastBufs.Clear();
                lastBufs.AddRange(actives);
            }
        }
    }

    private static ConditionalWeakTable<BattleUnitModel, List<BattleUnitBuf>> _lastBufList = new();

    private static ConditionalWeakTable<BattleUnitBuf, BufStack> _lastBufStack = new();

    private class BufStack
    {
        public int value = -1;
    }
}
