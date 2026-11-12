# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdditionalOnlyCard

特定のコアページに専用ページを追加します。

例:  
```cs
new AdditonalOnlyCard(new LorId(260004)).AddCards(new LorId(705011));
```

この例はオリヴィエのコアページ(ID: 260004)にクレッシェンド(ID: 705011)を追加するコードです。

## Specified usage

### `Constructor(LorId)`

```cs
var onlyCard = new AdditonalOnlyCard(new LorId("あなたのMODID", 1));
```

あなたのMODのID:1のコアページを対象に準備します。まだ追加はされません。

### `AddCards(params LorId[])`

```cs
onlyCard.AddCards(new LorId("あなたのMODID", 1));
```

あなたのMODのID:1のバトルページを対象に専用ページとして追加します。
