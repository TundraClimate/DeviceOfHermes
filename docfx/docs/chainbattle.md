# `using DeviceOfHermes;`

名前空間`DeviceOfHermes`に存在するクラスです。  
この記事はこれを前提に書かれています。

# ChainBattle

司書を他階層から徴収する連続戦闘システムを使用して接待を作成できます。

連続戦闘システムは**接待単位で有効化**されます。  
連続戦闘システムでは他の階層の司書を待機させて接待に挑むことが可能です。  
指定司書を含むかどうかは接待によって設定できますが、デフォルトでは**含めない**設定になっています。  

待機司書は戦闘中の司書が死亡・退却したときに戦闘に参加します。  
幻想体ページは戦闘中の階のページが引き続き使用されます。

連続戦闘システムは、`ChainBattleStageManager`を実装した`EnemyTeamStageManager`を作成・適用することで有効化されます。  

実装例:  
```cs
public class EnemyTeamStageManager_TestStage : ChainBattleStageManager
{
    // 待機階層として指定が可能な最大数
    public override int MaximumRearFloorNum => 2;

    // 指定司書も待機司書に含めたいならばこの下の行をコメント削除する
    // public override bool IsSephirahRemove => false;

    // 司書が待機解除したときに実行
    public override OnArrived(BattleUnitModel unit)
    {
    }

    // 連続戦闘環境下でキャラクターが死亡したときに実行
    public override OnDead()
    {
    }
}
```

※ 補充型戦闘

連続戦闘を有効化した場合、デフォルトで有効になります。  
`StageInfo.xml`にてUnitが5体を超過した場合、超過したユニットが待機人員として自動追加されます。

```xml
  <Unit>1</Unit>
  <Unit>2</Unit>
  <Unit>3</Unit>
  <Unit>4</Unit>
  <Unit>5</Unit>
  <Unit>6</Unit>
```

※ 退却

`ChainBattleRetreatBuf`をbufListDetailにAddBufすると幕の終了時に退却します。  
連続戦闘が有効でない場合も退却しますが、その場合帰ってきません。

## Specified usage

### `ChainBattleStageManager.IsChainBattle`

trueを返す場合、連続戦闘が有効化されます。

### `ChainBattleStageManager.IsSupplyEnemy`

trueを返す場合、補充型戦闘が有効化されます。

### `ChainBattleStageManager.IsSephirahRemove`

trueを返す場合、待機階層の指定司書が待機司書として取り込まれなくなります。

### `ChainBattleStageManager.MaximumRearFloorNum`

指定可能な待機階層の指定数です。

### `ChainBattleStageManager.OnArrived(BattleUnitModel)`

```cs
public override void OnArrived(BattleUnitModel unit)
{
}
```

キャラクターが待機解除したときに実行されます。

- `unit`: 対象のキャラクターです。

### `ChainBattleStageManager.OnRetreat(BattleUnitModel)`

```cs
public override void OnRetreat(BattleUnitModel unit)
{
}
```

キャラクターが退却したときに実行されます。

- `unit`: 対象のキャラクターです。

### `ChainBattleStageManager.OnDead(BattleUnitModel)`

```cs
public override void OnDead(BattleUnitModel unit)
{
}
```

連続戦闘環境下にてキャラクターが死亡して置き換えられるときに実行されます。

- `unit`: 対象のキャラクターです。

### `ChainBattleStageManager.GetSelectedFloors()`

```cs
ChainBattleStageManager.GetSelectedFloors();
```

戦闘開始時に指定していた使用する階層と待機階層をまとめたリストを返します。

### `ChainBattleRetreatBuf.OnRetreat()`

```cs
public override void OnRetreat()
{
}
```

退却バフによって実際に退却したときに実行されます。
