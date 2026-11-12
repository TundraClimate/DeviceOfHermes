# `using DeviceOfHermes.AdvancedBase;`

名前空間`DeviceOfHermes.AdvancedBase`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdvancedCardBuf

効果拡張版`BattleDiceCardBuf`です。

```cs
public class DiceCardBuf_AdvCardBuf : AdvancedCardBuf
{
    protected override string keywordIconId => "アイコンのID";

    public override bool IsTargetableAllUnit => false;

    public override void OnStartBattle()
    {
    }

    public override void OnStartBattle_inHand()
    {
    }

    public override void OnStartBattle_inDeck()
    {
    }

    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
    }

    public override void OnSuccessAttack(BattleDiceBehavior behavior)
    {
    }

    public override void OnSuccessAttack_inHand(BattleDiceBehavior behavior)
    {
    }

    public override void OnSuccessAttack_inDeck(BattleDiceBehavior behavior)
    {
    }

    public override void OnEndEncounter(BattleUnitModel? target, bool isParrying, bool wasAttacker)
    {
    }
}
```

## Specified usage

### `keywordIconId`

```cs
protected override string keywordIconId => id;
```

バフのアイコンを設定します。  
アイコンはユニットバフと同じ方法で追加することが出来ます。

- `id`: バフアイコンのIDです。

### `IsTargetableAllUnit`

```cs
public override bool IsTargetableAllUnit => targetable;
```

付与したページが全ユニットをターゲットできるかを指定します。

- `targetable`: trueならば全ユニットをターゲット可能になります。

### `OnStartBattle()`

```cs
public override void OnStartBattle()
{
}
```

バフの付与されたページをセットして戦闘を開始したときに実行されます。

### `OnStartBattle_inHand()`

```cs
public override void OnStartBattle_inHand()
{
}
```

戦闘を開始したときに実行されます。  
手札に存在するページ全てを対象にします。

### `OnStartBattle_inDeck()`

```cs
public override void OnStartBattle_inDeck()
{
}
```

戦闘を開始したときに実行されます。  
デッキ内に存在しているページ全てを対象にします。

### `BeforeRollDice(BattleDiceBehavior)`

```cs
public override void BeforeRollDice(BattleDiceBehavior behavior)
{
}
```

バフが付与されたページでダイスを振る前に実行されます。

- `behavior`: 対象のダイスです。

### `OnSuccessAttack(BattleDiceBehavior)`

```cs
public override void OnSuccessAttack(BattleDiceBehavior behavior)
{
}
```

バフが付与されたページでダイスが的中したときに実行されます。

- `behavior`: 対象のダイスです。

### `OnSuccessAttack_inHand(BattleDiceBehavior)`

```cs
public override void OnSuccessAttack_inHand(BattleDiceBehavior behavior)
{
}
```

ダイスが的中したときに手札のページを対象に実行されます。

- `behavior`: 対象のダイスです。

### `OnSuccessAttack_inDeck(BattleDiceBehavior)`

```cs
public override void OnSuccessAttack_inDeck(BattleDiceBehavior behavior)
{
}
```

ダイスが的中したときにデッキのページを対象に実行されます。

- `behavior`: 対象のダイスです。

### `OnEndEncounter(BattleUnitModel?, bool, bool)`

```cs
public override void OnEndEncounter(BattleUnitModel? target, bool isParrying, bool wasAttacker)
{
}
```

接敵終了時に実行されます。  
currentDiceActionはnullです。

- `target`: 接敵した相手です。
- `isParrying`: その接敵がマッチであったかどうかです。
- `wasAttacker`: ページを使用していたかどうかです。
