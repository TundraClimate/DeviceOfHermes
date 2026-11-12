# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Nails

リンバスの"狂信"と同じような挙動をするバフです。

> 今回の幕の間、釘の付与されている敵に対して攻撃ダイス威力が{0}だけ増加

```cs
// 狂信を3獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.Fanatic, 3);
```
