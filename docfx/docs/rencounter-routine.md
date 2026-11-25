# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# `RencounterRoutine`

`BattleUnitView`に対するマッチ中などで使用できる操作集です。  
モーションの変更等は含まれていません。

```cs
// AdvancedBehaviourAction
protected override IEnumerator GetMoving(RencounterManager.ActionAfterBehaviour self, RencounterManager.ActionAfterBehaviour opponent)
{
    // 前方向に少しだけ直進
    yield return self.view.MoveForward(opponent.view, forward: 10f);

    // 後方向に少しだけ直進
    yield return self.view.MoveBack(opponent.view, back: 10f);

    // 対象に到達するまで直進
    yield return self.view.MoveOpponent(opponent.view, margin: 3f);

    // 対象の面前に瞬間移動
    yield return self.view.TeleportFront(opponent.view, distance: 3f);

    // 対象の背後に瞬間移動
    yield return self.view.TeleportBack(opponent.view, distance: 3f);

    // 対象をノックバックさせます
    yield return opponent.view.Knockback(self.view);
}
```

引数などは`RencounterManager.MovingAction`に沿って実装されています。
