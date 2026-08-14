using System.Runtime.CompilerServices;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes;

/// <summary>A extension of decks</summary>
public static class DeckExtension
{
    extension(BattleAllyCardDetail ally)
    {
        /// <summary>Returns unit decks</summary>
        public ref List<BattleDiceCardModel> Deck() => ref _unitDecks(ally);

        /// <summary>Returns unit hands</summary>
        public ref List<BattleDiceCardModel> Hand() => ref _unitHands(ally);

        /// <summary>Returns unit uses</summary>
        public ref List<BattleDiceCardModel> Uses() => ref _unitUses(ally);

        /// <summary>Returns unit discards</summary>
        public ref List<BattleDiceCardModel> Discard() => ref _unitDisces(ally);

        /// <summary>Returns unit reserves</summary>
        public ref List<BattleDiceCardModel> Reserved() => ref _unitReserved(ally);
    }

    extension(BattleUnitModel owner)
    {
        /// <summary>Get or init to alter decks</summary>
        public List<BattleDiceCardModel> AlterDeck(string name)
        {
            var decks = _altDecks.GetValue(owner, _ => new());

            if (!decks.ContainsKey(name))
            {
                decks[name] = new();
            }

            return decks[name];
        }
    }

    private static ConditionalWeakTable<BattleUnitModel, Dictionary<string, List<BattleDiceCardModel>>> _altDecks = new();

    private static AccessTools.FieldRef<BattleAllyCardDetail, List<BattleDiceCardModel>> _unitDecks
        = typeof(BattleAllyCardDetail).FieldRefAccess<List<BattleDiceCardModel>>("_cardInDeck");

    private static AccessTools.FieldRef<BattleAllyCardDetail, List<BattleDiceCardModel>> _unitHands
        = typeof(BattleAllyCardDetail).FieldRefAccess<List<BattleDiceCardModel>>("_cardInHand");

    private static AccessTools.FieldRef<BattleAllyCardDetail, List<BattleDiceCardModel>> _unitUses
        = typeof(BattleAllyCardDetail).FieldRefAccess<List<BattleDiceCardModel>>("_cardInUse");

    private static AccessTools.FieldRef<BattleAllyCardDetail, List<BattleDiceCardModel>> _unitDisces
        = typeof(BattleAllyCardDetail).FieldRefAccess<List<BattleDiceCardModel>>("_cardInDiscarded");

    private static AccessTools.FieldRef<BattleAllyCardDetail, List<BattleDiceCardModel>> _unitReserved
        = typeof(BattleAllyCardDetail).FieldRefAccess<List<BattleDiceCardModel>>("_cardInReserved");
}
