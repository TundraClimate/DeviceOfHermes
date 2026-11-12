# `using DeviceOfHermes.AdvancedBase;`

名前空間`DeviceOfHermes.AdvancedBase`に存在するクラスです。  
この記事はこれを前提に書かれています。

# ShimPassiveBase

ゆらぎ用に改良された`PassiveAbilityBase`です。

```cs
public class PassiveAbility_MyShimmering : ShimPassiveBase
{
    // MODのIDをセットする
    public override void Init() => PackageId = "MyPackageId";

    // パターンを適用する
    public override void ApplyPattern() => Patterns.ApplyNextPattern(this);

    // パターンを設定する
    private PatternList Patterns = new(true)
    {
        // 1幕目: idが1, 2, 3のページをセット
        // 速度ダイスが余ればidが4のページで埋める
        new(1, 2, 3) { filler = Filler.All(new LorId("MyPackageId", 4)) },

        // 2幕目: idが4, 5, 6のページをセット
        // 速度ダイスが余ればid1とid2のページを順番にセットする
        new(4, 5, 6) { filler = Filler.Const(new LorId("MyPackageId", 1), new LorId("MyPackageId", 2)) },
    };

    // 速度ダイス数を調整
    public override int SpeedDiceNumAdder()
    {
        // 速度ダイスを合計5個にする
        return base.owner.emotionDetail.EmotionLevel >= 4 ? 3 : 4;
    }
}
```

## Specified usage

### `Init()`

```cs
public override void Init() => PackageId = id;
```

ページの追加に使用するIDを指定します。

- `id`: パッケージIDです。

### `ApplyPattern()`

```cs
public override void ApplyPattern()
{
}
```

行動パターンを設定します。  
毎幕ページは初期化され、光が全回復します。

`AddCard`等を使用してページを追加し、パターンとして設定することが出来ます。

### `Elapsed`

```cs
int pat = Elapsed % 3;
```

経過幕数が記録されます。

### `Shimmering()`

```cs
Shimmering()
```

光を全回復し、全てのページを手札・デッキから消去します。

### `AddCard(int, bool = false)`

```cs
AddCard(id, vannila: false);
```

ページを手元に追加します。  
`PackageId`が自動で設定されますが、`vannila`がtrueであるならバニラページを追加します。

- `id`: ページのIDです。
- `vannila`: バニラページのIDを使用するかどうかです。基本値はfalse。

### `AddCard(LorId)`

```cs
AddCard(lorid);
```

ページを手元に追加します。  
`LorId`をそのまま使用します。

- `lorid`: ページのIDです。

### `OnStartTurn()`

```cs
OnStartTurn();
```

幕の開始時に実行されている処理のセットです。
