using System.Collections;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HarmonyLib;
using HarmonyExtension;
using UI;
using BattleCharacterProfile;

namespace LimbufOfHermes;

/// <summary>A barrier marker</summary>
public sealed class BattleUnitBuf_Limbuf_Barrier : LimbufBase
{
    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.Barrier;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.None;

    static BattleUnitBuf_Limbuf_Barrier? GetBarrier(BattleUnitModel owner)
    {
        return owner.bufListDetail.GetActivatedBuf(LimKeywordBuf.Barrier) as BattleUnitBuf_Limbuf_Barrier;
    }

    /// <summary>Impl OnAddBuf</summary>
    public override void OnAddBuf(int addedStack)
    {
        BottomBarriers
            .GetValue(base._owner.view.unitBottomStatUI, botUI =>
                botUI.gameObject.AddComponent<BattleUnitBottomBarrierUI>().Also(ui => ui.Init(botUI))
            );

        ProfileBarriers
            .GetValue(BattleCharacterProfileBarrierUI.FindProfile(base._owner)!, profile =>
                BattleCharacterProfileBarrierUI.Apply(profile)
            );
    }

    bool IsKeepBarrier()
    {
        return false;
    }

    int ConsumeStack(int dmg)
    {
        var res = dmg - this.stack;

        if (0 > res)
        {
            ChangeStack(_ => -res);

            return 0;
        }
        else
        {
            ChangeStack(_ => 0);

            return res;
        }
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        if (!IsKeepBarrier())
        {
            Destroy();
        }
    }

    static ConditionalWeakTable<BattleUnitBottomStatUI, BattleUnitBottomBarrierUI> BottomBarriers = new();

    static ConditionalWeakTable<BattleCharacterProfileUI, BattleCharacterProfileBarrierUI> ProfileBarriers = new();

    static ConditionalWeakTable<BattleCardBehaviourResult, Box<int>> BarrierResult = new();

    [HarmonyPatch(typeof(BattleUnitModel), "TakeDamage")]
    class PatchShield
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdloc(),
                CodeMatch.IsOpCode(OpCodes.Ldc_I4_0),
                CodeMatch.IsOpCode(OpCodes.Ble),
                CodeMatch.IsLdarg(0),
                CodeMatch.IsLdloc(),
                CodeMatch.Calls(typeof(BattleUnitModel).Method("OnLoseHp"))
            )
                .Advance(1)
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Call(typeof(PatchShield).Method("InjectBefore"))
                );

            matcher.MatchStartForward(
                CodeMatch.IsLdarg(0),
                CodeMatch.IsLdarg(0),
                CodeMatch.Calls(typeof(BattleUnitModel).Method("get_hp")),
                CodeMatch.IsLdloc(),
                CodeMatch.IsOpCode(OpCodes.Conv_R4),
                CodeMatch.IsOpCode(OpCodes.Sub),
                CodeMatch.Calls(typeof(BattleUnitModel).Method("set_hp"))
            )
                .Advance(4)
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Call(typeof(PatchShield).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static int InjectBefore(int loseHp, BattleUnitModel __instance)
        {
            if (GetBarrier(__instance) is BattleUnitBuf_Limbuf_Barrier barrier)
            {
                return 0.Max(loseHp - barrier.stack);
            }

            return loseHp;
        }

        static int InjectMethod(int dmg, BattleUnitModel __instance)
        {
            if (GetBarrier(__instance) is BattleUnitBuf_Limbuf_Barrier barrier)
            {
                return barrier.ConsumeStack(dmg).Also(_ =>
                {
                    if (__instance.battleCardResultLog?.CurbehaviourResult is BattleCardBehaviourResult res)
                    {
                        BarrierResult.GetValue(res, _ => new(0)).value = barrier.stack;
                    }
                });
            }

            return dmg;
        }
    }

    [HarmonyPatch(typeof(BattleUnitBottomStatUI), "SetHp")]
    class PatchOnSetHpBot
    {
        static void Prefix(BattleUnitBottomStatUI __instance, BattleUnitView ____view)
        {
            if (BottomBarriers.TryGetValue(__instance, out var ui) && !StageController.Instance.IsLogState())
            {
                if (GetBarrier(____view.model) is BattleUnitBuf_Limbuf_Barrier barrier)
                {
                    ui.UpdateHealth(barrier.stack);
                }
                else
                {
                    ui.UpdateHealth(0);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleCharacterProfileUI), "SetHpUI")]
    class PatchOnSetHpProf
    {
        static void Prefix(BattleCharacterProfileUI __instance)
        {
            if (ProfileBarriers.TryGetValue(__instance, out var ui) && !StageController.Instance.IsLogState())
            {
                if (GetBarrier(__instance.UnitModel) is BattleUnitBuf_Limbuf_Barrier barrier)
                {
                    ui.UpdateHealth(barrier.stack);
                }
                else
                {
                    ui.UpdateHealth(0);
                }
            }
        }
    }

    [HarmonyPatch(typeof(RencounterManager), "PrintDamage")]
    class PatchOnPrintDamage
    {
        static Exception Finalizer(
            Exception __exception,
            BattleUnitView ____librarian,
            BattleCardBehaviourResult ____currentLibrarianBehaviourResult,
            BattleUnitView ____enemy,
            BattleCardBehaviourResult ____currentEnemyBehaviourResult
        )
        {
            void Update(BattleUnitView view, BattleCardBehaviourResult res)
            {
                if (BarrierResult.TryGetValue(res, out var num))
                {
                    if (BottomBarriers.TryGetValue(view.unitBottomStatUI, out var botUI))
                    {
                        botUI.UpdateHealth(num.value);
                    }

                    if (ProfileBarriers.TryGetValue(BattleCharacterProfileBarrierUI.FindProfile(view.model)!, out var profUI))
                    {
                        profUI.UpdateHealth(num.value);
                    }
                }
            }

            Update(____librarian, ____currentLibrarianBehaviourResult);
            Update(____enemy, ____currentEnemyBehaviourResult);

            return __exception;
        }
    }

    class BattleUnitBottomBarrierUI : MonoBehaviour
    {
        public void Init(BattleUnitBottomStatUI statUI)
        {
            origin = statUI;

            var hpMask = _hpMaskRef(statUI);
            var txtHp = _txtHpRef(statUI);

            box = UnityObject.Instantiate<GameObject>(hpMask.gameObject, hpMask.gameObject.transform.parent);
            barBg = box.transform.Find("[Image]Hp").gameObject.GetComponent<Image>();
            healthTxt = UnityObject.Instantiate<TextMeshProUGUI>(txtHp, txtHp.transform.parent);

            barBg.color = new Color32(0, 127, 255, 255);
            barBg.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

            healthTxt.color = new Color32(0, 127, 255, 255);
            healthTxt.transform.localPosition = new(-78f, -85f, 0f);
            healthTxt.fontSize = 35f;
        }

        public void UpdateHealth(int value)
        {
            if (value > 0)
            {
                barBg?.gameObject?.SetActive(true);
                healthTxt?.gameObject?.SetActive(true);
                healthTxt?.text = value.ToString();
            }
            else
            {
                barBg?.transform?.rotation = Quaternion.Euler(0f, 0f, 90f);
                barBg?.gameObject?.SetActive(false);
                healthTxt?.gameObject?.SetActive(false);
            }

            if (anim is not null)
            {
                StopCoroutine(anim);
            }

            anim = StartCoroutine(UpdateAnimRoutine(value));
        }

        IEnumerator UpdateAnimRoutine(int value)
        {
            barBg?.gameObject?.SetActive(true);

            var view = _viewRef(origin!);
            var max = view.model.MaxHp;
            var start = Mathf.InverseLerp(90f, 0f, barBg?.gameObject?.transform?.localEulerAngles.z ?? 0f);
            var end = value == 0 ? 0f : (float)max.Min(value) / (float)max;

            if (start == 0 && end == 0)
            {
                barBg?.transform?.localRotation = Quaternion.Euler(0f, 0f, 90f);
                barBg?.gameObject?.SetActive(false);
                healthTxt?.gameObject?.SetActive(false);

                yield break;
            }

            var tick = 0f;

            while (tick < 0.5f)
            {
                barBg?.gameObject?.transform?.localRotation
                    = Quaternion.Euler(0f, 0f, Mathf.Lerp(90f, 0f, Mathf.Lerp(start, end, tick / 0.5f)));

                tick += Time.deltaTime;

                yield return null;
            }

            barBg?.transform?.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(90f, 0f, end));

            if (1 > value)
            {
                barBg?.transform?.localRotation = Quaternion.Euler(0f, 0f, 90f);
                barBg?.gameObject?.SetActive(false);
                healthTxt?.gameObject?.SetActive(false);
            }
        }

        private BattleUnitBottomStatUI? origin;

        private GameObject? box;

        private Image? barBg;

        private TextMeshProUGUI? healthTxt;

        private Coroutine? anim;

        static AccessTools.FieldRef<BattleUnitBottomStatUI, Mask> _hpMaskRef
            = typeof(BattleUnitBottomStatUI).FieldRefAccess<Mask>("hpBarMask");

        static AccessTools.FieldRef<BattleUnitBottomStatUI, TextMeshProUGUI> _txtHpRef
            = typeof(BattleUnitBottomStatUI).FieldRefAccess<TextMeshProUGUI>("_txtHp");

        static AccessTools.FieldRef<BattleUnitBottomStatUI, BattleUnitView> _viewRef
            = typeof(BattleUnitBottomStatUI).FieldRefAccess<BattleUnitView>("_view");
    }

    class BattleCharacterProfileBarrierUI : MonoBehaviour
    {
        public static BattleCharacterProfileUI? FindProfile(BattleUnitModel owner)
        {
            var res = BattleManagerUI.Instance.ui_unitListInfoSummary.enemyarray.FirstOrDefault(i => i.UnitModel == owner);

            if (res is not null)
            {
                return res;
            }

            res = BattleManagerUI.Instance.ui_unitListInfoSummary.allyarray.FirstOrDefault(i => i.UnitModel == owner);

            if (res is not null)
            {
                return res;
            }

            return null;
        }

        public static BattleCharacterProfileBarrierUI Apply(BattleCharacterProfileUI profileUI)
        {
            var uiRoot = _uiRootRef(profileUI);
            var root = uiRoot.gameObject.AddChildObject("Barrier").Also(root => root.transform.localScale = Vector3.one);

            return root.AddComponent<BattleCharacterProfileBarrierUI>().Also(ui => ui.Init(profileUI));
        }

        public void Init(BattleCharacterProfileUI profileUI)
        {
            var mask = gameObject.AddChildObject("BarrierMask");

            mask.transform.localPosition = Vector3.zero;
            mask.transform.localScale = Vector3.one;

            mask.AddComponent<Mask>();
            mask.AddComponent<Image>().Also(img =>
            {
                img.color = new Color(0f, 0f, 0f, 0.018f);
                img.rectTransform.sizeDelta = new(500f, 100f);
            });

            var uiRoot = _uiRootRef(profileUI);
            var hpBar = _hpBarRef(profileUI);
            var txt_hp = _txt_hpRef(profileUI);

            origin = profileUI;

            barBg = UnityObject.Instantiate<Image>(hpBar.img, mask.transform);
            healthTxt = UnityObject.Instantiate<Text>(txt_hp, gameObject.transform);

            UnityObject.Destroy(healthTxt.GetComponent<UITextDataLoader>());

            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;

            barBg.color = new Color32(0, 127, 255, 255);
            barBg.transform.localPosition = new(-756f, -44f, 0f);
            barBg.transform.localScale = new(1f, 0.5f, 1f);
            barBg.gameObject.SetActive(false);

            healthTxt.color = new Color32(0, 127, 255, 255);
            healthTxt.fontSize = 12;
            healthTxt.text = "0";
            healthTxt.transform.localPosition = new(410f, -130f, 0f);
            healthTxt.transform.localScale = new(healthTxt.transform.position.x < 0f ? -1f : 1f, 1f, 1f);
            healthTxt.gameObject.SetActive(false);
        }

        public void UpdateHealth(int value)
        {
            if (barAnim is not null)
            {
                StopCoroutine(barAnim);
            }

            if (txtAnim is not null)
            {
                StopCoroutine(txtAnim);
            }

            barAnim = StartCoroutine(BarUpdateAnimRoutine(value));
            txtAnim = StartCoroutine(TxtUpdateAnimRoutine(value));
        }

        IEnumerator BarUpdateAnimRoutine(int value)
        {
            barBg?.gameObject?.SetActive(true);

            var view = origin!.UnitModel.view;
            var max = view.model.MaxHp;
            var start = Mathf.InverseLerp(-756f, -284f, barBg?.transform?.localPosition.x ?? 0f);
            var end = value == 0 ? 0f : (float)max.Min(value) / (float)max;

            if (start == 0 && end == 0)
            {
                barBg?.gameObject?.SetActive(false);
                healthTxt?.gameObject?.SetActive(false);

                yield break;
            }

            var tick = 0f;

            while (tick < 1f)
            {
                barBg?.transform?.localPosition = Vector3.Lerp(new Vector3(-756f, -44f, 0f), new Vector3(-284f, -44f, 0f), Mathf.Lerp(start, end, tick));

                tick += Time.deltaTime;

                yield return null;
            }

            barBg?.transform?.localPosition = Vector3.Lerp(new Vector3(-756f, -44f, 0f), new Vector3(-284f, -44f, 0f), end);

            if (1 > value)
            {
                barBg?.gameObject?.SetActive(false);
            }
        }

        IEnumerator TxtUpdateAnimRoutine(int value)
        {
            healthTxt?.gameObject?.SetActive(true);

            var view = origin!.UnitModel.view;
            var max = view.model.MaxHp;
            var start = float.Parse(healthTxt?.text ?? "0");
            var end = Mathf.Floor(value);

            if (start == 0 && end == 0)
            {
                barBg?.gameObject?.SetActive(false);
                healthTxt?.gameObject?.SetActive(false);

                yield break;
            }

            var tick = 0f;

            while (tick < 1f)
            {
                healthTxt?.text = Mathf.FloorToInt(Mathf.Lerp(start, end, tick)).ToString();

                tick += Time.deltaTime;

                yield return null;
            }

            healthTxt?.text = end.ToString();

            if (1 > value)
            {
                healthTxt?.gameObject?.SetActive(false);
            }
        }

        private BattleCharacterProfileUI? origin;

        private Image? barBg;

        private Text? healthTxt;

        private Coroutine? barAnim;

        private Coroutine? txtAnim;

        static AccessTools.FieldRef<BattleCharacterProfileUI, Transform> _uiRootRef
            = typeof(BattleCharacterProfileUI).FieldRefAccess<Transform>("uiRoot");

        static AccessTools.FieldRef<BattleCharacterProfileUI, BattleCharacterProfileUI.HpBar> _hpBarRef
            = typeof(BattleCharacterProfileUI).FieldRefAccess<BattleCharacterProfileUI.HpBar>("hpBar");

        static AccessTools.FieldRef<BattleCharacterProfileUI, Text> _txt_hpRef
            = typeof(BattleCharacterProfileUI).FieldRefAccess<Text>("txt_hp");
    }
}
