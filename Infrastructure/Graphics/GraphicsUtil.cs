using System.Drawing;

namespace Infrastructure.Graphics
{
    public class GraphicsUtil
    {
        /// <summary>
        /// Imprime um texto ao centro de uma determinada área do canvas
        /// </summary>
        public static void DrawCenteredString(
            System.Drawing.Graphics graphics,
            string text,
            Font textFont,
            Brush brush,
            int col,
            int lin,
            int height,
            int width)
        {
            Rectangle rect1 = new Rectangle(col, lin, width, height);
            var sf = new StringFormat { Alignment = StringAlignment.Center };
            graphics.DrawString(text, textFont, brush, rect1, sf);
        }
    }
}
