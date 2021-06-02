namespace Net.Business.Entities
{
    public class BE_Stock
    {
        /// <summary>
        /// Codigo de Articulo - SAP
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// Descripción del Articulo
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// Indicador de Lote
        /// N => Trabaja sin Lote
        /// S => Tranaja con Lote
        /// </summary>
        public string ManBtchNum { get; set; }
        /// <summary>
        /// Indicador de Serie
        /// N => Trabaja sin Serie
        /// S => Tranaja con Serie
        /// </summary>
        public string ManSerNum { get; set; }
        /// <summary>
        /// Familia
        /// </summary>
        public string U_SYP_CS_FAMILIA { get; set; }
        /// <summary>
        /// Codigo de producto de CSFE
        /// </summary>
        public string U_SYP_CS_SIC { get; set; }
        public string codproducto { get => U_SYP_CS_SIC; }
        /// <summary>
        /// Estado de Abastecimiento
        /// N => Normal
        /// A => Agotado
        /// </summary>
        public string U_SYP_CS_EABAS { get; set; }
        /// <summary>
        /// Tipo clasificación
        /// Alto riesgo => Alto riesgo
        /// Tratamiento => Tratamiento
        /// </summary>
        public string U_SYP_CS_CLASIF { get; set; }
        /// <summary>
        /// Laboratorio
        /// </summary>
        public string U_SYP_CS_LABORATORIO { get; set; }
        /// <summary>
        /// Codigo Lista Precio
        /// </summary>
        public int PriceList { get; set; }
        /// <summary>
        /// Codigo Almacén
        /// </summary>
        public string WhsCode { get; set; }
        /// <summary>
        /// Lote
        /// </summary>
        public string BatchNum { get; set; }
        /// <summary>
        /// Stock Total (Todos los almacenes)
        /// </summary>
        public decimal OnHand { get; set; }
        /// <summary>
        /// Cantidad Solicitada (En Orden de Compra)
        /// </summary>
        public decimal OnOrder { get; set; }
        /// <summary>
        /// Cantidad Comprometida (Todos los almacenes)
        /// </summary>
        public decimal IsCommited { get; set; }
        /// <summary>
        /// Precio
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Stock Total (Solo del almacén)
        /// </summary>
        public decimal OnHand_1 { get; set; }
        /// <summary>
        /// Cantidad Solicitada (Solo del almacén)
        /// </summary>
        public decimal OnOrder_1 { get; set; }
        /// <summary>
        /// Cantidad Comprometida (Solo del almacén)
        /// </summary>
        public decimal IsCommited_1 { get; set; }
        /// <summary>
        /// Stock Lote (Solo del almacén)
        /// </summary>
        public decimal Quantity { get; set; }
        
        /// <summary>
        /// Cantidad Solicitada Lote (Solo del almacén)
        /// </summary>
        public decimal IsCommited_2 { get; set; }
        /// <summary>
        /// Cantidad Comprometida Lote (Solo del almacén)
        /// </summary>
        public decimal OnOrder_2 { get; set; }
        public string WhsName { get; set; }
    }
}
