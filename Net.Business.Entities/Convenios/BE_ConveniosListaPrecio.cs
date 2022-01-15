using System;

namespace Net.Business.Entities
{
    public class BE_ConveniosListaPrecio
    {
        public int idconvenio { get; set; }
        //public DateTime fechadocumento { get; set; }
        public string codalmacen { get; set; }
        public string tipomovimiento { get; set; }
        public string codtipocliente { get; set; }
        public string codcliente { get; set; }
        public string codpaciente { get; set; }
        public string codaseguradora { get; set; }
        public string codcia { get; set; }
        public string nomalmacen { get; set; }
        //public DateTime fechainicio { get; set; }
        //public DateTime fechafin { get; set; }
        //public bool excepto { get; set; }
        public string nomtipocliente { get; set; }
        public string nomcliente { get; set; }
        public string nompaciente { get; set; }
        public string nomaseguradora { get; set; }
        public string nomcia { get; set; }
        public double monto { get; set; }
        public string moneda { get; set; }
        //public bool estado { get; set; }
        public int pricelist { get; set; }
        public string nompricelist { get; set; }
        public int flgeliminado { get; set; }
        public int regcreateidusuario { get; set; }
    }
}
