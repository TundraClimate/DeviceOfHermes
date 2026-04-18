using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes;

/// <summary>The extensions of ui on unit model</summary>
public static class UnitUIExtension
{
    static UnitUIExtension()
    {
        var harmony = new Harmony("DeviceOfHermes.UnitUIExtension");

        harmony.CreateClassProcessor(typeof(PatchUpdator)).Patch();
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

        if (routine != null)
        {
            dialog.StopCoroutine(routine);
            routine = null;
            canvas.enabled = false;
        }

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
        cg.blocksRaycasts = true;
        routine = dialog.StartCoroutine(Routine(canvas, cg, duration, view, scale));
    }

    static IEnumerator Routine(Canvas canvas, CanvasGroup cg, float duration, BattleUnitView vRef, float scale)
    {
        var reScale = cg.transform.localScale;

        cg.transform.localScale = new Vector3(scale, scale, 1f);

        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * 5f;
            cg.alpha = elapsed;
            yield return null;
        }

        yield return YieldCache.WaitForSeconds(duration);

        elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * 5f;
            cg.alpha = 1f - elapsed;
            yield return null;
        }

        cg.transform.localScale = reScale;
        canvas.enabled = false;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        _table.Remove(vRef);

        yield break;
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

    private static ConditionalWeakTable<BattleUnitView, DialogContext> _table = new();

    private static AccessTools.FieldRef<BattleDialogUI, TextMeshProUGUI> _txtAbnormalityDlg
        = typeof(BattleDialogUI).FieldRefAccess<TextMeshProUGUI>("_txtAbnormalityDlg");

    private static AccessTools.FieldRef<BattleDialogUI, Canvas> _canvas
        = typeof(BattleDialogUI).FieldRefAccess<Canvas>("_canvas");

    private static AccessTools.FieldRef<BattleDialogUI, Coroutine> _routine
        = typeof(BattleDialogUI).FieldRefAccess<Coroutine>("_routine");

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
