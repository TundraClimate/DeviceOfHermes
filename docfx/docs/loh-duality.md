# `using LimbufOfHermes;`

名前空間`LimbufOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Duality

リンバスの"双極性"と同じような挙動をするページバフです。

> 味方を攻撃対象にできる  
> 対象が味方の場合、攻撃が的中せず、最大の速度でページを使用する

```cs
// 双極性を付与
card.AddBuf(new BattleDiceCardBuf_Limbuf_Duality());

// ページ効果で自動追加
class DiceCardSelfAbility_Duality : AdvancedCardBase
{
    public override Type[] AutoSetCardBufs => [typeof(BattleDiceCardBuf_Limbuf_Duality)];
}
```
