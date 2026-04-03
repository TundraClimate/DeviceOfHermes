using System.Reflection;
using HarmonyLib;
using LOR_XML;
using static HarmonyLib.AccessTools;

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

    /// <summary>Set BattleCardAbilityDesc with desc</summary>
    /// <param name="desc">A desc data</param>
    /// <param name="replace">Is replace if contains same desc ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. drawCard) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetBattleCardAbilityDesc(new BattleCardAbilityDesc()
    /// {
    ///     id = "drawCard",
    ///     desc = ["Draws HYPERMAXED card"],
    /// }, true);
    /// </code></example>
    public static void SetBattleCardAbilityDesc(BattleCardAbilityDesc desc, bool replace = false)
    {
        ref var dict = ref AbilityDescDict;

        if (dict.ContainsKey(desc.id))
        {
            if (replace)
            {
                dict[desc.id] = desc;
            }
            else
            {
                Hermes.Say($"Skipped: BattleCardAbilityDesc the '{desc.id}' is already exists.", MessageLevel.Warn);
            }

            return;
        }

        dict.Add(desc.id, desc);
    }


    /// <summary>Set BattleCardAbilityDesc with descs</summary>
    /// <param name="descs">The desc data</param>
    /// <param name="replace">Is replace if contains same desc ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. drawCard) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetBattleCardAbilityDescs([
    ///     new BattleCardAbilityDesc()
    ///     {
    ///         id = "drawCard",
    ///         desc = ["Draws HYPERMAXED card"],
    ///     }
    /// ], true);
    /// </code></example>
    public static void SetBattleCardAbilityDescs(IEnumerable<BattleCardAbilityDesc> descs, bool replace = false)
    {
        foreach (var desc in descs)
        {
            SetBattleCardAbilityDesc(desc, replace);
        }
    }

    /// <summary>Set BattleCardDesc with desc</summary>
    /// <param name="desc">A desc data</param>
    /// <param name="pid">The pid of desc</param>
    /// <param name="replace">Is replace if contains same desc ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. LorId(602008)) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetBattleCardDesc(new BattleCardDesc()
    /// {
    ///     cardID = 602008,
    ///     cardName = "Gloden knuckle",
    /// }, replace: true);
    /// </code></example>
    public static void SetBattleCardDesc(BattleCardDesc desc, string? pid = null, bool replace = false)
    {
        var lid = pid switch
        {
            null or "" or "@origin" => new LorId(desc.cardID),
            _ => new LorId(pid, desc.cardID),
        };

        ref var dict = ref CardDescDict;

        if (dict.ContainsKey(lid))
        {
            if (replace)
            {
                dict[lid] = desc;
            }
            else
            {
                if (lid.IsBasic())
                {
                    Hermes.Say($"Skipped: BattleCardDesc the '{desc.cardID}' is already exists.", MessageLevel.Warn);
                }
                else
                {
                    Hermes.Say($"Skipped: BattleCardDesc the '{desc.cardID}' is already exists in {pid}.", MessageLevel.Warn);
                }
            }

            return;
        }

        dict.Add(lid, desc);
    }

    /// <summary>Set BattleCardDesc with descs</summary>
    /// <param name="descs">The desc data with pids</param>
    /// <param name="replace">Is replace if contains same desc ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. LorId(602008)) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetBattleCardDescs([(null, new BattleCardDesc()
    /// {
    ///     cardID = 602008,
    ///     cardName = "Gloden knuckle",
    /// })], true);
    /// </code></example>
    public static void SetBattleCardDescs(IEnumerable<(string?, BattleCardDesc)> descs, bool replace = false)
    {
        foreach (var (pid, desc) in descs)
        {
            SetBattleCardDesc(desc, pid, replace);
        }
    }

    /// <summary>Set Character dialog in group</summary>
    /// <param name="character">A <see cref="BattleDialogCharacter"/> of to add</param>
    /// <param name="groupName">The group of character found</param>
    /// <param name="replace">Is replace if contains same desc ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. AwlOfNight, Named) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetCharacterDialog(new BattleDialogCharacter()
    /// {
    ///     characterID = "Named",
    ///     dialogTypeList = [
    ///         new BattleDialogType()
    ///         {
    ///             dialogType = DialogType.START_BATTLE,
    ///             dialogList = [
    ///                 new BattleDialog()
    ///                 {
    ///                     dialogID = "START_BATTLE_0",
    ///                     dialogContent = ";(",
    ///                 }
    ///             ],
    ///         }
    ///     ],
    /// }, "AwlOfNight", true);
    /// </code></example>
    public static void SetCharacterDialog(BattleDialogCharacter character, string groupName = "Workshop", bool replace = false)
    {
        ref var dict = ref DialogGroupDict;

        if (!dict.ContainsKey(groupName))
        {
            var root = new BattleDialogRoot();

            root.groupName = groupName;
            root.characterList = new();

            dict.Add(groupName, root);
        }

        var charList = dict[groupName].characterList;
        var fdIdx = charList.FindIndex(ch => ch.characterID == character.characterID);

        if (fdIdx != -1)
        {
            if (replace)
            {
                charList[fdIdx] = character;
            }
            else
            {
                Hermes.Say($"Skipped: BattleCardDesc the '{character.characterID}' is already exists in {groupName}.", MessageLevel.Warn);
            }

            return;
        }

        charList.Add(character);
    }

    /// <summary>Set Character dialogs in group</summary>
    /// <param name="characters">THe <see cref="BattleDialogCharacter"/> of to add</param>
    /// <param name="groupName">The group of character found</param>
    /// <param name="replace">Is replace if contains same desc ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. AwlOfNight, Named) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetCharacterDialogs([new BattleDialogCharacter()
    /// {
    ///     characterID = "Named",
    ///     dialogTypeList = [
    ///         new BattleDialogType()
    ///         {
    ///             dialogType = DialogType.START_BATTLE,
    ///             dialogList = [
    ///                 new BattleDialog()
    ///                 {
    ///                     dialogID = "START_BATTLE_0",
    ///                     dialogContent = ";(",
    ///                 }
    ///             ],
    ///         }
    ///     ],
    /// }], "AwlOfNight", true);
    /// </code></example>
    public static void SetCharacterDialogs(
        IEnumerable<BattleDialogCharacter> characters,
        string groupName = "Workshop",
        bool replace = false
    )
    {
        foreach (var character in characters)
        {
            SetCharacterDialog(character, groupName, replace);
        }
    }

    private static ref Dictionary<string, BattleEffectText> EffectTextDict =>
        ref _effTxtRef(BattleEffectTextsXmlList.Instance);

    private static readonly FieldRef<BattleEffectTextsXmlList, Dictionary<string, BattleEffectText>> _effTxtRef =
        typeof(BattleEffectTextsXmlList).FieldRefAccess<Dictionary<string, BattleEffectText>>("_dictionary");

    private static ref Dictionary<string, BattleCardAbilityDesc> AbilityDescDict =>
        ref _abilityDescRef(BattleCardAbilityDescXmlList.Instance);

    private static readonly FieldRef<BattleCardAbilityDescXmlList, Dictionary<string, BattleCardAbilityDesc>> _abilityDescRef =
        typeof(BattleCardAbilityDescXmlList).FieldRefAccess<Dictionary<string, BattleCardAbilityDesc>>("_dictionary");

    private static ref Dictionary<LorId, BattleCardDesc> CardDescDict =>
        ref _cardDescRef(BattleCardDescXmlList.Instance);

    private static readonly FieldRef<BattleCardDescXmlList, Dictionary<LorId, BattleCardDesc>> _cardDescRef =
        typeof(BattleCardDescXmlList).FieldRefAccess<Dictionary<LorId, BattleCardDesc>>("_dictionary");

    private static ref Dictionary<string, BattleDialogRoot> DialogGroupDict =>
        ref _dialogGroupRef(BattleDialogXmlList.Instance);

    private static readonly FieldRef<BattleDialogXmlList, Dictionary<string, BattleDialogRoot>> _dialogGroupRef =
        typeof(BattleDialogXmlList).FieldRefAccess<Dictionary<string, BattleDialogRoot>>("_dictionary");

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
