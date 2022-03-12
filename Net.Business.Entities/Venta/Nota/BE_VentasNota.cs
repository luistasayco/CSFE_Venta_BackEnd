using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_VentasNota
    {
        public string codnota { get; set; }
        public string codtipocliente { get; set; }
        public string codcliente { get; set; }
        public string codatencion { get; set; }
        public string anombredequien { get; set; }
        public string concepto { get; set; }
        public string direccion { get; set; }
        public string ruc { get; set; }
        public string nombrepaciente { get; set; }
        public string codpaciente { get; set; }
        public string codcomprobante { get; set; }
        public DateTime fechagenera { get; set; }
        public DateTime fechaemision { get; set; }
        public DateTime fechacancelacion { get; set; }
        public DateTime fechaanulacion { get; set; }
        public decimal porcentajeimpuesto { get; set; }
        public decimal monto { get; set; }
        public decimal montoimpuesto { get; set; }
        public string tipo { get; set; }
        public string estado { get; set; }
        public string coduser { get; set; }
        public string codcentro { get; set; }
        public string numeroplanilla { get; set; }
        public string codventa { get; set; }
        public string codmotivo { get; set; }
        public string codnotae { get; set; }
        public bool flg_electronico { get; set; }
        public bool flg_gratuito { get; set; }
        public string moneda { get; set; }
        public double tipodecambio { get; set; }
        public double montodolares { get; set; }
        public double montoimpuestodolares { get; set; }
        public string cardcode { get; set; }
        public int regcreateidusuario { get; set; }
        public DateTime regcreate { get; set; }
        public int regupdateidusuario { get; set; }
        public DateTime regupdate { get; set; }
        public int flgeliminado { get; set; }
        public string nombretipocliente { get; set; }
        public string nombreestado { get; set; }
        public int doc_entry { get; set; }
    }
}
