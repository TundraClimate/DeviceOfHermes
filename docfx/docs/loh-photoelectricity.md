# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# PhotoElectricity

リンバスの"光電"と同じような挙動をするバフです。

> 最大値: 3
> 被ダメージ時、攻撃者に充電を{0}だけ付与(ページごとに1回)。
> 攻撃者の充電数値が5以下なら、充電を追加で3付与。
> 幕の終了時に消滅。

```cs
// 光電を3獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.PhotoElectricity, 3);
```
