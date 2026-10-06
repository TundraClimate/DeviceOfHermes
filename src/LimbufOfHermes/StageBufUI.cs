using HarmonyLib;
using DeviceOfHermes.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LimbufOfHermes;

internal class StageBufListUI : BattleUIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action OnCleanup = () => { };

    public override void OnInitialize()
    {
        Reset();

        OnCleanup.Invoke();
    }

    public override void OnEndBattle()
    {
        Reset();

        OnCleanup.Invoke();
    }

    void Awake()
    {
        InitUI(1003);

        gameObject.AddComponent<GraphicRaycaster>();

        gameObject.AddContainer(InitOrigin);
        gameObject.AddContainer(InitExpanded);

        origin!.Disable();
        origin!.Hide();

        expanded!.Disable();
        expanded!.Hide();
    }

    public void UpdateBufUI(List<StageBufBase> bufs)
    {
        if (!active && bufs.Count > 0)
        {
            active = true;

            origin!.Enable();
            origin!.Show();
        }

        if (active && bufs.Count <= 0)
        {
            active = false;

            origin!.Disable();
            origin!.Hide();
        }

        if (!active)
        {
            return;
        }

        if (originIcon!.sprite != bufs[0].GetBufIcon())
        {
            originIcon.sprite = bufs[0].GetBufIcon();
            originIcon.rectTransform.sizeDelta = new(250f, 250f);
        }

        var width = bufs.Count * 75 + 100;

        if (expandedBgRect!.sizeDelta.y != width)
        {
            SetWidth(width);
        }

        if (expandedUnitList.Count > bufs.Count)
        {
            for (var i = bufs.Count; expandedUnitList.Count > i; i++)
            {
                UnityObject.Destroy(expandedUnitList[i]);
            }
        }

        foreach (var (i, buf) in bufs.Enumerate())
        {
            if (expandedUnitList.Count > i)
            {
                var bufUI = expandedUnitList[i];

                bufUI.buf = buf;
            }
            else
            {
                var go = expandedUnitRoot!.AddChildObject($"{i}", LayerMask.LayerToName(expandedUnitRoot!.layer));
                var bufUI = go.AddComponent<StageBufUI>();

                bufUI.Init(buf, i);

                expandedUnitList.Add(bufUI);
            }
        }

        foreach (var ui in expandedUnitList)
        {
            ui.UpdateUI();
        }
    }

    void Reset()
    {
        origin!.Disable();
        origin!.Hide();

        expanded!.Disable();
        expanded!.Hide();

        active = false;
    }

    void SetWidth(float width)
    {
        expandedBgRect!.sizeDelta = new(100f, width);
        expandedFrameRect!.sizeDelta = new(100f, width);
    }

    void InitOrigin(GameObject root)
    {
        origin = root.AddComponent<CanvasGroup>();

        root.name = "StageBufList";

        root.MoveTo(new(0f, 0.8f));

        root.GetComponent<RectTransform>().Let(rect =>
        {
            rect.sizeDelta = new(100f, 150f);
            rect.pivot = new(0f, 0.5f);
        });

        root.AddContainer(bg =>
        {
            bg.name = "Bg";

            bg.MoveTo(new(0f, 0.5f));

            var image = bg.AddComponent<Image>();

            image.color = new(0f, 0f, 0f, 0.3f);
            image.rectTransform.pivot = new(0f, 0f);
            image.rectTransform.sizeDelta = new(100f, 100f);
            image.rectTransform.localEulerAngles = new(0f, 0f, -90f);
        });

        UnityObject.Instantiate(_sephirahFrameImgRef(BattleManagerUI.Instance.ui_emotionInfoBar).gameObject, root.transform).Also(frame =>
        {
            frame.name = "Frame";

            frame.MoveTo(new(0f, 0.5f));

            (frame.transform as RectTransform)?.pivot = new(0f, 0f);
            (frame.transform as RectTransform)?.sizeDelta = new(100f, 100f);
            frame.transform.localEulerAngles = new(0f, 0f, -90f);
        });

        root.AddContainer(icon =>
        {
            icon.name = "Icon";

            icon.MoveTo(new(0.5f, 0.2f));

            originIcon = icon.AddComponent<Image>();

            icon.transform.localScale = new(0.3f, 0.3f, 1f);
        });
    }

    void InitExpanded(GameObject root)
    {
        expanded = root.AddComponent<CanvasGroup>();

        root.name = "StageBufList_Expanded";

        root.MoveTo(new(0f, 0.8f));

        root.GetComponent<RectTransform>().Let(rect =>
        {
            rect.sizeDelta = new(1000f, 300f);
            rect.pivot = new(0f, 0.5f);
        });

        root.AddContainer(bg =>
        {
            bg.name = "Bg";

            bg.MoveTo(new(0f, 0.5f));

            var image = bg.AddComponent<Image>();

            image.color = new(0f, 0f, 0f, 0.3f);
            image.rectTransform.pivot = new(0f, 0f);
            image.rectTransform.sizeDelta = new(150f, 1000f);
            image.rectTransform.localEulerAngles = new(0f, 0f, -90f);

            expandedBgRect = image.rectTransform;
        });

        UnityObject.Instantiate(_sephirahFrameImgRef(BattleManagerUI.Instance.ui_emotionInfoBar).gameObject, root.transform).Also(frame =>
        {
            frame.name = "Frame";

            frame.MoveTo(new(0f, 0.5f));

            expandedFrameRect = frame.transform as RectTransform;

            (frame.transform as RectTransform)?.pivot = new(0f, 0f);
            (frame.transform as RectTransform)?.sizeDelta = new(150f, 1000f);
            frame.transform.localEulerAngles = new(0f, 0f, -90f);
        });

        root.AddContainer(units =>
        {
            units.name = "Units";

            units.MoveTo(new(0f, 0.5f));

            units.transform.localPosition = new(100f, 0f, 0f);

            expandedUnitRoot = units;
        });
    }

    public void OnPointerEnter(PointerEventData data)
    {
        expanded!.Enable();
        expanded!.Show();

        origin!.Disable();
        origin!.Hide();
    }

    public void OnPointerExit(PointerEventData data)
    {
        origin!.Enable();
        origin!.Show();

        expanded!.Disable();
        expanded!.Hide();
    }

    private CanvasGroup? origin;

    private CanvasGroup? expanded;

    private bool active;

    private Image? originIcon;

    private RectTransform? expandedBgRect;

    private RectTransform? expandedFrameRect;

    private GameObject? expandedUnitRoot;

    private List<StageBufUI> expandedUnitList = new();

    static AccessTools.FieldRef<BattleEmotionInfoManagerUI, Image> _sephirahFrameImgRef
        = AccessTools.FieldRefAccess<BattleEmotionInfoManagerUI, Image>("sephirahFrameImg");
}

[RequireComponent(typeof(RectTransform))]
internal class StageBufUI : MonoBehaviour
{
    public void Init(BattleUnitBuf buf, int idx)
    {
        gameObject.name = "StageBuf";

        this.buf = buf;

        mainRect = gameObject.GetComponent<RectTransform>();

        mainRect.sizeDelta = new(150f, 100f);
        mainRect.pivot = new(0f, 0.3f);
        mainRect.localPosition = new(idx * 75, 0, 0);
        mainRect.localScale = new(1.5f, 1.5f, 1f);

        var info = BattleManagerUI.Instance.ui_unitInformation;

        UnityObject.Instantiate(_BuffIconSlotRef(_bufflistmanagerRef(info))[0], gameObject.transform).Let(icon =>
        {
            slot = icon;

            icon.name = "Icon";

            icon.gameObject.MoveTo(new(0f, 0f));

            (icon.transform as RectTransform)?.sizeDelta = new(50f, 50f);

            icon.SetCamera(info.GetCanvas().worldCamera);
            icon.SetData(buf.GetBufIcon(), buf.stack);
            icon.SetToolTipData(buf);
        });
    }

    public void UpdateUI()
    {
        slot!.SetData(buf!.GetBufIcon(), buf.stack);
        slot.SetToolTipData(buf);
    }

    private RectTransform? mainRect;

    private BattleUnitInformationUI_BuffSlot? slot;

    public BattleUnitBuf? buf;

    static AccessTools.FieldRef<BattleUnitInformationUI, BattleUnitInformationUI_BuffList> _bufflistmanagerRef
        = AccessTools.FieldRefAccess<BattleUnitInformationUI, BattleUnitInformationUI_BuffList>("bufflistmanager");

    static AccessTools.FieldRef<BattleUnitInformationUI_BuffList, List<BattleUnitInformationUI_BuffSlot>> _BuffIconSlotRef
        = AccessTools.FieldRefAccess<BattleUnitInformationUI_BuffList, List<BattleUnitInformationUI_BuffSlot>>("BuffIconSlot");
}
