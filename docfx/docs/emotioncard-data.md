# EmotionCardXml

XMLファイルを配置することで幻想体ページ設定を追加することが出来ます。  
条件は`DeviceOfHermes`のサブスクライブのみです。  

[HermesConfig](hermes-config.md)に従って読み込みます。  
フォルダ・ファイルが存在しない場合は無視します。

`EmotionCard.xml`
```xml
<EmotionCardXmlRoot>

    <!-- 幻想体ページそのもの -->
    <EmotionCard ID="0">

        <!-- IDを指定する -->
        <Name>MY_ABNORMALITY</Name>

        <!-- 幻想体ページの色 -->
        <!-- Positive or Negative -->
        <State>Positive</State>

        <!-- 帰属する階 -->
        <!-- ETCであるならMOD固有として追加 -->
        <Sephirah>ETC</Sephirah>

        <!-- 獲得可能になる階のレベル -->
        <Level>1</Level>

        <!-- 選択可能になる感情レベル -->
        <EmotionLevel>1</EmotionLevel>

        <!-- 出現するP/N比 -->
        <EmotionRate>0</EmotionRate>

        <!-- 付与対象 -->
        <TargetType>SelectOne</TargetType>

        <!-- 紐づくEmotionCardAbility -->
        <Script>Script</Script>
    </EmotionCard>

</EmotionCardXmlRoot>
```

`AbnoCard.xml`
```xml
<AbnormalityCardsRoot>
  <!-- SephirahTypeは関係ない -->
  <Sephirah SephirahType="ETC">

    <!-- EmotionCard.xmlで指定したID -->
    <AbnormalityCard ID="MY_ABNORMALITY">

      <Abnormality>幻想体名</Abnormality>
      <CardName>幻想体ページ名</CardName>
      <AbilityDesc>効果説明文</AbilityDesc>
      <FlaborText>フレーバーテキスト</FlaborText>
      <Dialogues>
        <Dialogue ID="1">付与時のセリフ1</Dialogue>
        <Dialogue ID="2">付与時のセリフ2</Dialogue>
      </Dialogues>
    </AbnormalityCard>

  </Sephirah>
</AbnormalityCardsRoot>
```
