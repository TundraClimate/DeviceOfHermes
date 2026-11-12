# `using DeviceOfHermes.UI;`

名前空間`DeviceOfHermes.UI`に存在するクラスです。  
この記事はこれを前提に書かれています。

# StoryLineMaker

接待アイコンやラインを配置します。

例:  
```cs
// 接待マップの移動上限を無効化
StoryLineMaker.EnableUnrestrictedMap();

// (0, 0)から(100, 100)を結ぶラインを配置
StoryLineMaker.RegisterLine(0f, 0f, 100f, 100f);

// (500, 500)に"FooStoryType"のアイコンを配置
StoryLineMaker.RegisterIcon("FooStoryType", 500f, 500f);

// (500, 500)に"FooStoryType"のアイコンを楽団形式で配置
StoryLineMaker.RegisterSpecialIcon("FooStoryType", 500f, 500f, /* 右バナーのSprite */, () => /* 接待開始のダイアログのテキスト */);
```

## Specified usage

### `EnableUnrestrictedMap()`

```cs
StoryLineMaker.EnableUnrestrictedMap();
```

接待アイコンの配置されているマップの上下左右の移動制限を無効化します。


### `RegisterLine(float, float, float, float)`

```cs
StoryLineMaker.RegisterLine(startX, startY, endX, endY);
```

開始地点と終端地点を指定してラインを配置します。

- `startX`: 開始地点のX座標です。
- `startY`: 開始地点のY座標です。
- `endX`: 終端地点のX座標です。
- `endY`: 終端地点のY座標です。

### `RegisterIcon(string, float, float)`

```cs
StoryLineMaker.RegisterIcon(storytype, x, y);
```

特定地点に通常のアイコンを配置します。

- `storytype`: \<StoryType\>ここに指定されているテキストです。\</StoryType\>
- `x`: 特定地点のX座標です。
- `y`: 特定地点のY座標です。

### `RegisterSpecialIcon(string, float, float, Sprite, Func<string>)`

```cs
StoryLineMaker.RegisterSpecialIcon(storytype, x, y, banner, text);
```

特定地点に楽団形式のアイコンを配置します。

- `storytype`: \<StoryType\>ここに指定されているテキストです。\</StoryType\>
- `x`: 特定地点のX座標です。
- `y`: 特定地点のY座標です。
- `banner`: 右側に表示されるバナー画像用のスプライトです。
- `text`: 接待を開始する直前の確認ダイアログに表示される文章です。
