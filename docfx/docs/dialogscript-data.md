# DialogScriptXml

"Data/Combat_Dialog.xml"からスクリプトを紐付けます。

`Combat_Dialog.xml`

```xml
<BattleDialogRoot>

  <Character ID="1">
    <!-- MyScriptと紐付ける -->
    <!-- 重複すると一方が使用される -->
    <DialogScript>MyScript</DialogScript>

    <!-- 他要素は省略 -->
  </Character>

</BattleDialogRoot>
```

紐づけたスクリプト名に`BattleDialogueModel_`を組み合わせた名前の型を自動で読み込みます。

`BattleDialogueModel_MyScript`

```cs
class BattleDialogueModel_MyScript : BattleDialogueModel
{
    public BattleDialogueModel_MyScript(BattleDialogCharacter xml) : base(xml)
    {
    }

    public override string GetBattleDlg(DialogType dlgType, List<BattleUnitModel> targets)
    {
        // ここはいい感じに
        // 返したstringをそのまま出力します

        return base.GetBattleDlg(dlgType, targets);
    }
}
```
