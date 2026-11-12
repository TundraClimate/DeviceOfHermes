# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# DlvUp

リンバスの"防御レベル増加"と同じような挙動をするバフです。

> 被ダメージ、被混乱ダメージ{0}%減少(最大50%)。
> 値5毎に守備ダイスの最大値1上昇(最大5)。
> 幕の終了時に除去される。

```cs
// 防御レベル増加を5獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.DlvUp, 5);
```
