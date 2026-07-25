using HarmonyExtension;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DeviceOfHermes.UI;

/// <summary>A ui of common levelup</summary>
public class CommonLevelUpUI(string name) : CustomLevelUpUI
{
    /// <summary>Icon of floor</summary>
    public Sprite? icon;

    /// <summary>Init overrides</summary>
    public override void Init(LevelUpUI ui)
    {
        foreach (var lv in ui._emotionLevels)
        {
            lv.gameObject.SetActive(false);
        }

        ((GameObject)typeof(LevelUpUI).Field("ob_EmotionPerUI").GetValue(ui)).SetActive(false);
        ((TextMeshProUGUI)typeof(LevelUpUI).Field("txt_SelectDesc").GetValue(ui)).text = _name;

        if (icon is not null)
        {
            ((Image)typeof(LevelUpUI).Field("FloorIconImage").GetValue(ui)).sprite = icon;
        }
    }

    /// <summary>Drop overrides</summary>
    public override void Drop(LevelUpUI ui)
    {
        foreach (var lv in ui._emotionLevels)
        {
            lv.gameObject.SetActive(true);
        }

        ((GameObject)typeof(LevelUpUI).Field("ob_EmotionPerUI").GetValue(ui)).SetActive(true);
    }

    private string _name = name;
}

