# `using DeviceOfHermes.AdvancedBase;`

名前空間`DeviceOfHermes.AdvancedBase`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdvancedDiceBase

効果拡張版`DiceCardAbilityBase`です。

例:

```cs
class DiceCardAbility_AdvDice : AdvancedDiceBase
{
    public override void OnAddToKeeped()
    {
    }

    public override bool IsKeeps()
    {
        return true;
    }

    public override ParryingResult GetParryingResult(ParryingResult origin)
    {
        return origin;
    }

    public override int GetDiceFinalResultValue(int origin)
    {
        return origin;
    }

    public override int GetFinalResultDamageValue(int origin)
    {
        return origin;
    }

    public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
    {
    }

    public override void BeforeRollDiceFirst()
    {
    }

    public override void BeforeRollDiceBefore()
    {
    }

    public override void BeforeRollDiceAfter()
    {
    }

    public override void BeforeRollDiceLast()
    {
    }
}
```

このクラスは`DiceCardAbilityBase`を継承しています。  
それの代わりに`AdvancedDiceBase`を継承し、幾つかの追加要素のあるそれとして扱うことを想定しています。

## Specified usage

### `OnAddToKeeped()`

```cs
public override void OnAddToKeeped()
{
}
```

反撃ダイス等でこのダイスが`BattleKeepedCardDataInUnitModel`に追加されたときに実行されます。

### `IsKeeps()`

```cs
public override bool IsKeeps()
{
    return false;
}
```

反撃ダイス等でこのダイスが`BattleKeepedCardDataInUnitModel`に追加されるかどうかを返します。

### `GetParryingResult(ParryingResult)`

```cs
public override ParryingResult GetParryingResult(ParryingResult origin)
{
    return origin;
}
```

マッチ結果を変更します。  
`ParryingResult.Win`、`ParryingResult.Lose`、`ParryingResult.Draw`のいずれかを返すとその結果になります。  
この処理はプレイヤー側の結果が優先されます。

- `origin`: 変更前のマッチ結果です。

### `GetDiceFinalResultValue(int)`

```cs
public override int GetDiceFinalResultValue(int origin)
{
    return origin;
}
```

最終威力をこのメソッドが返した値に変更します。

- `origin`: 変更前の最終威力です。

### `GetFinalResultDamageValue(int)`

```cs
public override int GetFinalResultDamageValue(int origin)
{
    return origin;
}
```

最終ダメージ量をこのメソッドが返した値に変更します。

- `origin`: 変更前の最終ダメージ量です。

### `GetForceResultDamageValue(int)`

```cs
public override int GetForceResultDamageValue(int origin)
{
    return origin;
}
```

全てのダメージ変更処理の後にダメージ量をこのメソッドが返した値に変更します。

- `origin`: 変更前の最終ダメージ量です。

### `OnUseCard(BattlePlayingCardDataInUnitModel)`

```cs
public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
{
}
```

自身が存在するページを使用したときに実行されます。

- `curCard`: 使用したページです。

### `BeforeRollDiceFirst()`

```cs
public override void BeforeRollDiceFirst()
{
}
```

`BeforeRollDice`より先に実行されます。

### `BeforeRollDiceBefore()`

```cs
public override void BeforeRollDiceBefore()
{
}
```

`BeforeRollDice`の直前に実行されます。

### `BeforeRollDiceAfter()`

```cs
public override void BeforeRollDiceAfter()
{
}
```

`BeforeRollDice`の直後に実行されます。

### `BeforeRollDiceLast()`

```cs
public override void BeforeRollDiceLast()
{
}
```

`BeforeRollDice`より後に実行されます。
