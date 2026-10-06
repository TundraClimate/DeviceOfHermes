using HarmonyLib;
using LOR_DiceSystem;

namespace DeviceOfHermes;

/// <summary>OnlyCard manage helper</summary>
/// <remarks>
/// Only supports the onlycard add.<br/>
/// Can add for vannila and mod keypage.
/// </remarks>
/// <example><code>
/// new AdditonalOnlyCard(new LorId(targetID)).AddCards(new LorId(onlycardId));
/// </code></example>
public class AdditonalOnlyCard
{
    static AdditonalOnlyCard()
    {
        var harmony = new Harmony("DeviceOfHermes.OnlyCard");

        harmony.CreateClassProcessor(typeof(PatchXmlInfoSetter)).Patch();
    }

    /// <summary>Creates with target book ID</summary>
    /// <param name="bookId">target keypage ID</param>
    public AdditonalOnlyCard(LorId bookId)
    {
        this._bookId = bookId;
    }

    /// <summary>Add onlycard for target</summary>
    /// <param name="cards">OnlyCard ID list</param>
    /// <example><code>
    /// new AdditonalOnlyCard(new LorId(targetID)).AddCards(new LorId(onlycardId));
    /// </code></example>
    public void AddCards(params LorId[] cards)
    {
        if (AdditonalOnlyCard._onlyCardDict.TryGetValue(this._bookId, out var stored))
        {
            stored.AddRange(cards);
        }
        else
        {
            AdditonalOnlyCard._onlyCardDict.Add(this._bookId, cards.ToList());
        }
    }

    private LorId _bookId;

    private static Dictionary<LorId, List<LorId>> _onlyCardDict = new();

    [HarmonyPatch(typeof(BookModel), "SetXmlInfo", [typeof(BookXmlInfo)])]
    class PatchXmlInfoSetter
    {
        static Exception Finalizer(Exception __exception, BookModel __instance, List<DiceCardXmlInfo> ____onlyCards)
        {
            if (LorId.IsBasicId(__instance.ClassInfo.workshopID))
            {
                if (AdditonalOnlyCard._onlyCardDict.TryGetValue(new LorId(__instance.ClassInfo._id), out var vcards))
                {
                    var cardXmls = vcards.Map(id => ItemXmlDataList.instance.GetCardItem(id, true)).Filter(card => card is not null);

                    ____onlyCards.AddRange(cardXmls);
                }

                return __exception;
            }

            if (AdditonalOnlyCard._onlyCardDict.TryGetValue(__instance.ClassInfo.id, out var cards))
            {
                var cardXmls = cards.Map(id => ItemXmlDataList.instance.GetCardItem(id, true)).Filter(card => card is not null);

                ____onlyCards.AddRange(cardXmls);
            }

            return __exception;
        }
    }
}
