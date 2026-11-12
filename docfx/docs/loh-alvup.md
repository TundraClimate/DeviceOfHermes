# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AlvUp

リンバスの"攻撃レベル増加"と同じような挙動をするバフです。

> 与ダメージ、与混乱ダメージ{0}%上昇(最大50%)。
> 値5毎に攻撃ダイスの最大値1上昇(最大5)。
> 幕の終了時に除去される。

```cs
// 攻撃レベル増加を5獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.AlvUp, 5);
```
