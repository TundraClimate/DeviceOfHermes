# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# UnitHistoryExtension

ユニット関連のHistory拡充です。

## Specified usage

### `GetDamageByBurn()`

```cs
owner.history.GetDamageByBurn();
```

累計で受けた火傷ダメージの合計を取得します。

### `GetDamageByBleed()`

```cs
owner.history.GetDamageByBleed();
```

累計で受けた出血ダメージの合計を取得します。

### `GetConsumedChargeStack()`

```cs
owner.history.GetConsumedChargeStack();
```

累計で消耗した充電の値を取得します。  
ただし、`UseStack()`により消耗した値のみを対象とします。

### `GetConsumedChargeStackAtOneRound()`

```cs
owner.history.GetConsumedChargeStackAtOneRound();
```

この幕で消耗した充電の値を取得します。  
ただし、`UseStack()`により消耗した値のみを対象とします。

### `GetRecoveredPlaypoint()`

```cs
owner.history.GetRecoveredPlaypoint();
```

累計で回復した光の値を取得します。

### `GetRecoveredPlaypointAtOneRound()`

```cs
owner.history.GetRecoveredPlaypointAtOneRound();
```

この幕で回復した光の値を取得します。

### `GetRecoveredPlaypointAtPrevRound()`

```cs
owner.history.GetRecoveredPlaypointAtPrevRound();
```

直前の幕で回復した光の値を取得します。

### `GetSpendedCost()`

```cs
owner.history.GetSpendedCost();
```

累計で消耗した光の値を取得します。

### `GetSpendedCostAtOneRound()`

```cs
owner.history.GetSpendedCostAtOneRound();
```

この幕で消耗した光の値を取得します。

### `GetSpendedCostAtPrevRound()`

```cs
owner.history.GetSpendedCostAtPrevRound();
```

直前の幕で消耗した光の値を取得します。
