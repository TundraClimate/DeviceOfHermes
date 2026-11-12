# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AssistDefense

リンバスの"援護防御"と同じような挙動をするバフです。

> 味方が敵に一方攻撃されるとき、数値を1消耗して敵の攻撃対象を自分に変更し、援護防御ページを使用
> 援護防御を保有する味方が複数名いるなら、最も高い数値を持つ味方が優先して発動
> 幕の終了時に消滅 

```cs
// 援護防御に援護防御ページをセット(1回のみ、OnWaveStartなどで)
owner.bufListDetail.AddBuf(new BattleUnitBuf_Limbuf_AssistDefenseCard(new LorId(/* ここにページのID */)));

// 援護防御を獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.AssistDefense, 1);

// ページが援護防御ページかどうか
card.IsAssistDefenseCard();
```

## Events

### `ILimbuf.OnAssistDefense.OnAssistDefense(BattleUnitModel victim)`

援護防御を発動させたときに実行されます。

- `victim`: 一方攻撃をされた対象です。

### `ILimbuf.OnSuccessAssistDefense.OnSuccessAssistDefense(BattleUnitModel victim, BattlePlayingCardDataInUnitModel defenseCard)`

援護防御でページが作成されたときに実行されます。

- `victim`: 一方攻撃をされた対象です。
- `defenseCard`: 作成された援護防御ページです。

### `ILimbuf.OnDefensedByAssist.OnDefensedByAssist(BattleUnitModel actor)`

援護防御をされたときに実行されます。

- `actor`: 援護防御を発動させた味方です。
