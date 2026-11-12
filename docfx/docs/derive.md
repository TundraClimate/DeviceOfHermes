# `using DeviceOfHermes.Derive;`

名前空間`DeviceOfHermes.Derive`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Caduceus REQUIRED

`Caduceus`を必要とします。  
詳細はトップページを参照してください。

# Derive

対応するテンプレートをクラスに指定し、自動で処理を実装する仕組みです。  
この自動実装は対応するメソッドがユーザーによって既に実装されている場合は**スキップされます。**  
対応するメソッドは大抵テンプレート名に含んでいるため、そちらを参照してください。

```cs
// クラスはpartialである必要があります
// テンプレート 'DestroyOnRoundEnd' を指定している
// ','で区切ることで複数のテンプレートを指定することが可能です
[Derive(typeof(DestroyOnRoundEnd))]
public partial class BattleUnitBuf_AutoDestroy : BattleUnitBuf
{
    // 以下のメソッドが自動で実装されます。
    /* 
    public override void OnRoundEnd()
    {
        base.OnRoundEnd();

        Destroy();
    }
    */

    // 自分で代わりに実装した場合は自動実装がスキップされます
    public override void OnRoundEnd()
    {
    }
}
```

組み込みテンプレートについては[DeriveTemplate](derive-template.md)を参照してください。

## Defines Template

テンプレートを自作することが出来ます。  

```cs
// public staticでクラスを作成してください
// 'DeriveTemplate'では対象とするクラスを指定してください(例: 'BattleUnitBuf'のメソッド用テンプレート)
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class MyTemplateInit
{
    // public static voidでメソッドを作成してください
    // Usageには対象とするメソッドを特定する情報を指定します
    // 宣言型、メソッド名、メソッド引数配列の順番です
    // DerivePriorityで指定した数値が高い順番に同じメソッド内のテンプレートが呼び出されます(初期値: 0)
    // 実際に宣言するメソッドの引数は第一引数にインスタンスが、それ以降は本来の引数が順番に注入されます。
    [DerivePriority(100)]
    [DeriveUsage(typeof(BattleUnitBuf), "Init", [ typeof(BattleUnitModel) ])]
    public static void OnRoundEnd(BattleUnitBuf instance, BattleUnitModel owner)
    {
        // instanceを使用して処理を実装する
    }
}
```
