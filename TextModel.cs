using System.Reflection;
using HarmonyLib;
using LOR_XML;

namespace DeviceOfHermes.Resource;

public static class TextModel
{
    static TextModel()
    {
        _effTxtRef = AccessTools.FieldRefAccess<BattleEffectTextsXmlList, Dictionary<string, BattleEffectText>>("_dictionary");

        var harmony = new Harmony("DeviceOfHermes.Resource.TextModel");

        harmony.CreateClassProcessor(typeof(TextModelPatch.PatchLoadObserver)).Patch();
        harmony.CreateClassProcessor(typeof(TextModelPatch.PatchOnetimeInvoke)).Patch();
    }

    public static event Action<string> OnLoadLocalize = lang => { };

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
