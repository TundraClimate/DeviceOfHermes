# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# HitVulnerable

リンバスの"打撃脆弱"と同じような挙動をするバフです。

> 今回の幕の間、打撃属性の攻撃によって受けるダメージ量が{0}増加

```cs
// targetに打撃脆弱を3付与
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.HitVulnerable, 3, owner);
```
