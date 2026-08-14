namespace DeviceOfHermes;

/// <summary>Styled text methods</summary>
public static class StyleExtension
{
    private static string EmbedColor(string self, string color) => $"<color={color}>{self}</color>";

    /// <summary>Into aqua embeds string</summary>
    public static string Aqua(this string self) => EmbedColor(self, "aqua");
    /// <summary>Into black embeds string</summary>
    public static string Black(this string self) => EmbedColor(self, "black");
    /// <summary>Into blue embeds string</summary>
    public static string Blue(this string self) => EmbedColor(self, "blue");
    /// <summary>Into brown embeds string</summary>
    public static string Brown(this string self) => EmbedColor(self, "brown");
    /// <summary>Into cyan embeds string</summary>
    public static string Cyan(this string self) => EmbedColor(self, "cyan");
    /// <summary>Into darkblue embeds string</summary>
    public static string DarkBlue(this string self) => EmbedColor(self, "darkblue");
    /// <summary>Into fuchsia embeds string</summary>
    public static string Fuchsia(this string self) => EmbedColor(self, "fuchsia");
    /// <summary>Into green embeds string</summary>
    public static string Green(this string self) => EmbedColor(self, "green");
    /// <summary>Into gray embeds string</summary>
    public static string Grey(this string self) => EmbedColor(self, "grey");
    /// <summary>Into lightblue embeds string</summary>
    public static string LightBlue(this string self) => EmbedColor(self, "lightblue");
    /// <summary>Into lime embeds string</summary>
    public static string Lime(this string self) => EmbedColor(self, "Lime");
    /// <summary>Into magenta embeds string</summary>
    public static string Magenta(this string self) => EmbedColor(self, "magenta");
    /// <summary>Into maroon embeds string</summary>
    public static string Maroon(this string self) => EmbedColor(self, "maroon");
    /// <summary>Into navy embeds string</summary>
    public static string Navy(this string self) => EmbedColor(self, "navy");
    /// <summary>Into olive embeds string</summary>
    public static string Olive(this string self) => EmbedColor(self, "olive");
    /// <summary>Into orange embeds string</summary>
    public static string Orange(this string self) => EmbedColor(self, "orange");
    /// <summary>Into purple embeds string</summary>
    public static string Purple(this string self) => EmbedColor(self, "purple");
    /// <summary>Into red embeds string</summary>
    public static string Red(this string self) => EmbedColor(self, "red");
    /// <summary>Into silver embeds string</summary>
    public static string Silver(this string self) => EmbedColor(self, "silver");
    /// <summary>Into teal embeds string</summary>
    public static string Teal(this string self) => EmbedColor(self, "teal");
    /// <summary>Into white embeds string</summary>
    public static string White(this string self) => EmbedColor(self, "white");
    /// <summary>Into yellow embeds string</summary>
    public static string Yellow(this string self) => EmbedColor(self, "yellow");
    /// <summary>Into Rgb embeds string</summary>
    public static string Rgb(this string self, int r, int g, int b) => EmbedColor(self, $"#{r}{g}{b}");
    /// <summary>Into Rgba embeds string</summary>
    public static string Rgba(this string self, int r, int g, int b, int a) => EmbedColor(self, $"#{r}{g}{b}{a}");
    /// <summary>Into Hex embeds string</summary>
    public static string Hex(this string self, string hex) => EmbedColor(self, hex);

    /// <summary>Into Bold embeds string</summary>
    public static string Bold(this string self) => $"<b>{self}</b>";
    /// <summary>Into Italic embeds string</summary>
    public static string Italic(this string self) => $"<i>{self}</i>";
    /// <summary>Into Underline embeds string</summary>
    public static string Underline(this string self) => $"<u>{self}</u>";
    /// <summary>Into Strikethrough embeds string</summary>
    public static string Strikethrough(this string self) => $"<s>{self}</s>";
    /// <summary>Into Alpha embeds string</summary>
    public static string Alpha(this string self, string alpha) => $"<alpha={alpha}>{self}</alpha>";
    /// <summary>Into SizeAbs embeds string</summary>
    public static string SizeAbs(this string self, int size) => $"<size={size}>{self}</size>";
    /// <summary>Into SizeRel embeds string</summary>
    public static string SizeRel(this string self, int size) => $"<size={(size >= 0 ? "+" : "-")}{size}>{self}</size>";
    /// <summary>Into Lower embeds string</summary>
    public static string Lower(this string self) => $"<lowercase>{self}</lowercase>";
    /// <summary>Into Upper embeds string</summary>
    public static string Upper(this string self) => $"<uppercase>{self}</uppercase>";
    /// <summary>Into Smallcaps embeds string</summary>
    public static string Smallcaps(this string self) => $"<smallcaps>{self}</smallcaps>";
    /// <summary>Into Mark embeds string</summary>
    public static string Mark(this string self, string hex) => $"<mark={hex}>{self}</mark>";
    /// <summary>Into Mark embeds string</summary>
    public static string Mark(this string self, int r, int g, int b) => $"<mark=#{r}{g}{b}>{self}</mark>";
    /// <summary>Into Mark embeds string</summary>
    public static string Mark(this string self, int r, int g, int b, int a) => $"<mark=#{r}{g}{b}{a}>{self}</mark>";
    /// <summary>Into LineHeight embeds string</summary>
    public static string LineHeight(this string self, string height) => $"<line-height={height}%>{self}</line-height>";
    /// <summary>Into Sup embeds string</summary>
    public static string Sup(this string self) => $"<sup>{self}</sup>";
    /// <summary>Into Sub embeds string</summary>
    public static string Sub(this string self) => $"<sub>{self}</sub>";
    /// <summary>Into Font embeds string</summary>
    public static string Font(this string self, string assetName) => $"<font=\"{assetName}\">{self}</font>";
    /// <summary>Into Gradient embeds string</summary>
    public static string Gradient(this string self, string gradient) => $"<gradient=\"{gradient}\">{self}</gradient>";
    /// <summary>Into Cspace embeds string</summary>
    public static string Cspace(this string self, string spacing) => $"<cspace={spacing}>{self}</cspace>";
}
