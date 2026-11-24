# HermesConfig

DeviceOfHermes用の設定ファイルです。

適用する自身のMODフォルダ内の`Hermes.toml`を対象として読み込みます。  
存在しない場合は設定ファイルを参照する全ての機能が実行されません。

最新の推奨設定は`DeviceOfHermes`のMODフォルダ内をの`Hermes.toml`を参照してください。

推奨設定:

```toml
# 自動でAssemblies/内のパッチを当てるかどうか
# trueにすると自動でパッチを当てます
auto-patch = false

[data]

# 自動マップ設定をロードするファイルのパス
automap = "Data/AutoMap.xml"

# 専用ページをロードするファイルのパス
onlycard = "Data/OnlyCard.xml"

# フォーメーションをロードするファイルのパス
formation = "Data/Formation.xml"

# 幻想体ページをロードするファイルのパス
emotioncard = "Data/EmotionCard.xml"

# コアページとタグのマッピングをロードするファイルのパス
booktag = "Data/BookTag.xml"

[localize]

# 幻想体ページのローカライズをロードするフォルダのパス
abnocard = "LocalizeBase/{0}/AbnoCard"

# コアページタグのローカライズをロードするフォルダのパス
booktag = "LocalizeBase/{0}/BookTag"

[resource]

# 自動マップ設定で画像データをロードするフォルダのパス
automap-artwork = "Resource/AutoMapArtwork"

# 自動マップ設定で音声データをロードするフォルダのパス
automap-audio = "Resource/AutoMapAudio"

# 幻想体ページの画像データをロードするフォルダのパス
abnocard-artwork = "Resource/AbnoCardArtwork"
```
