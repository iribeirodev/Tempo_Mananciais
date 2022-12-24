using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain.Requests;
using Infrastructure.Files;
using Infrastructure.Response;

namespace MediaProcessing.UseCases
{
    public class GenerateMediaUseCase
    {
        private readonly string _workDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Work");
        private readonly string _imagesDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "Images");
        private readonly string _fontsDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "Fonts");
        private readonly string _trackDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Track");
        private PrivateFontCollection fontCollection = new PrivateFontCollection();
        private readonly int FRAMES_PER_INFO = 15;

        /// <summary>
        /// Remove os recursos do diretório de trabalho 
        /// </summary>
        /// <returns></returns>
        private Task ClearAllResources()
        {
            return Task.Run(() =>
            {
                File.Delete(Path.Combine(_workDir, "track.mp3"));
                File.Delete(Path.Combine(_workDir, "forecast-temp.jpg"));
                File.Delete(Path.Combine(_workDir, "reservoir-temp.jpg"));
                File.Delete(Path.Combine(_workDir, "output.mp4"));

                var pngs = Directory.GetFiles(Path.Combine(_workDir)).Where(x => x.EndsWith("png")).ToList();
                foreach(var png in pngs)
                    File.Delete(png);

                var ffmpegWorkingDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "work", "ffmpeg");
                FileUtils.RemoveDirectory(ffmpegWorkingDir);

                Console.WriteLine("Cleared");
            });
        }

        /// <summary>
        /// Copia os recursos para o diretório de trabalho
        /// </summary>
        /// <returns></returns>
        private Task CopyResources()
        {
            return Task.Run(() =>
            {
                // Track
                File.Copy(Path.Combine(_trackDir, "track.mp3"),
                            Path.Combine(_workDir, "track.mp3"));

                // Imagens
                File.Copy(Path.Combine(_imagesDir, "forecast-background.jpg"),
                            Path.Combine(_workDir, "forecast-temp.jpg"));
                File.Copy(Path.Combine(_imagesDir, "reservoir-background.jpg"),
                            Path.Combine(_workDir, "reservoir-temp.jpg"));

                // Utilitário FFMPEG
                var ffmpegDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ffmpeg");

                FileUtils.CloneDirectory(ffmpegDir, Path.Combine(_workDir, "ffmpeg"));

            });
        }

        /// <summary>
        /// Carrega os arquivos de fontes true-type
        /// </summary>
        private void LoadFonts()
        {
            var files = Directory.GetFiles(_fontsDir);

            foreach (var file in files)
                fontCollection.AddFontFile(file);
        }

        /// <summary>
        /// Obtém uma fonte com determinado tamanho e estilo.
        /// </summary>
        private Font GetFont(string fontName, int fontSize, FontStyle fontStyle) => 
                new Font(fontCollection.Families.Where(f => f.Name == fontName).FirstOrDefault(), fontSize, fontStyle);

        /// <summary>
        /// Imprime um texto ao centro de uma determinada área do canvas
        /// </summary>
        private void DrawCenteredString(
            Graphics graphics, 
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

        /// <summary>
        /// Preenche as imagens base de reservatório e forecast com as informações recebidas.
        /// </summary>
        /// <returns></returns>
        private Task WriteData(ProcessDataRequest processDataDTO)
        {
            return Task.Run(() =>
            {
                LoadFonts();

                var forecast = processDataDTO.ForecastInfo;
                var reservoir = processDataDTO.ReservoirInfo;

                // FORECAST - Inicio
                var imageFile = Path.Combine(_workDir, "forecast-temp.jpg");
                var saveFile = Path.Combine(_workDir, "pic0001.png");

                var titleFont = GetFont("Open Sans", 52, FontStyle.Regular);
                var subTitleFont = GetFont("Open Sans", 24, FontStyle.Regular);
                var temperatureFont = new Font("Open Sans", 112, FontStyle.Bold); 
                var descriptionFont = GetFont("Open Sans", 34, FontStyle.Regular);
                var descriptionFont2 = GetFont("Open Sans", 42, FontStyle.Regular);
                var footerFont = GetFont("Open Sans", 38, FontStyle.Regular);
                var footerFont2 = GetFont("Open Sans", 34, FontStyle.Regular);

                var headerColor = Brushes.White;
                var infoColor = new SolidBrush(Color.FromArgb(12, 12, 12));
                var minColor = new SolidBrush(Color.FromArgb(128, 0, 0));
                var maxColor = new SolidBrush(Color.FromArgb(0, 0, 128));

                using (var image = new Bitmap(imageFile))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                        // Header
                        graphics.DrawString("Tempo e Mananciais", titleFont, headerColor, new PointF(10f, 15f));
                        graphics.DrawString("P R E V I S Ã O  D O  T E M P O", subTitleFont, headerColor, new PointF(140f, 110f));

                        graphics.DrawString("Mínima", descriptionFont, infoColor, new PointF(275f, 290f));
                        graphics.DrawString($"{forecast.Temp_Min}", temperatureFont, minColor, new PointF(250f, 330f));

                        graphics.DrawString("Máxima", descriptionFont, infoColor, new PointF(610f, 290f));
                        graphics.DrawString($"{forecast.Temp_Max}", temperatureFont, maxColor, new PointF(590f, 330f));

                        DrawCenteredString(graphics, $"{forecast.TendenciaTemperatura}", descriptionFont2, infoColor, 90, 520, 100, 890);
                        DrawCenteredString(graphics, $"Umidade Mínima: {forecast.Umidade} %", descriptionFont2, infoColor, 90, 590, 100, 890);

                        var solInfo = $"O sol nasceu às {forecast.Nascer} e vai se pôr às {forecast.Ocaso}.  Estação do Ano: {forecast.Estacao}";
                        graphics.DrawString(solInfo, footerFont, headerColor, new PointF(20, 930));
                    }
                    image.Save(saveFile, ImageFormat.Png);
                }

                for (int i = 2; i <= FRAMES_PER_INFO; i++)
                {
                    var targetFile = Path.Combine(_workDir, $"pic{i.ToString().PadLeft(4, '0')}.png");
                    File.Copy(saveFile, targetFile);
                }

                // FORECAST - Término

                // RESERVATÓRIOS - Início
                imageFile = Path.Combine(_workDir, "reservoir-temp.jpg");
                saveFile = Path.Combine(_workDir, "pic0016.png");

                using (var image = new Bitmap(imageFile))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                        // Header
                        graphics.DrawString("Tempo e Mananciais", titleFont, headerColor, new PointF(10f, 15f));
                        graphics.DrawString("N Í V E L  D O S  R E S E R V A T Ó R I O S", subTitleFont, headerColor, new PointF(70f, 110f));


                        Pen pen = new Pen(Color.Black, 2);
                        pen.Alignment = PenAlignment.Inset;

                        var initialRow = 210;
                        reservoir.Lagos.ForEach(lago =>
                        {
                            graphics.DrawString(lago.Nome, titleFont, infoColor, new PointF(100, initialRow));

                            CreateLevelBar(graphics, 700, initialRow + 20, lago.VolumePorcentagem, titleFont);

                            initialRow += 90;
                        });

                        var info = "Informações disponíveis através do Instituto de Meteorologia - INMET e SABESP.";
                        graphics.DrawString(info, footerFont2, headerColor, new PointF(20, 940));
                    }
                    image.Save(saveFile, ImageFormat.Png);
                }

                for (int i = 17; i <= FRAMES_PER_INFO * 2; i++)
                {
                    var targetFile = Path.Combine(_workDir, $"pic{i.ToString().PadLeft(4, '0')}.png");
                    File.Copy(saveFile, targetFile);
                }

                // RESERVATÓRIOS - Término
            });
        }

        /// <summary>
        /// Cria uma barra de tarefas fixa
        /// </summary>
        private void CreateLevelBar(
            Graphics graphics, 
            int col, 
            int row, 
            string percentValue,
            Font font)
        {

            int width = 800;
            int height = 60;

            var percentage = float.Parse(percentValue.Replace(",", "."), new System.Globalization.CultureInfo("en-US"));

            var fillColor = Color.White;
            if (percentage >= 0 && percentage <= 20)
                fillColor = Color.FromArgb(200, 250, 94, 94);
            else if (percentage > 20 && percentage <= 40)
                fillColor = Color.FromArgb(200, 227, 161, 89);
            else if (percentage > 40 && percentage <= 60)
                fillColor = Color.FromArgb(200, 250, 236, 110);
            else if (percentage > 60 && percentage <= 80)
                fillColor = Color.FromArgb(200, 77, 230, 60);
            else
                fillColor = Color.FromArgb(200, 110, 255, 245);

            width = Convert.ToInt32((width * percentage) / 100);

            Pen pen = new Pen(Color.Black, 1) { Alignment = PenAlignment.Inset };
            graphics.DrawRectangle(pen, new Rectangle(col, row, width, height));

            var rect1 = new Rectangle(col + 2, row + 2, width - 4, height - 4);
            graphics.FillRectangle(new SolidBrush(fillColor), rect1);

            graphics.DrawString($"{percentValue} %", font, Brushes.Black, new PointF(col + width + 20, row - 20));
        }

        /// <summary>
        /// Executa a batch de geração do arquivo de vídeo
        /// </summary>
        /// <returns></returns>
        private Task GenerateMedia()
        {
            // TESTAR
            return Task.Run(() =>
            {
                var process = new System.Diagnostics.Process();
                var startInfo = new System.Diagnostics.ProcessStartInfo();

                startInfo.WorkingDirectory = _workDir;
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = @"/C ffmpeg\bin\ffmpeg.exe -r 1 -i pic%04d.png -i track.mp3 -pix_fmt yuv420p output.mp4";

                process.StartInfo = startInfo;

                process.Start();
                process.WaitForExit();
            });
        }

        public async Task<ProcessResponse> Process(ProcessDataRequest processDataRequest)
        {
            try
            {
                await ClearAllResources();
                await CopyResources();
                await WriteData(processDataRequest);
                await GenerateMedia();

                return new ProcessResponse
                {
                    Message = "Processamento de imagens concluído."
                };
            }
            catch (Exception exc)
            {
                return new ProcessResponse
                {
                    IsSuccessFull = false,
                    Message = exc.Message
                };
            }
        }
    }
}
