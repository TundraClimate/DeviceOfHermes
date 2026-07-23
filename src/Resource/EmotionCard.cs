using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes.Resource;

/// <summary>A emotion card data</summary>
public static class EmotionCard
{
    /// <summary>Adds EmotionCard</summary>
    public static void Add(string pid, EmotionCardXmlInfo card)
    {
        ref var list = ref InfoList;

        card.id += 10000000;

        if (card.Sephirah == SephirahType.ETC)
        {
            card.Sephirah = (SephirahType)pid.GetHashCode();
        }

        list.Add(card);
    }

    /// <summary>Get EmotionCard</summary>
    public static EmotionCardXmlInfo? Get(string pid, int id)
    {
        return InfoList.Find(info => info.id == id + 10000000 && info.Sephirah == (SephirahType)pid.GetHashCode());
    }

    private static ref List<EmotionCardXmlInfo> InfoList =>
        ref _listRef(EmotionCardXmlList.Instance);

    private static AccessTools.FieldRef<EmotionCardXmlList, List<EmotionCardXmlInfo>> _listRef
        = typeof(EmotionCardXmlList).FieldRefAccess<List<EmotionCardXmlInfo>>("_list");
}
