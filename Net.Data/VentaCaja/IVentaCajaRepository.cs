﻿using Net.Business.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using iTextSharp.text;
using System;

namespace Net.Data
{
    public interface IVentaCajaRepository
    {
        Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaCabeceraPorCodVenta(string codVenta);
        Task<ResultadoTransaccion<BE_VentasDetalle>> GetVentaDetallePorCodVenta(string codVenta);
        Task<ResultadoTransaccion<AsegContPaci>> GetRucConsultav2PorFiltro(string codPaciente, string codAseguradora, string codCia);
        Task<ResultadoTransaccion<B_MdsynPagos>> GetMdsynPagosConsulta(long ide_pagos_bot, int ide_mdsyn_reserva, int ide_correl_reserva, string cod_liquidacion, string cod_venta, int orden);
        Task<ResultadoTransaccion<object>> GetLimiteConsumoPersonalPorCodPersonal(string codPersonal);
        Task<ResultadoTransaccion<string>> GetWebServicesPorCodTabla(string codTabla);
        Task<ResultadoTransaccion<object>> GetCorrelativoConsulta();
        //Task<ResultadoTransaccion<string>> ComprobantesRegistrar(BE_Comprobante value, string tipocomprobante, string correo, string codTipoafectacionigv, bool wFlg_electronico,string maquina);
        Task<ResultadoTransaccion<string>> ComprobantesRegistrar(BE_Comprobante value, string tipocomprobante, string correo, string codTipoafectacionigv, bool wFlg_electronico, string maquina, long idePagosBot, bool flgPagoUsado, int flg_otorgar, string tipoCodigoBarrahash, string wStrURL);
        Task<ResultadoTransaccion<string>> GetDatoCardCodeConsulta(string tipoCliente, string codCliente);
        Task<ResultadoTransaccion<BE_ComprobanteElectronicoPrint>> GetComprobanteElectroncioCodVenta(string codComprobante,
            int estadoRegistro, int estadoCdr, string fechaIni, string fechaFin, string codSistema, string tipoCompsunat, string tipoCompcsf, string codComprobanteElec, int orden);
        Task<ResultadoTransaccion<BE_ConsumoPersonal>> GetCsLimiteConsumoPersonalPorCodPersonal(string codPersonal);
        //ejemplo
        Task<ResultadoTransaccion<MemoryStream>> GenerarPreVistaPrint(string codcomprobante, string maquina, string archivoImg, int idusuario, int orden);
        Task<ResultadoTransaccion<BE_ComprobanteElectronicoLogXmlCabPrint>> GetComprobanteElectroncioLogXmlCab_print(string codcomprobante, string maquina,
            int idusuario, int orden);

        Task<ResultadoTransaccion<BE_ComprobanteElectronicoPrint>> GetComprobanteElectroncioVB(string codEmpresa,
            string codComprobantePK, string codComprobante_e, string codSistema, string tipoCompsunat, int orden);

        Task<ResultadoTransaccion<BE_VentasCabecera>> ComprobanteElectronicoUpd(string campo, string nuevovalor, string xml, Byte[] codigobarrajpg, string codigo);
        //Task<ResultadoTransaccion<string>> Comprobante_baja(BE_ComprobantesBaja value);
        Task<int> GetExisteVentaAnuladaPorCodVenta(string codVenta);
        Task<ResultadoTransaccion<string>> RegistrarComunicadoBajoComprobante(BE_ComprobantesBaja value, string xUrlWebService, string xRucEmisor);

    }
}
