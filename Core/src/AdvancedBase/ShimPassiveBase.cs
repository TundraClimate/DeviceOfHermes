using System.Collections;

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

        AddCard(lid);
    }

    /// <summary>Adds temp card to hand</summary>
    public void AddCard(LorId id)
    {
        var card = this.owner.allyCardDetail.AddTempCard(id);

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

        if (!base.owner.IsBreakLifeZero())
        {
            ApplyPattern();

            ResolvePriorities();
        }

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
    public int Elapsed { get; private set; }

    /// <summary>A list of patterns</summary>
    protected virtual PatternList Patterns { get; } = new();

    /// <summary>A list of pattern</summary>
    protected class PatternList(bool loop = false) : IEnumerable<PatternInfo>
    {
        /// <summary>Applies pattern and next</summary>
        public void ApplyNextPattern(ShimPassiveBase self)
        {
            if (_value.Count == 0)
            {
                return;
            }

            _index += 1;

            if (_index >= 0 && _value.Count > _index)
            {
                _value[_index].ApplyPattern(self);
            }
            else if (_loop)
            {
                _index = -1;

                ApplyNextPattern(self);
            }
        }

        /// <summary>Adds element</summary>
        public void Add(PatternInfo element)
        {
            _value.Add(element);
        }

        /// <summary>Impls GetEnumerator</summary>
        public IEnumerator<PatternInfo> GetEnumerator() => _value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _value.GetEnumerator();

        private List<PatternInfo> _value = new();

        private int _index = -1;

        private bool _loop = loop;
    }

    /// <summary>A information of pattern</summary>
    protected class PatternInfo(params List<object> ids)
    {
        internal void ApplyPattern(ShimPassiveBase self)
        {
            var pid = self.PackageId;
            var ids = _ids.Filter(i => i is int or LorId);

            foreach (var id in ids)
            {
                if (id is int num)
                {
                    self.AddCard(num);
                }
                else if (id is LorId lid)
                {
                    self.AddCard(lid);
                }
            }

            var count = ids.Count();
            var max = self.owner.Book.GetSpeedDiceRule(self.owner).Roll(self.owner).Count;

            for (var i = count; max > i; i++)
            {
                filler?.Let(f => self.AddCard(f.fn(i)));
            }
        }

        /// <summary>A filler</summary>
        public Filler? filler = null;

        private List<object> _ids = ids;
    }

    /// <summary>A filler</summary>
    protected class Filler(Func<int, LorId> fn)
    {
        /// <summary>All uses id</summary>
        public static Filler All(LorId id) => new(_ => id);

        /// <summary>Constatnt uses id</summary>
        public static Filler Const(params List<LorId> ids)
        {
            var q = new Queue<LorId>(ids);

            return new(_ => q.Dequeue());
        }

        internal Func<int, LorId> fn = fn;
    }
}
