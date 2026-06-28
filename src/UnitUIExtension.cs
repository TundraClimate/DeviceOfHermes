using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using HarmonyLib;
using HarmonyExtension;
using LOR_BattleUnit_UI;

namespace DeviceOfHermes;

/// <summary>The extensions of ui on unit model</summary>
public static class UnitUIExtension
{
    internal static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.UnitUIExtension");

        harmony.CreateClassProcessor(typeof(PatchUpdator)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnAddUnit)).Patch();
    }

    /// <summary>Says by unit on character dialog</summary>
    /// <param name="owner">A unit that says dialog</param>
    /// <param name="txt">A text of show dialog</param>
    public static void Say(this BattleUnitModel owner, string txt)
    {
        BattleManagerUI.Instance.ui_unitListInfoSummary.DisplayDlg(txt, owner, false, MentalState.Positive);
    }

    /// <summary>Says by unit on character overhead</summary>
    /// <param name="view">A unit view to display text</param>
    /// <param name="txt">A text to display</param>
    /// <param name="duration">The duration of display without fade</param>
    /// <param name="overhead">A height of on overhead</param>
    /// <param name="scale">A text scale</param>
    public static void Say(this BattleUnitView view, string txt, float duration = 1f, float overhead = 3.2f, float scale = 0.7f)
    {
        var cg = view.dialogUI.GetComponent<CanvasGroup>();
        var dialog = view.dialogUI;
        var txtAbnormalityDlg = _txtAbnormalityDlg(dialog);
        var canvas = _canvas(dialog);
        ref var routine = ref _routine(dialog);

        txtAbnormalityDlg.text = txt;
        txtAbnormalityDlg.fontMaterial.SetColor("_GlowColor", new Color(0, 0, 0, 0));
        txtAbnormalityDlg.color = new Color(255, 255, 255, 255);

        dialog.StopAllCoroutines();

        if (_table.TryGetValue(view, out var ctx))
        {
            if (ctx.overhead != overhead)
            {
                _table.Remove(view);
                _table.Add(view, new DialogContext(dialog) { overhead = overhead });
            }
        }
        else
        {
            _table.Add(view, new DialogContext(dialog) { overhead = overhead });
        }

        canvas.enabled = true;
        routine = dialog.StartCoroutine(Routine(canvas, cg, duration, view, scale));
    }

    static IEnumerator Routine(Canvas canvas, CanvasGroup cg, float duration, BattleUnitView vRef, float scale)
    {
        var reScale = cg.transform.localScale;

        cg.transform.localScale = new Vector3(scale, scale, 1f);

        yield return CommonCoroutine.CanvasGroupFadein(cg, 0.2f);
        yield return CommonCoroutine.CanvasGroupFadeout(cg, duration, 0.2f);

        cg.transform.localScale = reScale;
        canvas.enabled = false;

        _table.Remove(vRef);

        yield break;
    }

    /// <summary>Add effect to unit canvas</summary>
    public static void AddEffect(this BattleUnitView view, Sprite effectImg, Vector2 pos, float duration = 1f, float feed = 0f, Vector2? sizeDelta = null, float sizeScale = 1f)
    {
        if (!_unitRootCanvas.TryGetValue(view, out var go))
        {
            return;
        }

        go.AddContainer(image =>
        {
            var img = image.Also(i => i.name = effectImg.name)
                .MoveTo(pos)
                .SetImage(effectImg, sizeDelta);

            if (sizeDelta is null)
            {
                img.transform.localScale *= 0.01f;
            }

            img.transform.localScale *= sizeScale;

            img.StartCoroutine(CommonCoroutine.ImageFadeout(img, duration, feed));
            UnityEngine.Object.Destroy(img, duration + feed);
        });
    }

    /// <summary>Returns selected speeddice</summary>
    public static int GetClickedSpeedDice(this BattleUnitModel self)
    {
        for (var i = 0; self.speedDiceCount > i; i++)
        {
            var dui = self.view?.speedDiceSetterUI?.GetSpeedDiceByIndex(i);

            if (dui is null)
            {
                continue;
            }

            if (_isClicked(dui))
            {
                return i;
            }
        }

        return -1;
    }

    [HarmonyPatch(typeof(BattleDialogUI), "Update")]
    class PatchUpdator
    {
        static void Postfix(BattleUnitView ___view)
        {
            if (_table.TryGetValue(___view, out var ctx))
            {
                var ui = ctx.ui;
                var overhead = ctx.overhead;

                ui.transform.localPosition = new Vector3(0f, overhead, 0f);
            }
        }
    }

    [HarmonyPatch(typeof(BattleObjectLayer), "AddUnit")]
    class PatchOnAddUnit
    {
        static void Postfix(BattleUnitModel model)
        {
            _unitRootCanvas.GetValue(
                model.view,
                _ => model.view.characterRotationCenter.gameObject.AddChildObject("BattleEffect", "Effect")
                    .Also(go =>
                    {
                        go.AddComponent<Canvas>();
                    })
            );
        }
    }

    private static ConditionalWeakTable<BattleUnitView, DialogContext> _table = new();

    private static ConditionalWeakTable<BattleUnitView, GameObject> _unitRootCanvas = new();

    private static AccessTools.FieldRef<BattleDialogUI, TextMeshProUGUI> _txtAbnormalityDlg
        = typeof(BattleDialogUI).FieldRefAccess<TextMeshProUGUI>("_txtAbnormalityDlg");

    private static AccessTools.FieldRef<BattleDialogUI, Canvas> _canvas
        = typeof(BattleDialogUI).FieldRefAccess<Canvas>("_canvas");

    private static AccessTools.FieldRef<BattleDialogUI, Coroutine> _routine
        = typeof(BattleDialogUI).FieldRefAccess<Coroutine>("_routine");

    private static AccessTools.FieldRef<SpeedDiceUI, bool> _isClicked
        = typeof(SpeedDiceUI).FieldRefAccess<bool>("isClicked");

    class DialogContext
    {
        public DialogContext(BattleDialogUI _ui)
        {
            ui = _ui;
        }

        public BattleDialogUI ui;

        public float overhead = 3.2f;
    }
}
