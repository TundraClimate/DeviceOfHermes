# `using DeviceOfHermes.CustomDice;`

名前空間`DeviceOfHermes.CustomDice`に存在するクラスです。  
この記事はこれを前提に書かれています。

# EqualDice

マッチ中の互いの最小値・最大値をそれぞれ平均化するダイスです。  
この効果は既存のダイス最低値・最大値を変更する効果適用後に適用されます。

例:
```cs
public class DiceCardAbility_Equal : EqualDice
{
    static string Desc = "公平ダイス";
}
```
