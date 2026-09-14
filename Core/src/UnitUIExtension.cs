using System.Runtime.CompilerServices;
using UnityEngine;
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
        if (view.dialogUI.transform.parent.GetComponentInChildren<BattleFloatingDialog>() is BattleFloatingDialog dlg)
        {
            UnityObject.Destroy(dlg.gameObject);
        }

        BattleFloatingDialog.PlayDialog(view, txt, duration, overhead, scale);
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

    /// <summary>Creates new unit-floating text</summary>
    public static void CreateTextEffect(
        this BattleUnitModel owner,
        string text,
        Sprite? sprite = null,
        Color? textColor = null,
        Color? spriteColor = null
    )
    {
        var go = owner.view.characterRotationCenter.gameObject.AddChildObject("TextEffect", "Effect");

        var effect = UnityObject.Instantiate<DamageTextEffect>(AttackEffectManager.Instance.damagedTextPrefab, owner.view.damageTextEffectRoot);

        effect.maxEffect = false;
        effect.isAtk = true;

        textColor ??= new Color(1, 1, 1, 1);
        spriteColor ??= new Color(1, 1, 1, 1);

        effect.img_resistIcon.sprite = sprite;
        effect.img_resistIcon.color = spriteColor.Value;
        effect.img_resistIconBg.color = Color.clear;
        effect.img_resistIconFg.color = Color.clear;
        effect.txt_resist.fontMaterial.SetColor("_UnderlayColor", textColor.Value);
        effect.txt_resist.color = textColor.Value;

        effect.txt_resist.text = text;
        effect.txt_resist.transform.localPosition -= new Vector3(0, 30, 0);

        AttackEffectManager.Instance.SetEffectSizeByCamZoom(effect);
        AttackEffectManager.Instance.SetEffectSizeByUnitHeight(owner, effect);

        go.AddComponent<AutoDestruct>().time = 1f;
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

    private static ConditionalWeakTable<BattleUnitView, GameObject> _unitRootCanvas = new();

    private static AccessTools.FieldRef<SpeedDiceUI, bool> _isClicked
        = typeof(SpeedDiceUI).FieldRefAccess<bool>("isClicked");
}
