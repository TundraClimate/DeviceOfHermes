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
                Hermes.Say($"Skipped: CharacterDialog the '{character.characterID}' is already exists in {groupName}.", MessageLevel.Warn);
            }

            return;
        }

        charList.Add(character);
    }

    /// <summary>Set Character dialogs in group</summary>
    /// <param name="characters">The <see cref="BattleDialogCharacter"/> of to add</param>
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

    /// <summary>Set BookDesc with pid</summary>
    /// <param name="desc">A desc of to add</param>
    /// <param name="pid">The target packageId</param>
    /// <param name="replace">Is replace if contains same desc ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. 250051) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetBookDesc(new BookDesc()
    /// {
    ///     bookID = 250051,
    ///     bookName = "KeyPage of Yan",
    ///     texts = [
    ///         "Yan"
    ///     ],
    ///     passives = [],
    /// }, replace: true);
    /// </code></example>
    public static void SetBookDesc(BookDesc desc, string? pid = null, bool replace = false)
    {
        if (pid is null or "" or "@origin")
        {
            ref var dict = ref OriginBookDescDict;

            if (dict.ContainsKey(desc.bookID))
            {
                if (replace)
                {
                    dict[desc.bookID] = desc;
                }
                else
                {
                    Hermes.Say($"Skipped: Vannila BookDesc the '{desc.bookID}' is already exists.", MessageLevel.Warn);
                }

                return;
            }

            dict.Add(desc.bookID, desc);
        }
        else
        {
            ref var dict = ref WorkshopBookDescDict;

            if (dict.ContainsKey(pid))
            {
                var ilist = dict[pid];
                var fdIdx = ilist.FindIndex(bd => bd.bookID == desc.bookID);

                if (fdIdx != -1)
                {
                    if (replace)
                    {
                        ilist[fdIdx] = desc;
                    }
                    else
                    {
                        Hermes.Say($"Skipped: Workshop BookDesc the '{desc.bookID}' is already exists.", MessageLevel.Warn);
                    }

                    return;
                }

                ilist.Add(desc);

                return;
            }

            dict.Add(pid, [desc]);
        }
    }

    /// <summary>Set BookDesc with pid</summary>
    /// <param name="descs">The desc of to add with pid</param>
    /// <param name="replace">Is replace if contains same desc ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. 250051) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetBookDescs([(null, new BookDesc()
    /// {
    ///     bookID = 250051,
    ///     bookName = "KeyPage of Yan",
    ///     texts = [
    ///         "Yan"
    ///     ],
    ///     passives = [],
    /// })], replace: true);
    /// </code></example>
    public static void SetBookDescs(IEnumerable<(string?, BookDesc)> descs, bool replace = false)
    {
        foreach (var (pid, desc) in descs)
        {
            SetBookDesc(desc, pid, replace);
        }
    }

    /// <summary>Set CharacterName with id</summary>
    /// <param name="id">An id of editing target</param>
    /// <param name="name">A new name</param>
    /// <param name="replace">Is replace if contains same character ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. 148) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetCharacterName(new LorId(148), "Distorted Yan", true);
    /// </code></example>
    public static void SetCharacterName(LorId id, string name, bool replace = false)
    {
        if (id.packageId is null or "" or "@origin")
        {
            ref var dict = ref CharacterNameDict;

            if (dict.ContainsKey(id.id))
            {
                if (replace)
                {
                    dict[id.id] = name;
                }
                else
                {
                    Hermes.Say($"Skipped: Vannila CharacterName the '{id.id}' is already exists.", MessageLevel.Warn);
                }

                return;
            }

            dict.Add(id.id, name);
        }
        else
        {
            ref var dict = ref ByWorkshopNameDict;

            if (!dict.ContainsKey(id.packageId))
            {
                Hermes.Say($"Skipped: Failures set '{id.id}' since package the '{id.packageId}' is not found.", MessageLevel.Warn);

                return;
            }

            var target = dict[id.packageId].Find(unit => unit.id == id);

            if (target is null)
            {
                Hermes.Say($"Skipped: Failures set '{id.id}' since it is not found in the '{id.packageId}'.", MessageLevel.Warn);

                return;
            }

            if (replace)
            {
                target.name = name;
            }
            else
            {
                Hermes.Say($"Skipped: Failures set '{id.id}' since the 'replace' flag is false.", MessageLevel.Warn);
            }
        }
    }

    /// <summary>Set CharacterNames with id</summary>
    /// <param name="names">An enumerable of (id, name) tuple</param>
    /// <param name="replace">Is replace if contains same character ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. 148) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetCharacterNames([(new LorId(148), "Distorted Yan")], true);
    /// </code></example>
    public static void SetCharacterNames(IEnumerable<(LorId, string)> names, bool replace = false)
    {
        foreach (var (id, name) in names)
        {
            SetCharacterName(id, name, replace);
        }
    }

    /// <summary>Set StageName with id</summary>
    /// <param name="id">An id of editing target</param>
    /// <param name="name">A new name</param>
    /// <param name="replace">Is replace if contains same character ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. 50014) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetStageName(new LorId(50014), "Distorted Yan", true);
    /// </code></example>
    public static void SetStageName(LorId id, string name, bool replace = false)
    {
        if (id.packageId is null or "" or "@origin")
        {
            ref var dict = ref StageNameDict;

            if (dict.ContainsKey(id.id))
            {
                if (replace)
                {
                    dict[id.id] = name;
                }
                else
                {
                    Hermes.Say($"Skipped: Vannila StageName the '{id.id}' is already exists.", MessageLevel.Warn);
                }

                return;
            }

            dict.Add(id.id, name);
        }
        else
        {
            ref var dict = ref WorkshopStageDict;

            if (!dict.ContainsKey(id.packageId))
            {
                Hermes.Say($"Skipped: Failures set '{id.id}' since package the '{id.packageId}' is not found.", MessageLevel.Warn);

                return;
            }

            var target = dict[id.packageId].Find(unit => unit.id == id);

            if (target is null)
            {
                Hermes.Say($"Skipped: Failures set '{id.id}' since it is not found in the '{id.packageId}'.", MessageLevel.Warn);

                return;
            }

            if (replace)
            {
                target.stageName = name;
            }
            else
            {
                Hermes.Say($"Skipped: Failures set '{id.id}' since the 'replace' flag is false.", MessageLevel.Warn);
            }
        }
    }

    /// <summary>Set StageName with id</summary>
    /// <param name="names">An enumerable of (id, name) tuple</param>
    /// <param name="replace">Is replace if contains same character ID</param>
    /// <remarks>
    /// If <c>replace</c> is true, replaces same ID(ex. 50014) data.<br/>
    /// </remarks>
    /// <example><code>
    /// TextModel.SetStageNames([(new LorId(50014), "Distorted Yan")], true);
    /// </code></example>
    public static void SetStageNames(IEnumerable<(LorId, string)> names, bool replace = false)
    {
        foreach (var (id, name) in names)
        {
            SetStageName(id, name, replace);
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

    private static ref Dictionary<int, BookDesc> OriginBookDescDict =>
        ref _originBookDescRef(BookDescXmlList.Instance);

    private static readonly FieldRef<BookDescXmlList, Dictionary<int, BookDesc>> _originBookDescRef =
        typeof(BookDescXmlList).FieldRefAccess<Dictionary<int, BookDesc>>("_dictionaryOrigin");

    private static ref Dictionary<string, List<BookDesc>> WorkshopBookDescDict =>
        ref _workshopBookDescRef(BookDescXmlList.Instance);

    private static readonly FieldRef<BookDescXmlList, Dictionary<string, List<BookDesc>>> _workshopBookDescRef =
        typeof(BookDescXmlList).FieldRefAccess<Dictionary<string, List<BookDesc>>>("_dictionaryWorkshop");

    private static ref Dictionary<int, string> CharacterNameDict =>
        ref _characterNameRef(CharactersNameXmlList.Instance);

    private static readonly FieldRef<CharactersNameXmlList, Dictionary<int, string>> _characterNameRef =
        typeof(CharactersNameXmlList).FieldRefAccess<Dictionary<int, string>>("_dictionary");

    private static ref Dictionary<string, List<EnemyUnitClassInfo>> ByWorkshopNameDict =>
        ref _byWorkshopNameRef(EnemyUnitClassInfoList.Instance);

    private static readonly FieldRef<EnemyUnitClassInfoList, Dictionary<string, List<EnemyUnitClassInfo>>> _byWorkshopNameRef =
        typeof(EnemyUnitClassInfoList).FieldRefAccess<Dictionary<string, List<EnemyUnitClassInfo>>>("_workshopEnemyDict");

    private static ref Dictionary<int, string> StageNameDict =>
        ref _stageNameRef(StageNameXmlList.Instance);

    private static readonly FieldRef<StageNameXmlList, Dictionary<int, string>> _stageNameRef =
        typeof(StageNameXmlList).FieldRefAccess<Dictionary<int, string>>("_dictionary");

    private static ref Dictionary<string, List<StageClassInfo>> WorkshopStageDict =>
        ref _workshopStageRef(StageClassInfoList.Instance);

    private static readonly FieldRef<StageClassInfoList, Dictionary<string, List<StageClassInfo>>> _workshopStageRef =
        typeof(StageClassInfoList).FieldRefAccess<Dictionary<string, List<StageClassInfo>>>("_workshopStageDict");

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
