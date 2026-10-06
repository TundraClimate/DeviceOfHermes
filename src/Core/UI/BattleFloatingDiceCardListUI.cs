using UnityEngine;
using UnityEngine.UI;

namespace DeviceOfHermes.UI;

/// <summary>A list ui of floating card</summary>
public class BattleFloatingDiceCardListUI : BattleUIBehaviour
{
    /// <summary>Runs on round start</summary>
    public event Action OnRoundStartCallBack = () => { };

    /// <summary>Runs on start battle</summary>
    public event Action OnStartBattleCallBack = () => { };

    /// <summary>Adds new BattleFloatingDiceCardUI</summary>
    public BattleFloatingDiceCardUI AddCard(BattleDiceCardModel card, Vector2 pos)
    {
        var go = gameObject.AddChildObject(layerName: LayerMask.LayerToName(gameObject.layer));

        var newCard = go.AddComponent<BattleFloatingDiceCardUI>();

        newCard.Init(card);

        go.SetActive(true);
        go.MoveTo(pos);

        go.name = $"[FloatingCard] {card.XmlData.workshopName}";

        go.AddComponent<CanvasGroup>().Let(cg =>
        {
            cg.Show();
            cg.Enable();
        });

        return newCard;
    }

    /// <summary>Runs on round start</summary>
    public override void OnRoundStart()
    {
        OnRoundStartCallBack.Invoke();
    }

    /// <summary>Runs on start battle</summary>
    public override void OnStartBattle()
    {
        OnStartBattleCallBack.Invoke();
    }

    void Awake()
    {
        InitUI(1005);

        gameObject.AddComponent<GraphicRaycaster>();
    }
}
