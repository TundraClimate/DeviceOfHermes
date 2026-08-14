using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Sound;
using LOR_DiceSystem;
using LOR_XML;
using UnityEngine;

namespace DeviceOfHermes;

/// <summary>A helper of FarAreaEffect</summary>
public class EasyAreaEffect : AdvancedAreaEffect
{
    /// <summary>Set default settings</summary>
    public override void Init(BattleUnitModel self, params object[] args)
    {
        base.Init(self);

        this.state = EffectState.None;
        this.isRunning = false;

        self.view.charAppearance.ChangeMotion(ActionDetail.Default);
    }

    /// <summary>Enable independent action</summary>
    public override bool HasIndependentAction => true;

    /// <summary>Default override</summary>
    public override IEnumerator ActionSeq(BattleUnitModel attacker, List<BattleFarAreaPlayManager.VictimInfo> victims)
    {
        _attacker = attacker;

        GetAction_Easy(new(attacker), new(victims));

        while (_seq.TryDequeue(out var res))
        {
            yield return res;
        }

        UnityEngine.Object.Destroy(this);
    }

    /// <summary>Build sequence for easy</summary>
    protected virtual void GetAction_Easy(Unit attacker, Victims victims)
    {
    }

    /// <summary>Add running task</summary>
    protected void AddTask(IEnumerator task)
    {
        _seq.Enqueue(task);
    }

    /// <summary>Waits for seconds</summary>
    protected void WaitForSecs(float wait)
        => AddTask(WaitForSecsRoutine(wait));

    /// <summary>Camera follows target</summary>
    protected void FollowUnit(params List<BattleUnitModel> targets)
        => AddTask(FollowUnitRoutine(targets));

    /// <summary>Camera follows target</summary>
    protected void FollowUnit(params List<Unit> targets)
        => AddTask(FollowUnitRoutine(targets.Map(t => t.unitModel).ToList()));

    /// <summary>Camera follows target</summary>
    protected void FollowUnit(Faction faction)
        => AddTask(FollowUnitRoutine(faction.AliveUnits));

    /// <summary>Update character direction</summary>
    protected void UpdateDirection(Func<Vector3> targetf)
        => AddTask(UpdateDirectionRoutine(targetf));

    /// <summary>Change character motion</summary>
    protected void ChangeMotion(ActionDetail motion)
        => AddTask(ChangeMotionRoutine(motion));

    /// <summary>Play dice effect</summary>
    protected void PlayDiceEffect(string resource, Unit victim, float scale = 1f, float time = 1f)
        => AddTask(PlayDiceEffectRoutine(resource, victim, scale, time));

    /// <summary>Play sound effect</summary>
    protected void PlaySound(string resource, bool loop = false, float volume = 1f)
        => AddTask(PlaySoundRoutine(resource, loop, volume));

    /// <summary>Give damage if win by far-area clash</summary>
    protected void GiveDamage(Unit target, bool force = false)
        => AddTask(GiveDamageRoutine(target, force));

    /// <summary>Move to dest</summary>
    protected void MoveTo(Func<Vector3> dstf, float duration, float speedMul = 1f, MoveOpts opts = MoveOpts.None)
        => AddTask(MoveToRoutine(dstf, duration, speedMul, opts));

    private IEnumerator WaitForSecsRoutine(float wait)
    {
        yield return wait;
    }

    private IEnumerator FollowUnitRoutine(params List<BattleUnitModel> targets)
    {
        BattleCamManager.Instance.FollowUnits(false, targets);

        yield break;
    }

    private IEnumerator UpdateDirectionRoutine(Func<Vector3> targetf)
    {
        var target = targetf();

        _attacker!.UpdateDirection(target);

        yield break;
    }

    private IEnumerator ChangeMotionRoutine(ActionDetail motion)
    {
        _attacker!.view.charAppearance.ChangeMotion(motion);

        yield break;
    }

    private IEnumerator PlayDiceEffectRoutine(string resource, Unit victim, float scale = 1f, float time = 1f)
    {
        DiceEffectManager.Instance.CreateBehaviourEffect(resource, scale, _attacker!.view, victim.unitModel.view, time)?.SetLayer("Effect");

        yield break;
    }

    private IEnumerator PlaySoundRoutine(string resource, bool loop = false, float volume = 1f)
    {
        SoundEffectManager.Instance.PlayClip(resource, loop, volume, null);

        yield break;
    }

    private IEnumerator MoveToRoutine(Func<Vector3> dstf, float duration, float speedMul = 1f, MoveOpts opts = MoveOpts.None)
    {
        var dst = dstf();

        if (opts.HasFlag(MoveOpts.Easing))
        {
            yield return CommonCoroutine.UnitEaseMoving(_attacker!.view, _attacker!.view.WorldPosition, dst, duration, speedMul);
        }
        else
        {
            yield return CommonCoroutine.UnitMoving(_attacker!.view, _attacker!.view.WorldPosition, dst, duration, speedMul);
        }
    }

    private IEnumerator GiveDamageRoutine(Unit target, bool force)
    {
        if (target.victimInfo is null)
        {
            yield break;
        }

        var shouldTakeDamage = false;

        if (_attacker?.currentDiceAction?.card?.GetSpec()?.Ranged == CardRange.FarArea)
        {
            var sum = target.victimInfo.playingCard?.GetDiceBehaviorList().Sum(dice => dice.DiceResultValue);

            shouldTakeDamage = _attacker.currentDiceAction.currentBehavior?.DiceResultValue > sum;

            if (shouldTakeDamage && target.victimInfo.playingCard is not null)
            {
                target.victimInfo.cardDestroyed = true;
            }
            else if (!DefenseVictims.Contains(target.victimInfo))
            {
                DefenseVictims.Add(target.victimInfo);
            }
        }
        else if (_attacker?.currentDiceAction?.card?.GetSpec()?.Ranged == CardRange.FarAreaEach)
        {
            shouldTakeDamage =
                target.victimInfo.playingCard?.currentBehavior is null ||
                _attacker.currentDiceAction.currentBehavior.DiceResultValue > target.victimInfo.playingCard.currentBehavior.DiceResultValue;

            if (shouldTakeDamage && target.victimInfo.playingCard?.currentBehavior is not null)
            {
                target.victimInfo.destroyedDicesIndex.Add(_attacker.currentDiceAction.currentBehavior.Index);
            }
            else if (!DefenseVictims.Contains(target.victimInfo))
            {
                DefenseVictims.Add(target.victimInfo);
            }
        }

        if (!shouldTakeDamage)
        {
            yield break;
        }

        _attacker?.currentDiceAction?.currentBehavior?.GiveDamage(target.unitModel);

        if (target.unitModel.IsDead())
        {
            target.unitModel.view.DisplayDlg(DialogType.DEATH, [_self]);
        }

        target.unitModel.view.charAppearance.ChangeMotion(ActionDetail.Damaged);
        BattleManagerUI.Instance.ui_unitListInfoSummary.UpdateCharacterProfile(target.unitModel, target.faction, target.unitModel.hp, target.unitModel.breakDetail.breakGauge, null);
        BattleManagerUI.Instance.ui_unitListInfoSummary.UpdateCharacterProfile(_attacker, _self.faction, _self.hp, _self.breakDetail.breakGauge, null);
    }

    /// <summary>MoveTo Options</summary>
    [Flags]
    protected enum MoveOpts
    {
        /// <summary>None</summary>
        None = 0,

        /// <summary>enable Easing</summary>
        Easing = 1 << 0,
    }

    private BattleUnitModel? _attacker;

    private Queue<IEnumerator> _seq = new();

    /// <summary>A wrapper of List&lt;BattleFarAreaPlayManager.VictimInfo&gt;</summary>
    protected class Victims(List<BattleFarAreaPlayManager.VictimInfo> victims)
    {
        /// <summary>Takes random victims one</summary>
        public bool TakeRandomOne([NotNullWhen(true)] out Unit? res, bool noDeadUnit = true)
        {
            if (noDeadUnit)
            {
                _victims.RemoveAll(v => v.unitModel.IsDead());
            }

            if (_victims.Count == 0)
            {
                res = null;

                return false;
            }

            var selected = RandomUtil.SelectOne(_victims);

            res = new Unit(selected);

            _victims.Remove(selected);

            return true;
        }

        private List<BattleFarAreaPlayManager.VictimInfo> _victims = new(victims);
    }

    /// <summary>Unit helper</summary>
    protected class Unit
    {
        /// <summary>Create Unit</summary>
        public Unit(BattleUnitModel model)
        {
            unitModel = model;
            playingCard = model.currentDiceAction;
        }

        /// <summary>Create Unit</summary>
        public Unit(BattleFarAreaPlayManager.VictimInfo info)
        {
            unitModel = info.unitModel;
            playingCard = info.playingCard;
            victimInfo = info;
        }

        /// <summary>Returns unitmodel</summary>
        public BattleUnitModel unitModel { get; private set; }

        /// <summary>Returns playingCard</summary>
        public BattlePlayingCardDataInUnitModel playingCard { get; private set; }

        /// <summary>Returns faction</summary>
        public Faction faction => unitModel.faction;

        /// <summary>Returns victimInfo</summary>
        public BattleFarAreaPlayManager.VictimInfo? victimInfo { get; private set; }

        /// <summary>Returns character center position</summary>
        public Func<Vector3> Center(float shift = 0f)
            => () => unitModel.view.WorldPosition + new Vector3(shift, 0f, 0f);

        /// <summary>Returns character front position</summary>
        public Func<Vector3> Front(float shift = 0f)
            => () => unitModel.view.WorldPosition + new Vector3(HexagonalMapManager.Instance.tileSize * 4f * unitModel.view.transform.localScale.x / 1.5f + shift, 0f, 0f) * (unitModel.direction == Direction.LEFT ? -1f : 1f);

        /// <summary>Returns character back position</summary>
        public Func<Vector3> Back(float shift = 0f)
            => () => unitModel.view.WorldPosition - new Vector3(HexagonalMapManager.Instance.tileSize * 4f * unitModel.view.transform.localScale.x / 1.5f - shift, 0f, 0f) * (unitModel.direction == Direction.LEFT ? -1f : 1f);

        /// <summary>Returns character center position</summary>
        public static Func<Vector3> Center(Unit target, float shift = 0f) => target.Center(shift);

        /// <summary>Returns character front position</summary>
        public static Func<Vector3> Back(Unit target, float shift = 0f) => target.Back(shift);

        /// <summary>Returns character back position</summary>
        public static Func<Vector3> Front(Unit target, float shift = 0f) => target.Front(shift);

        /// <summary>Returns all unit</summary>
        public static List<Unit> All => BattleObjectManager.instance.GetAliveList().Map(unit => new Unit(unit)).ToList();
    }

    /// <summary>Stage helper</summary>
    protected class Stage
    {
        /// <summary>Returns center of stage</summary>
        public static Func<Vector3> Center => () => Vector3.zero;
    }
}
