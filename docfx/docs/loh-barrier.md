# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  

# Barrier

リンバスのバリアのような挙動をするバフです。  

付与された値だけその幕限りの一時体力を獲得します。

```cs
// バリアを100獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.Barrier, 100, owner);
```

## Events

### `ILimbuf.OnLoseBarrier.OnLoseBarrier(int value)`

バリアを失ったときに実行されます。

- `value`: 失った値です。

### `ILimbuf.IsKeepBarrier.IsKeepBarrier()`

バリアを次の幕に持ち越すかどうかです。  
`true`を返す場合維持されます。

### `ILimbuf.OnBreakBarrier.OnBreakBarrier()`

バリアが破壊されたときに実行されます。

### `ILimbuf.OnTakeBarrier.OnTakeBarrier(int value)`

バリアを獲得したときに実行されます。

- `value`: 獲得した値です。
