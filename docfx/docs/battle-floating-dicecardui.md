# `using DeviceOfHermes.UI;`

名前空間`DeviceOfHermes.UI`に存在するクラスです。  
この記事はこれを前提に書かれています。

# BattleFloatingDiceCardUI

`BattleDiceCardUI`を単独で存在させるUIです。  

```cs
var ui = gameObject.AddComponent<BattleFloatingDiceCardUI>();

// 初期化をする
ui.Init(BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(/* ページID */)));

ui.OnClickCallBack += ev =>
{
    // ページクリック時の処理
};
```

## Specified usage

### `OnClickCallBack`

```cs
newSlot.OnClickCallBack += ev =>
{
};
```

表示されたページのクリック時の処理を追加します。

### `Init(BattleDiceCardModel)`

```cs
ui.Init(card);
```

表示されるページを初期化します。

- `card`: 表示される`BattleDiceCardModel`です。

### `SetCard(BattleDiceCardModel)`

```cs
ui.SetCard(card);
```

表示されるページを変更します。

- `card`: 表示される`BattleDiceCardModel`です。
