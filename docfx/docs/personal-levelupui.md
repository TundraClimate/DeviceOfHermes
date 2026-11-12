# `using DeviceOfHermes.UI;`

名前空間`DeviceOfHermes.UI`に存在するクラスです。  
この記事はこれを前提に書かれています。

# PersonalLevelUpUI

ユニットに紐付けてUIを起動するために実装された`CustomLevelUpUI`です。

```cs
CustomLevelUpUI.QueueLevelUp(new PersonalLevelUpUI(owner) { name = /* UIのタイトル */, icon = /* 階層位置に位置するSprite */ }, /* ページ選択段階 */, /* ページリスト */);
```

## Specified usage

### `Ctor(BattleUnitModel)`

```cs
new PersonalLevelUpUI(owner);
```

紐付けるユニットを設定します。

- `owner`: 紐付ける対象ユニットです。

### `name`

UI左上に表示されるタイトル文章を指定します。

- `title`: 指定対象です。

### `icon`

UI左上に表示されるアイコンの画像を指定します。

- `icon`: 対象の`Sprite`です。
