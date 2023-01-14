using System.Drawing;

namespace Infrastructure.Graphics
{
    /// <summary>
    /// Tratamento de pincéis de cores pré-definidos
    /// </summary>
    public class CustomBrushes
    {
        public SolidBrush HeaderColor { get => new SolidBrush(Color.FromArgb(255, 255, 255)); }
        public SolidBrush InfoColor {  get => new SolidBrush(Color.FromArgb(12, 12, 12)); }
        public SolidBrush MinColor { get => new SolidBrush(Color.FromArgb(128, 0, 0)); }
        public SolidBrush MaxColor { get => new SolidBrush(Color.FromArgb(0, 0, 128)); }
    }
}
