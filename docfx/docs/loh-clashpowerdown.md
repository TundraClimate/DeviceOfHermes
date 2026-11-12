# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# ClashPowerDown

リンバスの"マッチ威力減少"と同じような挙動をするバフです。

> 今回の幕の間、マッチ時にダイス威力-{0}、ダメージ量+{0}

```cs
// マッチ威力減少を3付与
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.ClashPowerDown, 3);
```
