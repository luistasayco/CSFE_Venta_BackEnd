using Net.Business.Entities;
using System;

namespace Net.Business.DTO
{
    public class DtoComprobanteResponse
    {
        public string codcomprobante { get; set; }
        public string codcomprobantee { get; set; }
        public string estado { get; set; }
        public string codventa { get; set; }
        public string codtipocliente { get; set; }
        public string anombrede { get; set; }
        public decimal montototal { get; set; }
        public double montototaldolares { get; set; }
        public string moneda { get; set; }
        public DateTime fechagenera { get; set; }
        public DateTime fechaemision { get; set; }
        public DateTime? fechacancelacion { get; set; }
        public DateTime? fechaanulacion { get; set; }
        public string codcliente { get; set; }
        public string numeroplanilla { get; set; }
        public string nombreestado { get; set; }
        public string nombretipocliente { get; set; }
        public string codatencion { get; set; }
        public bool flg_gratuito { get; set; }

        public DtoComprobanteResponse RetornaDtoVentaCabeceraResponse(BE_Comprobante value)
        {
            return new DtoComprobanteResponse()
            {
                codcomprobante = value.codcomprobante,
                codcomprobantee = value.codcomprobantee,
                estado = value.estado,
                codventa = value.codventa,
                codtipocliente = value.codtipocliente,
                anombrede = value.anombrede,
                montototal = value.montototal,
                montototaldolares = value.montototaldolares,
                moneda = value.moneda,
                fechagenera = value.fechagenera,
                fechaemision = value.fechaemision,
                fechacancelacion = value.fechacancelacion,
                fechaanulacion = value.fechaanulacion,
                codcliente = value.codcliente,
                numeroplanilla = value.numeroplanilla,
                nombreestado = value.nombreestado,
                nombretipocliente = value.nombretipocliente,
                codatencion = value.codatencion,
                flg_gratuito = value.flg_gratuito
            };
        }
    }
}
