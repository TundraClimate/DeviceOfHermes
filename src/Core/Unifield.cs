namespace DeviceOfHermes;

/// <summary>A field the unique</summary>
public class Unifield
{
    internal static void Init()
    {
        StageEvents.BeforeRoundStart += () => fields.ForEach(f => f.BeforeRoundStart());
        StageEvents.OnRoundStart += () => fields.ForEach(f => f.OnRoundStart());
        StageEvents.BeforeRoundEnd += () => fields.ForEach(f => f.BeforeRoundEnd());
        StageEvents.OnRoundEnd += () => fields.ForEach(f => f.OnRoundEnd());
        StageEvents.OnUseCard += card => fields.ForEach(f => f.OnUseCard(card));
        StageEvents.BeforeRollDice += dice => fields.ForEach(f => f.BeforeRollDice(dice));
        StageEvents.OnCleanup += OnCleanup;
    }

    /// <summary>Activates field</summary>
    public static void ActivateField<T>(T field)
        where T : Unifield
    {
        field.OnActivate();

        if (fields.IndexOf(f => f is T) is int idx && idx > -1)
        {
            Mem.Replace(ref fields.AsRef()[idx]!, field);

            return;
        }

        field.OnEnable();

        fields.Add(field);
    }

    /// <summary>Deactivates field</summary>
    public static void DeactivateField<T>()
        where T : Unifield
    {
        var find = fields.Find(f => f is T);

        find?.Cleanup();
        find?.Let(f => fields.Remove(f));
    }

    /// <summary>Runs on activated</summary>
    public virtual void OnActivate()
    {
    }

    /// <summary>Runs on enabled</summary>
    public virtual void OnEnable()
    {
    }

    /// <summary>Runs before round start</summary>
    public virtual void BeforeRoundStart()
    {
    }

    /// <summary>Runs on round start</summary>
    public virtual void OnRoundStart()
    {
    }

    /// <summary>Runs before round end</summary>
    public virtual void BeforeRoundEnd()
    {
    }

    /// <summary>Runs on round end</summary>
    public virtual void OnRoundEnd()
    {
    }

    /// <summary>Runs on card used</summary>
    public virtual void OnUseCard(BattlePlayingCardDataInUnitModel card)
    {
    }

    /// <summary>Runs before roll dice</summary>
    public virtual void BeforeRollDice(BattleDiceBehavior dice)
    {
    }

    /// <summary>Execute cleanup process</summary>
    public virtual void Cleanup()
    {
    }

    static void OnCleanup()
    {
        fields.ForEach(f => f.Cleanup());

        fields.Clear();
    }

    static List<Unifield> fields = new();
}
