# `using DeviceOfHermes.AdvancedBase;`

名前空間`DeviceOfHermes.AdvancedBase`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdvancedCardBase

効果拡張版`DiceCardSelfAbilityBase`です。

例:

```cs
public class DiceCardSelfAbility_AdvCard : AdvancedCardBase
{
    static DiceCardSelfAbility_AdvCard()
    {
        RegisterKeyDownEvent(typeof(DiceCardSelfAbility_AdvCard), KeyCode.Tab, (owner, _) =>
        {
            owner.ReplaceHoveredHand(BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(607006)));
        });
    }

    public override bool IsClashable => true;

    public override bool IsClashableWithStandby => true;

    public override bool IsIgnoreSpeedByMatch => false;

    public override bool IsTakeAmmoEffectNotFar => false;

    public override bool CanDestroyCard => true;

    public override bool DrawsOnWaveStart => false;

    public override bool MoveToPersonal => false;

    public override bool UseOnStartBattle => false;

    public override Type[] AutoSetCardBufs => [];

    public override bool IsEquippable(BattleUnitModel unit, BattleDiceCardModel card, BattleDiceCardModel targetCard)
    {
        return false;
    }

    public override void OnEquip(BattleUnitModel unit, BattleDiceCardModel card, BattleDiceCardModel targetCard)
    {
    }

    public override bool CanDiscardByAbility(BattleDiceCardModel self)
    {
    }

    public override int SpecialPriorityAdder(int slot, int speed)
    {
        return 0;
    }

    public override int GetFinalResultDamageValue(int origin)
    {
        return origin;
    }

    public override void BeforeUseCard(ref BattlePlayingCardDataInUnitModel card)
    {
    }

    public override bool OnAddKeepDice(BattleDiceBehavior behavior)
    {
        return true;
    }

    public override void BeforeRollDiceFirst(BattleDiceBehavior behavior)
    {
    }

    public override void BeforeRollDiceBefore(BattleDiceBehavior behavior)
    {
    }

    public override void BeforeRollDiceAfter(BattleDiceBehavior behavior)
    {
    }

    public override void BeforeRollDiceLast(BattleDiceBehavior behavior)
    {
    }

    public override void OnSelectTarget(BattleDiceCardModel card, BattleUnitModel target, int targetSlot)
    {
    }

    public override void OnWinParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }
}
```

このクラスは`DiceCardSelfAbilityBase`を継承しています。  
それの代わりに`AdvancedCardBase`を継承し、幾つかの追加要素のあるそれとして扱うことを想定しています。

## Specified usage

### `RegisterKeyDownEvent(Type, KeyCode, Action<BattleUnitModel, BattleDiceCardModel>)`

```cs
RegisterKeyDownEvent(self, key, handler);
```

ページ選択中にキー入力したときのイベントハンドラを追加します。  
staticコンストラクタ内で呼び出してください。

- `self`: その型そのものです。他の型を入れると想定外の処理が走る可能性があります。
- `key`: 入力を検知するKeyCodeです。
- `handler`: キー入力がされたときに実行する処理です。

### `IsClashable`

```cs
public override bool IsClashable => true;
```

マッチ可能かを指定するプロパティです。  
falseを指定するとマッチ不可になります。

### `IsClashableWithStandby`

```cs
public override bool IsClashableWithStandby => true;
```

反撃とマッチ可能化を指定するプロパティです。  
falseを指定するとマッチ不可になります。

### `IsIgnoreSpeedByMatch`

```cs
public override bool IsIgnoreSpeedByMatch => false;
```

速度を無視してマッチできるかを指定するプロパティです。  
trueを返すとマッチに速度を参照しなくなります。

### `IsTakeAmmoEffectNotFar`

```cs
public override bool IsTakeAmmoEffectNotFar => false;
```

弾丸の効果を受けるかどうかを指定するプロパティです。  
trueを返すと弾丸の効果を受けます。

### `CanDestroyCard`

```cs
public override bool CanDestroyCard => true;
```

ページが破壊されうるかどうかです。  
falseを返すと破壊されなくなります。

### `DrawsOnWaveStart`

```cs
public override bool DrawsOnWaveStart => false;
```

舞台開始時に自動で抽出されるかを指定するプロパティです。  
trueを返すと自動で抽出されるようになります。

### `MoveToPersonal`

```cs
public override bool MoveToPersonal => false;
```

舞台開始時に個人デッキに移動するかを指定するプロパティです。  
trueを返すと個人デッキに移動するようになります。

### `UseOnStartBattle`

```cs
public override bool UseOnStartBattle => false;
```

戦闘開始時に同距離のページから最速で使用されるかどうかです。  
trueなら戦闘開始時に同距離のうちで最速使用されます。

### `AutoSetCardBufs`

```cs
public override Type[] AutoSetCardBufs => [];
```

ページ作成時に自動で追加されるページバフです。

### `IsEquippable(BattleUnitModel, BattleDiceCardModel, BattleDiceCardModel)`

```cs
public override bool IsEquippable(BattleUnitModel unit, BattleDiceCardModel card, BattleDiceCardModel targetCard)
{
    return false;
}
```

このページがページに装着可能かどうかです。  
`true`を返す場合、このページで他のページを選択したままクリックすることで装着することが出来ます。

- `unit`: 装着するユニットです。
- `card`: このアビリティを保有するページです。
- `targetCard`: 装着しようとしているページです。

### `OnEquip(BattleUnitModel, BattleDiceCardModel, BattleDiceCardModel)`

```cs
public override void OnEquip(BattleUnitModel unit, BattleDiceCardModel card, BattleDiceCardModel targetCard)
{
}
```

このページをページに装着したときに実行されます。

- `unit`: 装着するユニットです。
- `card`: このアビリティを保有するページです。
- `targetCard`: 装着したページです。

### `CanDiscardByAbility(BattleDiceCardModel)`

```cs
public override bool CanDiscardByAbility(BattleDiceCardModel self)
{
}
```

このページが捨てられる前に実行されます。  
falseを返すと捨てられるのをキャンセルすることが出来ます。

- `self`: この効果を持っているページ。

### `SpecialPriorityAdder(int, int)`

```cs
public override int SpecialPriorityAdder(int slot, int speed)
{
    return 0;
}
```

特定の速度・スロットを条件にPriorityに加算される数値を返します。

- `slot`: 計算に使用する速度スロットです。
- `speed`: スロットに対応する速度です。

### `BeforeUseCard(ref BattlePlayingCardDataInUnitModel)`

```cs
public override void BeforeUseCard(ref BattlePlayingCardDataInUnitModel card)
{
}
```

ベージの使用前に実行されます。  
`card`に再代入した場合、そのページを代わりに使用します。

- `card`: 使用しているページです。

### `OnAddKeepDice(BattleDiceBehavior)`

```cs
public override bool OnAddKeepDice(BattleDiceBehavior behavior)
{
    return true;
}
```

ダイスが反撃として追加される直前に実行されます。  
`false`を返すとそのダイス追加がキャンセルされます。

- `behavior`: 対象のダイスです。

### `BeforeRollDiceFirst(BattleDiceBehavior)`

```cs
public override void BeforeRollDiceFirst(BattleDiceBehavior behavior)
{
}
```

`BeforeRollDice`より先に実行されます。

- `behavior`: 対象のダイスです。

### `BeforeRollDiceBefore(BattleDiceBehavior)`

```cs
public override void BeforeRollDiceBefore(BattleDiceBehavior behavior)
{
}
```

`BeforeRollDice`の直前に実行されます。

- `behavior`: 対象のダイスです。

### `BeforeRollDiceAfter(BattleDiceBehavior)`

```cs
public override void BeforeRollDiceAfter(BattleDiceBehavior behavior)
{
}
```

`BeforeRollDice`の直後に実行されます。

- `behavior`: 対象のダイスです。

### `BeforeRollDiceLast(BattleDiceBehavior)`

```cs
public override void BeforeRollDiceLast(BattleDiceBehavior behavior)
{
}
```

`BeforeRollDice`より後に実行されます。

- `behavior`: 対象のダイスです。

### `OnSelectTarget(BattleDiceCardModel, BattleUnitModel, int)`

```cs
public override void OnSelectTarget(BattleDiceCardModel card, BattleUnitModel target, int targetSlot)
{
}
```

ページをセットしたときに実行されます。

- `card`: セットしたページです。
- `target`: 攻撃対象です。
- `targetSlot`: 対象の速度スロットです。

### `OnWinParrying(BattleDiceBehavior, BattleDiceBehavior)`

```cs
public override void OnWinParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
{
}
```

マッチに勝利したときに実行されます。

- `behavior`: マッチに勝利した自分のダイスです。
- `targetDice`: マッチに敗北した相手のダイスです。
