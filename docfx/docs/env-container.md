# `using DeviceOfHermes.Resource;`

名前空間`DeviceOfHermes.Resource`に存在するクラスです。  
この記事はこれを前提に書かれています。

# EnvContainer

個人用セーブデータを管理します。

例:  
```cs
// Containerを登録
var container = EnvContainer.Register<FooMOD>();

// Crateを追加
var crate = container.AddCrate("Crate1");

// Crateにデータを追加
crate["Value1"] = "Text";
crate["Value2"] = 2;

// テキストに変換できないデータはバイトデータとして追加
crate.SetData("Image1", "Image1.png", new byte[0]);

// 読み込み
Hermes.Say($"Value1: {crate["Value1"]}");
Hermes.Say($"Value2: {crate["Value2"]}");
Hermes.Say($"Image1: {crate.GetData("Image1")}");

// セーブして保存
container.Save();
```

## Specified usage

## `EnvContainer`

### `Register<T>()`

```cs
EnvContainer.Register<T>();
```

`EnvContainer`を登録します。

- `T`: ModPackageを実装している必要があります。

### `AddCrate(string)`

```cs
container.AddCrate(name);
```

`EnvCrate`を作成します。

- `name`: Crateの名前です。

### `Save()`

```cs
container.Save();
```

データを保存します。

## `EnvCrate`

### `GetData(string)`

```cs
crate.GetData(key);
```

バイト列として保存しているデータを取得します。

- `key`: データに紐付いている名前です。

### `SetData(string, string, byte[])`

```cs
crate.SetData(key, file, data);
```

テキストに変換できないデータをバイト列として保存します。

- `key`: データに紐づく名前です。
- `file`: データの名前+保存形式です。(例: `Foo.png`)

### `Save()`

```cs
crate.Save();
```

データを保存します。
