using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace DeviceOfHermes;

internal class BattleFloatingDialog : MonoBehaviour, IPointerDownHandler
{
    public static void PlayDialog(BattleUnitView view, string text, float duration, float overhead, float scale)
    {
        var dialog = CreateText(view);

        dialog.rootCanvas?.enabled = true;
        dialog.text?.text = text;
        dialog.textTransform?.localScale = new(scale, scale, 1f);
        dialog.gameObject.transform.localPosition = new(0f, overhead, 9f);

        IEnumerator F()
        {
            yield return CommonCoroutine.CanvasGroupFadein(dialog.cg!, 0.2f);

            dialog.disposable = true;

            yield return CommonCoroutine.CanvasGroupFadeout(dialog.cg!, duration, 0.2f);
        }

        dialog.StartCoroutine(F());
    }

    static BattleFloatingDialog CreateText(BattleUnitView view)
    {
        var go = UnityObject.Instantiate(view.dialogUI.gameObject, view.dialogUI.transform.parent);

        go.name = "[DoH]BattleFloatingDialog";

        go.GetComponent<BattleDialogUI>().enabled = false;

        var dialog = go.AddComponent<BattleFloatingDialog>();

        dialog.rootCanvas = go.GetComponent<Canvas>();
        dialog.cg = go.GetComponent<CanvasGroup>();

        dialog.cg.Enable();

        var textBase = go.transform.Find("Scale").transform.Find("[Text]AbnormalityDialog").Also(go =>
        {
            dialog.textTransform = go.transform;

            go.name = "FloatingDialog";

            go.GetComponent<TextMeshProMaterialSetter>().enabled = false;
            go.GetComponent<UITextDataLoader>().enabled = false;
            go.GetComponent<AbnormalityDlgEffect>().enabled = false;
            go.GetComponent<EventTrigger>().enabled = false;
        });

        dialog.text = textBase.GetComponent<TextMeshProUGUI>().Also(text =>
        {
            text.text = "";
            text.alpha = 1;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.raycastTarget = true;
            text.enableAutoSizing = false;
            text.enableWordWrapping = false;

            LocalizedFontSetter.Instance.SetLocalizedFont(text, FontType.FONT_HEAD);

            text.fontMaterial = new Material(text.fontMaterial).Also(mat =>
            {
                mat.SetColor(ShaderUtilities.ID_GlowColor, new(0, 0, 0, 0));
                mat.SetColor(ShaderUtilities.ID_OutlineColor, Color.white);
                mat.EnableKeyword(ShaderUtilities.Keyword_Underlay);
                mat.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);
                mat.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.6f);
                mat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0f);
            });
        });

        return dialog;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (disposable && !destroy)
        {
            StopAllCoroutines();
            StartCoroutine(CommonCoroutine.CanvasGroupFadeout(cg!, 0f, 0.2f));

            destroy = true;
        }
    }

    void Update()
    {
        if (this.disposable && cg?.alpha == 0)
        {
            UnityObject.Destroy(gameObject);
        }
    }

    Canvas? rootCanvas;

    CanvasGroup? cg;

    Transform? textTransform;

    TextMeshProUGUI? text;

    bool disposable;

    bool destroy;
}
