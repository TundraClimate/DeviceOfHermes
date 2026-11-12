# `using DeviceOfHermes.CustomDice;`

名前空間`DeviceOfHermes.CustomDice`に存在するクラスです。  
この記事はこれを前提に書かれています。

# SecondlyDice

マッチ敗北時とこのダイスが余った時のみ振られるダイスです。  
反撃ダイスのみ適用でき、戦闘開始時にそのページにストックされます。  
ストックされたダイスはそのページのダイスがマッチに負けるときかマッチ後の一方攻撃のときに振られ、このとき出目で勝ったのならマッチ敗北時のダメージを打ち消しマッチ結果を勝利に変更します。  
このダイスが回避の場合、マッチに勝利した場合は再利用することが出来ます。

```cs
public class DiceCardAbility_Secondly : SecondlyDice
{
}
```

このクラスは`DiceCardAbilityBase`を継承しています。  
それの代わりに`SecondlyDice`を継承し、幾つかの追加要素のあるそれとして扱うことを想定しています。

## Specified usage

### `AddSecondlyDice(BattlePlayingCardDataInUnitModel, params List<BattleDiceBehavior>)`

```cs
SecondlyDice.AddSecondlyDice(card, dices);
```

ページに対してこのダイスをストックします。

- `card`: ストックする対象のページです。
- `dices`: 追加するダイスです。
