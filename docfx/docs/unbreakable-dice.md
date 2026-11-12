# `using DeviceOfHermes.CustomDice;`

名前空間`DeviceOfHermes.CustomDice`に存在するクラスです。  
この記事はこれを前提に書かれています。

# UnbreakableDice

特殊な攻撃ダイスです。  
マッチ敗北時に保存され、敵のページ使用終了時に再使用します。  
ただし、出目範囲が最低値に固定され、ページ効果が消滅します。  
また、再使用するページは再使用前のページの速度ダイスをターゲットしている他のページに自動でマッチします。

例:  
```cs
class DiceCardAbility_Unbreakable : UnbreakableDice
{
    public static string Desc = "破壊不能ダイス";

    public override void OnUseBreaked(BattlePlayingCardDataInUnitModel card)
    {
    }
}
```

このクラスは`DiceCardAbilityBase`を継承しています。  
それの代わりに`UnbreakableDice`を継承し、幾つかの追加要素のあるそれとして扱うことを想定しています。

## Specified usage

### `OnUseBreaked(BattlePlayingCardDataInUnitModel)`

```cs
public override void OnUseBreaked(BattlePlayingCardDataInUnitModel card)
{
}
```

破壊されたダイスの使用時に実行されます。

- `card`: そのダイスが実際に使用されるページ。

### `IsBreaked`

```cs
var breaked = this.IsBreaked;
```

ダイスが破壊されているかどうかを示すプロパティ。

### `IsUnbreakableDice(BattleDiceBehavior)`

```cs
UnbreakableDice.IsUnbreakableDice(behavior);
```

特定の`BattleDiceBehavior`が`UnbreakableDice`かどうかを返します。

- `behavior`: 対象のダイスです。

### `IsBreakedDice(BattleDiceBehavior)`

```cs
UnbreakableDice.IsBreakedDice(behavior);
```

特定の`BattleDiceBehavior`が破壊済みの`UnbreakableDice`かどうかを返します。

- `behavior`: 対象のダイスです。
