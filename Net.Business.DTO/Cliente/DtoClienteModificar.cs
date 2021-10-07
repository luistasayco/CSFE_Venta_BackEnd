using Net.Business.Entities;
using System;
namespace Net.Business.DTO
{
    public class DtoClienteModificar: EntityBase
    {
        public string codcliente { get; set; }
        public string codpaciente { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string coddistrito { get; set; }
        public string codprovincia { get; set; }
        public string telefono { get; set; }
        public string codcivil { get; set; }
        public DateTime fechanacimiento { get; set; }
        public string ruc { get; set; }
        public string observaciones { get; set; }
        public string estado { get; set; }
        public string vip { get; set; }
        public string sexo { get; set; }
        public string correo { get; set; }
        public string cod_tipopersona { get; set; }
        public string dsc_appaterno { get; set; }
        public string dsc_apmaterno { get; set; }
        public string dsc_segundonombre { get; set; }
        public string dsc_primernombre { get; set; }
        public string cod_ubigeo { get; set; }
        public string docidentidad { get; set; }
        public string tipdocidentidad { get; set; }

        public BE_ClienteLogistica RetornaModelo()
        {
            return new BE_ClienteLogistica
            {
                codcliente = this.codcliente,
                codpaciente = this.codpaciente,
                nombre = this.nombre,
                direccion = this.direccion,
                coddistrito = this.coddistrito,
                codprovincia = this.codprovincia,
                telefono = this.telefono,
                codcivil = this.codcivil,
                fechanacimiento = this.fechanacimiento,
                ruc = this.ruc,
                observaciones = this.observaciones,
                estado = this.estado,
                vip = this.vip,
                sexo = this.sexo,
                correo = this.correo,
                cod_tipopersona = this.cod_tipopersona,
                dsc_appaterno = this.dsc_appaterno,
                dsc_apmaterno = this.dsc_apmaterno,
                dsc_segundonombre = this.dsc_segundonombre,
                dsc_primernombre = this.dsc_primernombre,
                cod_ubigeo = this.cod_ubigeo,
                docidentidad = this.docidentidad,
                tipdocidentidad = this.tipdocidentidad,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
