using System.IO;

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
                var newDirectory = Path.Combine(dest, Path.GetFileName(directory));
                Directory.CreateDirectory(newDirectory);
                CloneDirectory(directory, newDirectory);
            }

            foreach (var file in Directory.GetFiles(root))
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));

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

        }
    }
}
