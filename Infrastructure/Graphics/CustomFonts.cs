using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;

namespace Infrastructure.Graphics
{
    /// <summary>
    /// Tratamento de fontes pré-definidas
    /// </summary>
    public class CustomFonts
    {
        private readonly PrivateFontCollection _fontCollection;
        private readonly string _fontDirectory;

        public Font TitleFont { get => CustomFont("Open Sans", 52, FontStyle.Regular); }
        public Font SubTitleFont { get => CustomFont("Open Sans", 24, FontStyle.Regular); }
        public Font TemperatureFont { get => CustomFont("Open Sans", 112, FontStyle.Bold); }
        public Font DescriptionFont { get => CustomFont("Open Sans", 34, FontStyle.Regular); }
        public Font DescriptionFontII { get => CustomFont("Open Sans", 42, FontStyle.Regular); }
        public Font FooterFont { get => CustomFont("Open Sans", 38, FontStyle.Regular); }
        public Font FooterFontII { get => CustomFont("Open Sans", 34, FontStyle.Regular); }

        public CustomFonts(string fontDirectory)
        {
            _fontDirectory = fontDirectory;
            _fontCollection = new PrivateFontCollection();

            var files = Directory.GetFiles(_fontDirectory);
            foreach (var file in files)
                _fontCollection.AddFontFile(file);

        }

        private Font CustomFont(string fontName, int fontSize, FontStyle fontStyle) =>
                new Font(_fontCollection.Families
                                    .Where(f => f.Name == fontName)
                                    .FirstOrDefault(), fontSize, fontStyle);
    }
}
