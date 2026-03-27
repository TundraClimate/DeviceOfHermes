# Device of Hermes

## Impl for System

`Hermes.Say(message, lvl = MessageLevel.Info)`:

```cs
Hermes.Say("Hey, Rien.");

Hermes.Say("The prescript cannot be defied", MessageLevel.Warn);

Hermes.Say("*beep*", MessageLevel.Error);
```

`Hermes.Store(data)`, `Hermes.Load()`:

```
Hermes.Store("Hi, Hermes!");

// "Hi, Hermes!"
Console.WriteLine(Hermes.Load<string>());

Hermes.Store<int>(12);

// "12"
Console.WriteLine(Hermes.Load<int>());
```

`Mem.Swap(ref a, ref b)`, `Mem.Take(ref self)`, `Mem.Replace(ref self, newValue)`, `Mem.ReplaceIf(ref self, newValue, exptected)`:

```cs
int a = 12;
int b = 22;

Mem.Swap(ref a, ref b);

// 22
Console.WriteLine(a);
// 12
Console.WriteLine(b);
```

```cs
string? data = "Elevens";

string? ejected = Mem.Take(ref data);

// null
Console.WriteLine(data);
// "Elevens"
Console.WriteLine(ejected);
```

```cs
string? text = "Hello, Hermes!";

string? old = Mem.Replace(ref text, "*beep*");

// "*beep*"
Console.WriteLine(text);
// "Hello, Hermes!"
Console.WriteLine(old);
```

```cs
string? pass = "_CLEAR._";

// "_CLEAR._"
Console.WriteLine(Mem.ReplaceIf(ref pass, "*beep*", "*beep*"));

// "_CLEAR._"
Console.WriteLine(Mem.ReplaceIf(ref pass, "*beep*", "_CLEAR._"));

// "*beep*"
Console.WriteLine(pass);
```

`Faction.FaceTo`

```cs
// Faction.Player
Faction.Enemy.FaceTo();
```

## Impl for System.Collction.Generic

`Peekable`

```cs
var peekable = new Peekable<string>(new List<string>() { "Foo", "Bar" });
string? elem = null;

peekable.Peek(out elem);

// "Foo"
Console.Write(elem);

peekable.MoveNext(out elem);

// "Foo"
Console.Write(elem);

peekable.MoveNext(out elem);

// "Bar"
Console.Write(elem);

peekable.MoveNext(out elem);

// null
Console.Write(elem);
```

## Impl for BattleUnitModel

`GetBuf<T>(this BattleUnitModel? model)`:

```cs
using DeviceOfHermes.BattleBuf;

BattleUnitBuf_burn? burn = base.owner.GetBuf<BattleUnitBuf_burn>();
```

`TryGetBuf<T>(this BattleUnitModel model, [NotNullWhen(true)] out T? buf)`:

```cs
using DeviceOfHermes.BattleBuf;

if (base.owner.TryGetBuf<BattleUnitBuf_burn>(out BattleUnitBuf_burn burn))
{
    // burn is Non-null
}

// burn is Nullable
```

`GetBufAndInitIfNull<T>(this BattleUnitModel model, Func<T> bufMake)`:

```cs
using DeviceOfHermes.BattleBuf;

// Throws the NullReferenceException if base.owner is null
BattleUnitBuf_burn buf = base.owner.GetBufAndInitIfNull<BattleUnitBuf_burn>(() => new BattleUnitBuf_burn());
```

`RemoveBuf<T>(this BattleUnitModel? model)`, `RemoveBufIf(this BattleUnitModel? model, Func<BattleUnitBuf, bool> cond)`:

```cs
using DeviceOfHermes.BattleBuf;

// Removes the BattleUnitBuf_burn
base.owner.RemoveBuf<BattleUnitBuf_burn>();

// Removes buf that has stack is zero
base.owner.RemoveBufIf(buf => buf.stack == 0);
```

`GetBufStack<T>(this BattleUnitModel? model)`:

```cs
using DeviceOfHermes.BattleBuf;

int stack = base.owner.GetBufStack<BattleUnitBuf_burn>() ?? 0;
```

## Impl for Schedule

`ScheduleRunner`:

```cs
using DeviceOfHermes.Schedule;

ScheduleRunner.AddSchedule(ScheduleTiming.RoundStart, () => Hermes.Say("Ahh, round started."));

ScheduleRunner.AddSchedule(ScheduleTiming.RoundEnd, () => Hermes.Say("Shhhh!"));
```

## Impl for Style

`StyleExt`:

```cs
using DeviceOfHermes.Style;

string red = "Red Text".Red();
string bold = "Bold Text".Bold();
```
