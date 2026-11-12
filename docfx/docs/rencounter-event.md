# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# RencounterEvent

マッチ結果のレンダリング中に実行されるイベントを追加できます。

```cs
BattleUnitModel owner = this.owner;

owner.AddRencounterEvent(RencounterEvent.TakeDamaged, () => Hermes.Say("Realtime Hello!"));
```

## Specified usage

### `AddRencounterEvent(this BattleUnitModel, RencounterEvent, Action)`

```cs
owner.AddRencounterEvent(event, action);
```

マッチ結果のレンダリング中に実行されるイベントを追加します。

- `event`: 実行タイミングです。
- `action`: 実行する処理です。
