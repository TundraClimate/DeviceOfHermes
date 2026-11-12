# `using DeviceOfHermes.Effect;`

名前空間`DeviceOfHermes.Effect`に存在するクラスです。  
この記事はこれを前提に書かれています。

# SimpleAura

シンプルなオーラを組み合わせることが出来るモジュールです。

```cs
// オーラの設定
class MyCustomAura : SimpleAuraBase
{
    public override void Init()
    {
        // オーラの形状を指定
        SetMaterial(AuraLayer.AuraM1, AuraSpec.Mat1);

        // オーラの色を指定
        SetColor(AuraLayer.AuraM1, Color.yellow);
    }
}

// OnWaveStartなど

// BattleUnitModelに対して付与する
SimpleAuraFactory.Attach(owner, new MyCustomAura());
```

## Customize

オーラ設定の詳細情報

```cs
class MyCustomAura : SimpleAuraBase
{
    public override void Init()
    {
        // 'AuraM1'レイヤーに対して'Mat1'を割り当てる
        SetMaterial(AuraLayer.AuraM1, AuraSpec.Mat1);

        // 'AuraM2'レイヤーに対して'Mat2'を割り当てる
        SetMaterial(AuraLayer.AuraM2, AuraSpec.Mat2);

        // 'AuraM1'レイヤーに対して'yellow'を割り当てる
        SetColor(AuraLayer.AuraM1, Color.yellow);

        // 'AuraM2'レイヤーに対して'red'を割り当てる
        SetColor(AuraLayer.AuraM2, Color.red);
    }
}
```

使用可能なAuraLayer
- Aura1: ユニット前方 一番前
- Aura2: ユニット前方 Aura1の後方
- Aura3: ユニット前方 キャラクターの面前
- AuraN3: ユニット後方 キャラクターの後背
- AuraN2: ユニット後方 AuraN3より更に後方
- AuraN1: ユニット後方 一番後ろ

使用可能なAuraSpec  
- Mat1: 一般的なオーラエフェクト
- Mat2: 少し線が少ないオーラエフェクト
- Mat2: 線が多めなオーラエフェクト

## Specified usage

### `SimpleAuraFactory.Attach(BattleUnitModel, SimpleAuraBase)`

```cs
SimpleAuraFactory.Attach(owner, aura);
```

`BattleUnitModel`に対して`aura`を付与します。  
基本的にユニットが除去されるまで付与されたままです。

- `owner`: 付与する対象のユニットです。
- `aura`: 付与するオーラ設定です。

### `SetMaterial(AuraLayer, AuraSpec)`

```cs
SetMaterial(layer, spec);
```

指定したレイヤーのエフェクト形状を変更します。

- `layer`: 変更するレイヤーです。
- `spec`: 設定するエフェクト形状です。

### `SetColor(AuraLayer, Color)`

```cs
SetColor(layer, color);
```

指定したレイヤーのカラーを変更します。

- `layer`: 変更するレイヤーです。
- `color`: 設定するカラーです。
