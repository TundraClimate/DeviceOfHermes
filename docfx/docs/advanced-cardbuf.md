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

    public override void OnWinParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice, bool isAtkDice)
    {
    }

    public override void OnWinParrying_inHand(BattleDiceBehavior behavior, BattleDiceBehavior targetDice, bool isAtkDice)
    {
    }

    public override void OnWinParrying_inDeck(BattleDiceBehavior behavior, BattleDiceBehavior targetDice, bool isAtkDice)
    {
    }

    public override void OnLoseParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    public override void OnLoseParrying_inHand(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    public override void OnLoseParrying_inDeck(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    public override void OnDrawParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    public override void OnDrawParrying_inHand(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    public override void OnDrawParrying_inDeck(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
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

### `OnWinParrying(BattleDiceBehavior, BattleDiceBehavior, bool)`

```cs
public override void OnWinParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice, bool isAtkDice)
{
}
```

マッチに勝利したときに実行されます。

- `behavior`: 勝利した自分のダイスです。
- `targetDice`: 勝利した相手のダイスです。
- `isAtkDice`: 攻撃ダイスでマッチ勝利したかどうかです。

### `OnWinParrying_inHand(BattleDiceBehavior, BattleDiceBehavior, bool)`

```cs
public override void OnWinParrying_inHand(BattleDiceBehavior behavior, BattleDiceBehavior targetDice, bool isAtkDice)
{
}
```

手札に存在する際にマッチに勝利したときに実行されます。

- `behavior`: 勝利した自分のダイスです。
- `targetDice`: 勝利した相手のダイスです。
- `isAtkDice`: 攻撃ダイスでマッチ勝利したかどうかです。

### `OnWinParrying_inDeck(BattleDiceBehavior, BattleDiceBehavior, bool)`

```cs
public override void OnWinParrying_inDeck(BattleDiceBehavior behavior, BattleDiceBehavior targetDice, bool isAtkDice)
{
}
```

デッキに存在する際にマッチに勝利したときに実行されます。

- `behavior`: 勝利した自分のダイスです。
- `targetDice`: 勝利した相手のダイスです。
- `isAtkDice`: 攻撃ダイスでマッチ勝利したかどうかです。

### `OnLoseParrying(BattleDiceBehavior, BattleDiceBehavior)`

```cs
public override void OnLoseParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
{
}
```

マッチに敗北したときに実行されます。

- `behavior`: 敗北した自分のダイスです。
- `targetDice`: 敗北した相手のダイスです。

### `OnLoseParrying_inHand(BattleDiceBehavior, BattleDiceBehavior)`

```cs
public override void OnLoseParrying_inHand(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
{
}
```

手札に存在する際にマッチに敗北したときに実行されます。

- `behavior`: 敗北した自分のダイスです。
- `targetDice`: 敗北した相手のダイスです。

### `OnLoseParrying_inDeck(BattleDiceBehavior, BattleDiceBehavior)`

```cs
public override void OnLoseParrying_inDeck(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
{
}
```

デッキに存在する際にマッチに敗北したときに実行されます。

- `behavior`: 敗北した自分のダイスです。
- `targetDice`: 敗北した相手のダイスです。

### `OnDrawParrying(BattleDiceBehavior, BattleDiceBehavior)`

```cs
public override void OnDrawParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
{
}
```

マッチに引き分けたときに実行されます。

- `behavior`: 引き分けた自分のダイスです。
- `targetDice`: 引き分けた相手のダイスです。

### `OnDrawParrying_inHand(BattleDiceBehavior, BattleDiceBehavior)`

```cs
public override void OnDrawParrying_inHand(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
{
}
```

手札に存在する際にマッチに引き分けたときに実行されます。

- `behavior`: 引き分けた自分のダイスです。
- `targetDice`: 引き分けた相手のダイスです。

### `OnDrawParrying_inDeck(BattleDiceBehavior, BattleDiceBehavior)`

```cs
public override void OnDrawParrying_inDeck(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
{
}
```

デッキに存在する際にマッチに引き分けたときに実行されます。

- `behavior`: 引き分けた自分のダイスです。
- `targetDice`: 引き分けた相手のダイスです。

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
