# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# ScheduleRunner

戦闘中の各イベントのタイミングに行う処理を追加します。

例:  
```cs
ScheduleRunner.AddSchedule(ScheduleTiming.RoundStart, () => Hermes.Say("Round started"));
```

この例は`OnRoundStart`のタイミングにログを出力するコードです。

## Timings

```cs
enum ScheduleTiming
{
    RoundStart,
    RollSpeedDice,
    StartBattle,
    UseCard,
    EndBattle,
    RoundEnd,
}
```

## Specified usage

### `AddSchedule(ScheduleTiming, Action)`

```cs
ScheduleRunner.AddSchedule(timing, invokeFn);
```

各タイミングに実行する処理を追加します。

- `timing`: 追加するタイミングです。
- `invokeFn`: 追加する処理です。
