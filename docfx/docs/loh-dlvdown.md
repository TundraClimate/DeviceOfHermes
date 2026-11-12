# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# DlvDown

リンバスの"防御レベル減少"と同じような挙動をするバフです。

> 被ダメージ、被混乱ダメージ{0}%上昇(最大50%)。
> 値5毎に守備ダイスの最大値1減少(最大5)。
> 幕の終了時に除去される。

```cs
// 防御レベル減少を5獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.DlvDown, 5);
```
