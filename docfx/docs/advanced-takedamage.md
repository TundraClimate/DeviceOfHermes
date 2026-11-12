# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdvancedTakeDamage

攻撃属性と耐性を指定してダメージを与えます。

例:  
```cs
// targetに999の打撃属性ダメージを与える
target.DealDamage(999, BehaviourDetail.Hit);

// targetに脆弱耐性として100の斬撃ダメージを与えます。
target.DealDamage(100, BehaviourDetail.Slash, resist: AtkResist.Weak);
```

## Specified usage

### `DealDamage(BattleUnitModel, int, LOR_DiceSystem.BehaviourDetail, DamageType, BattleUnitModel, KeywordBuf, AtkResist)`

```cs
target.DealDamage(dmg, detail, type, attacker, keyword, resist);
```

- `target`: ダメージを与える対象です。
- `dmg`: 耐性が適用される前のダメージです。
- `detail`: 攻撃属性です。LOR_DiceSystem.BehaviourDetailを指定します。
- `type`: 攻撃の種類です。省略可能。
- `attacker`: 攻撃を与える側です。省略可能。
- `keyword`: ダメージ原因の`KeywordBuf`です。省略可能。
- `resist`: 攻撃を受ける耐性です。強制的に変更されるので、必要ない場合は指定しないでください。

### `StyledDamage(BattleUnitModel, int, Sprite, DamageType, KeywordBuf)`

```cs
target.DealDamage(dmg, img, type, keyword);
```

耐性アイコンの代わりに任意の画像を指定できます。

- `target`: ダメージを与える対象です。
- `dmg`: ダメージ量です。
- `img`: 表示する任意の画像です。
- `type`: 攻撃の種類です。省略可能。
- `keyword`: ダメージ原因の`KeywordBuf`です。省略可能。

### `StyledBreakDamage(BattleUnitModel, int, Sprite, DamageType, KeywordBuf)`

```cs
target.DealDamage(dmg, img, type, keyword);
```

耐性アイコンの代わりに任意の画像を指定できます。

- `target`: 混乱ダメージを与える対象です。
- `dmg`: 混乱ダメージ量です。
- `img`: 表示する任意の画像です。
- `type`: 攻撃の種類です。省略可能。
- `keyword`: ダメージ原因の`KeywordBuf`です。省略可能。
