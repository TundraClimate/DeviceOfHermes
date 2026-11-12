# AutoMapXml

XMLファイルを配置することで自動マップ設定を追加することが出来ます。  
条件は`DeviceOfHermes`のサブスクライブのみです。  

[AutoMapSetter](automap-setter.md)のXML指定ツールです。

[HermesConfig](hermes-config.md)に従って読み込みます。  
フォルダ・ファイルが存在しない場合は無視します。  

`automap-artwork`及び`automap-audio`が設定されている必要があります。

`AutoMap.xml`の内容例:  
```xml
<AutoMapXmlRoot>
  <!-- MapのID -->
  <AutoMap MapInfo="MyMap">

    <!-- 必須 -->
    <!-- 背景画像を指定する -->
    <!-- automap-artworkからの相対パスで指定 -->
    <BgImage>Bg.png</BgImage>

    <!-- 必須 -->
    <!-- 底面画像を指定する -->
    <!-- automap-artworkからの相対パスで指定 -->
    <FloorImage>Floor.png</FloorImage>

    <!-- BGMを指定する -->
    <!-- automap-audioからの相対パスで指定 -->
    <BGM>BGM.mp3</BGM>

    <!-- 接待中に表示される背景セリフの色を設定する -->
    <!-- 例: #FFFFFF、#FF0000 -->
    <DialogColor>#00FFFF</DialogColor>

    <!-- 接待中に表示される背景セリフのテキストIDを設定する -->
    <Dialog>mymap_dialog_0</Dialog>
    <Dialog>mymap_dialog_1</Dialog>
    <Dialog>mymap_dialog_2</Dialog>
    
    <!-- その他オプション -->
    <!-- NoBlackFrame: 画面端の黒色フレームを非表示 -->
    <Option>NoBlackFrame</Option>

  </AutoMap>
</AutoMapXmlRoot> 
```
