# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  

# CritDmgUp

リンバスの"クリティカルダメージ量"と同じような挙動をするバフです。

> 今回の幕の間、クリティカルダメージ量 +{0}%

```cs
// クリティカルダメージ量増加を1獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.CritDmgUp, 1, owner);
```
