---
_layout: landing
---

# Device Of Hermes

Library of Ruina Moddingのライブラリです。  

コーディングの簡略化、パッチが必要な処理のフレームワーク化が主要素。  

## In Workshop

サブスクライブが必要なアイテム→ [Workshop URL](https://steamcommunity.com/sharedfiles/filedetails/?id=3689874580)

DLLの共通参照先も兼ねています。  
参照先→ `C:\Program Files (x86)\Steam\steamapps\workshop\content\1256670\3689874580\Assemblies\DeviceOfHermes.dll`

VisualStudioなど、dllの参照が必要であるならこの場所にあるファイルを使うべきです。

## Includes Your MOD

ワークショップの`Device Of Hermes`をサブスクライブし、dllを参照に追加したなら、それで準備は完了です。  
dll本体は`Device Of Hermes`自体に含まれているので、個別のダウンロード・コピーは不要です。  

ゲームを起動後、`Device Of Hermes`にチェックを入れて続けてください。

初回起動時に発生するエラーは、読み込み順改変前に発生し得る正常なエラーです。  
一度ゲーム開始画面まで進めることでエラーを解消することが出来ます。  
`System.Reflection.ReflectionTypeLoadException`以外であるならば、ご自身の環境で検証した後ご報告ください。

### LimbufOfHermes

Limbus Companyのバフ効果を一部再現した物を含んでいるライブラリです。  
追加で参照することによって使用可能になります。  

参照先→ `C:\Program Files (x86)\Steam\steamapps\workshop\content\1256670\3689874580\Assemblies\HermesAssemblies\LimbufOfHermes.dll`

大半の機能は`using LimbufOfHermes;`とすることで使用可能になります。

### Caduceus

.NET Compilerが使用するAnalyzerです。  
**Assemblies下に入れないでください**  

DeviceOfHermesの一部機能はこのAnalyzerを前提としています。  
.csprojファイルに以下のセクションを追加してください。  

```xml
  <!-- DeviceOfHermes.Caduceus -->
  <ItemGroup>
    <!-- Steamのインストール先を変更している場合は同じように変更してください -->
    <Analyzer Include="C:\Program Files (x86)\Steam\steamapps\workshop\content\1256670\3689874580\Caduceus.dll" />
  </ItemGroup>
```

## Contact

管理者: つんどら

Discord: TundraClimate/つんどら
よくいるサーバー: [【Project Moon】LC/LoR/Limbus 外郭サーバー](https://discord.gg/KpceJAuhdt)
