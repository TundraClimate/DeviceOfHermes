# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# PassiveResolver

パッシブの帰属可否を管理します。

例:  
```cs
PassiveResolver.AddRestrict(PassiveResolver.LockAlways(new LorId("あなたのMODID", 1)));

PassiveResolver.AddRestrict(PassiveResolver.Conflict(new LorId(250322), new LorId(250024)));
```

`LockAlways`は指定したIDのパッシブを帰属不可にします。  
`Conflict`は指定したID同士で重複できないようにします。

## Specified usage

### `AddRestrict(Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool>)`

```cs
PassiveResolver.AddRestrict(restrict);
```

帰属制約を追加します。

- `restrict(List<PassiveXmlInfo>, PassiveXmlInfo): bool`: 保有しているパッシブから制限を与えるか決める関数です。trueを返すと帰属を制限できます。

### `LockAlways(LorId)`

```cs
PassiveResolver.LockAlways(id);
```

特定のIDのパッシブは常に帰属できないという制約です。

- `id`: 制約を与えるパッシブのIDです。

### `LockIf(LorId, Func<List<PassiveXmlInfo>, bool>)`

```cs
PassiveResolver.LockIf(id, cond);
```

特定のIDのパッシブは一定条件下では帰属できないという制約です。

- `id`: 制約を与えるパッシブのIDです。
- `cond(List<PassiveXmlInfo>): bool`: 保有しているパッシブから制限を与えるか決める関数です。trueを返すと帰属を制限できます。

### `LockIfExists(LorId, params LorId[])`

```cs
PassiveResolver.LockIfExists(id, passives...);
```

特定のIDのパッシブはまた別の特定のIDのパッシブが帰属されているなら帰属できないという制約です。

- `id`: 制約を与えるパッシブのIDです。
- `params passives`: 帰属制限をするためのパッシブのIDです。

### `Conflict(params LorId[])`

```cs
PassiveResolver.Conflict(cand...);
```

幾つかのIDのパッシブは重複できないという制約です。

- `params cand`: 重複を制限するパッシブのIDです。
