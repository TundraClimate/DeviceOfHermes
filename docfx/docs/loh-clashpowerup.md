# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# ClashPowerUp

リンバスの"マッチ威力増加"と同じような挙動をするバフです。

> 今回の幕の間、マッチ時にダイス威力+{0}、ダメージ量-{0}

```cs
// マッチ威力増加を3獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.ClashPowerUp, 3);
```
