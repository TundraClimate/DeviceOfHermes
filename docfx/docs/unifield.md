# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Unifield

舞台全体に影響を与える効果を作成します。

```cs
class Unifield_Sample : Unifield
{
    // 有効化を試みたときに実行される
    public override void OnActivate()
    {
    }

    // 有効化されたときに実行される
    public override void OnEnable()
    {
    }

    public override void BeforeRoundStart()
    {
    }

    public override void OnRoundStart()
    {
    }

    public override void BeforeRoundEnd()
    {
    }

    public override void OnRoundEnd()
    {
    }

    public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
    {
    }

    public override void BeforeRollDice(BattleDiceBehavior dice)
    {
    }

    public override void Cleanup()
    {
    }
}
```

## Specified usage

### `ActivateField<T: Unifield>(T)`

```cs
Unifield.ActivateField(field);
```

その舞台で`field`を有効化します。  
`is T`な`Unifield`が既に有効化されている場合、インスタンスのみ入れ替わります。

- `field`: 有効化する`Unifield`です。

### `DeactivateField<T>()`

```cs
Unifield.DeactivateField<T>();
```

その舞台の`T`を無効化します。  

- `T`: 対象の`Unifield`です。
