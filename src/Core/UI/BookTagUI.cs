using HarmonyLib;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeviceOfHermes.UI;

internal class BookTagUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.UI.BookTagUI");

        harmony.CreateClassProcessor(typeof(PatchOnSetData)).Patch();

        foreach (var (id, data) in HermesConstants.BuiltinBookTagData)
        {
            var lid = new LorId(int.Parse(id));

            BookTag.SetTags(lid, data);
        }
    }

    public static BookTagUI CreateUI(UIEquipPagePreviewPanel __instance)
    {
        var ownTransform = __instance.gameObject.transform.Find("[Rect]Content");
        var root = ownTransform.gameObject.AddChildObject("[DoH]BookTag");

        root.transform.localPosition = new(15, -50, 0);

        var invBtn = SceneManager.GetActiveScene().GetRootGameObjects()
            .FlatMap(go => go.GetComponentsInChildren<Transform>(true))
            .FirstOrDefault(tf => tf.gameObject.name == "[Button]MenuItem_Invitation");

        var sprite = invBtn.Find("Image").GetComponent<Image>().sprite;
        var icon = root.AddChildObject("Icon");

        icon.AddComponent<Image>().sprite = sprite;
        icon.transform.localPosition = new(220f, -35f, 0f);
        icon.transform.localScale = new(0.5f, 0.5f, 1f);

        var tagListRoot = root.AddChildObject("[Transform]TagList");
        var frameRoot = ownTransform.Find("[Rect]Frame").gameObject;
        var ui = root.AddComponent<BookTagUI>();
        var bgRect = UnityObject.Instantiate(frameRoot.transform.Find("[Image]BlackBg").gameObject, tagListRoot.transform).GetComponent<RectTransform>();
        var topRect = UnityObject.Instantiate(frameRoot.transform.Find("[Image]UpperFrame").gameObject, tagListRoot.transform).GetComponent<RectTransform>();
        var bookName = (TextMeshProUGUI)__instance.GetType().Field("txt_BookName").GetValue(__instance);
        var uiText = UnityObject.Instantiate(bookName.gameObject, tagListRoot.transform);

        tagListRoot.transform.localPosition = new(-220f, -3.2f, 0f);
        bgRect.localPosition = new(368.06f, 4.6f, 0f);
        bgRect.sizeDelta = new(200f, 470f);
        topRect.localPosition = new(368.06f, 0.94f, 0f);
        topRect.sizeDelta = new(200f, 500f);
        uiText.gameObject.transform.localPosition = new(400f, -33.7f, 0f);
        uiText.GetComponent<TextMeshProUGUI>().text = "Tags";
        uiText.name = "[Text]BookTagTop";

        ui._tagList = tagListRoot;
        ui._bgRect = bgRect;
        ui._frameRect = topRect;

        foreach (var i in 0..10)
        {
            var txtObj = UnityObject.Instantiate(uiText.gameObject, tagListRoot.transform);
            var txt = txtObj.GetComponent<TextMeshProUGUI>();

            txtObj.gameObject.transform.localPosition = new(400f, Mathf.Lerp(-72.84f, -451.8599f, i / 10f), 0f);
            txtObj.name = $"[Text]Tag_{i}";

            txt.fontSize = 15f;
            txt.fontSizeMax = 15f;
            txt.text = "";

            ui._txtList.Add(txt);
        }

        return ui;
    }

    public void Init(UIEquipPagePreviewPanel __instance, BookModel model)
    {
        gameObject.SetActive(true);
        _tagList?.SetActive(false);

        var tags = model.GetTags().Take(10).ToList();

        if (tags.Count == 0)
        {
            gameObject.SetActive(false);

            return;
        }

        var color = UIColorManager.Manager.GetEquipRarityColor(model.GetRarity());

        _frameRect?.gameObject.GetComponent<Image>().color = color;

        foreach (var i in 0..this._txtList.Count)
        {
            if (i < tags.Count)
            {
                var (id, del) = tags[i].StripSuffix(":Delete") is not null ? (tags[i][..(tags[i].Length - 7)], true) : (tags[i], false);

                var text = TextDataModel.GetText($"DeviceOfHermes_BookTag_{id}") is string txt && !string.IsNullOrEmpty(txt) ? txt : id;
                var elem = this._txtList[i];
                var setter = elem.GetComponent<TextMeshProMaterialSetter>();

                elem.text = $"- {text}";

                setter.underlayColor = color;

                if (del)
                {
                    elem.text = elem.text.Strikethrough();
                    elem.color = new(0.8f, 0.2f, 0.2f, 1f);

                    setter.underlayColor = new(0.8f, 0.2f, 0.2f, 1f);
                }

                setter.InitMaterialProperty();
            }
            else
            {
                this._txtList[i].text = "";
            }
        }

        var uiHeight = 60f + Mathf.Lerp(0f, 410f, tags.Count / 10f);

        _bgRect?.sizeDelta = new(200f, uiHeight);
        _frameRect?.sizeDelta = new(200f, uiHeight);
    }

    public void OnPointerEnter(PointerEventData ev)
    {
        _tagList?.SetActive(true);
    }

    public void OnPointerExit(PointerEventData ev)
    {
        _tagList?.SetActive(false);
    }

    GameObject? _tagList = null;

    RectTransform? _bgRect = null;

    RectTransform? _frameRect = null;

    List<TextMeshProUGUI> _txtList = [];

    [HarmonyPatch(typeof(UIEquipPagePreviewPanel), "SetData")]
    class PatchOnSetData
    {
        static Exception Finalizer(Exception __exception, UIEquipPagePreviewPanel __instance, BookModel book)
        {
            ui ??= CreateUI(__instance);

            ui.Init(__instance, book);

            return __exception;
        }

        static BookTagUI? ui;
    }
}
