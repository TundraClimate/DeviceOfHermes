# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# LazyUnitBuf

効果を遅延させて発生させるバフです。

```cs
/// 3幕後にownerを死亡させる
owner.bufListDetail.AddBuf(new LazyUnitBuf(3) { onActivate = owner => owner.Die() });
```
