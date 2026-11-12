# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Ammo

リンバスの"弾丸"と同じような挙動をするバフです。

> 最大値: {0}  
>
> 特定の攻撃ダイスで弾丸を消耗することが出来る  
> 弾丸消耗時、弾丸が足りないなら該当のダイスは的中しない  

```cs
// 最大12発で弾丸を得る
owner.InitAmmo(new BattleUnitBuf_Limbuf_Ammo(), 12);

// diceで2発弾丸を消耗
if (owner.ConsumeAmmo(dice, 2))
{
    // 消耗できた時の処理
}

// diceは弾丸を消耗したかどうか
if (dice.IsConsumedAmmo())
{
    // 消耗できていた時の処理
}
```

## Ammo

弾丸及び特殊弾丸について

基底クラス: `BattleUnitBuf_Limbuf_Ammo`  

```cs
// OnWaveStart()

// 弾丸数値を初期化する
// 弾丸最大値を同時に指定
owner.InitAmmo(new BattleUnitBuf_Limbuf_Ammo(), 12);
```

弾丸バフは最大値が指定される必要があります。  
これ以降の処理は`InitAmmo`がされている前提です。  

```cs
// 弾丸を指定数値だけ消耗する
// 返り値は指定数値だけ弾丸を消耗できたかどうか
var success = owner.ConsumeAmmo(dice, 1);

if (success)
{
    // 弾丸が消耗できた時の処理
}

// diceが弾丸を消耗できたかどうか
var isConsumed = dice.IsConsumedAmmo();

if (isConsumed)
{
    // diceが弾丸を消耗出来ている場合にのみ実行される
}
```

`ConsumeAmmo`は消耗数値以上の弾丸数値を保有している場合のみ弾丸を消耗します。  
消耗できるだけ消耗したい場合は数値1を指定して複数回呼び出してください。  

```cs
// 再装填
owner.bufListDetail.AddBuf(new BattleUnitBuf_Limbuf_Reload<BattleUnitBuf_Limbuf_Ammo>());
```

`BattleUnitBuf_Limbuf_Reload`をAddBufすることで即時再装填を行います。

```cs
// 特殊弾丸
public class BattleUnitBuf_Ammo_Foo : BattleUnitBuf_Limbuf_Ammo
{
    // 弾丸が空でも攻撃が的中するかどうか
    // trueを返すなら空でも攻撃が的中する
    public override bool CanFireWhileEmpty => false;

    // 弾丸を消耗する前に実行される
    // dice: 消耗する対象のダイス
    // current: 消耗する前の弾丸数値
    // num: 消耗しようとしている弾丸数値
    public override int BeforeConsume(BattleDiceBehavior dice, int current, int num)
    {
        // 1以上を返す場合、返した数値が弾丸消耗要求数値に加算される
        return 0;
    }

    // 弾丸が消耗されたときに実行される
    // dice: 消耗した対象のダイス
    // num: 消耗した弾丸数値
    public override void OnConsume(BattleDiceBehavior dice, int num)
    {
    }

    // 弾丸が消耗されて空になったときに実行される
    // dice: 消耗した対象のダイス
    public override void OnEmpty(BattleDiceBehavior dice)
    {
    }

    // 弾丸が再装填されたときに実行される
    public override void OnReload()
    {
    }
}
```

この他に`keywordId`・`keywordIconId`などを設定することで特殊弾丸を作成することが出来ます。

```cs
// バフが'弾丸'であるか判定する
if (battleUnitBuf is BattleUnitBuf_Limbuf_Ammo)
{
    // battleUnitBufが弾丸であった場合の処理
}

// 特殊弾丸の初期化(最大値6)
owner.InitAmmo(new BattleUnitBuf_Ammo_Foo(), 6);

// diceで1発特殊弾丸を消耗
if (owner.ConsumeAmmo<BattleUnitBuf_Ammo_Foo>(dice, 1))
{
    // 消耗できた時の処理
}

// diceは特殊弾丸を消耗したかどうか
if (dice.IsConsumedAmmo<BattleUnitBuf_Ammo_Foo>())
{
    // 消耗できていた時の処理
}

// 特殊弾丸を再装填
owner.bufListDetail.AddBuf(new BattleUnitBuf_Limbuf_Reload<BattleUnitBuf_Ammo_Foo>());
```

特殊弾丸を扱う場合も通常の弾丸と同じようにすることが出来ます。

```cs
// 特殊再装填
public class Foo_Reload : BattleUnitBuf_Limbuf_Reload<BattleUnitBuf_Ammo_Foo>
{
    // 再装填が実行される直前に実行される
    public override void BeforeReload()
    {
    }
}
```

アイコンなどを変更したい場合は特殊再装填を作成することも出来ます。

```cs
// 文字エフェクト付きで再装填
owner.bufListDetail.AddBuf(new BattleUnitBuf_Limbuf_Reload<BattleUnitBuf_Limbuf_Ammo>() { appliesEffect = true });
```

文字エフェクト付き再装填も実装されています。

## Events

### `ILimbuf.OnConsumeAmmo.OnConsumeAmmo(BattleUnitBuf ammo, BattleDiceBehavior dice, int num)`

自分が弾丸を消耗したときに実行されます。

### `ILimbuf.OnAmmoEmpty.OnAmmoEmpty(BattleUnitBuf ammo, BattleDiceBehavior dice)`

自分が弾丸を消耗して空になったときに実行されます。

### `ILimbuf.OnReloadAmmo.OnReloadAmmo(BattleUnitBuf ammo)`

弾丸を再装填したときに実行されます。

## Specified usage

### `BattleUnitModel.InitAmmo<T: BattleUnitBuf_Limbuf_Ammo>(T, int)`

```cs
owner.InitAmmo(ammo, max);
```

ユニットの保有する弾丸を初期化します。  
弾丸を複数保有する場合、それぞれ実行する必要があります。

- `ammo`: 弾丸バフの実体です。
- `max`: 弾丸ごとに紐付ける最大値です。

### `BattleUnitModel.ConsumeAmmo<T: BattleUnitBuf_Limbuf_Ammo>(BattleDiceBehavior, int)`

```cs
bool success = owner.ConsumeAmmo<T>(dice, num);
```

ダイスが弾丸を消耗します。  
`T`は対象の弾丸型を指定してください。  

消耗数以上弾丸を保有している場合はtrue、そうでないならfalseを返します。

- `dice`: 対象のダイスです。
- `num`: 消耗する数値です。

### `BattleDiceBehavior.IsConsumedAmmo()`

```cs
bool isConsumed = dice.IsConsumedAmmo();
```

ダイスで弾丸を消耗したかを返します。  
消耗したのであればtrue、そうでないならfalseを返します。

### `BattleUnitModel.ReloadAmmo<T>()`

```cs
owner.ReloadAmmo<T>();
```

即座に弾丸を再装填します。  
`T`は対象の弾丸型を指定してください。

### `BattleUnitBuf_Limbuf_Ammo.CanFireWhileEmpty`

```cs
public override bool CanFireWhileEmpty => false;
```

弾丸が空でも攻撃が的中するかどうかのプロパティです。  
trueを返すなら攻撃が的中します。

### `BattleUnitBuf_Limbuf_Ammo.TotalConsumed`

```cs
int total = ammo.TotalConsumed;
```

弾丸の合計消耗量を獲得できます。

### `BattleUnitBuf_Limbuf_Ammo.Consume(BattleDiceBehavior, int)`

```cs
bool success = ammo.Consume(dice, num);
```

弾丸を消耗します。  
`ConsumeAmmo`を参照してください。

- `dice`: 対象のダイスです。
- `num`: 消耗数値です。

### `BattleUnitBuf_Limbuf_Ammo.BeforeConsume(BattleDiceBehavior, int, int)`

```cs
public override int BeforeConsume(BattleDiceBehavior dice, int current, int num)
{
    return 0;
}
```

消耗処理が実行される前に実行されます。  
1以上の数値を返した場合はその数値だけ消耗要求値が加算されます。

- `dice`: 対象のダイスです。
- `current`: 消耗前の弾丸数値です。
- `num`: 消耗しようとする弾丸数値です。

### `BattleUnitBuf_Limbuf_Ammo.OnConsume(BattleDiceBehavior, int)`

```cs
public override void OnConsume(BattleDiceBehavior dice, int num)
{
}
```

弾丸が消耗されたときに実行されます。

- `dice`: 対象のダイスです。
- `num`: 消耗した弾丸数値です。

### `BattleUnitBuf_Limbuf_Ammo.OnEmpty(BattleDiceBehavior)`

```cs
public override void OnEmpty(BattleDiceBehavior dice)
{
}
```

弾丸が消耗されて空になったときに実行されます。  

- `dice`: 消耗したダイスです。

### `BattleUnitBuf_Limbuf_Ammo.Reload()`

```cs
ammo.Reload();
```

弾丸を即座に再装填します。

### `BattleUnitBuf_Limbuf_Ammo.OnReload()`

```cs
public override void OnReload()
{
}
```

弾丸が再装填されたときに実行されます。

### `BattleUnitBuf_Limbuf_Reload.BeforeReload()`

```cs
public override void BeforeReload()
{
}
```

再装填が実行される前に実行されます。

### `BattleUnitBuf_Limbuf_Reload.appliesEffect`

```cs
new BattleUnitBuf_Limbuf_Reload() { appliesEffect = true };
```

再装填時に文字エフェクトを表示するかどうかのフラグです。
