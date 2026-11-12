# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# VannilaUnitBuf

バニラのバフを編集します。  
ただしKeywordBufが有効なバフも影響を受けます。  

例:  
```cs
VannilaUnitBuf.SetMaxForcely<BattleUnitBuf_warpCharge>(20);

VannilaUnitBuf.AddMaxIf<BattleUnitBuf_smoke>(20, (buf, owner) => owner.passiveDetail.HasPassive<PassiveAbility_cigar>());

VannilaUnitBuf.AddAltIcon<BattleUnitBuf_bleeding>("AltBleeding", (buf, owner) => true);

VannilaUnitBuf.AddAltId<BattleUnitBuf_bleeding>("AltBleeding", (buf, owner) => true);
```

強制的に上限値を変える場合は`SetMaxForcely`を使用します。  
条件付きの上限値を変更する場合は`AddMaxIf`を使用します。  

バフ効果の特殊アイコンを追加する場合は`AddAltIcon`を使用します。  
`KeywordId`ごと変更する場合は`AddAltId`を使用します。

## Specified usage

### `SetMaxForcely<T>(int)`

```cs
VannilaUnitBuf.SetMaxForcely<T>(max);
```

バフの上限値を強制的に変更します。

- `T`: 変更する対象のバフです。`bufType`がNoneで無いことが必要です。
- `max`: 強制的に変更する最大値です。

### `SetMaxIf<T>(int, Func<BattleUnitBuf, BattleUnitModel, bool>)`

```cs
VannilaUnitBuf.SetMaxIf<T>(max, cond);
```

バフの上限値の設定を初期化します。

- `T`: 変更する対象のバフです。`bufType`がNoneで無いことが必要です。
- `max`: 強制的に変更する最大値です。
- `cond(BattleUnitBuf, BattleUnitModel?): bool`: 対象のバフとバフの保有者から、上限値が適用されるかの条件式です。trueを返した場合のみ上限が適用されます。

### `AddMaxIf<T>(int, Func<BattleUnitBuf, BattleUnitModel, bool>)`

```cs
VannilaUnitBuf.AddMaxIf<T>(max, cond);
```

バフの上限値の設定を追加します。

- `T`: 変更する対象のバフです。`bufType`がNoneで無いことが必要です。
- `max`: 強制的に変更する最大値です。
- `cond(BattleUnitBuf, BattleUnitModel?): bool`: 対象のバフとバフの保有者から、上限値が適用されるかの条件式です。trueを返した場合のみ上限が適用されます。

### `AddAltIcon<T>(string, Func<BattleUnitBuf, BattleUnitModel, bool>)`

```cs
VannilaUnitBuf.AddAltIcon<T>(iconId, cond);
```

バフアイコンを条件付きで変更します。

- `T`: 変更する対象のバフです。`bufType`がNoneで無いことが必要です。
- `iconId`: 変更先のアイコン画像のIDです。
- `cond(BattleUnitBuf, BattleUnitModel?): bool`: 対象のバフとバフの保有者から、バフアイコンを変更するかの条件式です。trueを返した場合のみアイコンが変更されます。

### `AddAltId<T>(string, Func<BattleUnitBuf, BattleUnitModel, bool>)`

```cs
VannilaUnitBuf.AddAltId<T>(id, cond);
```

バフのIDを条件付きで変更します。

- `T`: 変更する対象のバフです。`bufType`がNoneで無いことが必要です。
- `id`: 変更先のIDです。
- `cond(BattleUnitBuf, BattleUnitModel?): bool`: 対象のバフとバフの保有者から、IDを変更するかの条件式です。trueを返した場合のみIDが変更されます。
