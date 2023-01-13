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
using System.Collections.Generic;

namespace MediaProcessing.UseCases
{
    public class GenerateMediaUseCase
    {
        private readonly string _workDir; 
        private readonly string _imagesDir; 
        private readonly string _fontsDir; 
        private readonly string _trackDir; 
        private readonly Dictionary<string, Font> _fonts;
        private readonly Dictionary<string, Brush> _brushes;

        public GenerateMediaUseCase()
        {
            _workDir = FileUtils.GetFullPath("Work");
            _imagesDir = FileUtils.GetFullPath("Assets", "Images");
            _fontsDir = FileUtils.GetFullPath("Assets", "Fonts");
            _trackDir = FileUtils.GetFullPath("Track");

            _fonts.Add("titleFont", GraphicsUtil.GetFont("Open Sans", 52, FontStyle.Regular));
            _fonts.Add("subTitleFont", GraphicsUtil.GetFont("Open Sans", 24, FontStyle.Regular));
            _fonts.Add("temperatureFont", GraphicsUtil.GetFont("Open Sans", 112, FontStyle.Bold));
            _fonts.Add("descriptionFont", GraphicsUtil.GetFont("Open Sans", 34, FontStyle.Regular));
            _fonts.Add("descriptionFont2", GraphicsUtil.GetFont("Open Sans", 42, FontStyle.Regular));
            _fonts.Add("footerFont", GraphicsUtil.GetFont("Open Sans", 38, FontStyle.Regular));
            _fonts.Add("footerFont2", GraphicsUtil.GetFont("Open Sans", 34, FontStyle.Regular));

            _brushes.Add("headerColor", Brushes.White);
            _brushes.Add("infoColor", new SolidBrush(Color.FromArgb(12, 12, 12)));
            _brushes.Add("minColor", new SolidBrush(Color.FromArgb(128, 0, 0)));
            _brushes.Add("maxColor", new SolidBrush(Color.FromArgb(0, 0, 128)));
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
                GraphicsUtil.LoadFonts(_fontsDir);

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
                        graphics.DrawString("Tempo e Mananciais", _fonts["titleFont"], headerColor, new PointF(10f, 15f));
                        graphics.DrawString("P R E V I S Ã O  D O  T E M P O", _fonts["subTitleFont"], headerColor, new PointF(140f, 110f));

                        graphics.DrawString("Mínima", _fonts["descriptionFont"], infoColor, new PointF(275f, 290f));
                        graphics.DrawString($"{forecast.Temp_Min}", _fonts["temperatureFont"], minColor, new PointF(250f, 330f));

                        graphics.DrawString("Máxima", _fonts["descriptionFont"], infoColor, new PointF(610f, 290f));
                        graphics.DrawString($"{forecast.Temp_Max}", _fonts["temperatureFont"], maxColor, new PointF(590f, 330f));

                        DrawCenteredString(graphics, $"{forecast.TendenciaTemperatura}", _fonts["descriptionFont2"], infoColor, 90, 520, 100, 890);
                        DrawCenteredString(graphics, $"Umidade Mínima: {forecast.Umidade} %", _fonts["descriptionFont2"], infoColor, 90, 590, 100, 890);

                        var solInfo = $"O sol nasceu às {forecast.Nascer} e vai se pôr às {forecast.Ocaso}.  Estação do Ano: {forecast.Estacao}";
                        graphics.DrawString(solInfo, _fonts["footerFont"], headerColor, new PointF(20, 930));
                    }
                    image.Save(saveFile, ImageFormat.Png);
                }
                
                for (int i = 2; i <= framesPerInfo; i++)
                {
                    var targetFile = Combine(_workDir, $"pic{i.ToString().PadLeft(4, '0')}.png");
                    File.Copy(saveFile, targetFile);
                }

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

                using (var image = new Bitmap(imageFile))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        setupGraphics(graphics);

                        // Header
                        graphics.DrawString("Tempo e Mananciais", _fonts["titleFont"], _brushes["headerColor"], new PointF(10f, 15f));
                        graphics.DrawString("N Í V E L  D O S  R E S E R V A T Ó R I O S", _fonts["subTitleFont"], _brushes["headerColor"], new PointF(70f, 110f));

                        Pen pen = new Pen(Color.Black, 2);
                        pen.Alignment = PenAlignment.Inset;

                        var initialRow = 210;
                        reservoir.Lagos.ForEach(lago =>
                        {
                            graphics.DrawString(lago.Nome, _fonts["titleFont"], _brushes["infoColor"], new PointF(100, initialRow));

                            CreateLevelBar(graphics, 700, initialRow + 20, lago.VolumePorcentagem, _fonts["titleFont"]);

                            initialRow += 90;
                        });

                        var info = "Informações disponíveis através do Instituto de Meteorologia - INMET e SABESP.";
                        graphics.DrawString(info, _fonts["footerFont2"], _brushes["headerColor"], new PointF(20, 940));
                    }
                    image.Save(saveFile, ImageFormat.Png);
                }

                for (int i = 17; i <= framesPerInfo * 2; i++)
                {
                    var targetFile = Combine(_workDir, $"pic{i.ToString().PadLeft(4, '0')}.png");
                    File.Copy(saveFile, targetFile);
                }
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
