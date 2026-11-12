# DynamicAbility

コマンドからページ・ダイス効果を生成します。

例:  
```xml
<Script>Card-TestMOD_ResourcePage-UseCard(light(5), draw(5))-Desc('[使用時] 光を5回復し、ページを5枚引く')</Script>
Script="Dice-TestMOD_Bleeding-SucceedAttack(infr('KeywordBuf_Bleeding', 8))-Desc('[的中] 次の幕 出血8を付与')"
```

## Syntax

基本構文: `{PREFIX}-{ABILITY_NAME}-{OPTION...}`

**PREFIX**:  
    このスクリプトがページ効果であるか、ダイス効果であるかを指定します。  
    ページ効果の場合は、`Card`・`DOHC`・`dohc`のいずれかが指定に使用できます。  
    ダイス効果の場合は、`Dice`・`DOHD`・`dohd`のいずれかが指定に使用できます。  

**ABILITY_NAME**:  
    Abilityを区別するために使用される固有の文字列です。  
    Localizeにて主に使用され、重複した場合はより後に設定したページ/ダイス説明文に統一されます。  

**OPTION**:  
    Abilityに対して指定する固有要素です。  
    `オプション名1(オプション内容1)-オプション名2(オプション内容2)`のように指定します。  
    詳細は下記を参照してください。  

## Types

オプション内で指定する値の種類を以下に示します。  

### `String`

文字列です。  
`'`で囲っている場合に`String`として解釈されます。  

例: `'ここに文字列'`

### `Number`

符号付き整数値です。  
`+`・`-`などの符号は省略可能です。

例: `1`、`-1`

### `Fn`

関数です。  
上記の`String`や`Number`を引数に指定して構成されます。  
引数同士は`,`で区切ることが出来ます。

例: `log('Card used')`、`draw(3)`、`statbonus('dmg', 100)`

## Options

### `UseCard(Fn...)`

対象: Card

ページ使用時の効果を設定します。

### `StartBattle(Fn...)`

対象: Card

戦闘開始時の効果を設定します。

### `StartParrying(Fn...)`

対象: Card

マッチ開始時の効果を設定します。

### `StartOneSide(Fn...)`

対象: Card

一方攻撃開始時の効果を設定します。

### `EndBattle(Fn...)`

対象: Card

一方攻撃・マッチの終了時の効果を設定します。

### `WinParrying(Fn...)`

対象: Card/Dice

マッチ勝利時の効果を設定します。

### `LoseParrying(Fn...)`

対象: Card/Dice

マッチ敗北時の効果を設定します。

### `DrawParrying(Fn...)`

対象: Card/Dice

マッチ引き分け時の効果を設定します。

### `BeforeRollDice(Fn...)`

対象: Card/Dice

ダイスを振る前の効果を設定します。

### `BeforeGiveDamage(Fn...)`

対象: Card/Dice

ダメージを与える前の効果を設定します。

### `SucceedAttack(Fn...)`

対象: Card/Dice

的中時の効果を設定します。

### `RollDice(Fn...)`

対象: Card/Dice

ダイスを振ったときの効果を設定します。

## Option Funcs

### `log(String)`

ログを出力します。  
`UnityEngine.Debug.Log`を使用します。

### `light(Number)`

光を回復します。

### `draw(Number)`

ページを引きます。

### `card(String, Number)`

手札にページを追加します。  
バニラページの場合は第一引数を空白文字列('')にします。

### `cardexhaust(String, Number)`

手札に使い捨てのページを追加します。  
バニラページの場合は第一引数を空白文字列('')にします。

### `heal(Number)`

体力を回復します。

### `bheal(Number)`

混乱抵抗値を回復します。

### `statbonus(String, Number, Number?)`

`DiceStatBonus`を適用します。  
第一引数に指定する文字列は`DiceStatBonus`の対象フィールド名です。  
第三引数はページのIndex位置のダイスに適用されます。

### `estatbonus(String, Number, Number?)`

敵に`DiceStatBonus`を適用します。  
第一引数に指定する文字列は`DiceStatBonus`の対象フィールド名です。  
第三引数はページのIndex位置のダイスに適用されます。

### `gain(String, Number)`

バフをこの幕に獲得します。  
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `gainr(String, Number)`

バフを次の幕に獲得します。  
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `gainrr(String, Number)`

バフを次の次の幕に獲得します。  
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `inf(String, Number)`

バフをこの幕に付与します。
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `infr(String, Number)`

バフを次の幕に付与します。
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `infrr(String, Number)`

バフを次の次の幕に付与します。
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `allgain(String, Number)`

全ての味方がバフをこの幕に獲得します。  
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `allgainr(String, Number)`

全ての味方がバフを次の幕に獲得します。  
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `allgainrr(String, Number)`

全ての味方がバフを次の次の幕に獲得します。  
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `allinf(String, Number)`

全ての敵にバフをこの幕に付与します。
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `allinfr(String, Number)`

全ての敵にバフを次の幕に付与します。
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `allinfrr(String, Number)`

全ての敵にバフを次の次の幕に付与します。
第一引数に指定する値はバフ名で、`KeywordBuf_`で開始した場合は`KeywordBuf`が使用され、  
それ以外の場合は`DynamicAbilityCfg`から検索します。

### `takedmg(Number)`

ダメージを受けます。

### `givedmg(Number)`

ダメージを与えます。

### `takebdmg(Number)`

混乱ダメージを受けます。

### `givebdmg(Number)`

混乱ダメージを与えます。

## Special Options

### `Desc(String)`

対象: Card/Dice

効果説明文を設定します。

### `Debug()`

対象: Card/Dice

デバッグ用です。  
情報を出力します。

# DynamicAbilityCfg

`DynamicAbility`の設定を行います。

## Specified usage

### `AddBattleUnitBufRoute<T>(string)`

```cs
DynamicAbilityCfg.AddBattleUnitBufRoute<T>(name);
```

自作した`BattleUnitBuf`を`DynamicAbility`で使用できるようにするための対応表を追加します。

- `T`: 対象の`BattleUnitBuf`です。
- `name`: `DynamicAbility`側で指定する名前です。
