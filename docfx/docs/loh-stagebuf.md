# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# StageBufListDetail

舞台に付与されるバフ効果を管理します。

`StageBuf`は専用のUIが左上に用意されます。  
有効なものが存在しない場合このUIは非表示になります。

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

### `ILimbuf.OnConsumeStageBuf.OnConsumeStageBuf(BattleUnitModel? consume, int stack)`

バフが消耗されたときに実行されます。

- `consume`: 消耗した対象です。
- `stack`: 消耗した値です。

## Specified usage

### `ActivateStageBuf(StageBuf)`

```cs
StageBufListDetail.ActivateStageBuf(stageBuf);
```

`StageBuf`を有効化します。  
既に有効化されている場合は何もしません。

- `stageBuf`: 有効化したいバフ効果です。

### `TryConsume(StageBuf, int, BattleUnitModel)`

```cs
if (StageBufListDetail.TryConsume(stageBuf, stack, consume))
{
}
```

`StageBuf`を消耗しようとします。  
`stack`だけ消耗しますが、足りない場合は消耗せず、`false`を返します。

- `stageBuf`: 消耗する対象のバフ効果です。
- `stack`: 消耗したい数量です。
- `consume`: 消耗したユニットです。指定しない場合はユニット以外からの消耗となります。

### `Consume(StageBuf, int, BattleUnitModel)`

```cs
int consumed = StageBufListDetail.Consume(stageBuf, stack, consume);
```

`StageBuf`を消耗します。  
最大`stack`だけ消耗し、実際に消耗できた値を返します。

- `stageBuf`: 消耗する対象のバフ効果です。
- `stack`: 消耗する最大値です。
- `consume`: 消耗したユニットです。指定しない場合はユニット以外からの消耗となります。

### `GetBufCount(StageBuf)`

```cs
int count = StageBufListDetail.GetBufCount(stageBuf);
```

`StageBuf`の数量を取得します。  
デフォルト値は0です。

- `stageBuf`: 対象のバフです。

### `AddBufCount(StageBuf, int)`

```cs
StageBufListDetail.AddBufCount(stageBuf, count);
```

`StageBuf`の加算処理を実行します。

- `stageBuf`: 対象のバフです。
- `count`: 加算量です。
