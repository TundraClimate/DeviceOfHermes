namespace DeviceOfHermes;

public static class StyleExtension
{
    private static string EmbedColor(string self, string color) => $"<color={color}>{self}</color>";

    public static string Aqua(this string self) => EmbedColor(self, "aqua");
    public static string Black(this string self) => EmbedColor(self, "black");
    public static string Blue(this string self) => EmbedColor(self, "blue");
    public static string Brown(this string self) => EmbedColor(self, "brown");
    public static string Cyan(this string self) => EmbedColor(self, "cyan");
    public static string DarkBlue(this string self) => EmbedColor(self, "darkblue");
    public static string Fuchsia(this string self) => EmbedColor(self, "fuchsia");
    public static string Green(this string self) => EmbedColor(self, "green");
    public static string Grey(this string self) => EmbedColor(self, "grey");
    public static string LightBlue(this string self) => EmbedColor(self, "lightblue");
    public static string Lime(this string self) => EmbedColor(self, "Lime");
    public static string Magenta(this string self) => EmbedColor(self, "magenta");
    public static string Maroon(this string self) => EmbedColor(self, "maroon");
    public static string Navy(this string self) => EmbedColor(self, "navy");
    public static string Olive(this string self) => EmbedColor(self, "olive");
    public static string Orange(this string self) => EmbedColor(self, "orange");
    public static string Purple(this string self) => EmbedColor(self, "purple");
    public static string Red(this string self) => EmbedColor(self, "red");
    public static string Silver(this string self) => EmbedColor(self, "silver");
    public static string Teal(this string self) => EmbedColor(self, "teal");
    public static string White(this string self) => EmbedColor(self, "white");
    public static string Yellow(this string self) => EmbedColor(self, "yellow");
    public static string Rgb(this string self, int r, int g, int b) => EmbedColor(self, $"#{r}{g}{b}");
    public static string Rgba(this string self, int r, int g, int b, int a) => EmbedColor(self, $"#{r}{g}{b}{a}");
    public static string Hex(this string self, string hex) => EmbedColor(self, hex);

    public static string Bold(this string self) => $"<b>{self}</b>";
    public static string Italic(this string self) => $"<i>{self}</i>";
    public static string Underline(this string self) => $"<u>{self}</u>";
    public static string Strikethrough(this string self) => $"<s>{self}</s>";
    public static string Alpha(this string self, string alpha) => $"<alpha={alpha}>{self}</alpha>";
    public static string SizeAbs(this string self, int size) => $"<size={size}>{self}</size>";
    public static string SizeRel(this string self, int size) => $"<size={(size >= 0 ? "+" : "-")}{size}>{self}</size>";
    public static string Lower(this string self) => $"<lowercase>{self}</lowercase>";
    public static string Upper(this string self) => $"<uppercase>{self}</uppercase>";
    public static string Smallcaps(this string self) => $"<smallcaps>{self}</smallcaps>";
    public static string Mark(this string self, string hex) => $"<mark={hex}>{self}</mark>";
    public static string Mark(this string self, int r, int g, int b) => $"<mark=#{r}{g}{b}>{self}</mark>";
    public static string Mark(this string self, int r, int g, int b, int a) => $"<mark=#{r}{g}{b}{a}>{self}</mark>";
    public static string LineHeight(this string self, string height) => $"<line-height={height}%>{self}</line-height>";
    public static string Sup(this string self) => $"<sup>{self}</sup>";
    public static string Sub(this string self) => $"<sub>{self}</sub>";
    public static string Font(this string self, string assetName) => $"<font=\"{assetName}\">{self}</font>";
    public static string Gradient(this string self, string gradient) => $"<gradient=\"{gradient}\">{self}</gradient>";
    public static string Cspace(this string self, string spacing) => $"<cspace={spacing}>{self}</cspace>";
}
