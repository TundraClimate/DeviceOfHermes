# `using DeviceOfHermes.Resource;`

名前空間`DeviceOfHermes.Resource`に存在するクラスです。  
この記事はこれを前提に書かれています。

# ReadXmlParser

XMLファイルを読み込み、特定の型として変換します。

例:  
```cs
ReadXmlParser.Read<DataInfo>("DataFile.xml");
```

この例は`DataFile.xml`の内容を`DataInfo`として解釈して取得するコードです。

## Specified usage

### `Read<T>(string)`

```cs
var value = ReadXmlParser.Read<T>(path);
```

XMLファイルを読み込み、特定の型として変換します。変換に失敗した場合はnullを返します。

- `T`: 変換する対象の型です。
- `path`: 読み込むファイルの場所です。
