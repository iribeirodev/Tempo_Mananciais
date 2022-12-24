using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Enums;

namespace DataImporter.Services
{
    public abstract class SupplierServiceBase<T>
    {
        public abstract Task<T> GetData();

        protected T GetMockedData(EnumSupplierType supplierType)
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mockPath = Path.Combine(currentDir, "Mock");

            string content = string.Empty;

            switch (supplierType)
            {
                case EnumSupplierType.ForecastSupplier:
                    content = File.ReadAllText(Path.Combine(mockPath, "forecast.json"), Encoding.UTF8);
                    break;
                case EnumSupplierType.ReservoirSupplier:
                    content = File.ReadAllText(Path.Combine(mockPath, "reservoir.json"), Encoding.UTF8);
                    break;
            }

            return JsonSerializer.Deserialize<T>(content);
        }
    }
}
