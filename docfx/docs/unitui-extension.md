# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# UnitUIExtension

キャラクターに関する表示を扱うクラスです。

例:  
```cs
owner.Say("は...");

owner.view.Say("ルール違反だ！", 1f);

owner.view.AddEffect(sprite, new Vector2(0.5f, 0.52f), 3f);

owner.GetClickedSpeedDice();

owner.GetHoveredCardUI();

owner.CreateEffectText("沈潜殺到", SinkingDeluge);

diceCardUI.ReplaceHand(card);

owner.ReplaceHoveredHand(card);
```

## Specified usage

### `Say(string)`

```cs
owner.Say(dlg);
```

通常と同じようにセリフを喋らせます。

- `owner`: 喋らせるキャラクターユニットです。
- `dlg`: キャラクターが喋るテキストです。

### `Say(string, float, float)`

```cs
owner.view.Say(dlg, duration, overhead, scale);
```

キャラクターの頭上にテキストを表示します。

- `owner`: 表示する対象のユニットです。
- `dlg`: 表示するテキストです。
- `duration`: フェードイン・アウトを除いた表示時間です。
- `overhead`: テキストを表示する高さです。デフォルトは`3.2f`です。
- `scale`: テキストのサイズ比です。デフォルトは`0.7f`です。

### `AddEffect(Sprite, Vector2, float, float, Vector2, float)`

```cs
owner.view.AddEffect(sprite, pos, duration, feed, sizeDelta, sizeScale);
```

キャラクターを中心としたエフェクトを画像から描画し、フェードアウトさせます。
`pos`は`Vector2(0.5f, 0.52f)`程度がキャラクターに重なる地点です。

- `sprite`: 描画するSpriteです。
- `pos`: 描画する場所です。(0, 0)から(1, 1)の範囲でなければなりません。
- `duration`: 描画する秒数です。
- `feed`: フェードアウトに所要する秒数です。
- `sizeDelta`: 描画サイズです。省略可能。
- `sizeScale`: 描画サイズ比です。省略可能。

### `GetClickedSpeedDice`

```cs
owner.GetClickedSpeedDice();
```

現在選択されている速度ダイスのスロット番号を返します。  
未選択の場合は-1を返します。

### `GetHoveredCardUI()`

```cs
owner.GetHoveredCardUI();
```

現在カーソルが乗っている自身のバトルページUIを返します。  
未選択の場合はnullを返します。

### `CreateEffectText(string, Sprite?, Color?, Color?)`

```cs
owner.CreateEffectText(text, sprite, textColor, spriteColor);
```

ダメージテキストのテキスト部分を表示します。

- `text`: 表示するテキストです。
- `sprite`: テキスト左に表示する画像です。
- `textColor`: テキストの色です。
- `spriteColor`: 画像の色です。

### `ReplaceHand(BattleDiceCardModel)`

```cs
diceCardUI.ReplaceHand(card);
```

ページUIの表示ごとユニットの手札を書き換えます。  
ユニットの選択中にのみ有効です。

- `card`: 置き換える`BattleDiceCardModel`です。

### `ReplaceHoveredHand(BattleDiceCardModel)`

```cs
owner.ReplaceHoveredHand(card);
```

現在カーソルを置いているページUIの表示ごとユニットの手札を書き換えます。  
ユニットの選択中にのみ有効です。

- `card`: 置き換える`BattleDiceCardModel`です。
