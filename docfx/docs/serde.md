# `using DeviceOfHermes.Resource;`

名前空間`DeviceOfHermes.Resource`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Serde

データ型のシリアライズ・デシリアライズを行います。  
XMLファイルを実データに変換したり、逆もまた可能です。

```cs
// data.xmlの中身をData型として読み込む
var data = Serde.FromXmlFile<Data>("data.xml");

// dataをXML形式に変換して出力
string text = Serde.ToXmlStr(data);

Hermes.Say(text);

// data.jsonの中身をData型として読み込む
var data = Serde.FromJsonFile<Data>("data.json");

// dataをJson形式に変換して出力
string text = Serde.ToJsonStr(data);

Hermes.Say(text);

// data.tomlの中身をData型として読み込む
var data = Serde.FromTomlFile<Data>("data.toml");

// dataをToml形式に変換して出力
string text = Serde.ToTomlStr(data);

Hermes.Say(text);
```

## Specified usage

### `FromXml<T>(XmlReader)`

```cs
Serde.FromXml<T>(reader);
```

任意の`XmlReader`を使用してデータを読み込みます。

- `T`: この型に変換します。
- `reader`: `XmlReader`です。

### `FromXmlStr<T>(string)`

```cs
Serde.FromXmlStr<T>(content);
```

文字列をXMLファイルの中身として解釈し、データに変換します。

- `T`: この型に変換します。
- `content`: XML形式の文字列です。

### `FromXmlFile<T>(string)`

```cs
Serde.FromXmlFile<T>(path);
```

指定のファイルからXMLデータを読み込みます。

- `T`: この型に変換します。
- `path`: XMLファイルのパスです。

### `ToXml<T>(T, XmlWriter)`

```cs
Serde.ToXml(value, writer);
```

データをXML形式に変換して書き込みます。

- `value`: 書き込む値です。
- `writer`: 書き込む対象です。

### `ToXmlStr<T>(T)`

```cs
Serde.ToXmlStr(value);
```

データをXML形式の文字列に変換します。

- `value`: 変換前の値です。

### `ToXmlFile<T>(T, string)`

```cs
Serde.ToXmlFile(value, path);
```

データをXMLファイルとして出力します。

- `value`: 出力する値です。
- `path`: 出力先のファイルパスです。

### `FromJson<T>(JsonReader)`

```cs
Serde.FromJson<T>(reader);
```

任意の`JsonReader`を使用してデータを読み込みます。

- `T`: この型に変換します。
- `reader`: `JsonReader`です。

### `FromJsonStr<T>(string)`

```cs
Serde.FromJsonStr<T>(content);
```

文字列をJsonファイルの中身として解釈し、データに変換します。

- `T`: この型に変換します。
- `content`: Json形式の文字列です。

### `FromJsonFile<T>(string)`

```cs
Serde.FromJsonFile<T>(path);
```

指定のファイルからJsonデータを読み込みます。

- `T`: この型に変換します。
- `path`: Jsonファイルのパスです。

### `ToJson<T>(T, JsonWriter)`

```cs
Serde.JsonWriter(value, writer);
```

データをJson形式に変換して書き込みます。

- `value`: 書き込む値です。
- `writer`: 書き込む対象です。

### `ToJsonStr<T>(T)`

```cs
Serde.ToJsonStr(value);
```

データをJson形式の文字列に変換します。

- `value`: 変換前の値です。

### `ToJsonFile<T>(T, string)`

```cs
Serde.ToJsonFile(value, path);
```

データをJsonファイルとして出力します。

- `value`: 出力する値です。
- `path`: 出力先のファイルパスです。

### `FromTomlStr<T>(string)`

```cs
Serde.FromTomlStr<T>(content);
```

文字列をTomlファイルの中身として解釈し、データに変換します。

- `T`: この型に変換します。
- `content`: Toml形式の文字列です。

### `FromTomlFile<T>(string)`

```cs
Serde.FromTomlFile<T>(path);
```

指定のファイルからTomlデータを読み込みます。

- `T`: この型に変換します。
- `path`: Tomlファイルのパスです。

### `ToTomlStr<T>(T)`

```cs
Serde.ToTomlStr(value);
```

データをToml形式の文字列に変換します。

- `value`: 変換前の値です。

### `ToTomlFile<T>(T, string)`

```cs
Serde.ToTomlFile(value, path);
```

データをTomlファイルとして出力します。

- `value`: 出力する値です。
- `path`: 出力先のファイルパスです。
