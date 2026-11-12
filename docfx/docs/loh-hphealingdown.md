# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# HPHealingDown

リンバスの"体力回復減少"と同じような挙動をするバフです。

> 今回の幕の間、体力回復量 -{0}

```cs
// targetに体力回復増加を3付与
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.HPHealingDown, 3, owner);
```
