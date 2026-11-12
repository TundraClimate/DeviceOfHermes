# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# PrescriptTarget

リンバスの"指令対象"と同じような挙動をするバフです。

> 毎幕1つの対象のみに適用される。

```cs
// ランダムな対象に指令対象を付与する
BattleObjectManager.instance.GetAliveList_random(Faction.Enemy, 1)[0].bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.PrescriptTarget, 1);
```
