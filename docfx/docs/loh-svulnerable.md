# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# SlashVulnerable

リンバスの"斬撃脆弱"と同じような挙動をするバフです。

> 今回の幕の間、斬撃属性の攻撃によって受けるダメージ量が{0}増加

```cs
// targetに斬撃脆弱を3付与
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.SlashVulnerable, 3, owner);
```
