# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# BattleTickAction

毎Tick実行される処理を管理します。  
処理速度により変動しますが、毎秒数十回は呼び出されることになるため気をつけるべきでしょう。

例:  
```cs
BattleTickAction.OnTick += () => { Hermes.Say("BOO!") };

BattleTickAction.OnFrame += () => { Hermes.Say("BOOM!") };
```

## Specified usage

### `OnTick`

```cs
BattleTickAction.OnTick += () => { };
```

毎Tickの処理を追加します。

### `OnFrame`

```cs
BattleTickAction.OnFrame += () => { };
```

毎フレーム実行される処理です。
