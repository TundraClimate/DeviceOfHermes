using HarmonyExtension;
using UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DeviceOfHermes.UI;

/// <summary>A ui of personal levelup ui</summary>
public class PersonalLevelUpUI(BattleUnitModel owner) : CustomLevelUpUI
{
    /// <summary>Name of ui</summary>
    public string? name;

    /// <summary>Icon of floor</summary>
    public Sprite? icon;

    /// <summary>Init overrides</summary>
    public override void Init(LevelUpUI ui)
    {
        ((GameObject)typeof(LevelUpUI).Field("ob_EmotionPerUI").GetValue(ui)).SetActive(false);

        if (name is not null)
        {
            ((TextMeshProUGUI)typeof(LevelUpUI).Field("txt_SelectDesc").GetValue(ui)).text = name;
        }

        var image = ((Image)typeof(LevelUpUI).Field("FloorIconImage").GetValue(ui));

        if (icon is not null)
        {
            image.sprite = icon;
        }
        else
        {
            var texture = (RenderTexture)UICharacterRenderer.Instance.GetRenderTextureByIndex(_owner.UnitData.TextureIndex);
            var width = texture.width;
            var t2d = new Texture2D(width, width, TextureFormat.RGBA32, false);
            var tmp = RenderTexture.active;

            RenderTexture.active = texture;

            t2d.ReadPixels(new Rect(0, 0, width, width), 0, 0);
            t2d.Apply();

            RenderTexture.active = tmp;

            image.sprite =
                Sprite.Create(
                    t2d,
                    new Rect(width / 6, width / 6, width - width / 6, width - width / 6),
                    new Vector2(0.5f, 0.5f),
                    50f
                );
            image.transform.localPosition += new Vector3(40, 40, 0);
            image.transform.localScale += new Vector3(0.2f, 0.2f, 0f);
        }
    }

    /// <summary>Drop overrides</summary>
    public override void Drop(LevelUpUI ui)
    {
        ((GameObject)typeof(LevelUpUI).Field("ob_EmotionPerUI").GetValue(ui)).SetActive(true);

        if (icon is null)
        {
            var image = ((Image)typeof(LevelUpUI).Field("FloorIconImage").GetValue(ui));

            image.transform.localPosition += new Vector3(-40, -40, 0);
            image.transform.localScale += new Vector3(-0.2f, -0.2f, 0f);
        }
    }

    /// <summary>OnSelect overrides</summary>
    public override bool OnSelect(EmotionCardXmlInfo info)
    {
        if (info.TargetType is EmotionTargetType.SelectOne)
        {
            _owner.emotionDetail.ApplyEmotionCard(info, true);
        }
        else
        {
            var b = true;

            foreach (var unit in Faction.Player.AliveUnits)
            {
                unit.emotionDetail.ApplyEmotionCard(info, b);

                b = false;
            }

            if (info.TargetType is EmotionTargetType.AllIncludingEnemy)
            {
                StageController.Instance.GetCurrentWaveModel().ApplyEmotionCard(info);
            }
        }

        return false;
    }

    private BattleUnitModel _owner = owner;
}
