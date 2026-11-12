# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# `BattleCardExtension`

ページ関連の処理拡張です。

```cs
// ownerが使用中のページを作成する
BattlePlayingCardDataInUnitModel card = owner.CreatePlayingCard(xml);

// card内のダイスを作成する
BattleDiceBehavior dice = card.CreateBattleDiceBehavior(diceBehaviour);

// ページ内のダイスにAbilityをそれぞれ適用する
card.ApplyDiceAbility<DiceCardAbility_XXX>(DiceMatch.AllDice);

// targetにlidのページを使用します。
owner.UseCard(target, lid);

DiceStatBonus bonus = dice.GetStatBonus();
```

## Specified usage

### `CreatePlayingCard(DiceCardXmlInfo, BattleUnitModel, int, int, List<BattleUnitModel>)`

```cs
owner.CreatePlayingCard(xml, target, targetSlot, speed, subTargets);
```

`BattlePlayingCardDataInUnitModel`を作成します。

- `owner`: ページの保有者です。
- `xml`: ページのXMLデータです。
- `target`: ページの使用対象です。
- `targetSlot`: ページの使用対象の速度ダイス番号です。
- `speed`: ページに紐づく速度の値です。
- `subTargets`: サブターゲットのリストです。

### `CreateBattleDiceBehavior(DiceBehaviour, int)`

```cs
card.CreateBattleDiceBehavior(dice, idx);
```

`BattleDiceBehavior`を作成します。

- `card`: ダイスが紐づくページです。
- `dice`: ダイスのXMLデータです。
- `idx`: ダイスのページ内番号です。

### `ApplyDiceAbility<T>(Predicate<DiceMatch>)`

```cs
card.ApplyDiceAbility<T>(diceMatch);
```

ページ内のダイスにそれぞれAbilityを適用します。

- `card`: 対象のページです。
- `diceMatch`: 適用するダイスを指定する条件式です。
- `T`: 適用するAbilityの型です。`DiceCardAbilityBase`である必要があります。

### `UseCard(BattleUnitModel, BattlePlayingCardDataInUnitModel, bool)`

```cs
owner.UseCard(target, playcard, originalSpeed);
```

`target`にページを使用します。

- `target`: ページを使用する対象です。
- `playcard`: 使用するページです。
- `originalSpeed`: ページの速度設定を維持するかです。

### `UseCard(BattleUnitModel, LorId, bool)`

```cs
owner.UseCard(target, lid, originalSpeed);
```

`target`にページを使用します。

- `target`: ページを使用する対象です。
- `lid`: 使用するページのIDです。
- `originalSpeed`: ページの速度設定を維持するかです。

### `GetStatBonus()`

```cs
dice.GetStatBonus();
```

`DiceStatBonus`を取得します。
