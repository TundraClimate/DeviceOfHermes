# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  

# Bloodfeast

リンバスの"血餐"と同じような挙動をする効果です。

> 最大値: 999
> この舞台で発生した出血ダメージを蓄積する。
> 舞台の終了時に初期化される。
> 
> この数値は全キャラクターで共有される。

```cs
// "血餐"を有効化する
StageBufListDetail.ActivateStageBuf(StageBuf.Bloodfeast);

// "血餐"を100消耗出来るなら100消耗する
// 消耗できた場合はtrueを返す
StageBufListDetail.TryConsume(StageBuf.Bloodfeast, 100, owner);

// "血餐"を100まで消耗する
// 実際に消耗した値を返す
StageBufListDetail.Consume(StageBuf.Bloodfeast, 100, owner);

// "血餐"の現在の値を取得する
StageBufListDetail.GetBufCount(StageBuf.Bloodfeast);
```

## Events

### `ILimbuf.OnAddBloodfeast.OnAddBloodfeast(int stack)`

血餐を取得したときに実行されます。

- `stack`: 獲得した血餐数です。
