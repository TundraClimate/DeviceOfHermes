# CAUTION

この記事の要素は**非推奨**です。  
この記事は互換性のためアーカイブされています。  

代替として、[AutoMapSetter](automap-setter.md)を使用してください。

# `using DeviceOfHermes.UI;`

名前空間`DeviceOfHermes.UI`に存在するクラスです。  
この記事はこれを前提に書かれています。

# BattleMapChanger

戦闘中のマップを変更します。

`MapManager`をComponentとして保持している`GameObject`、または背景・床面の`Sprite`を指定してマップを適用することが出来ます。  
`AudioClip`を指定してBGMを指定することも可能です。

```cs
public override void OnWaveStart()
{
    BattleMapChanger.SetMap(/* BackGround */, /* Floor */, /* BGM */, [ /* Ids */ ], /* Dialog color */);
}
```

## Specified usage

### `SetMap(MapManager, AudioClip?)`

```cs
BattleMapChanger.SetMap(map, bgm);
```

`MapManager`とそれに紐づくGameObjectからマップを設定します。

- `map`: AddComponent済みの`MapManager`です。
- `bgm`: BGMに使用される`AudioClip`です。指定しなかった場合はデフォルトのBGMが使われます。

### `SetMap(Sprite, Sprite, AudioClip?, List<string>?, Color?)`

```cs
BattleMapChanger.SetMap(bg, flr, bgm, dids, dcolr);
```

背景・底面の画像を指定してマップを設定します。  
第4引数はダイアログそのものではなく`TextDataModel`のidです。

- `bg`: 背景画像用のスプライトです。
- `flr`: 底面画像用のスプライトです。
- `bgm`: BGMに使用される`AudioClip`です。指定しなかった場合はデフォルトのBGMが使われます。
- `dids`: 表示されるダイアログが紐付いたIdのリストです。
- `dcolr`: ダイアログのメインカラーです。
