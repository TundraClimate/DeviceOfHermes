# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Sinking

リンバスの"沈潜"をLoR風に作成したバフです。

> 最大値: 99
> 
> 値が増加するにつれてダイスの素の威力が低い出目になりやすくなる。
> 幕の終了時、値が最大値なら次の幕にパニック状態となりこの効果が除去される。

```cs
// targetに沈潜を7付与
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.Sinking, 7, owner);
```

## Panic

パニック状態:  
> この効果は沈潜を99得ている状態と同じだけの状態異常効果を受ける。
> 
> この幕の間沈潜を獲得することが出来ない。
> 幕の終了時に除去される。

## SinkingDeluge

沈潜殺到をLoR風に再現したバフです。

> (沈潜 * 2)の混乱ダメージを与え、沈潜を除去する。
> この効果で超過した混乱ダメージは物理ダメージとして与える。

```cs
// 沈潜殺到
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.SinkingDeluge, 1, owner);
```

## Events

### `ILimbuf.OnPanic.OnPanic()`

パニックになったときに実行されます。
