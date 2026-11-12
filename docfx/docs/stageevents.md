# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# StageEvents

接待中のイベントを監視してフックします。

```cs
// 舞台開始前に実行される
StageEvents.BeforeWaveStart += () => Hermes.Say($"start wave before");

// 舞台開始時に実行される
StageEvents.OnWaveStart += () => Hermes.Say($"start wave");

// 幕の開始前に実行される
StageEvents.BeforeRoundStart += () => Hermes.Say($"round start before");

// 幕の開始時に実行される
StageEvents.OnRoundStart += () => Hermes.Say($"round start");

// 速度ダイスをUI上で決定したときに実行される
StageEvents.OnStopSpeedDice += () => Hermes.Say($"stop speed dice");

// 戦闘開始前に実行される
StageEvents.BeforeStartBattle += () => Hermes.Say($"start battle before");

// 戦闘開始時に実行される
StageEvents.OnStartBattle += () => Hermes.Say($"start battle");

// 幕の終了前に実行される
StageEvents.BeforeRoundEnd += () => Hermes.Say($"round end before");

// 幕の終了時に実行される
StageEvents.OnRoundEnd += () => Hermes.Say($"round end");

// ページ使用時に実行される
StageEvents.OnUseCard += card => Hermes.Say($"card used");

// ダイスを振る前に実行される
StageEvents.BeforeRollDice += dice => Hermes.Say($"before roll dice");

// 接待のCleanup処理の直後に実行される
StageEvents.OnCleanup += () => Hermes.Say($"end cleanup");
```
