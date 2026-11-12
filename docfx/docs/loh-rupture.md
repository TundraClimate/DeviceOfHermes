# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Rupture

リンバスの"破裂"と同じような挙動をするバフです。

> 被ダメージ時、破裂の値だけ物理追加ダメージを受ける。
> 効果発動後、値が2/3に減少。(端数切り捨て)

```cs
// targetに破裂を7付与
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.Rupture, 7, owner);
```

## Events

### `ILimbuf.OnRupture.OnRupture(int stack)`

破裂が発動したときに実行されます。

- `stack`: 破裂の値です。
