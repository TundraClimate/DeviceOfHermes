# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Shin

リンバスの"シン(心)"っぽいエフェクトを使えます。

```cs
// オリジナルのシン(心)
class BattleUnitBuf_MyShin : BattleUnitBuf_Limbuf_Shin
{
    // シン(心)の効果を実装する
}
```

`BattleUnitBuf_Limbuf_Shin`を実装するだけでエフェクトが付与されるようになります。

## Specified usage

### `CreateShinAura(BattleUnitModel, BattleUnitBuf)`

オーラだけ作成します。

- `owner`: 付与対象です。
- `shin`: 紐付けるバフ効果です。
