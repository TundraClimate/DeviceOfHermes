# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# DeckExtension

デッキ関連の拡張メソッドです。

```cs
// 現時点の山札を直接取得します。
var deck = owner.allyCardDetail.Deck();

// 現時点の手札を直接取得します。
var hand = owner.allyCardDetail.Hand();

// 現時点の使用済みページのリスト
var uses = owner.allyCardDetail.Uses();

// 現時点の墓地を直接取得します。
var discards = owner.allyCardDetail.Discard();

// 現時点の仕様予約がされているページのリスト
var reserves = owner.allyCardDetail.Reserved();

// 個人ページの現時点の山札を直接取得します。
var personalAll = owner.personalEgoDetail.All();

// 個人ページ現時点の手札を直接取得します。
var handPersonal = owner.personalEgoDetail.Hand();

// 個人ページの現時点の使用済みページのリスト
var usesPersonal = owner.personalEgoDetail.Uses();

// 個人ページの現時点の仕様予約がされているページのリスト
var reservesPersonal = owner.personalEgoDetail.Reserved();

// BattleUnitModelに紐づくList<BattleDiceCardModel>を文字列キーで取得します。
var subDeck = owner.AlterDeck("SubDeck");
```

## Specified usage

### `BattleAllyCardDetail.Deck()`

```cs
var deck = owner.allyCardDetail.Deck();
```

現時点の山札を直接取得します。

### `BattleAllyCardDetail.Hand()`

```cs
var hand = owner.allyCardDetail.Hand();
```

現時点の手札を直接取得します。

### `BattleAllyCardDetail.Uses()`

```cs
var uses = owner.allyCardDetail.Uses();
```

現時点の使用済みページのリスト

### `BattleAllyCardDetail.Discard()`

```cs
var discards = owner.allyCardDetail.Discard();
```

現時点の墓地を直接取得します。

### `BattleAllyCardDetail.Reserved()`

```cs
var reserves = owner.allyCardDetail.Reserved();
```

現時点の仕様予約がされているページのリスト

### `BattleUnitModel.AlterDeck(string)`

```cs
var subDeck = owner.AlterDeck(key);
```

`BattleUnitModel`に紐づく`List<BattleDiceCardModel>`を文字列キーで取得します。

- `key`: Dictionaryのキーです。
