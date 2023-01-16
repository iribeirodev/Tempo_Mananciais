using System;
using System.IO;
using static System.IO.Path;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using Domain.Forecast;
using Domain.Requests;
using Domain.Reservoir;
using Infrastructure.Files;
using Infrastructure.Response;
using Infrastructure.Graphics;

namespace MediaProcessing.UseCases
{
    public class GenerateMediaUseCase
    {
        private readonly string _workDir; 
        private readonly string _imagesDir; 
        private readonly string _trackDir;
        private readonly CustomFonts customFonts;
        private readonly CustomBrushes customBrushes;

        public GenerateMediaUseCase(CustomFonts customFonts, CustomBrushes customBrushes)
        {
            _workDir = FileUtils.GetFullPath("Work");
            _imagesDir = FileUtils.GetFullPath("Assets", "Images");
            _trackDir = FileUtils.GetFullPath("Track");

            this.customFonts = customFonts;
            this.customBrushes = customBrushes;
        }

        public async Task<ProcessResponse> Process(ProcessDataRequest processDataRequest)
        {
            try
            {
                await ClearAllResources();
                await CopyResources();
                await WriteForecastData(processDataRequest.ForecastInfo);
                await WriteReservoirData(processDataRequest.ReservoirInfo);
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

        /// <summary>
        /// Remove os recursos do diretório de trabalho 
        /// </summary>
        /// <returns></returns>
        private Task ClearAllResources()
        {
            return Task.Run(() =>
            {
                File.Delete(Combine(_workDir, "track.mp3"));
                File.Delete(Combine(_workDir, "forecast-temp.png"));
                File.Delete(Combine(_workDir, "reservoir-temp.png"));
                File.Delete(Combine(_workDir, "output.mp4"));

                var pngs = Directory.GetFiles(Combine(_workDir)).Where(x => x.EndsWith("png")).ToList();
                foreach(var png in pngs)
                    File.Delete(png);

                var ffmpegWorkingDir = FileUtils.GetFullPath("work", "ffmpeg");
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
                File.Copy(Combine(_trackDir, "track.mp3"), Combine(_workDir, "track.mp3"));

                // Imagens
                File.Copy(Combine(_imagesDir, "forecast-background.png"), Combine(_workDir, "forecast-temp.png"));
                File.Copy(Combine(_imagesDir, "reservoir-background.png"), Combine(_workDir, "reservoir-temp.png"));

                // Utilitário FFMPEG
                var ffmpegDir = FileUtils.GetFullPath("ffmpeg");

                FileUtils.CloneDirectory(ffmpegDir, FileUtils.GetFullPath("work", "ffmpeg"));
            });
        }

        private void setupGraphics(Graphics graphics)
        {
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

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

        private Task WriteForecastData(ForecastDTO forecast)
        {
            return Task.Run(() =>
            {
                var imageFile = Combine(_workDir, "forecast-temp.png");
                var saveFile = Combine(_workDir, "pic0001.png");
                var framesPerInfo = int.Parse(Environment.GetEnvironmentVariable("FRAMES_PER_INFO"));

                var headerColor = Brushes.White;
                var infoColor = new SolidBrush(Color.FromArgb(12, 12, 12));
                var minColor = new SolidBrush(Color.FromArgb(128, 0, 0));
                var maxColor = new SolidBrush(Color.FromArgb(0, 0, 128));

                using (var image = new Bitmap(imageFile))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        setupGraphics(graphics);

                        // Header
                        graphics.DrawString("Tempo & Mananciais", customFonts.TitleFont, headerColor, new PointF(10f, 15f));
                        graphics.DrawString("P R E V I S Ã O  D O  T E M P O", customFonts.SubTitleFont, headerColor, new PointF(140f, 110f));

                        graphics.DrawString("Mínima", customFonts.DescriptionFont , infoColor, new PointF(275f, 290f));
                        graphics.DrawString($"{forecast.Temp_Min}", customFonts.TemperatureFont, minColor, new PointF(250f, 330f));

                        graphics.DrawString("Máxima", customFonts.DescriptionFont, infoColor, new PointF(610f, 290f));
                        graphics.DrawString($"{forecast.Temp_Max}", customFonts.TemperatureFont, maxColor, new PointF(590f, 330f));

                        DrawCenteredString(graphics, $"{forecast.TendenciaTemperatura}", customFonts.DescriptionFontII, infoColor, 90, 520, 100, 890);
                        DrawCenteredString(graphics, $"Umidade Mínima: {forecast.Umidade} %", customFonts.DescriptionFontII, infoColor, 90, 590, 100, 890);

                        var solInfo = $"O sol nasceu às {forecast.Nascer} e vai se pôr às {forecast.Ocaso}.  Estação do Ano: {forecast.Estacao}";
                        graphics.DrawString(solInfo, customFonts.FooterFont, headerColor, new PointF(20, 930));
                    }
                    image.Save(saveFile, ImageFormat.Png);
                }

                FileUtils.ReplicateFiles(saveFile, 15);
            });
        }

        private Task WriteReservoirData(ReservoirDTO reservoir)
        {
            return Task.Run(() =>
            {
                // RESERVATÓRIOS - Início
                var imageFile = Combine(_workDir, "reservoir-temp.png");
                var saveFile = Combine(_workDir, "pic0016.png");
                var framesPerInfo = int.Parse(Environment.GetEnvironmentVariable("FRAMES_PER_INFO"));

                Bitmap bitmapSource = (Bitmap)Image.FromFile(imageFile);
                Bitmap temporaryBitmap = new Bitmap(bitmapSource.Width, bitmapSource.Height); 

                using (var image = new Bitmap(temporaryBitmap))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        setupGraphics(graphics);

                        graphics.DrawImage(bitmapSource, 0, 0);

                        // Header
                        graphics.DrawString("Tempo & Mananciais", customFonts.TitleFont, customBrushes.HeaderColor, new PointF(10f, 15f));
                        graphics.DrawString("N Í V E L  D O S  R E S E R V A T Ó R I O S", customFonts.SubTitleFont, customBrushes.HeaderColor, new PointF(70f, 110f));

                        Pen pen = new Pen(Color.Black, 2);
                        pen.Alignment = PenAlignment.Inset;

                        var initialRow = 210;
                        reservoir.Lagos.ForEach(lago =>
                        {
                            graphics.DrawString(lago.Nome, customFonts.TitleFont, customBrushes.InfoColor, new PointF(100, initialRow));

                            CreateLevelBar(graphics, 700, initialRow + 20, lago.VolumePorcentagem, customFonts.TitleFont);

                            initialRow += 90;
                        });

                        var info = "Informações disponíveis através do Instituto de Meteorologia - INMET e SABESP.";
                        graphics.DrawString(info, customFonts.FooterFontII, customBrushes.HeaderColor, new PointF(20, 940));
                    }
                    image.Save(saveFile, ImageFormat.Png);
                }

                FileUtils.ReplicateFiles(saveFile, 15);
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
                startInfo.Arguments = @"/C ffmpeg\ffmpeg.exe -r 1 -i pic%04d.png -i track.mp3 -pix_fmt yuv420p output.mp4";

                process.StartInfo = startInfo;

                process.Start();
                process.WaitForExit();
            });
        }
    }
}
