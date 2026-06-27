namespace DeviceOfHermes.AdvancedBase;

/// <summary>Shimmering passive base</summary>
public abstract class ShimPassiveBase : AdvancedPassiveBase
{
    /// <summary>Construct with initializer</summary>
    public ShimPassiveBase()
    {
        Init();
    }

    /// <summary>Init shimmering passive</summary>
    public abstract void Init();

    /// <summary>Applies hand pattern</summary>
    public abstract void ApplyPattern();

    /// <summary>Clear all cards, gain full light</summary>
    public void Shimmering()
    {
        this.owner.allyCardDetail.ExhaustAllCards();
        this.owner.cardSlotDetail.RecoverPlayPoint(base.owner.cardSlotDetail.GetMaxPlayPoint());
    }

    /// <summary>Adds temp card to hand</summary>
    public void AddCard(int id, bool vannila = false)
    {
        var lid = vannila ? new LorId(id) : new LorId(PackageId, id);

        var card = this.owner.allyCardDetail.AddTempCard(lid);

        card?.SetCostToZero();
    }

    private void ResolvePriorities()
    {
        var hands = this.owner?.allyCardDetail?.GetHand() ?? new();
        var z = 2 << 12;

        foreach (var (i, card) in hands.Enumerate())
        {
            card.SetPriorityAdder(z - i * 11);
        }
    }

    /// <summary>Processes on turn start</summary>
    public void OnStartTurn()
    {
        Shimmering();

        ApplyPattern();

        ResolvePriorities();

        Elapsed += 1;
    }

    /// <summary>Override</summary>
    public override void OnRoundStartAfter()
    {
        OnStartTurn();
    }

    /// <summary>PackageId setter</summary>
    public string PackageId { private get; set; } = "";

    /// <summary>A turn of elapsed</summary>
    public int Elapsed { get; set; }
}
