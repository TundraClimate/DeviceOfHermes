# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# DebugExtension

デバッグ用の機能を提供します。

```cs
try
{
    var _ = owner.bufListDetail.GetActivatedBuf(KeywordBuf.Blurry).stack;
}
catch (Exception e)
{
    // { Method: {Method}, ILOffset: 0x12 }
    Hermes.Say($"{e.GetInfo()}", MessageLevel.Warn);

    throw;
}
```

## Specified usage

### `Exception.GetInfo()`

Exceptionの発生位置を取得します。  
この機能で取得したILオフセットは最適化等によりズレが生じる場合があります。
