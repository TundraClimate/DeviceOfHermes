# `using DeviceOfHermes.CustomDice;`

名前空間`DeviceOfHermes.CustomDice`に存在するクラスです。  
この記事はこれを前提に書かれています。

# RevengeDice

特殊な反撃ダイスです。  
攻撃反撃ダイスのみ適用でき、被撃時にページ分のダイスを纏めて一方攻撃で使用します。  

例:  
```cs
class DiceCardAbility_Revenge : RevengeDice
{
    static string Desc = "復讐ダイス";

    public override void OnBeforeDecideRevenge(BattlePlayingCardDataInUnitModel card, BattleDiceBehavior revengeBy)
    {
    }

    public override void OnBeforeRevenge(BattlePlayingCardDataInUnitModel card, BattleDiceBehavior revengeBy)
    {
    }

    public override void OnRevenge(BattlePlayingCardDataInUnitModel card)
    {
    }
}
```

このクラスは`DiceCardAbilityBase`を継承しています。  
それの代わりに`RevengeDice`を継承し、幾つかの追加要素のあるそれとして扱うことを想定しています。

## IRevengeCard

ページ効果に`IRevengeCard`を実装することで、ページ単位での反撃を戦闘開始時に追加できます。  

例:  
```cs
class DiceCardSelfAbility_Revenge : DiceCardSelfAbilityBase, IRevengeCard
{
}
```

この効果をページ効果として追加したページは戦闘開始時にマッチ状態が解除され、反撃を除くページがそのまま特殊反撃ページとしてスタックされます。

## Specified usage

### `OnBeforeDecideRevenge(BattlePlayingCardDataInUnitModel, BattleDiceBehavior)`

```cs
public override void OnBeforeDecideRevenge(BattlePlayingCardDataInUnitModel card, BattleDiceBehavior revengeBy)
{
}
```

被撃する直前に特殊反撃を使用するであろうダイスで実行されます。  
まだ使用は確定していません。

- `card`: そのダイスが実際に使用されるページ。
- `revengeBy`: 反撃のトリガーになった敵のダイスです。

### `OnBeforeRevenge(BattlePlayingCardDataInUnitModel, BattleDiceBehavior)`

```cs
public override void OnBeforeRevenge(BattlePlayingCardDataInUnitModel card, BattleDiceBehavior revengeBy)
{
}
```

被撃した瞬間に、使用が決定したダイスで実行されます。

- `card`: そのダイスが実際に使用されるページ。
- `revengeBy`: 反撃のトリガーになった敵のダイスです。

### `OnRevenge(BattlePlayingCardDataInUnitModel)`

```cs
public override void OnRevenge(BattlePlayingCardDataInUnitModel card)
{
}
```

特殊反撃でページが使用された瞬間に含まれるダイスで実行されます。

- `card`: そのダイスが実際に使用されるページ。

### `AddRevengeCard(BattleUnitModel, DiceCardXmlInfo, List<BattleDiceBehavior>)`

```cs
RevengeDice.AddRevengeCard(unit, display, behaviors);
```

デフォルトのダイス、もしくは指定のダイスでページを作成し、反撃ページとして追加します。  
戦闘中にのみ有効です。  

ページ効果は消滅します。

- `unit`: 追加する対象です。
- `display`: このページとしてダイスを使用します。
- `behaviors`: 指定した場合、ダイスが上書きされます。

### `AddRevengeCard(BattleUnitModel, BattlePlayingCardDataInUnitModel)`

```cs
RevengeDice.AddRevengeCard(unit, card);
```

既存のページを特殊反撃ページとして追加します。  
戦闘中にのみ有効です。  

ページ効果は維持されます。

- `unit`: 追加する対象です。
- `card`: 追加対象のページです。

### `IsRevengeNow(BattleUnitModel)`

```cs
RevengeDice.IsRevengeNow(unit);
```

`unit`が現在反撃中・もしくは反撃の直前であるならば`true`を返します。

- `unit`: 対象のBattleUnitModelです。

### `RemainingRevenges(BattleUnitModel)`

```cs
int remain = RevengeDice.RemainingRevenges(unit);
```

残りの反撃回数です。

- `unit`: 対象のBattleUnitModelです。

### `isReuse`

```cs
revengeDice.isReuse = true;
```

`OnBeforeRevenge`までに`true`を代入した場合、1度その特殊反撃ページを再利用するようになります。
