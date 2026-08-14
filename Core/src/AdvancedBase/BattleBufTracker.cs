namespace DeviceOfHermes.AdvancedBase;

internal static class BattleBufTracker
{
    static BattleBufTracker()
    {
        BattleTickAction.OnTick += OnTick;
    }

    public static void Init()
    {
    }

    static void OnTick()
    {
        var bufs = BattleObjectManager.instance.GetAliveList()
            .FlatMap(unit => unit.bufListDetail.GetActivatedBufList())
            .Collect();

        foreach (var buf in bufs)
        {
            var data = BufData.New(buf);

            if (!_state.TryGetValue(data, out var lastStack))
            {
                _state[data] = buf.stack;

                data.owner?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Collect()?.Foreach(p =>
                {
                    p.OnChangeBufStack(buf, -1);
                    p.OnActivatedBuf(buf);
                    p.OnAddBuf(buf, buf.stack);
                });

                data.owner?.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Collect()?.Foreach(b =>
                {
                    b.OnStackChangeAll(buf, -1);
                    b.OnAddBufAll(buf, buf.stack);
                });

                if (buf is AdvancedUnitBuf adv)
                {
                    adv.OnStackChange(-1);
                    adv.OnAddBuf(buf.stack);
                }

                continue;
            }

            if (lastStack == buf.stack)
            {
                continue;
            }

            _state[data] = buf.stack;

            var isAdd = buf.stack > lastStack;

            data.owner?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Collect()?.Foreach(p =>
            {
                p.OnChangeBufStack(buf, lastStack);

                if (isAdd)
                {
                    p.OnAddBuf(buf, buf.stack);
                }
            });

            data.owner?.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Collect()?.Foreach(b =>
            {
                b.OnStackChangeAll(buf, lastStack);

                if (isAdd)
                {
                    b.OnAddBufAll(buf, buf.stack);
                }
            });

            if (buf is AdvancedUnitBuf advBuf)
            {
                advBuf.OnStackChange(lastStack);

                if (isAdd)
                {
                    advBuf.OnAddBuf(buf.stack);
                }
            }
        }

        HashSet<BattleUnitBuf> set = new(bufs);
        List<BufData> dropped = new();

        foreach (var key in _state.Keys)
        {
            if (!set.Contains(key.buf))
            {
                dropped.Add(key);
            }
        }

        foreach (var rm in dropped)
        {
            _state.Remove(rm);
        }
    }

    private static Dictionary<BufData, int> _state = new();

    private struct BufData
    {
        public BattleUnitBuf buf;

        public BattleUnitModel owner;

        public static BufData New(BattleUnitBuf buf)
        {
            return new BufData() { buf = buf, owner = buf.Owner };
        }
    }
}
