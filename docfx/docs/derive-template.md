# `using DeviceOfHermes.Derive;`

名前空間`DeviceOfHermes.Derive`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Caduceus REQUIRED

`Caduceus`を必要とします。  
詳細はトップページを参照してください。

# DeriveTemplate

組み込みテンプレートのリストです。

## DiceCardSelfAbilityBase

### `Draws1CardOnUseCard`

ページを1枚引く

### `Recover1CostOnUseCard`

光を1回復する

## DiceCardAbilityBase

### `DestroyOppoAllDiceOnWinParrying`

マッチ勝利時に相手の全ダイスを破壊する

### `DestroyOppoNextDiceOnWinParrying`

マッチ勝利時に相手の次のダイスを破壊する

### `DestroySelfAllDiceOnLoseParrying`

マッチ敗北時に自分の全ダイスを破壊する

### `DestroySelfAllDiceOnDrawParrying`

マッチ引き分け時に自分の全ダイスを破壊する

## BattleUnitBuf

### `DestroyOnRoundEnd`

幕の終了時に除去される

### `DestroyIfZeroOnRoundEnd`

幕の終了時に数値が0なら除去される

### `Decrease1StackOnRoundEnd`

幕の終了時に数値が1減少

### `DecreaseHalfStackOnRoundEnd`

幕の終了時に数値が半減

### `Into1s3StackOnRoundEnd`

幕の終了時に数値が1/3になる

### `Into2s3StackOnRoundEnd`

幕の終了時に数値が2/3になる

### `Into4s5StackOnRoundEnd`

幕の終了時に数値が4/5になる

### `Maximum1StackOnAddbuf`

最大値が1

### `Maximum10StackOnAddbuf`

最大値が10

### `Maximum20StackOnAddbuf`

最大値が20

### `Maximum50StackOnAddbuf`

最大値が50

### `Maximum100StackOnAddbuf`

最大値が100
