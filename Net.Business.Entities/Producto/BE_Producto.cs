namespace Net.Business.Entities
{
    public class BE_Producto
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
        /// Stock
        /// </summary>
        public decimal QuantityOnStock { get; set; }
        //public decimal AvgStdPrice { get; set; }
        /// <summary>
        /// Stock
        /// </summary>
        public decimal AvgStdPrice { get => 20; }
        //public string U_SYP_CS_LABORATORIO { get; set; }
        /// <summary>
        /// Laboratorio
        /// </summary>
        public string U_SYP_CS_LABORATORIO { get => "GLAXO OTC"; }
        /// <summary>
        /// Familia
        /// </summary>
        public string U_SYP_CS_FAMILIA { get; set; }
        /// <summary>
        /// Codigo de producto de CSFE
        /// </summary>
        public string U_SYP_CS_SIC { get; set; }
        /// <summary>
        /// Codigo Articulo -CSFE
        /// </summary>
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
        /// Moneda Articulo
        /// S => Soles
        /// D => Dolar
        /// </summary>
        public string U_SYP_MONART { get; set; }
        /// <summary>
        /// Indicador Restringido
        /// True
        /// False
        /// </summary>
        public bool flgrestringido { get; set; }

        /// <summary>
        /// Grupo de Articulos
        /// </summary>
        public int ItemsGroupCode { get; set; }
    }
}

