# `using DeviceOfHermes.AdvancedBase;`

名前空間`DeviceOfHermes.AdvancedBase`に存在するクラスです。  
この記事はこれを前提に書かれています。

# BattleAmmoBuf

"弾丸"のバフを作成・管理するためのクラスです。

例:  
```cs
class BattleUnitBuf_Ammo : BattleAmmoBuf
{
    public override int GainByReload => this.DefaultStack;

    public override bool DiceBlockWithNotConsumable => true;

    public override void OnBeforeConsume(ref int require)
    {
    }

    public override void OnConsume(int consumed)
    {
    }

    public override void OnCancelled()
    {
    }

    public override bool IsConsumable(int num = 1)
    {
        return base.stack != 0 && base.stack >= num;
    }

    public override bool OnReload()
    {
        return true;
    }
}

var buf = owner.GetBuf<BattleUnitBuf_Ammo>();

buf.Reload();
buf.Consume(1);

var consumed = buf.ConsumedStack;
var losed = buf.LosedStack;

class BattleUnitBuf_Reload : ReloadAmmoBuf<BattleUnitBuf_Ammo>
{
}

owner.bufListDetail.AddBuf(new ReloadAmmoBuf<BattleUnitBuf_Ammo>());

owner.ConsumeAmmo<BattleUnitBuf_Ammo>(2);
owner.ReloadAmmo<BattleUnitBuf_Ammo>();
```

このクラスは`BattleUnitBuf`を継承しています。  
それの代わりに`BattleAmmoBuf`を継承し、幾つかの追加要素のあるそれとして扱うことを想定しています。

## Specified usage

### `GainByReload`

```cs
public override int GainByReload => this.DefaultStack;
```

リロードで獲得するスタック数を指定するプロパティです。

### `DiceBlockWithNotConsumable`

```cs
public override bool DiceBlockWithNotConsumable => true;
```

弾丸を消耗できないとき、対象の攻撃ダイスをブロックするか指定するプロパティです。

### `OnBeforeConsume(int)`

```cs
public override void OnBeforeConsume(ref require)
{
}
```

弾丸を消耗する前に実行されます。  
`require`の値を変更することで消耗量を変化させることが出来ます。

- `require`: 弾丸を消耗しようとする値です。

### `OnConsume(int)`

```cs
public override void OnConsume(int consumed)
{
}
```

弾丸が消耗されたときに実行されます。

- `consumed`: 消耗された弾丸の値です。

### `OnCancelled()`

```cs
public override void OnCancelled()
{
}
```

攻撃がブロックされたときに実行されます。  
`DiceBlockWithNotConsumable`がtrueでない場合は実行されません。

### `IsConsumable(int)`

```cs
public override bool IsConsumable(num)
{
    return base.stack != 0 && base.stack >= num;
}
```

弾丸を`num`だけ消耗できるかを返します。  

- `num`: 消耗できるか確認する値です。

### `OnReload()`

```cs
public override bool OnReload()
{
    return true;
}
```

弾丸がリロードされたときに実行されます。  
falseを返した場合はリロードがキャンセルされます。

### `Reload()`

```cs
buf.Reload();
```

弾丸をリロードします。

- `buf`: 対象の弾丸バフです。

### `Consume(int)`

```cs
buf.Consume(num);
```

弾丸を消耗します。

- `buf`: 対象の弾丸バフです。
- `num`: 消耗する弾丸の数です。

### `ConsumedStack`

```cs
var consumed = buf.ConsumedStack;

```

消耗によって失った弾丸の数です。  
リロードを含みません。

- `buf`: 対象の弾丸バフです。

### `LosedStack`

```cs
var losed = buf.LosedStack;
```

消耗によって失った弾丸の数です。  
リロードを含みます。

- `buf`: 対象の弾丸バフです。

### `ReloadAmmoBuf<T>`

```cs
class BattleUnitBuf_Reload : ReloadAmmoBuf<BattleUnitBuf_Ammo>
{
}

owner.bufListDetail.AddBuf(new ReloadAmmoBuf<BattleUnitBuf_Ammo>());
```

`ReloadAmmoBuf<T>`はリロード用の即時適用型バフです。  
継承したバフを追加するか、`ReloadAmmoBuf<T>`自体を追加することで即座にリロードを行います。

- `T`: 対象の弾丸バフの型です。

### `ConsumeAmmo<T>(int)`

```cs
owner.ConsumeAmmo<T>(num);
```

弾丸を消耗します。  
弾丸を保有していないなら何もしません。

- `T`: 対象の弾丸バフの型です。
- `num`: 消耗する弾丸の数です。

### `ReloadAmmo<T>()`

```cs
owner.ReloadAmmo<T>();
```

弾丸をリロードします。  
弾丸を保有していないなら何もしません。

- `T`: 対象の弾丸バフの型です。
