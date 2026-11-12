# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# BattleBufExtension

`BattleUnitBuf`の取得管理を拡充します。

例:  
```cs
var getBuf = owner.GetBuf<BattleUnitBuf_MyUnitBuf>();

if (owner.TryGetBuf<BattleUnitBuf_MyUnitBuf>(out var tryGetBuf))
{
}

var getOrInit = owner.GetBufAndInitIfNull<BattleUnitBuf_MyUnitBuf>(() => new BattleUnitBuf_MyUnitBuf());

var getReadyBuf = owner.GetReadyBuf<BattleUnitBuf_MyUnitBuf>();

if (owner.TryGetReadyBuf<BattleUnitBuf_MyUnitBuf>(out var tryGetReadyBuf))
{
}

var getOrInitReady = owner.GetReadyBufAndInitIfNull<BattleUnitBuf_MyUnitBuf>(() => new BattleUnitBuf_MyUnitBuf());

owner.RemoveBuf<BattleUnitBuf_MyUnitBuf>();

owner.RemoveBufIf(buf => buf is BattleUnitBuf_MyUnitBuf);

var stack = owner.GetBufStack<BattleUnitBuf_MyUnitBuf>() ?? 0;

var readyStack = owner.GetBufStack<BattleUnitBuf_MyUnitBuf>() ?? 0;

owner.AddBufStack<BattleUnitBuf_MyUnitBuf>(2, () => new BattleUnitBuf_MyUnitBuf(owner));
owner.AddBufStack<BattleUnitBuf_MyUnitBuf>(2);

owner.AddReadyBufStack<BattleUnitBuf_MyUnitBuf>(2, () => new BattleUnitBuf_MyUnitBuf(owner));
owner.AddReadyBufStack<BattleUnitBuf_MyUnitBuf>(2);

buf.UpdateBufIcon();
owner.UpdateBufIcons();

owner.UseCharge(5, false);
```

## Specified usage

### `GetBuf<T>()`

```cs
var getBuf = owner.GetBuf<T>();
```

現在有効な特定バフを取得します。有効でないならばnullを返します。

- `owner`: 取得対象です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。

### `TryGetBuf<T>(out BattleUnitBuf)`

```cs
if (owner.TryGetBuf<T>(out var tryGetBuf))
{
}
```

現在な有効な特定バフを取得します。有効でないならfalseを返します。

- `owner`: 取得対象です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。
- `tryGetBuf`: 取得したバフです。falseを返した場合はnullになります。

### `GetBufAndInitIfNull<T>(Func<BattleUnitBuf>)`

```cs
var getOrInit = owner.GetBufAndInitIfNull<T>(init);
```

現在な有効な特定バフを取得します。取得に失敗した場合は`init`を実行して有効化したものを返します。

- `owner`: 取得対象です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。
- `init: T`: `T`自体のインスタンスを返す関数です。バフを取得できたなら実行されません。

### `GetReadyBuf<T>()`

```cs
var getReadyBuf = owner.GetReadyBuf<T>();
```

次の幕に有効な特定バフを取得します。有効でないならばnullを返します。

- `owner`: 取得対象です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。

### `TryGetReadyBuf<T>(our BattleUnitBuf)`

```cs
if (owner.TryGetReadyBuf<T>(out var tryGetReadyBuf))
{
}
```

次の幕に有効な特定バフを取得します。有効でないならfalseを返します。

- `owner`: 取得対象です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。
- `tryGetReadyBuf`: 取得したバフです。falseを返した場合はnullになります。

### `GetReadyBufAndInitIfNull<T>(Func<BattleUnitBuf>)`

```cs
var getOrInitReady = owner.GetReadyBufAndInitIfNull<T>(init);
```

次の幕に有効な特定バフを取得します。取得に失敗した場合は`init`を実行して有効化したものを返します。

- `owner`: 取得対象です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。
- `init: T`: `T`自体のインスタンスを返す関数です。バフを取得できたなら実行されません。

### `RemoveBuf<T>()`

```cs
owner.RemoveBuf<T>();
```

現在有効なバフから特定のバフのみ消去します。

- `owner`: 消去対象です。
- `T`: 消去対象のバフ型です。`BattleUnitBuf`である必要があります。

### `RemoveBufIf(Func<BattleUnitBuf, bool>)`

```cs
owner.RemoveBufIf(cond);
```

現在有効なバフから条件式に合致したバフだけ消去します。

- `owner`: 消去対象です。
- `cond(BattleUnitBuf): bool`: 条件式です。trueを返した`BattleUnitBuf`を消去します。

### `GetBufStack<T>()`

```cs
var stack = owner.GetBufStack<T>() ?? 0;
```

現在有効なバフから特定のバフのスタック数を取得します。特定のバフが有効でないならnullを返します。

- `owner`: 取得対象です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。

### `GetReadyBufStack<T>()`

```cs
var readyStack = owner.GetReadyBufStack<T>() ?? 0;
```

次の幕に有効なバフから特定のバフのスタック数を取得します。特定のバフが有効でないならnullを返します。

- `owner`: 取得対象です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。

### `AddBufStack<T>(int, Func<BattleUnitBuf>)`

```cs
owner.AddBufStack<T>(num, init);
```

現在有効なバフから特定のバフのスタック数だけ加算します。  
バフが有効でないなら`init`を実行して有効化します。

- `owner`: 取得対象です。
- `num`: 追加するスタック数です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。
- `init: T`: `T`自体のインスタンスを返す関数です。バフを取得できたなら実行されません。

### `AddBufStack<T>(int)`

```cs
owner.AddBufStack<T>(num);
```

現在有効なバフから特定のバフのスタック数だけ加算します。  
バフが有効でないなら新規に有効化してスタック数を加算します。

- `owner`: 取得対象です。
- `num`: 追加するスタック数です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。

### `AddReadyBufStack<T>(int, Func<BattleUnitBuf, bool>)`

```cs
owner.AddReadyBufStack<T>(num, init);
```

次の幕に有効なバフから特定のバフのスタック数だけ加算します。  
バフが有効でないなら`init`を実行して有効化します。

- `owner`: 取得対象です。
- `num`: 追加するスタック数です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。
- `init: T`: `T`自体のインスタンスを返す関数です。バフを取得できたなら実行されません。

### `AddReadyBufStack<T>(int)`

```cs
owner.AddReadyBufStack<T>(num);
```

次の幕に有効なバフから特定のバフのスタック数だけ加算します。  
バフが有効でないなら新規に有効化してスタック数を加算します。

- `owner`: 取得対象です。
- `num`: 追加するスタック数です。
- `T`: 取得対象のバフ型です。`BattleUnitBuf`である必要があります。

### `UpdateBufIcon()`

```cs
buf.UpdateBufIcon();
```

表示されているバフアイコンを更新します。

- `buf`: 対象の`BattleUnitBuf`です。

### `UpdateBufIcons()`

```cs
owner.UpdateBufIcons();
```

表示されている全てのバフアイコンを更新します。

- `owner`: 更新対象のユニットです。

### `UseCharge(int, bool)`

```cs
owner.UseCharge(num, isCard);
```

`BattleUnitBuf_warpCharge.UseStack`のエイリアスです。

- `num`: 消耗する充電数値です。
- `isCard`: ページによる消耗かどうかです
