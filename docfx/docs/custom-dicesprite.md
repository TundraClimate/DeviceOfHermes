# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# CustomDiceSprite

ページ上のダイス画像を書き換えます。

例:
```cs
CustomDiceSprite.AddSeqence(beh => beh.Type == BehaviourType.Standby, newSprite);
```

この例は反撃ダイスを`newSprite`に置き換えるコードです。

## Specified usage

### `AddSeqence(Func<LOR_DiceSystem.DiceBehaviour, bool>, Sprite, Color)`

```cs
CustomDiceSprite.AddSeqence(cond, newSprite, behColor);
```

スプライト変換シーケンスを追加します。

- `cond(DiceBehaviour): bool`: 適用する対象を特定する条件式です。trueを返した要素を対象とします。
- `newSprite`: 新しいスプライトです。
- `behColor`: ページ情報のダイス結果範囲の色合いです。UnityEngine.Colorを使用します。
