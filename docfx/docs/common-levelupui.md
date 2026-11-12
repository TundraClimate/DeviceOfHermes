# `using DeviceOfHermes.UI;`

名前空間`DeviceOfHermes.UI`に存在するクラスです。  
この記事はこれを前提に書かれています。

# CommonLevelUpUI

ページの選択UIとして不要なものを削ぎ落とした`CustomLevelUpUI`です。

```cs
CustomLevelUpUI.QueueLevelUp(new CommonLevelUpUI(/* UIのタイトル */) { icon = /* 階層位置に位置するSprite */ }, 0, /* ページリスト */);
```

## Specified usage

### `Ctor(string)`

```cs
new CommonLevelUpUI(title);
```

UI左上に表示されるタイトル文章を指定します。

- `title`: 指定対象です。

### `icon`

UI左上に表示されるアイコンの画像を指定します。

- `icon`: 対象の`Sprite`です。
