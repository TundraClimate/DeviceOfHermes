# `using DeviceOfHermes.UI;`

名前空間`DeviceOfHermes.UI`に存在するクラスです。  
この記事はこれを前提に書かれています。

# BattleUIBehaviour

拡張型`MonoBehaviour`です。

```cs
class ExampleUI : BattleUIBehaviour
{
    void Awake()
    {
        InitUI();
    }

    void Start()
    {
        gameObject.AddContainer(head =>
        {
            head.Also(s => s.name = "Panel")
                .MoveTo(new Vector2(0.5f, 0.5f))
                .SetImage(Artwork.CreateSprite(Path.Combine(typeof(ExampleUI).GetAsmDirectory(), "Artwork", "Panel.png")));
        });
    }

    public override OnInitalize()
    {
    }

    public override OnRoundStart()
    {
    }

    public override OnStartBattle()
    {
    }

    public override OnEndBattle()
    {
    }
}
```

```cs
BattleManagerUI.Instance.AddBehaviour<ExampleUI>("exampleUI");
```

この例は画面の中心に`Panel.png`の画像を表示します。

## Specified usage

### `OnInitalize()`

```
public override OnInitalize()
{
}
```

接待の初期設定時に実行されます。

### `OnRoundStart()`

```cs
public override OnRoundStart()
{
}
```

幕の開始時に実行されます。

### `OnStartBattle()`

```cs
public override OnStartBattle()
{
}
```

戦闘開始時に実行されます。

### `OnEndBattle()`

```cs
public override OnEndBattle()
{
}
```

接待終了時に実行されます。

### `InitUI(int)`

```cs
InitUI(order);
```

UI用にGameObjectを初期化し、Canvasを返します。

- `order`: CanvasのsortingOrderです。値が大きいほど手前に表示されます。

### `AddBehaviour<T>(string)`

```cs
BattleManagerUI.Instance.AddBehaviour<T>(name);
```

`BattleManagerUI`にMonoBehaviourを追加します。

- `name`: 同時に作成されるGameObjectの名称です。

### `GetBehaviour<T>(string)`

```cs
BattleManagerUI.Instance.GetBehaviour<T>(name);
```

`AddBehaviour`によって追加されたBehaviourを取得します。  
OnInitializeMod時点では初期化されておらず、nullを返す点を留意してください。

- `name`: 同時に作成されるGameObjectの名称です。
