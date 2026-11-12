# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# Remainder

"残りX回"を直感的に管理するためのモジュールです。

```cs
var remainder = new Remainder(2);

// True
Hermes.Say($"{remainder.Remains}");

remainder.Lose();

// True
Hermes.Say($"{remainder.Remains}");

remainder.Lose();

// False
Hermes.Say($"{remainder.Remains}");

remainder.Reset();

// True
Hermes.Say($"{remainder.Remains}");
```

## Specified usage

### `Remains`

```cs
remainder.Remains;
```

`Remainder`がまだカウントを残しているかどうかです。

### `Lose()`

```cs
remainder.Lose();
```

1カウント消耗します。

### `Reset()`

```cs
remainder.Reset();
```

カウントをリセットします。
