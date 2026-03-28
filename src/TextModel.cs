using System.Reflection;
using HarmonyLib;
using LOR_XML;

namespace DeviceOfHermes.Resource;

/// <summary>The localize helper</summary>
/// <remarks>
/// Set text data for directory.<br/>
/// Additional data reset when change language, or restart game.<br/>
/// Therefore, an event the <c>OnLoadLocalize</c> is hooks localize initialize.
/// </remarks>
/// <example><code>
/// TextModel.OnLoadLocalize += lang =>
/// {
///     Hermes.Say($"Language changed to {lang}!");
/// }
/// </code></example>
public static class TextModel
{
    static TextModel()
    {
        _effTxtRef = AccessTools.FieldRefAccess<BattleEffectTextsXmlList, Dictionary<string, BattleEffectText>>("_dictionary");

        var harmony = new Harmony("DeviceOfHermes.Resource.TextModel");

        harmony.CreateClassProcessor(typeof(TextModelPatch.PatchLoadObserver)).Patch();
        harmony.CreateClassProcessor(typeof(TextModelPatch.PatchOnetimeInvoke)).Patch();
    }

    /// <summary>Invokes on localize data initialized</summary>
    /// <remarks>Game start, language reset or etc.</remarks>
    public static event Action<string> OnLoadLocalize = lang => { };

    /// <summary>Set BattleEffectText with text</summary>
    /// <param name="text">A dataset of <see cref="BattleEffectText"/></param>
    /// <param name="replace">Is replace if contains same text ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. Burn) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetBattleEffectText(new BattleEffectText()
    /// {
    ///     ID = "Burn",
    ///     Name = "Burn",
    ///     Desc = "Will die",
    /// }, true);
    /// </code></example>
    public static void SetBattleEffectText(BattleEffectText text, bool replace = false)
    {
        ref var dict = ref EffectTextDict;

        if (dict.ContainsKey(text.ID))
        {
            if (replace)
            {
                dict[text.ID] = text;
            }
            else
            {
                Hermes.Say($"Skipped: BattleEffectText the '{text.ID}' is already exists.", MessageLevel.Warn);
            }

            return;
        }

        dict.Add(text.ID, text);
    }

    /// <summary>Set BattleEffectText with texts</summary>
    /// <param name="texts">The datasets of <see cref="BattleEffectText"/></param>
    /// <param name="replace">Is replace if contains same text ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. Burn) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetBattleEffectTexts([
    ///     new BattleEffectText()
    ///     {
    ///         ID = "Burn",
    ///         Name = "Burn",
    ///         Desc = "Will die",
    ///     },
    ///     new BattleEffectText()
    ///     {
    ///         ID = "Bleeding",
    ///         Name = "Bleeding",
    ///         Desc = "Will burst",
    ///     }
    /// ], true);
    /// </code></example>
    public static void SetBattleEffectTexts(IEnumerable<BattleEffectText> texts, bool replace = false)
    {
        foreach (var text in texts)
        {
            SetBattleEffectText(text, replace);
        }
    }

    private static ref Dictionary<string, BattleEffectText> EffectTextDict => ref _effTxtRef(BattleEffectTextsXmlList.Instance);

    private static readonly AccessTools.FieldRef<BattleEffectTextsXmlList, Dictionary<string, BattleEffectText>> _effTxtRef;

    private class TextModelPatch
    {
        [HarmonyPatch]
        public class PatchLoadObserver
        {
            static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(LocalizedTextLoader), "Load", [
                    typeof(string),
                typeof(Dictionary<string, string>).MakeByRefType(),
            ]);
            }

            static void Postfix(string currentLanguage)
            {
                TextModel.OnLoadLocalize.Invoke(currentLanguage);
            }
        }

        [HarmonyPatch(typeof(GameSceneManager), "Start")]
        public class PatchOnetimeInvoke
        {
            static void Postfix()
            {
                var lang = GlobalGameManager.Instance.CurrentOption.language.ToLower();

                TextModel.OnLoadLocalize.Invoke(lang);
            }
        }
    }
}
