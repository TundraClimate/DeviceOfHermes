using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using HarmonyExtension;
using LOR_DiceSystem;
using Sound;

namespace LimbufOfHermes;

/// <summary>A unit buf the Poise</summary>
public class BattleUnitBuf_Limbuf_Poise : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Poise";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.Poise;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Positive;

    void UsePoise()
    {
        ChangeStack(stack => ((int)(stack * 0.75f)));
    }

    static BattleUnitBuf_Limbuf_Poise? GetPoise(BattleUnitModel owner)
    {
        return owner.bufListDetail?.GetActivatedBuf(LimKeywordBuf.Poise) as BattleUnitBuf_Limbuf_Poise;
    }

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        var isCrit = CritData.GetValue(behavior, _ => new(false));

        isCrit.value = false;

        var prob = this.stack * 5 / 100f;

        if (prob >= RandomUtil.valueForProb)
        {
            OnRollCritical(behavior);

            isCrit.value = true;
        }
    }

    /// <summary>Impl OnSuccessAttack</summary>
    public override void OnSuccessAttack(BattleDiceBehavior behavior)
    {
        if (CritData.TryGetValue(behavior, out var isCrit) && isCrit.value)
        {
            UsePoise();
            OnActivate(this.stack);
            OnCriticalAttack(behavior, behavior.card.target);
        }
    }

    /// <summary>Impl OnSuccessAreaAttack</summary>
    public override void OnSuccessAreaAttack(BattleDiceBehavior behavior, BattleUnitModel target)
    {
        if (CritData.TryGetValue(behavior, out var isCrit) && isCrit.value)
        {
            OnActivate(this.stack);
            OnCriticalAttack(behavior, target);
        }
    }

    /// <summary>Impl OnActivate</summary>
    public override void OnActivate(int stack)
    {
        if (StageController.Instance.IsLogState())
        {
            base._owner.AddRencounterEvent(RencounterEvent.PrintEffect, () => SoundEffectManager.Instance.PlayClip("creature/greed_strongatk_defensed", false, 1f, null));
        }
        else
        {
            SoundEffectManager.Instance.PlayClip("creature/greed_strongatk_defensed", false, 1f, null);
        }
    }

    /// <summary>On rolled critical</summary>
    public void OnRollCritical(BattleDiceBehavior behavior)
    {
        if (behavior.card.card.GetSpec().Ranged is CardRange.FarArea or CardRange.FarAreaEach)
        {
            UsePoise();
        }

        base._owner.EachPassiveOf<ILimbuf.OnRollCritical>(i => i.OnRollCritical(behavior));
        base._owner.EachUnitBufOf<ILimbuf.OnRollCritical>(i => i.OnRollCritical(behavior));
    }

    /// <summary>On critical attcked</summary>
    public void OnCriticalAttack(BattleDiceBehavior behavior, BattleUnitModel target)
    {
        base._owner.EachPassiveOf<ILimbuf.OnCriticalAttack>(i => i.OnCriticalAttack(behavior));
        base._owner.EachUnitBufOf<ILimbuf.OnCriticalAttack>(i => i.OnCriticalAttack(behavior));
    }

    /// <summary>A damage adder of critical</summary>
    public double GetCriticalDamageAdder(BattleDiceBehavior beh)
    {
        return 0.2 + CritDamageAdder.GetValue(beh, _ => new(0)).value;
    }

    private bool isCritActive;

    private static ConditionalWeakTable<BattleDiceBehavior, Box<bool>> CritData = new();

    internal static ConditionalWeakTable<BattleDiceBehavior, Box<double>> CritDamageAdder = new();

    [HarmonyPatch(typeof(BattleUnitModel), "ChangeDamage")]
    class PatchChangeDamage
    {
        static void Prefix(BattleUnitModel __instance, BattleUnitModel attacker, ref double dmg)
        {
            GetPoise(attacker)?.isCritActive = false;

            if (attacker?.currentDiceAction?.currentBehavior is BattleDiceBehavior beh
                && CritData.TryGetValue(beh, out var isCrit) && isCrit.value
            )
            {
                GetPoise(attacker)?.isCritActive = true;

                var rate = (double)BookModel.GetResistRate(__instance.GetResistHP(beh.behaviourInCard.Detail));

                dmg /= rate;
                dmg *= rate + GetPoise(attacker)?.GetCriticalDamageAdder(beh) ?? 0.0;
            }
        }
    }

    [HarmonyPatch(typeof(BattleCardTotalResult), "SetDamageGiven")]
    class PatchDamageGiven
    {
        static Exception Finalizer(Exception __exception, BattleCardTotalResult __instance)
        {
            if (GetPoise(__instance.playingCard.owner)?.isCritActive == true)
            {
                var dmgList = __instance.playingCard?.target?.battleCardResultLog?.CurbehaviourResult?.dmgListTaken;

                if (dmgList is not null && dmgList.Count > 0)
                {
                    var origin = dmgList.Last();

                    dmgList.RemoveAt(dmgList.Count - 1);
                    dmgList.Add(new(origin.damagedType, origin.dmg, origin.maxValue, (AtkResist)"LimbufOfHermes_Poise".GetHashCode()));
                }
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitView), "Damaged")]
    class PatchDamaged
    {
        static void Prefix(BattleUnitModel attacker, ref AtkResist atkResist)
        {
            if (GetPoise(attacker)?.isCritActive == true)
            {
                atkResist = (AtkResist)"LimbufOfHermes_Poise".GetHashCode();
            }
        }
    }

    [HarmonyPatch(typeof(AttackEffectManager), "CreateDamagedTextEffect")]
    class PatchDamagedText
    {
        static bool Prefix(AttackEffectManager __instance, int damage, BehaviourDetail detail, BattleUnitModel unit, BattleUnitModel attacker, AtkResist atkResist, bool add, int colorIdx)
        {
            if (atkResist == (AtkResist)"LimbufOfHermes_Poise".GetHashCode())
            {
                var effect = UnityObject.Instantiate<DamageTextEffect>(__instance.damagedTextPrefab, unit.view.damageTextEffectRoot);

                effect.maxEffect = add;
                effect.isAtk = true;

                var color = new Color(1, 1, 0, 1);

                effect.txt_resist.color = color;
                effect.txt_resist.fontMaterial.SetColor("_UnderlayColor", color);
                effect.txt_resist.text = "CRITICAL";

                switch (detail)
                {
                    case BehaviourDetail.Slash:
                        effect.img_resistIcon.sprite = BattleManagerUI.Instance.ui_unitInformation.hpResists[0].icons[0];

                        break;
                    case BehaviourDetail.Penetrate:
                        effect.img_resistIcon.sprite = BattleManagerUI.Instance.ui_unitInformation.hpResists[1].icons[0];

                        break;
                    case BehaviourDetail.Hit:
                        effect.img_resistIcon.sprite = BattleManagerUI.Instance.ui_unitInformation.hpResists[2].icons[0];

                        break;
                }

                effect.img_resistIconBg.enabled = false;
                effect.img_resistIconFg.enabled = false;

                if (damage > 0)
                {
                    var log = (int)Mathf.Log10((float)damage);
                    var digit = (int)Mathf.Pow(10f, (float)log);
                    var value = damage / digit;
                    var dmgNum = UnityObject.Instantiate<DamageNumber>(__instance.damageNumberPrefabs[value], effect.damageNumParent);

                    dmgNum.SetColor(color, atkResist);

                    effect.numberList.Add(dmgNum);

                    if (damage > 10)
                    {
                        dmgNum.transform.localScale *= 1.2f;
                    }

                    while (digit / 10 > 0)
                    {
                        var num4 = damage % digit;

                        digit /= 10;
                        value = num4 / digit;
                        dmgNum = UnityObject.Instantiate<DamageNumber>(__instance.damageNumberPrefabs[value], effect.damageNumParent);
                        dmgNum.SetColor(color, atkResist);
                        effect.numberList.Add(dmgNum);
                    }
                }
                else if (damage == 0)
                {
                    var dmgNum = UnityObject.Instantiate<DamageNumber>(__instance.damageNumberPrefabs[0], effect.damageNumParent);

                    dmgNum.SetColor(color, atkResist);
                    effect.numberList.Add(dmgNum);
                }

                effect.rotatePivot.transform.localScale *= Mathf.Min(3f, (float)(damage + 150) * 0.01f);

                __instance.SetEffectSizeByCamZoom(effect);
                __instance.SetEffectSizeByUnitHeight(unit, effect);

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(DamageNumber), "SetColor")]
    class PatchSetColor
    {
        static bool Prefix(DamageNumber __instance, Color c, AtkResist resist)
        {
            if (resist == (AtkResist)"LimbufOfHermes_Poise".GetHashCode())
            {
                __instance.imgs[0].color = c;
                _dmgColorRef(__instance) = c;
                __instance.StartCoroutine(ChangeTextColor(__instance));

                return false;
            }

            return true;
        }

        static IEnumerator ChangeTextColor(DamageNumber dmgNum)
        {
            var initial = Color.white;
            var last = new Color(1f, 0.5f, 0f);
            var dstColor = _dmgColorRef(dmgNum);

            dmgNum.imgs[1].color = initial;
            dmgNum.imgs[1].GetComponent<Outline>().effectColor = initial;

            yield return new WaitForSeconds(0.1f);

            float elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;

                dmgNum.imgs[1].color = Vector4.Lerp(dmgNum.imgs[1].color, dstColor, elapsed * 5f);
                dmgNum.imgs[1].GetComponent<Outline>().effectColor = Vector4.Lerp(dmgNum.imgs[1].GetComponent<Outline>().effectColor, last, elapsed * 5f);

                yield return new WaitForEndOfFrame();
            }

            dmgNum.imgs[1].color = dstColor;
            dmgNum.imgs[1].GetComponent<Outline>().effectColor = last;
        }

        static AccessTools.FieldRef<DamageNumber, Color> _dmgColorRef
            = typeof(DamageNumber).FieldRefAccess<Color>("dmgColor");
    }
}
