# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AlvDown

リンバスの"攻撃レベル減少"と同じような挙動をするバフです。

> 与ダメージ、与混乱ダメージ{0}%減少(最大50%)。
> 値5毎に攻撃ダイスの最大値1減少(最大5)。
> 幕の終了時に除去される。

```cs
// 攻撃レベル減少を5獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.AlvDown, 5);
```
