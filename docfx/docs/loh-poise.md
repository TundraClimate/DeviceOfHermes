# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Poise

リンバスの"呼吸"と同じような挙動をするバフです。

> 攻撃ダイス判定時、(呼吸 * 5)%の確率でクリティカルを与える。
> クリティカル発動時は与えるダメージが20%増加する。
> クリティカル発動後、この効果の値が3/4に減少する。(端数切り捨て)

```cs
// 呼吸を20獲得
owner.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.Poise, 20);

// クリティカル倍率0.2に2.3を加算する
owner.currentDiceAction.ApplyCritDamageAdder(2.3);

// クリティカル時、ダメージ量 +20%
owner.currentDiceAction.ApplyCritDamageRateAdder(20);
```

## Events

### `ILimbuf.OnRollCritical.OnRollCritical(BattleDiceBehavior behavior)`

クリティカルがロールされたときに実行されます。

- `behavior`: 対象のダイスです。

### `ILimbuf.OnCriticalAttack.OnCriticalAttack(BattleDiceBehavior behavior)`

クリティカル的中したときに実行されます。

- `behavior`: 対象のダイスです。

### `ILimbuf.IsKeepPoise.IsKeepPoise()`

呼吸を消耗するかどうかを指定します。  
`true`を返している限り、消耗しません。
