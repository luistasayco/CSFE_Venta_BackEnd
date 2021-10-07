using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_Cliente
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string MailAddress { get; set; }
        public string Phone1 { get; set; }
        public string FederalTaxID { get; set; }
        public string U_SYP_BPAP { get; set; }
        public string U_SYP_BPAM { get; set; }
        public string U_SYP_BPNO { get; set; }
        public string U_SYP_BPN2 { get; set; }
        public string U_SYP_BPTP { get; set; }
        public string U_SYP_BPTD { get; set; }
    }

    public class BE_ClienteLogistica: EntityBase
    {
        public string codcliente { get; set; }
        public string codpaciente { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string coddistrito { get; set; }
        public string codprovincia { get; set; }
        public string telefono { get; set; }
        public string codcivil { get; set; }
        public string nomcivil { get; set; }
        public DateTime fechanacimiento { get; set; }
        public string ruc { get; set; }
        public string observaciones { get; set; }
        public string estado { get; set; }
        public string nomestado { get; set; }
        public string vip { get; set; }
        public string sexo { get; set; }
        public string correo { get; set; }
        public string cod_tipopersona { get; set; }
        public string nomtipopersona { get; set; }
        public string dsc_appaterno { get; set; }
        public string dsc_apmaterno { get; set; }
        public string dsc_segundonombre { get; set; }
        public string dsc_primernombre { get; set; }
        public string cod_ubigeo { get; set; }
        public string docidentidad { get; set; }
        public string tipdocidentidad { get; set; }
        public string nomtipdocidentidad { get; set; }
    }
}
