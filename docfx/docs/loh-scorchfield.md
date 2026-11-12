# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  

# Scorchfield

リンバスの"灼熱地帯"と同じような挙動をする効果です。

> 最大値: 999
> この舞台で発生した火傷ダメージを蓄積する。
> 舞台の終了時に初期化される。
> 
> この数値は全キャラクターで共有される。

```cs
// "灼熱地帯"を有効化する
StageBufListDetail.ActivateStageBuf(StageBuf.Scorchfield);

// "灼熱地帯"を100消耗出来るなら100消耗する
// 消耗できた場合はtrueを返す
StageBufListDetail.TryConsume(StageBuf.Scorchfield, 100, owner);

// "灼熱地帯"を100まで消耗する
// 実際に消耗した値を返す
StageBufListDetail.Consume(StageBuf.Scorchfield, 100, owner);

// "灼熱地帯"の現在の値を取得する
StageBufListDetail.GetBufCount(StageBuf.Scorchfield);
```

## Events

### `ILimbuf.OnAddScorchfield.OnAddScorchfield(int stack)`

灼熱地帯を取得したときに実行されます。

- `stack`: 獲得した灼熱地帯数です。
