namespace LimbufOfHermes;

/// <summary>A root of interfaces</summary>
public static class ILimbuf
{
    /// <summary>Interface</summary>
    public interface OnRuptured
    {
        /// <summary>On rupture activated</summary>
        public void OnRuptured(int value);
    }

    /// <summary>Interface</summary>
    public interface OnApplyTremorBurst
    {
        /// <summary>On applies tremor burst</summary>
        public void OnApplyTremorBurst();
    }

    /// <summary>Interface</summary>
    public interface OnTremorBurst
    {
        /// <summary>On tremor burst</summary>
        public void OnTremorBurst(int value);
    }

    /// <summary>Interface</summary>
    public interface OnTremorConversion
    {
        /// <summary>On tremor burst</summary>
        public void OnTremorConversion();
    }

    /// <summary>Interface</summary>
    public interface OnTremorEntangle
    {
        /// <summary>On tremor burst</summary>
        public void OnTremorEntangle();
    }

    /// <summary>Interface</summary>
    public interface OnPanic
    {
        /// <summary>On panic</summary>
        public void OnPanic();
    }

    /// <summary>Interface</summary>
    public interface OnRollCritical
    {
        /// <summary>On panic</summary>
        public void OnRollCritical(BattleDiceBehavior behavior);
    }

    /// <summary>Interface</summary>
    public interface OnCriticalAttack
    {
        /// <summary>On panic</summary>
        public void OnCriticalAttack(BattleDiceBehavior behavior);
    }

    internal static void EachPassiveOf<T>(this BattleUnitModel owner, Action<T> f)
    {
        owner.passiveDetail?.PassiveList?.Filter(passive => passive.isActiavted)?.OfType<T>().Foreach(f);
    }

    internal static void EachUnitBufOf<T>(this BattleUnitModel owner, Action<T> f)
    {
        owner.bufListDetail?.GetActivatedBufList()?.Filter(buf => !buf.IsDestroyed())?.OfType<T>()?.Foreach(f);
    }
}
