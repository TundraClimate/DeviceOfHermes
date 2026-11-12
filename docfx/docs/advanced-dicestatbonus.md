# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdvancedDiceStatBonus

既存の`DiceStatBonus`に追加で要素を追加した拡張版です。

```cs
dice.ApplyDiceStatBonus(new AdvancedDiceStatBonus()
{
    // 威力をX倍に受ける
    powerMultiplier = 500,

    // 威力の効果がX%増加
    powerRate = 25000,

    // KeywordBufの付与時に適用される変更
    // 例: バフの数値付与量+1000
    kwdBufModifier = (origin, ref buf, target) => buf.stack += 1000,

    // 威力が無効になる
    ignorePower = true,

    // ダイスロールの出目が最大値に近くなる重みを加算する
    // 初期値: 100
    // 合計で400-500程度になるとほぼ最大値が出るようになる
    highrollGlobalWeight = 400,

    // 攻撃的中時に追撃する確率を加算(100以上で確定追撃1回に変換)
    // 1回の確定追撃
    // 50%の確率で追撃
    ferocity = 150,

    // 攻撃が的中せず、ダメージも与えない
    noHit = true,

    // カスタムキーを作成して適用する
    customFields = new() { ["foo"] = 1 };
});
```

## Specified usage

### `powerMultiplier`

```cs
powerMultiplier = mul,
```

威力の影響をX倍に受けます。  
複数回適用した場合、それぞれ乗算されます。

- `mul`: 倍率です。

### `powerRate`

```cs
powerRate = rate,
```

威力の影響をX%増加します。  
複数回適用した場合、それぞれ加算されます。

- `rate`: 加算割合です。

### `kwdBufModifier`

```cs
kwdBufModifier = (origin, ref buf, target) => { },
```

`KeywordBuf`を付与した場合に適用される変更です。  
`buf`は再代入可能であり、代入結果が実際に適用されるバフ効果になります。  
nullを代入した場合対象のバフを付与する処理はスキップされます。

- `origin`: 変更元のバフ効果です。
- `buf`: 実際に使用されるバフ効果です。
- `target`: バフを付与する対象です。

### `ignorePower`

```cs
ignorePower = true
```

威力が無効になります。

### `highrollGlobalWeight`

```cs
highrollGlobalWeight = weight;
```

ダイスロールの出目が最大値に近くなる重みを加算します。

基礎値が100で、通常の確率です。  
400-500付近でほとんど最大値が出るようになります。  
逆に0-99は最低値が出やすくなります。

- `weight`: 重みの加算数値です。負の値も使えます。

### `ferocity`

```cs
ferocity = addper;
```

攻撃的中時に追撃する確率を加算します。  
100につき確定で追撃する回数1を得ます。  
残りの確率で追撃回数を得ます。

- `addper`: 追撃確率の加算数値です。

### `noHit`

```cs
noHit = true;
```

攻撃が的中せず、ダメージも与えません。

### `customFields`

```cs
customFields = new() { ["foo"] = 1 };
```

任意で設定できるカスタム設定です。  
キーが重複した場合はそれらを加算します。
