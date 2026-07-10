using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace DeviceOfHermes.UI;

/// <summary>A ui of BattleDiceCardUI wrapper</summary>
public class BattleFloatingDiceCardUI : MonoBehaviour
{
    static BattleFloatingDiceCardUI()
    {
        var harmony = new Harmony("DeviceOfHermes.UI.BattleFloatingDiceCardUI");

        harmony.CreateClassProcessor(typeof(PatchOnPointerDown)).Patch();
    }

    /// <summary>Runs on card clicked</summary>
    public event Action<PointerEventData> OnClickCallBack = _ => { };

    /// <summary>Initialize</summary>
    public void Init(BattleDiceCardModel card)
    {
        SetCard(card);
        _ui.gameObject.transform.SetParent(gameObject.transform);

        _ui.transform.localScale = _ui.scaleOrigin;
    }

    /// <summary>Set new Card</summary>
    public void SetCard(BattleDiceCardModel card)
    {
        _ui.SetCard(card);
    }

    private void OnClick(PointerEventData eventData)
    {
        OnClickCallBack.Invoke(eventData);
    }

    void Awake()
    {
        _list.Add(this);
    }

    void OnEnable()
    {
        _ui.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        _list.Remove(this);
    }

    private static BattleDiceCardUI CreateNewClonedUI()
    {
        BattleDiceCardUI? slot = null;

        foreach (var ob in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            var res = ob.GetComponentsInChildren<BattleDiceCardUI>(true);

            foreach (var card in res)
            {
                if (card.name.Contains("Full"))
                {
                    slot = card;

                    break;
                }
            }
        }

        if (slot is null)
        {
            throw new InvalidProgramException("BattleDiceCardUI not found");
        }

        var newSlot = UnityObject.Instantiate(slot);

        return newSlot;
    }

    [HarmonyPatch(typeof(BattleDiceCardUI), "OnPointerDown")]
    class PatchOnPointerDown
    {
        static bool Prefix(BattleDiceCardUI __instance, PointerEventData eventData)
        {
            if (_list.Find(e => e._ui == __instance) is BattleFloatingDiceCardUI ui)
            {
                ui.OnClick(eventData);

                return false;
            }

            return true;
        }
    }

    private BattleDiceCardUI _ui = CreateNewClonedUI();

    private static List<BattleFloatingDiceCardUI> _list = new();
}
