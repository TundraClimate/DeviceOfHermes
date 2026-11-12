# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# BattleUnitExtension

ユニット関連の拡張メソッドです。

※ メソッドの呼び出し時点でユニット上限が10まで拡充されます。

```cs
// ID=43005のユニットで残りの敵配置枠を埋めます。
while (StageController.Instance.TryAddNewEnemy(new LorId(43005)))
{
    Hermes.Say($"Add id=43005 unit");
}

// ID=43005のユニットで残りの味方配置枠を埋めます。
while (StageController.Instance.TryAddNewLibrarian(new LorId(43005)))
{
    Hermes.Say($"Add id=43005 unit");
}

// UIを更新
BattleObjectManager.instance.InitUI();
```

## Specified usage

### `TryAddNewEnemy(LorId, int, int)`

```cs
StageController.Instance.TryAddNewEnemy(id, idx, height)
```

敵ユニットを追加します。  
`idx`を省略した場合、indexが連番になるよう調整してユニットを配置します。  
配置に失敗した場合、falseを返します。

- `id`: 配置するユニットのIDです。
- `idx`: 省略可能。配置するindexです。
- `height`: 省略可能。ユニットの垂直距離を変更します。

### `TryAddNewLibrarian(LorId, int, int)`

味方ユニットを追加します。  
`idx`を省略した場合、indexが連番になるよう調整してユニットを配置します。  
配置に失敗した場合、falseを返します。

- `id`: 配置するユニットのIDです。
- `idx`: 省略可能。配置するindexです。
- `height`: 省略可能。ユニットの垂直距離を変更します。
