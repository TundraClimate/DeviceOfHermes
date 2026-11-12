# OnlyCardXml

XMLファイルを配置することで専用ページを追加することが出来ます。  
条件は`DeviceOfHermes`のサブスクライブのみです。  

[HermesConfig](hermes-config.md)に従って読み込みます。  
フォルダ・ファイルが存在しない場合は無視します。

`OnlyCard.xml`の内容例:  
```xml
<?xml version="1.0" encoding="utf-8"?>
<OnlyCardXmlRoot>

    <!-- MODのID="10000001"のコアページを対象に -->
    <OnlyCard ID="10000001">
        <!-- MODのID="1"のページを専用ページとして追加 -->
        <Card>1</Card>
        <!-- バニラのID="701001"のページを専用ページとして追加 -->
        <Card Pid="@origin">701001</Card>
    </OnlyCard>

    <!-- MODのID="10000002"のコアページを対象に -->
    <OnlyCard ID="10000002">
        <!-- MODのID="2"のページを専用ページとして追加 -->
        <Card>2</Card>
    </OnlyCard>

</OnlyCardXmlRoot>
```
