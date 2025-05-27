using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOLFSOFT001C8.Models;

namespace WOLFSOFT001C8.Services
{
    public class SyncService
    {
        private static readonly string localDbPath = @"C:\WOLFSOFT001C\productos_local.sqlite";
        private static readonly string sqlitePath = $"Data Source={localDbPath}";

        private const string azureConnection = "Server=tcp:servidorbbddbeto.database.windows.net,1433;" +
            "Initial Catalog=BDcomercioON;Persist Security Info=False;" +
            "User ID=betoadmin;Password=Bodoque25;" +
            "MultipleActiveResultSets=False;Encrypt=True;" +
            "TrustServerCertificate=False;Connection Timeout=30;";

        public void SincronizarProductosDesdeAzure()
        {
            Console.WriteLine("🔄 Iniciando sincronización...");

            // Asegura existencia de carpeta base local
            var carpeta = Path.GetDirectoryName(localDbPath);
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            CrearBaseLocalSiNoExiste();
            var productos = LeerDesdeAzure();
            GuardarEnLocal(productos);

            Console.WriteLine("✅ Sincronización completa.");
        }

        private void CrearBaseLocalSiNoExiste()
        {
            using var con = new SQLiteConnection(sqlitePath);
            con.Open();

            var cmd = new SQLiteCommand(@"
                CREATE TABLE IF NOT EXISTS Productos (
                    Id INTEGER PRIMARY KEY,
                    CodigoBarras TEXT,
                    Descripcion TEXT,
                    Rubro TEXT,
                    PrecioCosto REAL,
                    PorcIVA REAL,
                    PorcUtil REAL,
                    PrecioVenta REAL,
                    Proveedor TEXT,
                    Stock INTEGER,
                    StockMin INTEGER,
                    StockMax INTEGER,
                    CtrlStock BOOLEAN,
                    Activo BOOLEAN
                )", con);

            cmd.ExecuteNonQuery();
        }

        private List<Producto> LeerDesdeAzure()
        {
            var productos = new List<Producto>();

            using var con = new SqlConnection(azureConnection);
            con.Open();

            var cmd = new SqlCommand("SELECT * FROM Productos", con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetInt32(0),
                    CodigoBarras = reader["CodigoBarras"]?.ToString(),
                    Descripcion = reader["Descripcion"]?.ToString(),
                    Rubro = reader["Rubro"]?.ToString(),
                    PrecioCosto = reader["PrecioCosto"] as decimal?,
                    PorcIVA = reader["PorcIVA"] as decimal?,
                    PorcUtil = reader["PorcUtil"] as decimal?,
                    PrecioVenta = reader["PrecioVenta"] as decimal?,
                    Proveedor = reader["Proveedor"]?.ToString(),
                    Stock = reader["Stock"] as int?,
                    StockMin = reader["StockMin"] as int?,
                    StockMax = reader["StockMax"] as int?,
                    CtrlStock = reader["CtrlStock"] as bool?,
                    Activo = reader["Activo"] as bool?
                });
            }

            return productos;
        }

        private void GuardarEnLocal(List<Producto> productos)
        {
            using var con = new SQLiteConnection(sqlitePath);
            con.Open();

            new SQLiteCommand("DELETE FROM Productos", con).ExecuteNonQuery();

            foreach (var p in productos)
            {
                var cmd = new SQLiteCommand(@"
                    INSERT INTO Productos 
                    (Id, CodigoBarras, Descripcion, Rubro, PrecioCosto, PorcIVA, PorcUtil, PrecioVenta,
                     Proveedor, Stock, StockMin, StockMax, CtrlStock, Activo)
                    VALUES 
                    (@Id, @CodigoBarras, @Descripcion, @Rubro, @PrecioCosto, @PorcIVA, @PorcUtil, @PrecioVenta,
                     @Proveedor, @Stock, @StockMin, @StockMax, @CtrlStock, @Activo)", con);

                cmd.Parameters.AddWithValue("@Id", p.Id);
                cmd.Parameters.AddWithValue("@CodigoBarras", p.CodigoBarras ?? "");
                cmd.Parameters.AddWithValue("@Descripcion", p.Descripcion ?? "");
                cmd.Parameters.AddWithValue("@Rubro", p.Rubro ?? "");
                cmd.Parameters.AddWithValue("@PrecioCosto", p.PrecioCosto ?? 0);
                cmd.Parameters.AddWithValue("@PorcIVA", p.PorcIVA ?? 0);
                cmd.Parameters.AddWithValue("@PorcUtil", p.PorcUtil ?? 0);
                cmd.Parameters.AddWithValue("@PrecioVenta", p.PrecioVenta ?? 0);
                cmd.Parameters.AddWithValue("@Proveedor", p.Proveedor ?? "");
                cmd.Parameters.AddWithValue("@Stock", p.Stock ?? 0);
                cmd.Parameters.AddWithValue("@StockMin", p.StockMin ?? 0);
                cmd.Parameters.AddWithValue("@StockMax", p.StockMax ?? 0);
                cmd.Parameters.AddWithValue("@CtrlStock", p.CtrlStock ?? false);
                cmd.Parameters.AddWithValue("@Activo", p.Activo ?? false);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
