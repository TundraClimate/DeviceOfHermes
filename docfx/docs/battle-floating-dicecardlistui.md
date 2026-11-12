# `using DeviceOfHermes.UI;`

名前空間`DeviceOfHermes.UI`に存在するクラスです。  
この記事はこれを前提に書かれています。

# BattleFloatingDiceCardListUI

`BattleFloatingDiceCardUI`をまとめて管理するUIです。

```cs
// Initializer等でUIを登録
BattleManagerUI.Instance.AddBehaviour<BattleFloatingDiceCardListUI>("cards");

public override void OnWaveStart()
{
    // ページを(0.5, 0.75)地点に追加
    _card = BattleManagerUI.Instance.GetBehaviour<BattleFloatingDiceCardListUI>("cards")
        .AddCard(BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(701001)), new Vector2(0.5f, 0.75f));
}

public override void OnEndBattlePhase()
{
    // ページを破棄
    UnityEngine.Object.Destroy(_card.gameObject);
}
```

## Specified usage

### `OnRoundStartCallBack`

```cs
ui.OnRoundStartCallBack += () => 
{
};
```

幕の開始時に実行される処理を追加します。

### `OnStartBattleCallBack`

```cs
ui.OnStartBattleCallBack += () => 
{
};
```

戦闘開始時に実行される処理を追加します。

### `AddCard(BattleDiceCardModel, Vector2)`

```cs
ui.AddCard(card, pos);
```

座標位置にページを配置します。  
座標は画面左下を(0, 0)、画面右上を(1, 1)として指定する必要があります。

接待を跨いでも存在し続けるため、不要なページは破棄することが推奨されます。

- `card`: 配置する`BattleDiceCardModel`です。
- `pos`: 配置場所の座標です。
