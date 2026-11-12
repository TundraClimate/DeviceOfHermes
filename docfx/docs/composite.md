# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# `Composite`

Abilityを合成する仕組みです。  
`IComposite`を実装したAbilityは指定した配列に存在するAbilityを合成します。

```cs
// 合成したAbilityに実装されているInterfaceがあるなら記述する
// [RouteInterfaces([typeof(...)])]
class DiceCardSelfAbility_Merged : DiceCardSelfAbilityBase, IComposite
{
    public static string Desc = "[使用時] ページを1枚引く\n[使用時] 体力を1回復";

    // 合成リスト
    public string[] children => ["drawCard", "recoverHp1"];
}
```

## Specified usage

### `IComposite`

```cs
: IComposite
```

実装したAbilityは指定した配列のAbilityを合成したものに変換されます。  
変換はインスタンスの作成時に実行されたため、既に作成されたものは変更されません。

一緒に実装したクラスをベースとして子のアビリティを合成します。

### `children`

```cs
public string[] children => abilities;
```

合成対象を指定します。  
このプロパティが呼び出されるのはインスタンスの作成時であるため、フィールドは全て未初期化状態です。

### `RouteInterfaces`

```cs
[RouteInterfaces(interfaces)]
```

指定した合成対象に実装されているインターフェースのリストを指定します。  
指定されているインターフェースのみ合成時に含まれて生成されます。
