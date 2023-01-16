using System.IO;
using static System.IO.Path;
using System.Reflection;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infrastructure.Files
{
    public class FileUtils
    {
        /// <summary>
        /// Clona um diretório recursivamente
        /// </summary>
        /// <param name="root">diretório de origem</param>
        /// <param name="dest">diretório de destino</param>
        public static void CloneDirectory(string root, string dest)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {
                var newDirectory = Combine(dest, GetFileName(directory));
                Directory.CreateDirectory(newDirectory);
                CloneDirectory(directory, newDirectory);
            }

            foreach (var file in Directory.GetFiles(root))
                File.Copy(file, Combine(dest, GetFileName(file)));

        }

        /// <summary>
        /// Remove um diretório recursivamente
        /// </summary>
        /// <param name="path">caminho do diretório</param>
        public static void RemoveDirectory(string path)
        {
            var di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
                file.Delete();

            foreach (DirectoryInfo dir in di.GetDirectories())
                dir.Delete(true);

            if (Directory.Exists(path))
                Directory.Delete(path);
        }

        /// <summary>
        /// Retorna um caminho completo baseado no diretório raíz da aplicação
        /// </summary>
        /// <param name="paths">sub-diretórios</param>
        public static string GetFullPath(params string[] paths) =>
            Combine(new string[] { GetDirectoryName(Assembly.GetExecutingAssembly().Location) }.Concat(paths).ToArray());
        
        /// <summary>
        /// Gera N cópias de um arquivo de acordo com a numeração do arquivo
        /// </summary>
        public static void ReplicateFiles(string filePath, int numberOfCopies)
        {
            // Obtém a numeração atual do arquivo
            var resultString = Regex.Match(GetFileName(filePath), @"\d+").Value;
            var leftZeroesQty = resultString.Length;
            if (resultString != null)
            {
                numberOfCopies += int.Parse(resultString);

                var nextVal = int.Parse(resultString) + 1;
                for (int i = nextVal; i < numberOfCopies; i++)
                {
                    var targetFile = filePath.Replace(resultString, i.ToString().PadLeft(leftZeroesQty, '0'));
                    File.Copy(filePath, targetFile);
                }
            }

            Console.WriteLine("Arquivos copiados.");
        }
    }
}
