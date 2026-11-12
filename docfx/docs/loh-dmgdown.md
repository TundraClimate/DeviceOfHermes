# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# DmgDown

リンバスの"ダメージ量減少"と同じような挙動をするバフです。

> 今回の幕の間、攻撃ダイスのダメージ量 -{0}

```cs
// ダメージ量減少を5獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.DmgDown, 5);
```
