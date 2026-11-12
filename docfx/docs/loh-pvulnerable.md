# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# PenetrateVulnerable

リンバスの"貫通脆弱"と同じような挙動をするバフです。

> 今回の幕の間、貫通属性の攻撃によって受けるダメージ量が{0}増加

```cs
// targetに貫通脆弱を3付与
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.PenetrateVulnerable, 3, owner);
```
