# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# KeywordBufExtender

`KeywordBuf`の拡張システムです。

```cs
// 付けるだけでいい
[KeywordBufExtend]
class MyKeywordBuf
{
    // KeywordBufと実際に付与する対象の紐付け
    [KeywordBuf(typeof(BattleUnitBuf_XXX))]
    public static KeywordBuf XXX { get; private set; }
}

class BattleUnitBuf_XXX : BattleUnitBuf
{
    // バフ側で使用する
    public override KeywordBuf bufType => MyKeywordBuf.XXX;
}

// バフを付与
owner.bufListDetail.AddKeywordBufByEtc(MyKeywordBuf.XXX, 999, owner);
```

## Specified usage

### `ExtendProps(Type)`

型に存在する`KeywordBuf`と`KeywordBufAttribute`を指定したプロパティを拡張に使用します。
