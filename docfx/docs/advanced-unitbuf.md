# `using DeviceOfHermes.AdvancedBase;`

名前空間`DeviceOfHermes.AdvancedBase`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdvancedUnitBuf

効果拡張版`BattleUnitBuf`です。

```cs
class BattleUnitBuf_AdvBuf : AdvancedUnitBuf
{
    public override int DefaultStack { get => 0; }

    public override bool IsInstant { get => false; }

    public override bool IsSaveWhileReception => true;

    public override int GetSpeedDiceMinAdder()
    {
        return 0;
    }

    public override int GetSpeedDiceMaxAdder()
    {
        return 0;
    }

    public override void OnInstant()
    {
    }

    public override void OnOtherInstant(AdvancedUnitBuf instant)
    {
    }

    public override void OnRoundStartFirst()
    {
    }

    public override void OnRoundStartLast()
    {
    }

    public override void OnStartBattle()
    {
    }

    public override int OnAddKeywordBufByCard(BattleUnitBuf buf, int stack)
    {
        return 0;
    }

    public override int OnAddKeywordBuf(BattleUnitBuf buf, int stack)
    {
        return 0;
    }

    public override int GetBreakDamageReductionAll(int dmg, DamageType dmgType, BattleUnitModel attacker)
    {
        return 0;
    }

    public override void OnStackChange(int last)
    {
    }

    public override void OnStackChangeAll(BattleUnitBuf buf, int last)
    {
    }

    public override void OnAddBufAll(BattleUnitBuf buf, int addedStack)
    {
    }

    public override void OnClick(ClickType ty)
    {
    }

    public override BattlePlayingCardDataInUnitModel? BeforeTakeOneSideAction(BattleUnitModel attacker)
    {
        return null;
    }

    public override void BeforeAlliesTakeOneSideAction(BattleUnitModel victim, BattleUnitModel attacker, BattlePlayingCardDataInUnitModel attckCard, ref BattlePlayingCardDataInUnitModel altCard)
    {
    }

    public override void OnRecoverHp(int recoverAmount)
    {
    }

    public override void OnRecoverPP(int gain)
    {
    }

    public override void OnSuccessAreaAttack(BattleDiceBehavior behavior, BattleUnitModel target)
    {
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

    public override void BeforeRollDiceFirst(BattleDiceBehavior behavior)
    {
    }

    public override void BeforeRollDiceLast(BattleDiceBehavior behavior)
    {
    }

    public override int GetDamageReductionAll(int dmg, DamageType dmgType, BattleUnitModel attacker, KeywordBuf buf)
    {
        return 0;
    }

    public BattleUnitModel LastInflictBy;
}
```

このクラスは`BattleUnitBuf`を継承しています。  
それの代わりに`AdvancedUnitBuf`を継承し、幾つかの追加要素のあるそれとして扱うことを想定しています。

## Specified usage

### `DefaultStack`

```cs
public override int DefaultStack { get => 0; }
```

バフが付与されたときのデフォルトのスタック数を指定できます。

### `IsInstant`

```cs
public override bool IsInstant { get => false; }
```

バフが即時適用型か指定します。  
trueを指定した場合、`AddBuf`時に即座に`OnInstant`が実行されます。

### `IsSaveWhileReception`

```cs
public override bool IsSaveWhileReception => value;
```

舞台を跨いでインスタンスを維持します。

- `value`: 維持するかどうかです。

### `LastInflictBy`

バフが`KeywordBuf`かつ`AdvancedUnitBuf`である場合に、このバフを付与したユニットが格納されます。  
ただし、自分が自分に付与した場合は変更されません。

初期値はnullです。

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

### `OnInstant()`

```cs
public override void OnInstant()
{
}
```

即時適用型のバフが付与されたときに実行されます。  
`IsInstant`がfalseであるならば実行されません。

### `OnOtherInstant(AdvancedUnitBuf)`

```cs
public override void OnOtherInstant(AdvancedUnitBuf instant)
{
}
```

他の即時適用型のバフが付与されたときに実行されます。

- `instant`: 付与された他のバフです。

### `OnRoundStartFirst()`

```cs
public override void OnRoundStartFirst()
{
}
```

`BattleUnitBuf`の`OnRoundStart`より先に実行されます。

### `OnRoundStartLast()`

```cs
public override void OnRoundStartLast()
{
}
```

`BattleUnitBuf`の`OnRoundStartAfter`より後に実行されます。

### `OnStartBattle()`

```cs
public override void OnStartBattle()
{
}
```

戦闘開始時に実行されます。

### `OnStackChange(int)`

```cs
public override void OnStackChange(int last)
{
}
```

バフのスタック数が変更されたときに実行されます。

- `last`: 変更される前のスタック数です。

### `OnStackChangeAll(BattleUnitBuf, int)`

```cs
public override void OnStackChangeAll(BattleUnitBuf buf, int last)
{
}
```

保有しているなんらかのバフを獲得、もしくはスタック数の変更時に実行されます。

- `buf`: 対象のバフです。
- `last`: 変更される前のスタック数です。

### `OnAddBufAll(BattleUnitBuf, int)`

```cs
public override void OnAddBufAll(BattleUnitBuf buf, int addedStack)
{
}
```

なんらかのバフを獲得、もしくはスタック数の増加時に実行されます。

- `buf`: 対象のバフです。
- `addedStack`: 増加したスタック量です。

### `OnClick(ClickType)`

```cs
public override void OnClick(ClickType ty)
{
}
```

バフのアイコンがクリックされたときに実行されます。

- `ty`: クリックの種類です。(Left/Right/Middle)

### `OnAddKeywordBufByCard(BattleUnitBuf, int)`

```cs
public override int OnAddKeywordBufByCard(BattleUnitBuf buf, int stack)
{
}
```

ページ効果によりバフが付与されたときに実行されます。

- `buf`: 付与されたバフです。
- `stack`: 付与されたスタック数です。

### `GetBreakDamageReductionAll(int, DamageType, BattleUnitModel)`

```cs
public override int GetBreakDamageReductionAll(int dmg, DamageType dmgType, BattleUnitModel attacker)
{
}
```

全ての混乱ダメージ量の軽減数値を返します。

- `dmg`: 元のダメージ量です。
- `dmgType`: ダメージの種類です。
- `attacker`: 攻撃者です。

### `BeforeTakeOneSideAction(BattleUnitModel)`

```cs
public override BattlePlayingCardDataInUnitModel? BeforeTakeOneSideAction(BattleUnitModel attacker)
{
    return null;
}
```

一方攻撃を受ける前に実行されます。  
null以外を返した場合、返したページで代わりにマッチをします。

- `attacker`: 一方攻撃をしている相手です。

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

### `OnRecoverHp(int)`

```cs
public override void OnRecoverHp(int recoverAmount)
{
}
```

体力を回復したときに実行されます。

- `recoverAmount`: 回復した値です。

### `OnRecoverPP(int)`

```cs
public override void OnRecoverPP(int gain)
{
}
```

光を回復したときに実行されます。

- `gain`: 回復しようとした値です。

### `OnSuccessAreaAttack(BattleDiceBehavior, BattleUnitModel)`

```cs
public override void OnSuccessAreaAttack(BattleDiceBehavior behavior, BattleUnitModel target)
{
}
```

広域攻撃が的中したときに実行されます。

- `behavior`: 的中したダイスです。
- `target`: 的中した対象です。

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

### `BeforeRollDiceFirst(BattleDiceBehavior)`

```cs
public override void BeforeRollDiceFirst(BattleDiceBehavior behavior)
{
}
```

`BeforeRollDice`より先に実行されます。

- `behavior`: 対象のダイスです。

### `BeforeRollDiceLast(BattleDiceBehavior)`

```cs
public override void BeforeRollDiceLast(BattleDiceBehavior behavior)
{
}
```

`BeforeRollDice`より後に実行されます。

- `behavior`: 対象のダイスです。

### `GetDamageReductionAll(int, DamageType, BattleUnitModel, KeywordBuf)`

```cs
public override int GetDamageReductionAll(int dmg, DamageType dmgType, BattleUnitModel attacker, KeywordBuf buf)
{
    return 0;
}
```

全てのダメージに対して軽減を得ます。

- `dmg`: 受ける元のダメージです。
- `dmgType`: ダメージの種類です。
- `attacker`: 攻撃元のユニットです。
- `buf`: キーワードバフ由来のダメージであればそのキーワードです。
