# `using DeviceOfHermes.Resource;`

名前空間`DeviceOfHermes.Resource`に存在するクラスです。  
この記事はこれを前提に書かれています。

# TextModel

テキストデータをゲームに追加します。  
また、翻訳が適用されるタイミングに実行される処理を追加出来ます。

## Specified usage

### `OnLoadLocalize`

```cs
TextModel.OnLoadLocalize += lang => 
{
}
```

翻訳が適用されたタイミングで実行される処理を追加します。

- `lang`: 対象の言語です。`en | jp | kr | cn | trcn`の5種類があります。

### `SetBattleEffectText(LOR_XML.BattleEffectText, bool)`

```cs
TextModel.SetBattleEffectText(text, replace);
```

ページ情報横に表示される情報テキストを追加・変更します。

- `text`: 追加する`BattleEffectText`です。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetBattleEffectTexts(List<LOR_XML.BattleEffectText>, bool)`

```cs
TextModel.SetBattleEffectTexts(texts, replace);
```

ページ情報横に表示される情報テキストを追加・変更します。

- `texts`: 追加する`BattleEffectText`のリストです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetBattleCardAbilityDesc(LOR_XML.BattleCardAbilityDesc, bool)`

```cs
TextModel.SetBattleCardAbilityDesc(desc, replace);
```

ページ効果のテキストを追加・変更します。

- `desc`: 追加する`BattleCardAbilityDesc`です。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetBattleCardAbilityDescs(List<LOR_XML.BattleCardAbilityDesc>, bool)`

```cs
TextModel.SetBattleCardAbilityDescs(descs, replace);
```

ページ効果のテキストを追加・変更します。

- `descs`: 追加する`BattleCardAbilityDesc`のリストです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetBattleCardDesc(LOR_XML.BattleCardDesc, string, bool)`

```cs
TextModel.SetBattleCardDesc(desc, pid, replace);
```

ページタイトル含めページのテキストを追加・変更します。

- `desc`: 追加する`BattleCardDesc`です。
- `pid`: 属するパッケージのIDです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetBattleCardDescs(List<LOR_XML.BattleCardDesc>, bool)`

```cs
TextModel.SetBattleCardDescs(descs, replace);
```

ページタイトル含めページのテキストを追加・変更します。

- `descs`: 追加する`BattleCardDesc`とpidのタプルリストです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetCharacterDialog(LOR_XML.BattleDialogCharacter, string, bool)`

```cs
TextModel.SetCharacterDialog(character, groupName, replace);
```

特定グループのキャラクターのセリフを追加・変更します。

- `character`: 追加する`BattleDialogCharacter`です。
- `groupName`: キャラクターが所属するグループです。MOD環境は`Workshop`です。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetCharacterDialogs(List<LOR_XML.BattleDialogCharacter>, string, bool)`

```cs
TextModel.SetCharacterDialogs(characters, groupName, replace);
```

特定グループのキャラクターのセリフを追加・変更します。

- `characters`: 追加する`BattleDialogCharacter`のリストです。
- `groupName`: キャラクターが所属するグループです。MOD環境は`Workshop`です。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetBookDesc(LOR_XML.BookDesc, string, bool)`

```cs
TextModel.SetBookDesc(desc, pid, replace);
```

特定の本のストーリーを追加・変更します

- `desc`: 追加する`BookDesc`です。
- `pid`: その本が所属するパッケージのIDです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetBookDescs(Lit<LOR_XML.BookDesc>, bool)`

```cs
TextModel.SetBookDescs(descs, replace);
```

特定の本のストーリーを追加・変更します

- `desc`: 追加する`BookDesc`のリストです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetCharacterName(LorId, string, bool)`

```cs
TextModel.SetCharacterName(id, name, replace);
```

特定のキャラクター名を追加・変更します。  
MOD由来のキャラクターは変更のみ可能です。

- `id`: キャラクター固有のIDです。
- `name`: 新しいキャラクターの名称です。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetCharacterName(List<(LorId, string)>, bool)`

```cs
TextModel.SetCharacterNames(names, replace);
```

特定のキャラクター名を追加・変更します。  
MOD由来のキャラクターは変更のみ可能です。

- `names`: キャラクターのIDと名称のリストです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetStageName(LorId, string, bool)`

```cs
TextModel.SetStageName(id, name, replace);
```

特定のステージ名を追加・変更します。  
MOD由来のステージは変更のみ可能です。

- `id`: ステージ固有のIDです。
- `name`: 新しいステージの名称です。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetStageNames(List<(LorId, string)>, bool)`

```cs
TextModel.SetStageNames(names, replace);
```

特定のステージ名を追加・変更します。  
MOD由来のステージは変更のみ可能です。

- `names`: ステージのIDと名称のリストです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetPassiveDesc(LOR_XML.PassiveDesc, bool)`

```cs
TextModel.SetPassiveDesc(desc, replace);
```

特定のパッシブのテキストを追加・変更します。

- `desc`: 追加・変更する`PassiveDesc`です。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetPassiveDescs(List<PassiveDesc>, bool)`

```cs
TextModel.SetPassiveDescs(descs, replace);
```

特定のパッシブのテキストを追加・変更します。

- `descs`: 追加・変更する`PassiveDesc`のリストです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetDropBookName(LorId, string)`

```cs
TextModel.SetDropBookName(id, name);
```

特定の本の名前を変更します。

- `id`: 本固有のIDです。
- `name`: 変更する名称です。

### `SetDropBookNames(List<(LorId, string)>)`

```cs
TextModel.SetDropBookNames(names);
```

特定の本の名前を変更します。

- `names`: 変更する名称とIDのリストです。

### `SetTextData(string, string, bool)`

```cs
TextModel.SetTextData(id, data, replace);
```

テキストデータを追加・変更します。

- `id`: テキストデータのIDです。
- `data`: テキストです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。

### `SetTextData(List<(string, string)>, bool)`

```cs
TextModel.SetTextData(data, replace);
```

テキストデータを追加・変更します。

- `data`: テキストデータとIDのリストです。
- `replace`: 重複する要素が存在する場合に置き換えるかどうかです。
