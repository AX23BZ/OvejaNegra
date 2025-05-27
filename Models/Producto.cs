using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOLFSOFT001C8.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public string Rubro { get; set; }
        public decimal? PrecioCosto { get; set; }
        public decimal? PorcIVA { get; set; }
        public decimal? PorcUtil { get; set; }
        public decimal? PrecioVenta { get; set; }
        public string Proveedor { get; set; }
        public int? Stock { get; set; }
        public int? StockMin { get; set; }
        public int? StockMax { get; set; }
        public bool? CtrlStock { get; set; }
        public bool? Activo { get; set; }
    }
}
