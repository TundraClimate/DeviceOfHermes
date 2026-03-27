using HarmonyLib;
using LOR_DiceSystem;

namespace DeviceOfHermes;

public class AdditonalOnlyCard
{
    static AdditonalOnlyCard()
    {
        var harmony = new Harmony("DeviceOfHermes.OnlyCard");

        harmony.CreateClassProcessor(typeof(PatchXmlInfoSetter)).Patch();
    }

    public AdditonalOnlyCard(LorId bookId)
    {
        this._bookId = bookId;
    }

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
        static void Postfix(BookModel __instance, List<DiceCardXmlInfo> ____onlyCards)
        {
            if (LorId.IsBasicId(__instance.ClassInfo.workshopID))
            {
                if (AdditonalOnlyCard._onlyCardDict.TryGetValue(new LorId(__instance.ClassInfo._id), out var vcards))
                {
                    var cardXmls = vcards.Map(id => ItemXmlDataList.instance.GetCardItem(id, true)).Filter(card => card is not null);

                    ____onlyCards.AddRange(cardXmls);
                }

                return;
            }

            if (AdditonalOnlyCard._onlyCardDict.TryGetValue(__instance.ClassInfo.id, out var cards))
            {
                var cardXmls = cards.Map(id => ItemXmlDataList.instance.GetCardItem(id, true)).Filter(card => card is not null);

                ____onlyCards.AddRange(cardXmls);
            }
        }
    }
}
