using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace Infrastructure.Graphics
{
    public class GraphicsUtil
    {
        private static PrivateFontCollection _fontCollection;

        public GraphicsUtil(string fontDirectory)
        {
            _fontCollection = new PrivateFontCollection();

            LoadFonts(fontDirectory);
        }

        public static void LoadFonts(string fontDirectory)
        {
            var files = Directory.GetFiles(fontDirectory);

            foreach (var file in files)
                _fontCollection.AddFontFile(file);
        }

        public static Font GetFont(string fontName, int fontSize, FontStyle fontStyle) =>
                new Font(_fontCollection.Families.Where(f => f.Name == fontName).FirstOrDefault(), fontSize, fontStyle);

    }
}
