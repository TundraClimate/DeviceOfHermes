# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AutoMapSetter

接待に紐付いた`MapInfo`と`MapManager`を紐付け、接待に自動で適用するモジュールです。  
XML版は[AutoMapXml](automap-data.md)を参照してください。  

```cs
// OnInitializeMod()
// 紐付けを行う

// 背景・底面を指定してマップを作成
// backGround・floorともに'Sprite'を指定
AutoMapSetter.SetMapFactory("MyMap", f => f.CreateNewMap(backGround, floor));

// AssetBundleなどから読み込む場合
AutoMapSetter.SetMapFactory("MyMap", _ => 
{
    var prefab = assetBundle.LoadAsset<GameObject>("MyMapPrefab");
    var go = UnityEngine.Object.Instatiate(prefab, BattleSceneRoot.Instance.transform);

    return go.GetComponent<MapManager>();
});
```

`StageInfo.xml`に記述を追加すると適用できます。
```xml
  <Stage id="1">
    <!-- 他の設定記述例は省略 -->

    <!-- 'AutoMap_'をマップのIDに付与する -->
    <MapInfo>AutoMap_MyMap</MapInfo>

  </Stage>
```

## Specified usage

### `SetMapFactory(string, Func<AutoMapSetter, MapManager>)`

```cs
AutoMapSetter.SetMapFactory(key, factory);
```

`MapInfo`のIDと`MapManager`を紐付けます。

- `key`: MapInfoで指定するIDの`AutoMap_`を除いたものです。
- `factory`: `MapManager`を生成する関数です。この関数内でマップのGameObjectが初期化される必要があります。

### `CreateNewMap(Sprite, Sprite, AudioClip[]?, Color?, List<string>?, NewMapOptions)`

```cs
AutoMapSetter.SetMapFactory("MyMap", f => f.CreateNewMap(bg, floor, bgm, dlgColor, dlgIds, opts));

// 例
AutoMapSetter.SetMapFactory("MyMap", f => f.CreateNewMap(bg, floor, [bgm], Color.white, ["mymod_dialog_0", "mymod_dialog_1"], NewMapOptions.NoBlackFrame));
```

`MapManager`とそのGameObjectを生成します。

- `bg`: 背景画像のSpriteです。
- `floor`: 底面画像のSpriteです。
- `bgm`: BGMのAudioClipです。
- `dlgColor`: 背景に流れるDialogの色です。
- `dlgIds`: 背景に流れるDialogが設定されたTextIdのリストです。
- `opts`: 追加で指定するオプションです。(`NoBlackFrame`: 画面端の黒色のフレームを表示しない)
