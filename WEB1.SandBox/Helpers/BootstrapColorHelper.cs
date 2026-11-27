namespace WEB1.SandBox.Helpers;

/// <summary>
/// Helper para la distribución secuencial y dinámica de colores Bootstrap en componentes e interfaces.
/// </summary>
public static class BootstrapColorHelper
{
    /// <summary>
    /// Paleta de 6 colores (Rojo, Naranja, Amarillo, Verde, Azul, Violeta) para distribuir cíclicamente.
    /// </summary>
    private static readonly string[] DefaultPalette = new[]
    {
        "danger",    // Rojo (Bootstrap nativo)
        "orange",    // Naranja (Extendida con soporte sutil)
        "warning",   // Amarillo (Bootstrap nativo)
        "success",   // Verde (Bootstrap nativo)
        "primary",   // Azul (Bootstrap nativo)
        "purple"     // Violeta (Extendida con soporte sutil)
    };

    /// <summary>
    /// Obtiene el nombre del color Bootstrap asignado al índice según la secuencia de la paleta.
    /// Si se añaden más tarjetas, la secuencia se repite cíclicamente (0, 1, 2, 3, 0, 1...).
    /// </summary>
    public static string GetColor(int index, string[]? customPalette = null)
    {
        var palette = (customPalette != null && customPalette.Length > 0) ? customPalette : DefaultPalette;
        var safeIndex = Math.Abs(index) % palette.Length;
        return palette[safeIndex];
    }

    /// <summary>
    /// Retorna un objeto con las clases utilitarias de Bootstrap para fondo sutil, texto y tema.
    /// </summary>
    public static BootstrapThemeInfo GetTheme(int index, string[]? customPalette = null)
    {
        var color = GetColor(index, customPalette);
        return new BootstrapThemeInfo(color);
    }

    /// <summary>
    /// Retorna una copia de la paleta con las posiciones de los colores barajadas aleatoriamente.
    /// </summary>
    public static string[] GetShuffledPalette()
    {
        var shuffled = (string[])DefaultPalette.Clone();
        Random.Shared.Shuffle(shuffled);
        return shuffled;
    }

    public static IReadOnlyList<string> Palette => DefaultPalette;
}

/// <summary>
/// Clases de utilidad Bootstrap generadas dinámicamente sin colores hardcodeados.
/// </summary>
public record BootstrapThemeInfo(string ColorName)
{
    public string TextClass => $"text-{ColorName}";
    public string BgSubtleClass => $"bg-{ColorName}-subtle";
    public string BorderSubtleClass => $"border-{ColorName}-subtle";
    public string CardThemeClass => $"theme-{ColorName}";
}
