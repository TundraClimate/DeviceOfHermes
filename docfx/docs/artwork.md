# `using DeviceOfHermes.Resource;`

名前空間`DeviceOfHermes.Resource`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Artwork

ゲーム内のSpriteなど描画要素に関するクラスです。

例:  
```cs
var sprite = Artwork.CreateSprite("Image.png");
```

この例は、`"Image.png"`からSpriteを作成して取得するコードです。

## Specified usage

### `CreateSprite(string, float)`

```cs
var sprite = Artwork.CreateSprite(path, pixPerUnit);
```

Spriteを`path`にある画像ファイルから作成します。  

- `path`: 画像ファイルの場所を示すパスです。
- `pixPerUnit`: Unityが使用する1ユニットあたりのピクセル量です。

### `CreateSprite(byte[], float)`

```cs
var sprite = Artwork.CreateSprite(bytes, pixPerUnit);
```

Spriteを`bytes`から作成します。

- `bytes`: 画像ファイルなどから読み出したデータです。
- `pixPerUnit`: Unityが使用する1ユニットあたりのピクセル量です。

### `SetAbnoCardArtwork(string, Sprite)`

```cs
Artwork.SetAbnoCardArtwork(name, sprite);
```

`Sprite`を名前付きで幻想体ページ画像として追加・変更します。

- `name`: 紐付ける名前です。
- `sprite`: 追加する`Sprite`です。

### `SetAbnoCardArtwork(string, string)`

```cs
Artwork.SetAbnoCardArtwork(path, name);
```

指定したパスの画像を幻想体ページ画像読み込んで追加・変更します。  
拡張子を覗いたファイル名がそのまま名前になります。

- `path`: 指定のファイルパスです。
- `name`: 名前を別で指定する場合に指定する名前です。

### `LoadAbnoCardArtworks(string)`

```cs
Artwork.LoadAbnoCardArtworks(rootDirPath);
```

指定したフォルダに含まれる全てのファイルを幻想体ページ画像として読み込みます。

- `rootDirPath`: 指定のフォルダパスです。

### `SetBattleUnitBufSprite(string, Sprite, bool)`

```cs
Artwork.SetBattleUnitBufSprite(id, sprite, replace);
```

バフ効果に使用されるSpriteを追加・変更します。

- `id`: 追加及び変更するために使用する`BattleUnitBuf`のID。
- `sprite`: 追加及び変更するために使用するSprite。
- `replace`: 同じIDのSpriteが存在する場合、無視して置き換えるか。デフォルトはfalse。

### `SetBattleUnitBufSprite(string, bool)`

```cs
Artwork.SetBattleUnitBufSprite(imgPath, replace);
```

バフ効果に使用されるSpriteを追加・変更します。

- `imgPath`: Spriteに使用するファイルの場所。ファイル名がIDになります。
- `replace`: 同じIDのSpriteが存在する場合、無視して置き換えるか。デフォルトはfalse。

### `SetBattleUnitBufSprite(string, string, bool)`

```cs
Artwork.SetBattleUnitBufSprite(id, imgPath, replace);
```

バフ効果に使用されるSpriteを追加・変更します。

- `id`: 追加及び変更するために使用する`BattleUnitBuf`のID。
- `imgPath`: Spriteに使用するファイルの場所。
- `replace`: 同じIDのSpriteが存在する場合、無視して置き換えるか。デフォルトはfalse。

### `LoadBattleUnitBufSprites(string, bool)`

```cs
Artwork.LoadBattleUnitBufSprites(rootDirPath, replace);
```

フォルダ内の全ての画像ファイルをバフ効果のSpriteとして追加します。

- `rootDirPath`: 対象のフォルダです。
- `replace`: 同じIDのSpriteが存在する場合、無視して置き換えるか。デフォルトはfalse。

### `SetStoryIconSprite(UIIconManager.IconSet, bool)`

```cs
Artwork.SetStoryIconSprite(icon, replace);
```

ストーリーのアイコンに使われるSpriteを追加・変更します。

- `icon`: 対象の`IconSet`です。
- `replace`: 同じIDのSpriteが存在する場合、無視して置き換えるか。デフォルトはfalse。

### `SetStoryIconSprite(string, Sprite, Sprite, bool)`

```cs
Artwork.SetStoryIconSprite(type, icon, iconGlow, replace);
```

ストーリーのアイコンに使われるSpriteを追加・変更します。

- `type`: アイコン固有のIDです。
- `icon`: 対象のSpriteです。
- `iconGlow`: 上部に表示されるSpriteです。
- `replace`: 同じIDのSpriteが存在する場合、無視して置き換えるか。デフォルトはfalse。

### `SetStoryIconSprite(string, string, string, bool)`

```cs
Artwork.SetStoryIconSprite(icon, iconGlow, type, replace);
```

ストーリーのアイコンに使われるSpriteを追加・変更します。

- `icon`: 対象の画像のパスです。
- `iconGlow`: 上部に表示される画像のパスです。
- `type`: アイコン固有のIDを上書きするIDです。
- `replace`: 同じIDのSpriteが存在する場合、無視して置き換えるか。デフォルトはfalse。

### `LoadStoryIconSprites(string, string, bool)`

```cs
Artwork.LoadStoryIconSprites(rootDirPath, glowSuffix, replace);
```

フォルダ内の全ての画像ファイルをストーリーアイコンのSpriteとして追加します。

- `rootDirPath`: 対象のフォルダです。
- `glowSuffix`: Glowに使用するファイルの接尾詞です。
- `replace`: 同じIDのSpriteが存在する場合、無視して置き換えるか。デフォルトはfalse。
