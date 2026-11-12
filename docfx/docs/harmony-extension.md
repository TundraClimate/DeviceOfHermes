# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# HarmonyExtension

Harmonyを使いやすくする拡張モジュールです。

## HarmonyInfo

当たっているパッチを取得します。

```cs
// MethodInfoからパッチ情報を取得する
// 'BattleUnitModel'の'TakeDamage'から情報を取得する
var patches = typeof(BattleUnitModel).Method("TakeDamage").GetPatches();

foreach (var patch in patches)
{
    // パッチの種類
    // Prefix, Postfix, Transpiler and Finalizer
    HarmonyExtension.PatchType ty = patch.PatchType;

    // パッチ元のHarmony
    // new Harmony("ここの部分");
    string owner = patch.Owner;

    // パッチ対象
    // [HarmonyPatch(ここの部分)]
    MethodInfo method = patch.Method;
}
```

## Specified usage

### `MethodInfo.GetPatches()`

```cs
List<HarmonyExtension.PatchInfo> patches = method.GetPatches();
```

メソッドからパッチのリストを取得します。

### `PatchInfo.PatchType`

パッチの種類です。

### `PatchInfo.Owner`

パッチ元のHarmonyのIDです。

### `PatchInfo.Method`

パッチ対象のメソッドです。

### `PatchInfo.Priority`

パッチそのもののPriorityです。
