# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Tremor

リンバスの"振動"と同じような挙動をするバフです。

> 振動爆発を受けるとき、振動の値だけ混乱追加ダメージを受ける。

```cs
// targetに振動を7付与
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.Tremor, 7, owner);

// 振動爆発
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.TremorBurst, 1, owner);

// 振動を5消耗
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.ConsumeTremor, 5, owner);

// 振動爆発による混乱ダメージ +50%
owner.currentDiceAction.ApplyTremorBreakRateAdder(50);
```

## Special Tremor

特殊振動を作成することが出来ます。  
`BattleUnitBuf_Limbuf_Tremor`を継承したクラスが必要になります。

```cs
class SpecialTremor : BattleUnitBuf_Limbuf_Tremor
{
    // keywordIdは各自設定してください
    protected override string keywordId => "SpecialTremor";
}
```

`bufType`は`None`、もしくは重複しないKeywordBufを作成・使用してください。  
[KeywordBufExtender](keywordbuf-extender.md)等を使用してください。

```cs
class SpecialTremor : BattleUnitBuf_Limbuf_Tremor
{
    // Noneを使用する場合、自動スタッキングが機能しません
    // public override KeywordBuf bufType => KeywordBuf.None;

    // KeywordBufExtender等を使用してKeywordBufを作成することを推奨します
    public override KeywordBuf bufType => MyKeywordBuf.SpecialTremor;
}
```

振動爆発時の処理は、`OnTremorBurst()`を実装して設定することが出来ます。

```cs
class SpecialTremor : BattleUnitBuf_Limbuf_Tremor
{
    public override KeywordBuf bufType => MyKeywordBuf.SpecialTremor;

    public override void OnTremorBurst(int stack)
    {
        // "振動の値だけ混乱追加ダメージを受ける。"の処理
        base.OnTremorBurst(stack);

        // 追加の処理をここに追記する
    }
}
```

## TremorConversion

振幅変換も可能です。  
直前で特殊振動を付与している必要があります。

> 対象の振動を別タイプの振動に変換。変換時、既存振動の値は維持される。

```cs
// 特殊振動を付与する(値は自動で揃うため、1の付与で良い)
target.bufListDetail.AddKeywordBufThisRoundByCard(MyKeywordBuf.SpecialTremor, 1, owner);

// キーワードを作成していない場合
// target.bufListDetail.AddBufWithoutDuplication(new SpecialTremor());

// 振幅変換する
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.TremorConversion, 1, owner);
```

## TremorEntangle

振幅絡縺を行うことが出来ます。  
直前で特殊振動を付与している必要があります。

> 絡縺させる振動効果と、対象の振動系効果を振動-重畳に合算。絡縺時、振動の値が維持される。
> 振動-重畳に振幅変換発動時、変換させる振動効果が絡縺で適用される。
>
> 幕の終了時、振動に変換される

```cs
// 特殊振動を付与する(値は自動で揃うため、1の付与で良い)
target.bufListDetail.AddKeywordBufThisRoundByCard(MyKeywordBuf.SpecialTremor, 1, owner);

// 振幅絡縺する
target.bufListDetail.AddKeywordBufThisRoundByCard(LimKeywordBuf.TremorEntangle, 1, owner);
```

## Events

### `ILimbuf.OnTremorBurst.OnTremorBurst(int stack)`

振動爆発によって影響を受けたときに実行されます。

- `stack`: 振動の値です。

### `ILimbuf.OnApplyTremorBurst.OnApplyTremorBurst()`

振動爆発が適用されたときに実行されます。

### `ILimbuf.OnTremorConversion.OnTremorConversion()`

振幅変換が適用されたときに実行されます。

### `ILimbuf.OnTremorEntangle.OnTremorEntangle()`

振幅絡縺が適用されたときに実行されます。

### `ILimbuf.OnExecTremorBurst.OnExecTremorBurst(BattleUnitModel)`

振動爆発させたときに実行されます。

# TremorScorch

リンバスの"振動-灼熱"と同じような挙動をするバフです。

> 振動爆発を受けるとき、{0}だけ混乱追加ダメージを受ける。
>
> 振動爆発を受ける時、(振動と火傷の合計÷2)だけ効果体力ダメージを受け、火傷数値が4/5に減少(端数切り捨て)。

```cs
// targetの振動を灼熱に変換
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorScorch, 1, owner);
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorConversion, 1, owner);

// targetの振動へ灼熱を絡縺
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorScorch, 1, owner);
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorEntangle, 1, owner);
```

# TremorHemorrhage

リンバスの"振動-大量出血"と同じような挙動をするバフです。

> 振動爆発を受けるとき、{0}だけ混乱追加ダメージを受ける。
>
> 振動爆発を受ける時、(振動と出血の合計÷2)だけ効果体力ダメージを受け、出血数値が4/5に減少(端数切り捨て)。

```cs
// targetの振動を大量出血に変換
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorHemorrhage, 1, owner);
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorConversion, 1, owner);

// targetの振動へ大量出血を絡縺
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorHemorrhage, 1, owner);
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorEntangle, 1, owner);
```

# TremorReverb

リンバスの"振動-反響"と同じような挙動をするバフです。

> 振動爆発を受けるとき、{0}だけ混乱追加ダメージを受ける。
>
> 振動爆発を受ける時、振動の数値だけ効果体力ダメージを受ける。

```cs
// targetの振動を反響に変換
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorReverb, 1, owner);
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorConversion, 1, owner);

// targetの振動へ反響を絡縺
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorReverb, 1, owner);
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorEntangle, 1, owner);
```

# TremorChain

リンバスの"振動-鎖"と同じような挙動をするバフです。

> 振動爆発を受けるとき、{0}だけ混乱追加ダメージを受ける。
>
> マッチ時、振動10につきダイス威力が1減少(最大3)

```cs
// targetの振動を鎖に変換
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorChain, 1, owner);
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorConversion, 1, owner);

// targetの振動へ鎖を絡縺
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorChain, 1, owner);
target.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorEntangle, 1, owner);
```
