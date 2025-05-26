using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Data.SqlClient;
using System.Linq;

class Program
{
    static void Main()
    {
        string csvPath = @"C:\Users\BETO\Documents\Proyectos\Oveja Negra\Productos.csv";

        string connectionString = "Server=tcp:servidorbbddbeto.database.windows.net,1433;" +
            "Initial Catalog=BDcomercioON;Persist Security Info=False;" +
            "User ID=betoadmin;Password=Bodoque25;" +
            "MultipleActiveResultSets=False;Encrypt=True;" +
            "TrustServerCertificate=False;Connection Timeout=30;";

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            Delimiter = ","
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, config);
        var productos = csv.GetRecords<ProductoCsv>().ToList();

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        foreach (var p in productos)
        {
            try
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Productos
                    (CodigoBarras, Descripcion, Rubro, PrecioCosto, PorcIVA, PorcUtil, PrecioVenta,
                     Proveedor, Stock, StockMin, StockMax, CtrlStock, Activo)
                    VALUES
                    (@CodigoBarras, @Descripcion, @Rubro, @PrecioCosto, @PorcIVA, @PorcUtil, @PrecioVenta,
                     @Proveedor, @Stock, @StockMin, @StockMax, @CtrlStock, @Activo)", connection);

                cmd.Parameters.AddWithValue("@CodigoBarras", p.CodigoBarras);
                cmd.Parameters.AddWithValue("@Descripcion", p.Descripcion);
                cmd.Parameters.AddWithValue("@Rubro", p.Rubro);
                cmd.Parameters.AddWithValue("@PrecioCosto", p.PrecioCosto ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PorcIVA", p.PorcIVA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PorcUtil", p.PorcUtil ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioVenta", p.PrecioVenta ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Proveedor", string.IsNullOrWhiteSpace(p.Proveedor) || p.Proveedor.ToLower() == "null" ? DBNull.Value : p.Proveedor);
                cmd.Parameters.AddWithValue("@Stock", p.Stock ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StockMin", p.StockMin ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StockMax", p.StockMax ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CtrlStock", p.CtrlStock ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Activo", p.Activo ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar producto ID {p.Id}: {ex.Message}");
            }
        }

        Console.WriteLine("Importación completada.");
    }
}
