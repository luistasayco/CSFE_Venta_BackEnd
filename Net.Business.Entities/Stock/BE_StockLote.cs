using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_StockLote
    {
        /// <summary>
        /// Codigo de Articulo - SAP
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// Descripción del Articulo
        /// </summary>
        public string ItemName { get; set; }
        public string BatchNum { get; set; }
        public decimal? QuantityLote { get; set; }
        public decimal Quantityinput { get; set; }
        /// <summary>
        /// Cantidad Solicitada Lote (Solo del almacén)
        /// </summary>
        public decimal? IsCommitedLote { get; set; }
        /// <summary>
        /// Cantidad Comprometida Lote (Solo del almacén)
        /// </summary>
        public decimal? OnOrderLote { get; set; }
        public DateTime ExpDate { get; set; }
    }
}
