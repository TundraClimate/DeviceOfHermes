# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# HPHealingUp

リンバスの"体力回復増加"と同じような挙動をするバフです。

> 今回の幕の間、体力回復量 +{0}

```cs
// 体力回復増加を3獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.HPHealingUp, 3, owner);
```
