# `using DeviceOfHermes.UI;`

名前空間`DeviceOfHermes.UI`に存在するクラスです。  
この記事はこれを前提に書かれています。

# CustomLevelUpUI

幻想体ページ選択UIをカスタムする機能です。

```cs
// 次の幕の終了時にUIを表示する
CustomLevelUpUI.QueueLevelUp(new CustomLevelUpUI(), 0, () => [ /* 表示されるページリスト */ ]);

// 即座にUIを表示する。ただし既に別の選択UIが表示中なら失敗する
CustomLevelUpUI.OpenLevelUpUI(new CustomLevelUpUI(), 0, () => [ /* 表示されるページリスト */ ]);
```

このクラスを実装したクラスも同じくCustomLevelUpUIとして使用することが可能。  
`Init`、`Drop`、そして`OnSelect`を実装することでさらに追加でカスタマイズすることが出来る。

```cs
class FooLevelUpUI : CustomLevelUpUI
{
    public override void Init(LevelUpUI ui)
    {
        // UIをカスタマイズする初期化処理を書く
    }

    public override void Drop(LevelUpUI ui)
    {
        // 変更したUIを元の状態に復帰させる処理を書く
    }

    public override bool OnSelect(EmotionCardXmlInfo info)
    {
        // ページを選択した時に実行される

        // falseを返した場合、ページ選択後のユニット選択フェーズをスキップする
        return true;
    }

    public override void OnCharacterClick(EmotionCardXmlInfo card, BattleUnitModel target)
    {
        // ページ選択後、キャラクターを選択したときに実行される
        // OnSelectでfalseを返したなら一緒にスキップされる
    }
}
```

## Specified usage

### `QueueLevelUp(CustomLevelUpUI, int, Func<List<EmotionCardXmlInfo>>)`

```cs
CustomLevelUpUI.QueueLevelUp(instance, level, candGen);
```

次の幕の終了時にページ選択UIの起動を予約します。

- `instance`: 対象のUIインスタンスです。
- `level`: UI上部に表示される現在の感情レベルUIの有効化数です。
- `candGen`: 表示されるページリストを返す関数です。

### `OpenLevelUpUI(CustomLevelUpUI, int, Func<List<EmotionCardXmlInfo>>)`

```cs
CustomLevelUpUI.OpenLevelUpUI(instance, level, candGen);
```

即座にUIを起動します。  
既に同種のUIが起動されている場合は失敗します。

- `instance`: 対象のUIインスタンスです。
- `level`: UI上部に表示される現在の感情レベルUIの有効化数です。
- `candGen`: 表示されるページリストを返す関数です。

### `Init(LevelUpUI)`

```cs
public override void Init(LevelUpUI ui)
{
}
```

UIを初期化する関数です。  
既存のUIに変更を加えます。

- `ui`: 変更を加える対象のUIです。

### `Drop(LevelUpUI)`

```cs
public override void Drop(LevelUpUI ui)
{
}
```

UIを元の状態に初期化する関数です。  
元のUIに影響を与えます。

- `ui`: 変更を加える対象のUIです。

### `OnSelect(EmotionCardXmlInfo info)`

```cs
public override bool OnSelect(EmotionCardXmlInfo info)
{
    return true;
}
```

ページを選択したときに実行される関数です。  
trueを返した場合は既存の処理を使いまわしますが、falseを返した場合はその処理をスキップするため、専用処理を実行することが出来ます。

- `info`: 選択されたページです。

### `OnCharacterClick(EmotionCardXmlInfo, BattleUnitModel)`

```cs
public override void OnCharacterClick(EmotionCardXmlInfo card, BattleUnitModel target)
{
}
```

ページ選択後にキャラクターをクリックしたときに実行されます。  
`OnSelect`にて`false`を返した場合は一緒にスキップされます。

- `card`: 選択したページです。
- `target`: 選択したキャラクターです。
