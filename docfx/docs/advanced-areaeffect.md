# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdvancedAreaEffect

IEnumeratorを使用してCoroutineのように構築できるようにした`FarAreaEffect`です。  
`ActionSeq`メソッドを実装することによって構築することが可能です。  

```cs
public class FarAreaEffect_Test : AdvancedAreaEffect
{
    public override void Init(BattleUnitModel self, params object[] args)
    {
        base.Init(self, args);
    }

    public override IEnumerator ActionSeq(BattleUnitModel attacker, List<BattleFarAreaPlayManager.VictimInfo> victims)
    {
    }
}
```

`yield return`にて`float`値を返した場合、その値を秒数とみなしてその時間待機します。  
`yield return`にて`IEnumerator`を返した場合、展開してその`IEnumerator`を順に実行します。
