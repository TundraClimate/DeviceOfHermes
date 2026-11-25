# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# AdvancedBehaviourAction

IEnumeratorを使用してCoroutineのように構築できるようにした`BehaviourActionBase`です。  
`GetMoving`メソッドを実装することによって構築することが可能です。

また、`Init`で設定することで複数ユニットを同時に動かすことが出来ます。  
このユニットもIEnumeratorを使用して行動を制御することが出来ます。  
ただし、メインのユニットが行動を終えた場合はその他のユニットも同時に行動を終了します。

実行中に行う操作の簡略化されたものについては、[RencounterRoutine](rencounter-routine.md)を参照してください。

```cs
public class BehaviourAction_Test : AdvancedBehaviourAction
{
    protected override IEnumerator GetMoving(RencounterManager.ActionAfterBehaviour self, RencounterManager.ActionAfterBehaviour opponent)
    {
        // フレームを終了
        yield return null;

        // 1.5秒待機
        yield return 1.5f;

        // SomeRoutineを順に実行する
        yield return SomeRoutine();

        // 回復エフェクト及びダメージ表示
        // 各Action内で1度のみ有効
        PrintRecovery();
        PrintDamage();

        // self.viewに攻撃エフェクトを表示
        PrintEffect("FX_PC_RolRang_Slash_UP", self.view, opponent.view, 1f);

        // self.viewから攻撃SEを再生
        PrintSound("Roland_Duralandal_Up", self.view);

        // キャラクターモーションに紐づくSEを再生
        PrintSound(self.view, ActionDetail.S1);
    }
}
```

`yield return`にて`float`値を返した場合、その値を秒数とみなしてその時間待機します。  
`yield return`にて`IEnumerator`を返した場合、展開してその`IEnumerator`を順に実行します。

```cs
protected override void Init(List<ParallelAction> parallels, RencounterManager.ActionAfterBehaviour self, RencounterManager.ActionAfterBehaviour opponent)
{
    // parallelsに追加することで動かすユニットとすることが出来る
    parallels.Add(new(/* 対象のBattleUnitViewインスタンス */, GetFamilyMoving(family.view, opponent.view)));
}

// キャラクター制御に使用するIEnumerator
// 引数などは自由
IEnumerator GetFamilyMoving(BattleUnitView view, BattleUnitView oppo)
{
    // GetMovingと同じように実装する
}
```

標準では"攻撃ダイス的中時"にActionScriptが再生されます。  
この条件を変更したい場合は`IsDiscardAction(RencounterManager.ActionAfterBehaviour self, RencounterManager.ActionAfterBehaviour opponent)`を実装してください。  
`false`を返す場合にはActionScriptが再生されます。

```cs
protected override bool IsDiscardAction(RencounterManager.ActionAfterBehaviour self, RencounterManager.ActionAfterBehaviour opponent)
{
    // 常に再生する
    return false;
}
```
