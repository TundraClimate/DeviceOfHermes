using System.Collections;

/// <summary>An advanced FarAreaEffect</summary>
public class AdvancedAreaEffect : FarAreaEffect
{
    /// <summary>Default override</summary>
    public override bool ActionPhase(float deltaTime, BattleUnitModel attacker, List<BattleFarAreaPlayManager.VictimInfo> victims, ref List<BattleFarAreaPlayManager.VictimInfo> defenseVictims)
    {
        if (!_init)
        {
            _init = true;

            _actions.Push(ActionSeq(attacker, victims));
        }

        DeltaTime = deltaTime;

        if (_elapsed >= _arriveTime)
        {
            _elapsed = 0f;
            _arriveTime = 0f;
        }
        else
        {
            _elapsed += deltaTime;

            return false;
        }

        if (_actions.TryPeek(out var top))
        {
            var res = top.MoveNext();

            if (res)
            {
                if (top.Current is float ws)
                {
                    _arriveTime = ws;
                }

                if (top.Current is IEnumerator nested)
                {
                    _actions.Push(nested);
                }
            }
            else
            {
                _actions.Pop();
            }

        }

        return _actions.Count == 0 && _arriveTime == 0f;
    }

    /// <summary>Coroutine likes ActionPhase</summary>
    public virtual IEnumerator ActionSeq(BattleUnitModel attacker, List<BattleFarAreaPlayManager.VictimInfo> victims)
    {
        yield break;
    }

    private bool _init;

    private float _elapsed;

    private float _arriveTime;

    private Stack<IEnumerator> _actions = new();

    /// <summary>Included deltaTime</summary>
    protected float DeltaTime { get; private set; }
}

