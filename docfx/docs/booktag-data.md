# BookTagXml

XMLファイルを配置することでコアページにタグを付与することが出来ます。  
条件は`DeviceOfHermes`のサブスクライブのみです。

[HermesConfig](hermes-config.md)に従って読み込みます。  
フォルダ・ファイルが存在しない場合は無視します。

付与したタグは[BookTag](booktag.md)を用いて参照することが出来ます。

`BookTag.xml`

```xml
<BookTagXmlRoot>

    <!-- そのMODのid=10000001のコアページにタグを付与 -->
    <Book ID="10000001">
        <!-- 'Ring'を付与 -->
        <Tag>Ring</Tag>

        <!-- 'Maestro'を無効化状態で付与 -->
        <Tag>Maestro:Delete</Tag>

        <!-- 1つのコアに対して10つまで付与可能 -->
    </Book>

</BookTagXmlRoot>
```

`BookTag.toml`(ローカライズ)

```toml
# "A" = "B"
# タグAのローカライズをBとして上書きします。
# 汎用的なタグを作成する場合、他のMODからも上書きされる場合があります。
"TerminationOffice" = "終端事務所"
# 以下同じように複数実装可能です。
"SweepBeepWorkshop" = "ちり取り小鳥工房"
```

# Builtin

```toml
"ACorp" = "A社"
"BCorp" = "B社"
"CCorp" = "C社"
"DCorp" = "D社"
"ECorp" = "E社"
"FCorp" = "F社"
"GCorp" = "G社"
"HCorp" = "H社"
"ICorp" = "I社"
"JCorp" = "J社"
"KCorp" = "K社"
"LCorp" = "L社"
"MCorp" = "M社"
"NCorp" = "N社"
"OCorp" = "O社"
"PCorp" = "P社"
"QCorp" = "Q社"
"RCorp" = "R社"
"SCorp" = "S社"
"TCorp" = "T社"
"UCorp" = "U社"
"VCorp" = "V社"
"WCorp" = "W社"
"XCorp" = "X社"
"YCorp" = "Y社"
"ZCorp" = "Z社"

"Group" = "組織"

"Finger" = "指"

"Thumb" = "親指"
"Index" = "人差し指"
"Middle" = "中指"
"Ring" = "薬指"
"Pinky" = "小指"

"Geomgye" = "剣契"
"Kurokumo" = "黒雲会"

"Fixer" = "フィクサー"

"Color" = "特色"

"1Assoc" = "ハナ協会"
"2Assoc" = "ツヴァイ協会"
"3Assoc" = "トレス協会"
"4Assoc" = "シ協会"
"5Assoc" = "センク協会"
"6Assoc" = "リウ協会"
"7Assoc" = "セブン協会"
"8Assoc" = "エイト協会"
"9Assoc" = "ヂェーヴィチ協会"
"10Assoc" = "ディエーチ協会"
"11Assoc" = "ウーフィ協会"

"Bloodfiend" = "血鬼"

"DongHwan" = "ドンファン"

"Head" = "頭"
"Eye" = "目"
"Claw" = "足爪"
```
