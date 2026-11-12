# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# StageLibrarianList

接待に参加するキャラクターを設定します。  

```cs
// あなたのMODのID=1の接待で左端のキャラクターをあなたのMODのID=1のコアページに変更します
StageLibrarianList.SetUnit(new LorId("あなたのMODのID", 1), 0, new LorId("あなたのMODのID", 1), "キャラクターの名前");

// あなたのMODのID=1の接待で左端にあなたのMODのID=1のコアページのキャラクターを追加します
StageLibrarianList.AddUnit(new LorId("あなたのMODのID", 1), new LorId("あなたのMODのID", 1), "キャラクターの名前");
```

## Specified usage

### `SetUnit(LorId, int, LorId, string)`

```cs
StageLibrarianList.SetUnit(stageId, idx, bookId, name);
```

`stageId`の接待で`idx`の位置のキャラクターを変更します。

- `stageId`: 対応する接待のIDです。
- `idx`: キャラクターの位置です。
- `bookId`: 使用するコアページのIDです。
- `name`: 指定される名前です。

### `AddUnit(stageId, LorId, string)`

```cs
StageLibrarianList.AddUnit(stageId, bookId, name);
```

`stageId`の接待で左端からキャラクターを追加します。  
溢れたキャラクターは表示されません。

- `stageId`: 対応する接待のIDです。
- `bookId`: 使用するコアページのIDです。
- `name`: 指定される名前です。
