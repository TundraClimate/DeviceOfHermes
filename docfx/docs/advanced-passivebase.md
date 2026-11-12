# `using DeviceOfHermes.AdvancedBase;`

名前空間`DeviceOfHermes.AdvancedBase`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdvancedPassiveBase

効果拡張版`PassiveAbilityBase`です。  

例:  
```cs
class PassiveAbility_TestAdvPassive : AdvancedPassiveBase
{
    public override int? HealthStopperLine => base.HealthStopperLine;

    public override int DrawCardAddr => 0;

    public override bool IsDieInsteadOfExtinction => base.IsDieInsteadOfExtinction;

    public override bool IsSaveWhileReception => true;

    public override int GetSpeedDiceMinAdder()
    {
        return 0;
    }

    public override int GetSpeedDiceMaxAdder()
    {
        return 0;
    }

    public override void OnWaveStartBefore()
    {
    }

    public override void OnWaveStartAfter()
    {
    }

    public override void OnRoundStartFirst()
    {
    }

    public override void OnRoundStartLast()
    {
    }

    public override bool IsClashable(BattlePlayingCardDataInUnitModel card)
    {
        return true;
    }

    public override bool IsClashable(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
    {
        return true;
    }

    public override bool IsIgnoreSpeedByMatch(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
    {
        return false;
    }

    public override void OnPreRoundEnd()
    {
    }

    public override bool IsAllowRoundEnd()
    {
        return true;
    }

    public override bool CanDiscardByAbility(BattleDiceCardModel card)
    {
        return true;
    }

    public override void OnActivatedBuf(BattleUnitBuf activate)
    {
    }

    public override void OnChangeBufStack(BattleUnitBuf changed, int last)
    {
    }

    public override void OnDropCard(BattlePlayingCardDataInUnitModel playcard)
    {
    }

    public override void OnClickUnit(ClickType ty)
    {
    }

    public override void OnAddBuf(BattleUnitBuf buf, int addedStack)
    {
    }

    public override bool OnChooseCard(BattleDiceCardModel card)
    {
        return true;
    }

    public override BattlePlayingCardDataInUnitModel? BeforeTakeOneSideAction(BattleDiceCardModel attacker, BattlePlayingCardDataInUnitModel attackCard)
    {
        return null;
    }

    public override void BeforeAlliesTakeOneSideAction(BattleUnitModel victim, BattleUnitModel attacker, BattlePlayingCardDataInUnitModel attckCard, ref BattlePlayingCardDataInUnitModel altCard)
    {
    }

    public override void OnExtinct()
    {
    }

    public override void OnRecoverPP(int gain)
    {
    }

    public override int OnAddKeywordBuf(BattleUnitBuf buf, int stack)
    {
        return 0;
    }

    public override void OnLoseHpByKeyword(int losed, KeywordBuf keyword)
    {
    }

    public override bool OnAddKeepDice(BattleDiceBehavior behavior)
    {
        return true;
    }

    public override void OnEndEncounter(BattleUnitModel? target, bool isParrying, bool wasAttacker)
    {
    }

    public override int GetHealingAdder(int origin)
    {
        return 0;
    }

    public override bool IsNullifyHealing(int healNum)
    {
        return false;
    }

    public override bool IsTakeAmmoEffectNotFar(BattlePlayingCardDataInUnitModel card)
    {
        return false;
    }

    public override void OnUseInstantCard(BattleDiceCardModel card, BattleUnitModel target, int targetSlot)
    {
    }

    public override void OnOtherUseInstantCard(BattleUnitModel unit, BattleDiceCardModel card, BattleUnitModel target, int targetSlot)
    {
    }

    public override void OnFlipEmotionCoin(EmotionCoinType coinType, int num)
    {
    }
}
```

このクラスは`PassiveAbilityBase`を継承しています。  
それの代わりに`AdvancedPassiveBase`を継承し、幾つかの追加要素のあるそれとして扱うことを想定しています。

## Specified usage

### `HealthStopperLine`

```cs
public override int? HealthStopperLine => value;
```

HPの減少のストッパーを設定します。

- `value`: 保たれるHPの値です。

### `DrawCardAddr`

```cs
public override int DrawCardAddr => num;
```

幕開始時のドローに加算する枚数を指定します。

- `num`: 加算する枚数です。

### `IsDieInsteadOfExtinction`

```cs
public override bool IsDieInsteadOfExtinction => value;
```

`Die`を`Extinct(true)`に置き換えます。

- `value`: 置き換えるかどうかです。

### `IsSaveWhileReception`

```cs
public override bool IsSaveWhileReception => value;
```

舞台を跨いでインスタンスを維持します。  
同じIDのインスタンスを複数維持することは出来ません。

- `value`: 維持するかどうかです。

### `GetSpeedDiceMinAdder()`

```cs
public override int GetSpeedDiceMinAdder()
{
    return 0;
}
```

速度の最低値に加算する値を返します。

### `GetSpeedDiceMaxAdder()`

```cs
public override int GetSpeedDiceMaxAdder()
{
    return 0;
}
```

速度の最大値に加算する値を返します。

### `OnWaveStartBefore()`

```cs
public override void OnWaveStartBefore()
{
}
```

`PassiveAbilityBase`の`OnWaveStart`より先に実行されます。

### `OnWaveStartAfter()`

```cs
public override void OnWaveStartAfter()
{
}
```

`PassiveAbilityBase`の`OnWaveStart`より後に実行されます。

### `OnRoundStartFirst()`

```cs
public override void OnRoundStartFirst()
{
}
```

`PassiveAbilityBase`の`OnRoundStart`より先に実行されます。

### `OnRoundStartLast()`

```cs
public override void OnRoundStartLast()
{
}
```

`PassiveAbilityBase`の`OnRoundStartAfter`より後に実行されます。

### `IsClashable(BattlePlayingCardDataInUnitModel)`

```cs
public override bool IsClashable(BattlePlayingCardDataInUnitModel card)
{
    return true;
}
```

ページがマッチ可能かを変更できます。  
falseを返すと`card`はマッチ不可になります。

- `card`: マッチするページです。

### `IsClashable(BattlePlayingCardDataInUnitModel, BattlePlayingCardDataInUnitModel)`

```cs
public override bool IsClashable(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
{
    return true;
}
```

ページがマッチ可能かを変更できます。  
falseを返すと`card`はマッチ不可になります。

- `card`: マッチするページです。
- `target`: マッチする相手のページです。

### `IsIgnoreSpeedByMatch(BattlePlayingCardDataInUnitModel, BattlePlayingCardDataInUnitModel)`

```cs
public override bool IsIgnoreSpeedByMatch(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
{
    return false;
}
```

ページが速度を無視してマッチできるかを変更できます。  
trueを返すと`card`は速度を無視してのマッチが可能になります。

- `card`: マッチするページです。
- `target`: マッチする相手のページです。

### `OnPreRoundEnd()`

```cs
public override void OnPreRoundEnd()
{
}
```

戦闘終了前に実行されます。  
`StageController._allCardList`が空でない場合、戦闘終了をキャンセルします。

### `IsAllowRoundEnd()`

```cs
public override bool IsAllowRoundEnd()
{
    return true;
}
```

戦闘を終了できるかどうかを指定します。  
falseを返すと戦闘終了をキャンセルし、trueが返されるまで呼び出し続けます。

### `CanDiscardByAbility(BattleDiceCardModel)`

```cs
public override bool CanDiscardByAbility(BattleDiceCardModel card)
{
    return true;
}
```

ページが捨てられる前に実行されます。  
falseを返すとそのページが捨てられるのをキャンセルすることが出来ます。

- `card`: この効果を持っているページです。

### `OnActivatedBuf(BattleUnitBuf)`

```cs
public override void OnActivatedBuf(BattleUnitBuf activate)
{
}
```

バフ効果が新たに有効化したときに実行されます。  

- `activate`: 新たに有効化されたバフです。

### `OnChangeBufStack(BattleUnitBuf, int)`

```cs
public override void OnChangeBufStack(BattleUnitBuf changed, int last)
{
}
```

バフ効果のスタック数が変更されたときに実行されます。  
付与されていない状態から新たに付与されたときは`last`が`-1`になります。

- `changed`: 変更されたバフです。
- `last`: 最後に記録されたバフのスタック数です。

### `OnDropCard(BattlePlayingCardDataInUnitModel)`

```cs
public override void OnDropCard(BattlePlayingCardDataInUnitModel playcard)
{
}
```

自らのページが使用完了しなかった場合に実行されます。

- `playcard`: 使用完了できなかったページです。

### `OnClickUnit(ClickType)`

```cs
public override void OnClickUnit(ClickType ty)
{
}
```

パッシブを保有するユニットをクリックしたときに実行されます。

- `ty`: クリックの種類です。(Left/Right/Middle)

### `OnAddBuf(BattleUnitBuf, int)`

```cs
public override void OnAddBuf(BattleUnitBuf buf, int addedStack)
{
}
```

なんらかのバフを獲得、もしくはスタック数の増加時に実行されます。

- `buf`: 対象のバフです。
- `addedStack`: 増加したスタック量です。

### `OnChooseCard(BattleDiceCardModel)`

```cs
public override bool OnChooseCard(BattleDiceCardModel card)
{
    return true;
}
```

使用可能なページを制限します。  
falseを返すと`card`は使用不可になります。

- `card`: 対象のページです。

### `BeforeTakeOneSideAction(BattleUnitModel, BattlePlayingCardDataInUnitModel)`

```cs
public override BattlePlayingCardDataInUnitModel? BeforeTakeOneSideAction(BattleUnitModel attacker, BattlePlayingCardDataInUnitModel attackCard)
{
    return null;
}
```

一方攻撃を受ける前に実行されます。  
null以外を返した場合、返したページで代わりにマッチをします。

- `attacker`: 一方攻撃をしている相手です。
- `attackCard`: 一方攻撃をしているページです。

### `BeforeAlliesTakeOneSideAction(BattleUnitModel, BattleUnitModel, BattlePlayingCardDataInUnitModel, BattlePlayingCardDataInUnitModel)`

```cs
public override void BeforeAlliesTakeOneSideAction(BattleUnitModel victim, BattleUnitModel attacker, BattlePlayingCardDataInUnitModel attckCard, ref BattlePlayingCardDataInUnitModel altCard)
{
}
```

同陣営の自分以外の味方が一方攻撃を受ける前に実行されます。  
`altCard`に代入した値で代わりにマッチします。

- `victim`: 一方攻撃を受ける味方です。
- `attacker`: 攻撃をしている相手です。
- `attckCard`: 攻撃に使用しているページです。
- `altCard`: 代わりにマッチするページです。null以外が代入されている場合もあります。

### `OnExtinct()`

```cs
public override void OnExtinct()
{
}
```

`Extinct(true)`が適用されたときに実行されます。

### `OnRecoverPP(int)`

```cs
public override void OnRecoverPP(int gain)
{
}
```
光を回復したときに実行されます。

- `gain`: 回復しようとした値です。

### `OnAddKeywordBuf(BattleUnitBuf, int)`

```cs
public override int OnAddKeywordBuf(BattleUnitBuf buf, int stack)
{
    return 0;
}
```

`KeywordBuf`を付与されたときに実行されます。

- `buf`: 付与されたバフです。
- `stack`: 付与されたスタック数です。

### `OnLoseHpByKeyword(int, KeywordBuf)`

```cs
public override void OnLoseHpByKeyword(int losed, KeywordBuf keyword)
{
}
```

`KeywordBuf`によって体力が減少したときに実行されます。  

- `losed`: 失った体力数値です。
- `keyword`: 受けた`KeywordBuf`です。

### `OnAddKeepDice(BattleDiceBehavior)`

```cs
public override bool OnAddKeepDice(BattleDiceBehavior behavior)
{
    return true;
}
```

ダイスが反撃として追加される直前に実行されます。  
`false`を返すとそのダイス追加がキャンセルされます。

- `behavior`: 対象のダイスです。

### `OnEndEncounter(BattleUnitModel?, bool, bool)`

```cs
public override void OnEndEncounter(BattleUnitModel? target, bool isParrying, bool wasAttacker)
{
}
```

接敵終了時に実行されます。  
currentDiceActionはnullです。

- `target`: 接敵した相手です。
- `isParrying`: その接敵がマッチであったかどうかです。
- `wasAttacker`: ページを使用していたかどうかです。

### `GetHealingAdder(int)`

```cs
public override int GetHealingAdder(int origin)
{
    return 0;
}
```

体力回復量に加算する数値を返します。

- `origin`: 加算前の回復数値です。

### `IsNullifyHealing(int)`

```cs
public override bool IsNullifyHealing(int healNum)
{
    return false;
}
```

体力回復数値を0にするかどうかです。

- `healNum`: 回復数値です。

### `IsTakeAmmoEffectNotFar(BattlePlayingCardDataInUnitModel)`

```cs
public override bool IsTakeAmmoEffectNotFar(BattlePlayingCardDataInUnitModel card)
{
    return false;
}
```

ページが弾丸の効果を得るかどうかです。  

- `card`: 対象のページです。

### `OnUseInstantCard(BattleDiceCardModel, BattleUnitModel, int)`

```cs
public override void OnUseInstantCard(BattleDiceCardModel card, BattleUnitModel target, int targetSlot)
{
}
```

自身が装着ページを装着したときに実行されます。

- `card`: 装着したページです。
- `target`: 装着時のターゲットです。
- `targetSlot`: 装着時のターゲットのスロットです。

### `OnOtherUseInstantCard(BattleUnitModel, BattleDiceCardModel, BattleUnitModel, int)`

```cs
public override void OnOtherUseInstantCard(BattleUnitModel unit, BattleDiceCardModel card, BattleUnitModel target, int targetSlot)
{
}
```

味方が装着ページを装着したときに実行されます。

- `unit`: 装着したユニットです。
- `card`: 装着したページです。
- `target`: 装着時のターゲットです。
- `targetSlot`: 装着時のターゲットのスロットです。

### `OnFlipEmotionCoin(EmotionCoinType, int)`

```cs
public override void OnFlipEmotionCoin(EmotionCoinType coinType, int num)
{
}
```

感情コインの獲得条件を満たしたときに実行されます。

- `coinType`: 対象のコインの種類です。
- `num`: 獲得した枚数です。
