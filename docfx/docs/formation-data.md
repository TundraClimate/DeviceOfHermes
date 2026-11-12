# FormationXml

XMLファイルを配置することでフォーメーションを追加することが出来ます。  
条件は`DeviceOfHermes`のサブスクライブのみです。  

[HermesConfig](hermes-config.md)に従って読み込みます。  
フォルダ・ファイルが存在しない場合は無視します。

`Formation.xml`の内容例:  
```xml
<?xml version="1.0" encoding="utf-8" ?>
<FormationXmlRoot>

    <!-- ID="100"でフォーメーションを追加 -->
    <Formation ID="100">
        <Position>
            <Vector x="12" y="0"/>
        </Position>
        <Position>
            <Vector x="6" y="-12"/>
        </Position>
        <Position>
            <Vector x="5" y="12"/>
        </Position>
        <Position>
            <Vector x="18" y="12"/>
        </Position>
        <Position>
            <Vector x="17" y="-12"/>
        </Position>
    </Formation>

</FormationXmlRoot>
```

この方法で追加したフォーメーションが有効なのは同じPackageIdで追加された接待のみです。  
また、`AddLibrarianMap`を実行することで特定の接待でのみ司書側のフォーメーションを設定できます。

```cs
// ファイルの頭に必要
using DeviceOfHermes.Resource;

// 接待と司書のフォーメーションを紐付ける
Formation.AddLibrarianMap("MODのID", /* 接待のID */, /* フォーメーションのID */);
```
