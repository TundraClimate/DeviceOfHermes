# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Nails

リンバスの"釘"と同じような挙動をするバフです。

> 幕の開始時、出血1を得る。
> 攻撃ダイスを振って出血減少時、代わりに釘数値が1減少

```cs
// 釘を3付与
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.Nails, 3);
```
