using System.Collections;
using Battle.DiceAttackEffect;
using UnityEngine;
using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>An advanced behaviour action</summary>
public class AdvancedBehaviourAction : BehaviourActionBase
{
    /// <summary>Impl GetMovingAction</summary>
    public sealed override List<RencounterManager.MovingAction> GetMovingAction(ref RencounterManager.ActionAfterBehaviour self, ref RencounterManager.ActionAfterBehaviour opponent)
    {
        if (IsDiscardAction(self, opponent))
        {
            return base.GetMovingAction(ref self, ref opponent);
        }

        var main = new ParallelAction(self.view, GetMoving(self, opponent));

        List<ParallelAction> parallels = new();

        Init(parallels, self, opponent);

        var init = false;

        bool F(float deltaTime)
        {
            var res = main.Step(deltaTime);

            if (!init && !res)
            {
                foreach (var parallel in parallels)
                {
                    parallel.Init();
                }

                init = true;
            }

            foreach (var parallel in parallels)
            {
                parallel.Step(deltaTime);
            }

            if (res && init)
            {
                foreach (var parallel in parallels)
                {
                    parallel.Clean();
                }
            }

            return res;
        }

        var ma = new RencounterManager.MovingAction(ActionDetail.NONE, CharMoveState.Custom, updateDir: false, delay: 0);

        ma.SetCustomMoving(F);
        ma.SetEffectTiming(EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT);

        opponent.infoList.Add(new(ActionDetail.NONE, CharMoveState.None, updateDir: false, delay: 0));

        return [ma];
    }

    /// <summary>Initialize parallels and more</summary>
    protected virtual void Init(List<ParallelAction> parallels, RencounterManager.ActionAfterBehaviour self, RencounterManager.ActionAfterBehaviour opponent)
    {
    }

    /// <summary>Returns Moving enumerator</summary>
    protected virtual IEnumerator GetMoving(RencounterManager.ActionAfterBehaviour self, RencounterManager.ActionAfterBehaviour opponent)
    {
        yield break;
    }

    /// <summary>Returns true if Is discards action is self</summary>
    protected virtual bool IsDiscardAction(RencounterManager.ActionAfterBehaviour self, RencounterManager.ActionAfterBehaviour opponent)
    {
        return self.result != Result.Win || self.data.actionType != ActionType.Atk || opponent.behaviourResultData?.IsFarAtk() == true;
    }

    /// <summary>Prints effect</summary>
    protected DiceAttackEffect? PrintEffect(string effectRes, BattleUnitView self, BattleUnitView opponent, float scale = 1f)
    {
        return DiceEffectManager.Instance?.CreateBehaviourEffect(effectRes, scale, self, opponent, 1f)?.Also(eff => eff.SetLayer("Effect"));
    }

    /// <summary>Prints sound</summary>
    protected void PrintSound(ActionDetail sound, BattleUnitView self, bool parryWin = true)
    {
        self?.charAppearance?.soundInfo?.PlaySound(MotionConverter.ActionToMotion(sound), parryWin);
    }

    /// <summary>Prints sound</summary>
    protected void PrintSound(string sound, BattleUnitView self)
    {
        PrintSound(Resources.Load<AudioClip>($"Sounds/MotionSound/{sound}"), self);
    }

    /// <summary>Prints sound</summary>
    protected void PrintSound(AudioClip audio, BattleUnitView self)
    {
        if (self?.charAppearance?.soundInfo is CharacterSound csnd && _soundPrefabRef(csnd) is BattleEffectSound pref)
        {
            var be = UnityObject.Instantiate<BattleEffectSound>(pref, BattleSoundManager.Instance.transform);

            be?.Init(audio, BattleSoundManager.Instance.VolumeFX);
        }
    }

    /// <summary>Prints recovery</summary>
    protected void PrintRecovery()
    {
        if (!_recovered)
        {
            PrintRecoveryImpl(RencounterManager.Instance);

            _recovered = true;
        }
    }

    /// <summary>Prints damage</summary>
    protected void PrintDamage()
    {
        if (!_damaged)
        {
            PrintDamageImpl(RencounterManager.Instance);

            _damaged = true;
        }
    }

    /// <summary>An action unit</summary>
    protected class ParallelAction(BattleUnitView view, IEnumerator first)
    {
        BattleUnitView view = view;

        internal void Init()
        {
            SetParallel(view);
        }

        internal void Clean()
        {
            ResetParallel(view);
        }

        internal bool Step(float deltaTime)
        {
            if (_elapsed >= _waited)
            {
                _elapsed = 0f;
                _waited = 0f;
            }
            else
            {
                _elapsed += deltaTime;

                return false;
            }

            if (_actions.TryPeek(out var top))
            {
                if (top.MoveNext())
                {
                    if (top.Current is float ws)
                    {
                        _waited = ws;
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

            return _actions.Count == 0 && _waited == 0f;
        }

        void SetParallel(BattleUnitView view)
        {
            view?.unitBottomStatUI?.SetIdentityLightActive(true);
            view?.charAppearance?.ChangeLayer("UI_WORLD");
            view?.unitBottomStatUI?.ChangeLayer("UI_WORLD", "UI_WORLD");
            view?.unitBottomStatUI?.ChangeChildCanvasOrder("Character", 0);
            view?.model?.OnLayerChanged("UI_WORLD");
            view?.keepUI?.Init(false);
        }

        void ResetParallel(BattleUnitView view)
        {
            view?.unitBottomStatUI?.SetIdentityLightActive(false);
            view?.charAppearance?.ChangeLayer("Character");
            view?.unitBottomStatUI?.ChangeLayer("CharacterUI", "Character");
            view?.unitBottomStatUI?.ChangeChildCanvasOrder("CharacterUI", 0);
            view?.charAppearance?.ChangeMotion(ActionDetail.Default);
            view?.model?.OnLayerChanged("Character");
            view?.unitBottomStatUI?.EnableCanvas(false);
            view?.keepUI?.Init(true);
        }

        Stack<IEnumerator> _actions = new([first]);

        float _elapsed;

        float _waited;
    }

    bool _recovered;

    bool _damaged;

    static AccessTools.FieldRef<CharacterSound, BattleEffectSound> _soundPrefabRef = typeof(CharacterSound).FieldRefAccess<BattleEffectSound>("_soundPrefab");

    static Action<RencounterManager> PrintRecoveryImpl { get; } = (Action<RencounterManager>)typeof(RencounterManager).Method("PrintRecovery").CreateDelegate(typeof(Action<RencounterManager>));

    static Action<RencounterManager> PrintDamageImpl { get; } = (Action<RencounterManager>)typeof(RencounterManager).Method("PrintDamage").CreateDelegate(typeof(Action<RencounterManager>));
}
