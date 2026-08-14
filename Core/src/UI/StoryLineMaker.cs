using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using HarmonyLib;
using HarmonyExtension;

using UIUIController = UI.UIController;

namespace DeviceOfHermes.UI;

/// <summary>Make storyline</summary>
public static class StoryLineMaker
{
    static StoryLineMaker()
    {
        var harmony = new Harmony("DeviceOfHermes.UI.StoryLineMaker");

        harmony.CreateClassProcessor(typeof(PatchStoryLine)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnSelectSlot)).Patch();
        harmony.CreateClassProcessor(typeof(PatchPanelType)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnClickSendButton)).Patch();
        harmony.CreateClassProcessor(typeof(PatchAlarmText)).Patch();
    }

    /// <summary>Enable unrestricted invitation map</summary>
    public static void EnableUnrestrictedMap()
    {
        if (!_unrestricted)
        {
            _unrestricted = true;
        }
    }

    /// <summary>Add icon</summary>
    public static void RegisterIcon(string storyType, float x, float y)
    {
        _icons[storyType] = (x, y);
    }

    /// <summary>Add special icon</summary>
    public static void RegisterSpecialIcon(string storyType, float x, float y, Sprite banner, Func<string> sendText)
    {
        RegisterIcon(storyType, x, y);

        _uninviteds[storyType] = (banner, sendText);
    }

    /// <summary>Add line</summary>
    public static void RegisterLine(float startX, float startY, float endX, float endY)
    {
        _lines.Add(((startX, startY), (endX, endY)));
    }

    [HarmonyPatch(typeof(UIStoryProgressPanel), "SelectedSlot")]
    class PatchOnSelectSlot
    {
        static Exception Finalizer(Exception __exception, UIStoryProgressPanel __instance, UIStoryProgressIconSlot slot, bool isSelected)
        {
            var ty = slot._storyData[0].storyType;

            if (_icons.ContainsKey(ty) &&
              UIUIController.Instance.CurrentUIPhase == UIPhase.Invitation && isSelected)
            {
                (UIUIController.Instance?.GetUIPanel(UIPanelType.Invitation) as UIInvitationPanel)?
                    .InvRightMainPanel.SetCustomInvToggle(true);

                __instance.currentSlot = slot;

                if (_uninviteds.TryGetValue(ty, out var res))
                {
                    __instance.currentSlot.currentStory = UIStoryLine.BlackSilence;

                    __instance.StartCoroutine(LateSelect(__instance, res.Item1));
                }
            }

            return __exception;
        }

        static IEnumerator LateSelect(UIStoryProgressPanel __instance, Sprite banner)
        {
            yield return null;

            __instance.SelectedStory(true, 0);

            var right = (UIUIController.Instance?.GetUIPanel(UIPanelType.Invitation) as UIInvitationPanel)?.InvRightMainPanel;

            if (right is not null)
            {
                _imgContentRef(right).sprite = banner;
            }
        }
    }

    [HarmonyPatch(typeof(UIStoryProgressPanel), "SetStoryLine")]
    class PatchStoryLine
    {
        static Exception Finalizer(Exception __exception, UIStoryProgressPanel __instance)
        {
            if (_unrestricted)
            {
                _scrollRectRef(__instance).movementType = ScrollRect.MovementType.Unrestricted;
            }

            foreach (var (story, data) in _stories)
            {
                data.SetSlotData(story);

                if (story[0].currentState != StoryState.Close)
                {
                    data.SetActiveStory(true);
                }
                else
                {
                    data.SetActiveStory(false);
                }
            }

            if (_phase is not null && _phase == UIUIController.Instance.CurrentUIPhase || _fullinit == true)
            {
                return __exception;
            }

            var icon = _iconListRef(__instance).Find(i => i.currentStory == UIStoryLine.Rats);

            if (icon is not null)
            {
                foreach (var (storyType, (x, y)) in _icons)
                {
                    var data = StageClassInfoList.Instance.GetAllDataList().FindAll(data => data.storyType == storyType);

                    _stories[data] = CreateNewIcon(__instance, icon, data, x, y);
                }

                ref var conLines = ref _connectLineRef(icon);

                foreach (var ((startX, startY), (endX, endY)) in _lines)
                {
                    conLines.Add(CreateNewLine(conLines[0], startX, startY, endX, endY));
                }
            }

            if (_fullinit == false)
            {
                _fullinit = true;

                return __exception;
            }

            _fullinit = false;
            _phase = UIUIController.Instance?.CurrentUIPhase;

            return __exception;
        }

        static UIStoryProgressIconSlot CreateNewIcon(
            UIStoryProgressPanel __instance,
            UIStoryProgressIconSlot icon,
            List<StageClassInfo> data,
            float x,
            float y
        )
        {
            var newIcon = UnityEngine.Object.Instantiate(icon, icon.transform.parent)
                .GetComponentInChildren<UIStoryProgressIconSlot>();

            newIcon.currentStory = icon.currentStory;
            newIcon.Initialized(__instance);

            ref var originalcolor = ref _originalcolorRef(newIcon);

            originalcolor = new Color(1, 1, 1, 1);

            var ofsy = -125f;

            newIcon.gameObject.transform.localPosition = new Vector3(x, y + ofsy);

            newIcon.SetSlotData(data);

            return newIcon;
        }

        static GameObject CreateNewLine(GameObject lineBase, float startX, float startY, float endX, float endY)
        {
            var ofsy = 250;

            (float, float) posA = (startX, startY + ofsy);
            (float, float) posB = (endX, endY + ofsy);

            var (x, y) = ((posA.Item1 + posB.Item1) / 2, (posA.Item2 + posB.Item2) / 2);
            var dx = posB.Item1 - posA.Item1;
            var dy = posB.Item2 - posA.Item2;
            var rotate = Math.Atan2(dy, dx) * 180f / Math.PI;
            var scale = (Math.Sqrt(dx * dx + dy * dy) / 200f);

            var newLine = UnityEngine.Object.Instantiate(lineBase, lineBase.transform.parent);

            newLine.transform.localPosition = new Vector3(x, y);
            newLine.transform.localRotation = Quaternion.Euler(1f, 1f, ((float)rotate));
            newLine.transform.localScale = new Vector3(((float)scale), 1f, 1f);

            return newLine;
        }

        static Dictionary<List<StageClassInfo>, UIStoryProgressIconSlot> _stories = new();
    }

    [HarmonyPatch(typeof(UIInvitationPanel), "GetTheBlueReverberationPrimaryStage")]
    class PatchPanelType
    {
        static Exception Finalizer(Exception __exception, UIInvitationPanel __instance, ref UIStoryLine __result)
        {
            if (_uninviteds.ContainsKey(__instance.CurrentStage?.storyType ?? ""))
            {
                __result = UIStoryLine.BlackSilence;
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(UIInvitationRightMainPanel), "OnClickSendButtonForBlue")]
    class PatchOnClickSendButton
    {
        static bool Prefix(UIInvitationRightMainPanel __instance, UIInvitationPanel ___invPanel)
        {
            var story = ___invPanel?.CurrentStage?.storyType ?? "";

            if (__instance.currentinvState == InvitationApply_State.BlackSilence && _uninviteds.TryGetValue(story, out var res))
            {
                UIAlarmPopup.instance.SetAlarmTextForBlue((UIAlarmType)9999, reply =>
                {
                    if (reply)
                    {
                        var bookRecipe = StageClassInfoList.Instance.GetAllDataList().Find(x => x.storyType == story);

                        UIUIController.Instance.SetStageInfo(bookRecipe);
                        StageController.Instance.SetCurrentSephirah(SephirahType.Keter);
                        StageController.Instance.InitStageByInvitation(bookRecipe, new List<LorId>());
                        GameSceneManager.Instance.ActivateUIController(false);
                        UIBgScreenChangeAnim.Instance.StartBg(UIScreenChangeType.EnterBattleSetting);
                        UISoundManager.instance.PlayEffectSound(UISoundType.Ui_Invite);
                    }
                }, story, UIStoryLine.BlackSilence);

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(UIAlarmPopup), "SetAlarmTextForBlue")]
    class PatchAlarmText
    {
        static Exception Finalizer(Exception __exception, TextMeshProUGUI ___txt_alarmForBlue, TextMeshProUGUI[] ___txt_alarmEffectTextForBlues, UIAlarmType alarmtype, string param)
        {
            if (alarmtype == (UIAlarmType)9999 && _uninviteds.TryGetValue(param, out var res))
            {
                var text = res.Item2();

                ___txt_alarmForBlue.SetText(text);

                foreach (var txt in ___txt_alarmEffectTextForBlues)
                {
                    txt.SetText(text);
                }
            }

            return __exception;
        }
    }

    private static bool? _fullinit;

    private static UIPhase? _phase;

    private static bool _unrestricted;

    private static Dictionary<string, (float, float)> _icons = new();

    private static Dictionary<string, (Sprite, Func<string>)> _uninviteds = new();

    private static List<((float, float), (float, float))> _lines = new();

    private static AccessTools.FieldRef<UIStoryProgressPanel, List<UIStoryProgressIconSlot>> _iconListRef
        = typeof(UIStoryProgressPanel).FieldRefAccess<List<UIStoryProgressIconSlot>>("iconList");

    private static AccessTools.FieldRef<UIStoryProgressIconSlot, List<GameObject>> _connectLineRef
        = typeof(UIStoryProgressIconSlot).FieldRefAccess<List<GameObject>>("connectLineList");

    private static AccessTools.FieldRef<UIStoryProgressIconSlot, Color> _originalcolorRef
        = typeof(UIStoryProgressIconSlot).FieldRefAccess<Color>("originalcolor");

    private static AccessTools.FieldRef<UIStoryProgressPanel, ScrollRect> _scrollRectRef
        = typeof(UIStoryProgressPanel).FieldRefAccess<ScrollRect>("scroll_viewPort");

    private static AccessTools.FieldRef<UIInvitationRightMainPanel, Image> _imgContentRef
        = typeof(UIInvitationRightMainPanel).FieldRefAccess<Image>("img_endcontents_content");
}
