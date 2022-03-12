﻿using Net.Business.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Net.Connection.ServiceLayer;
using System.Text.RegularExpressions;
using System;
using Microsoft.Data.SqlClient;
using Net.Connection;
using System.Linq;

namespace Net.Data
{
    public class SapDocumentsRepository : RepositoryBase<SapDocument>, ISapDocumentsRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        const string DB_ESQUEMA = "";
        const string SP_SET_CREATE_DOCUMENT = DB_ESQUEMA + "";

        public SapDocumentsRepository(IHttpClientFactory clientFactory, IConfiguration configuration, IConnectionSQL context)
             : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }
        public async Task<ResultadoTransaccion<SapSelectDocument>> GetListSapDocument(string U_SYP_EXTERNO)
        {
            ResultadoTransaccion<SapSelectDocument> vResultadoTransaccion = new ResultadoTransaccion<SapSelectDocument>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<SapSelectDocument> data = await _connectServiceLayer.GetAsync<SapSelectDocument>("DeliveryNotes?$filter = U_SYP_EXTERNO eq '"+ U_SYP_EXTERNO + "' &$select=DocEntry");

            vResultadoTransaccion.dataList = data;
            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetCreateDocument(BE_VentasCabecera valueVenta)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<SapDocumentLines> sapDocumentLines = new List<SapDocumentLines>();
            List<SapBatchNumbers> sapBatchNumbers = new List<SapBatchNumbers>();
            List<SapBinAllocations> sapBinAllocations = new List<SapBinAllocations>();

            try
            {

                BE_VentasCabecera value = valueVenta;

                int lineaDetalleLote = 0;

                var document = new SapDocument
                {
                    DocType = "dDocument_Items",
                    DocDate = value.fechaemision,
                    DocDueDate = value.fechaemision,
                    CardCode = value.cardcode,
                    DocCurrency = "S/",
                    TaxDate = value.fechaemision,
                    Comments = value.observacion,
                    DocObjectCode = "oDeliveryNotes",
                    DocumentLines = new List<SapDocumentLines>(),
                    //Campos de Usuario
                    U_SYP_EXTERNO = value.tipomovimiento + value.codventa,
                    Reference2 = value.tipomovimiento + value.codventa,
                    U_SYP_MDTS = "TSI",
                    U_SYP_MDMT = "01",
                    U_SYP_TPOSALME = value.tipomovimiento,
                    U_SYP_ALMACENERO = value.usuario,
                    U_SYP_SOLICITANTE = value.usuario,
                    U_SYP_CS_PRESOTOR = value.codpresotor,
                    U_SYP_CS_OA = value.codatencion,
                    U_SYP_CS_DNI_PAC = value.docidentidad,
                    U_SYP_CS_NOM_PAC = value.nombre,
                    U_SYP_CS_PAC_HC = value.codpaciente,
                    U_SYP_CS_MOV_ORIGEN = value.codventadevolucion,
                    U_SBA_ORIG = "SBA"
                };

                foreach (BE_VentasDetalle item in value.listaVentaDetalle)
                {
                    var linea = new SapDocumentLines
                    {
                        ItemCode = item.codproducto,
                        ItemDescription = item.nombreproducto,
                        Quantity = item.cantidad,
                        UnitPrice = item.d_ventaunitario_sinigv,
                        TaxCode = value.porcentajeimpuesto > 0 ? "IGV" : "EXO_IGV",
                        AccountCode = item.AccountCode,
                        WarehouseCode = value.codalmacen,
                        CostingCode = item.CostingCode,
                        CostingCode2 = item.CostingCode2,
                        CostingCode3 = item.CostingCode3,
                        CostingCode4 = item.CostingCode4,
                        U_SYP_EXT_LINEA = item.coddetalle,
                        BatchNumbers = new List<SapBatchNumbers>(),
                        DocumentLinesBinAllocations = new List<SapBinAllocations>()
                    };

                    sapBatchNumbers = new List<SapBatchNumbers>();
                    sapBinAllocations = new List<SapBinAllocations>();
                    lineaDetalleLote = 0;

                    if (item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);

                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }

                                    lineaDetalleLote++;

                                }

                                linea.BatchNumbers = sapBatchNumbers;
                                linea.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    if (item.manBtchNum && !item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);
                                }

                                linea.BatchNumbers = sapBatchNumbers;
                            }
                        }
                    }

                    if (!item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }
                                }
                                linea.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    document.DocumentLines.Add(linea);

                }

                var cadena = "DeliveryNotes";
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<SapDocument>>(cadena, document);

                if (data.DocEntry == 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje;
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
                vResultadoTransaccion.data = data;

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetReturnsDocumentBase(BE_VentasCabecera valueVenta)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<SapDocumentLines> sapDocumentLines = new List<SapDocumentLines>();
            List<SapBatchNumbers> sapBatchNumbers = new List<SapBatchNumbers>();
            List<SapBinAllocations> sapBinAllocations = new List<SapBinAllocations>();

            try
            {

                BE_VentasCabecera value = valueVenta;

                int lineaDetalleLote = 0;

                var document = new SapDocumentReturnBase
                {
                    DocType = "dDocument_Items",
                    DocDate = value.fechaemision,
                    DocDueDate = value.fechaemision,
                    CardCode = value.cardcode,
                    DocCurrency = "S/",
                    TaxDate = value.fechaemision,
                    Comments = value.observacion,
                    DocObjectCode = "oReturns",
                    DocumentLines = new List<SapDocumentLinesBase>(),
                    //Campos de Usuario
                    U_SYP_EXTERNO = value.tipomovimiento + value.codventa,
                    Reference2 = value.tipomovimiento + value.codventa,
                    //U_SYP_MDTS = "TSI",
                    U_SYP_MDMT = "24",
                    U_SYP_TPOSALME = value.tipomovimiento,
                    U_SYP_ALMACENERO = value.usuario,
                    U_SYP_SOLICITANTE = value.usuario,
                    U_SYP_CS_PRESOTOR = value.codpresotor,
                    U_SYP_CS_OA = value.codatencion,
                    U_SYP_CS_DNI_PAC = value.docidentidad,
                    U_SYP_CS_NOM_PAC = value.nombre,
                    U_SYP_CS_PAC_HC = value.codpaciente,
                    U_SYP_CS_MOV_ORIGEN = value.codventadevolucion,
                    U_SBA_ORIG = "SBA"
                };

                foreach (BE_VentasDetalle item in value.listaVentaDetalle)
                {

                    SapDocumentLinesBase lineaBase = new SapDocumentLinesBase();

                    lineaBase = new SapDocumentLinesBase
                    {
                        ItemCode = item.codproducto,
                        ItemDescription = item.nombreproducto,
                        Quantity = item.cantidad,
                        UnitPrice = item.d_ventaunitario_sinigv,
                        TaxCode = value.porcentajeimpuesto > 0 ? "IGV" : "EXO_IGV",
                        AccountCode = item.AccountCode,
                        WarehouseCode = value.codalmacen,
                        CostingCode = item.CostingCode,
                        CostingCode2 = item.CostingCode2,
                        CostingCode3 = item.CostingCode3,
                        CostingCode4 = item.CostingCode4,
                        BaseType = 15,
                        BaseEntry = item.baseentrydevolucion,
                        BaseLine = item.baselinedevolucion,
                        U_SYP_EXT_LINEA = item.coddetalle,
                        BatchNumbers = new List<SapBatchNumbers>(),
                        DocumentLinesBinAllocations = new List<SapBinAllocations>()
                    };

                    sapBatchNumbers = new List<SapBatchNumbers>();
                    sapBinAllocations = new List<SapBinAllocations>();
                    lineaDetalleLote = 0;

                    if (item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);

                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }

                                    lineaDetalleLote++;

                                }

                                lineaBase.BatchNumbers = sapBatchNumbers;
                                lineaBase.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    if (item.manBtchNum && !item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);
                                }

                                lineaBase.BatchNumbers = sapBatchNumbers;
                            }
                        }
                    }

                    if (!item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }
                                }
                                lineaBase.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    document.DocumentLines.Add(lineaBase);

                }

                var cadena = "Returns";
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<SapDocument>>(cadena, document);

                if (data.DocEntry == 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje;
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
                vResultadoTransaccion.data = data;

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetReturnsDocument(BE_VentasCabecera valueVenta)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<SapDocumentLines> sapDocumentLines = new List<SapDocumentLines>();
            List<SapBatchNumbers> sapBatchNumbers = new List<SapBatchNumbers>();
            List<SapBinAllocations> sapBinAllocations = new List<SapBinAllocations>();

            try
            {

                BE_VentasCabecera value = valueVenta;

                int lineaDetalleLote = 0;

                var document = new SapDocumentReturn
                {
                    DocType = "dDocument_Items",
                    DocDate = value.fechaemision,
                    DocDueDate = value.fechaemision,
                    CardCode = value.cardcode,
                    DocCurrency = "S/",
                    TaxDate = value.fechaemision,
                    Comments = value.observacion,
                    DocObjectCode = "oReturns",
                    DocumentLines = new List<SapDocumentLines>()
                };

                foreach (BE_VentasDetalle item in value.listaVentaDetalle)
                {

                    SapDocumentLines lineaBase = new SapDocumentLines();

                    lineaBase = new SapDocumentLines
                    {
                        ItemCode = item.codproducto,
                        ItemDescription = item.nombreproducto,
                        Quantity = item.cantidad,
                        UnitPrice = item.d_ventaunitario_sinigv,
                        TaxCode = value.porcentajeimpuesto > 0 ? "IGV" : "EXO_IGV",
                        AccountCode = item.AccountCode,
                        WarehouseCode = value.codalmacen,
                        CostingCode = item.CostingCode,
                        CostingCode2 = item.CostingCode2,
                        CostingCode3 = item.CostingCode3,
                        CostingCode4 = item.CostingCode4,
                        BatchNumbers = new List<SapBatchNumbers>(),
                        DocumentLinesBinAllocations = new List<SapBinAllocations>()
                    };

                    sapBatchNumbers = new List<SapBatchNumbers>();
                    sapBinAllocations = new List<SapBinAllocations>();
                    lineaDetalleLote = 0;

                    if (item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);

                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }

                                    lineaDetalleLote++;

                                }

                                lineaBase.BatchNumbers = sapBatchNumbers;
                                lineaBase.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    if (item.manBtchNum && !item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);
                                }

                                lineaBase.BatchNumbers = sapBatchNumbers;
                            }
                        }
                    }

                    if (!item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }
                                }
                                lineaBase.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    document.DocumentLines.Add(lineaBase);

                }

                var cadena = "Returns";
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<SapDocument>>(cadena, document);

                if (data.DocEntry == 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje;
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
                vResultadoTransaccion.data = data;

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetCreateAsociateReserveDocument(BE_VentasCabecera valueVenta)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<SapDocumentLines> sapDocumentLines = new List<SapDocumentLines>();
            List<SapBatchNumbers> sapBatchNumbers = new List<SapBatchNumbers>();
            List<SapBinAllocations> sapBinAllocations = new List<SapBinAllocations>();

            try
            {

                BE_VentasCabecera value = valueVenta;

                int lineaDetalleLote = 0;

                var document = new SapDocumentReservaBase
                {
                    DocType = "dDocument_Items",
                    DocDate = value.fechaemision,
                    DocDueDate = value.fechaemision,
                    CardCode = value.cardcode,
                    DocCurrency = "S/",
                    TaxDate = value.fechaemision,
                    Comments = value.observacion,
                    DocObjectCode = "oDeliveryNotes",
                    DocumentLines = new List<SapDocumentLinesBase>(),

                    //Campos de Usuario
                    U_SYP_EXTERNO = value.tipomovimiento + value.codventa,
                    Reference2 = value.tipomovimiento + value.codventa,
                    //U_SYP_MDTS = "TSI",
                    U_SYP_MDMT = "24",
                    U_SYP_TPOSALME = value.tipomovimiento,
                    U_SYP_ALMACENERO = value.usuario,
                    U_SYP_SOLICITANTE = value.usuario,
                    U_SYP_CS_PRESOTOR = value.codpresotor,
                    U_SYP_CS_OA = value.codatencion,
                    U_SYP_CS_DNI_PAC = value.docidentidad,
                    U_SYP_CS_NOM_PAC = value.nombre,
                    U_SYP_CS_PAC_HC = value.codpaciente,
                    U_SYP_CS_MOV_ORIGEN = value.codventadevolucion,
                    U_SBA_ORIG = "SBA"
                };

                int BaseLine = 0;

                foreach (BE_VentasDetalle item in value.listaVentaDetalle)
                {
                    var linea = new SapDocumentLinesBase
                    {
                        ItemCode = item.codproducto,
                        ItemDescription = item.nombreproducto,
                        Quantity = item.cantidad,
                        UnitPrice = item.d_ventaunitario_sinigv,
                        TaxCode = value.porcentajeimpuesto > 0 ? "IGV" : "EXO_IGV",
                        AccountCode = item.AccountCode,
                        WarehouseCode = value.codalmacen,
                        CostingCode = item.CostingCode,
                        CostingCode2 = item.CostingCode2,
                        CostingCode3 = item.CostingCode3,
                        CostingCode4 = item.CostingCode4,
                        BaseType = 13,
                        BaseEntry = value.ide_docentrysap,
                        BaseLine = BaseLine,
                        BatchNumbers = new List<SapBatchNumbers>(),
                        DocumentLinesBinAllocations = new List<SapBinAllocations>()
                    };

                    sapBatchNumbers = new List<SapBatchNumbers>();
                    sapBinAllocations = new List<SapBinAllocations>();
                    lineaDetalleLote = 0;

                    if (item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);

                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }

                                    lineaDetalleLote++;

                                }

                                linea.BatchNumbers = sapBatchNumbers;
                                linea.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    if (item.manBtchNum && !item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);
                                }

                                linea.BatchNumbers = sapBatchNumbers;
                            }
                        }
                    }

                    if (!item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }
                                }
                                linea.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    document.DocumentLines.Add(linea);

                    BaseLine++;

                }

                var cadena = "DeliveryNotes";
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<SapDocument>>(cadena, document);

                if (data.DocEntry == 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje;
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
                vResultadoTransaccion.data = data;

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetCancelDocument(int docentry)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = string.Format("DeliveryNotes({0})/Cancel", docentry);
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostCancelAsyncSBA<SapBaseResponse<SapDocument>>(cadena);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetReturnsDocumentNota(BE_VentasCabecera valueVenta)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<SapDocumentLines> sapDocumentLines = new List<SapDocumentLines>();
            List<SapBatchNumbers> sapBatchNumbers = new List<SapBatchNumbers>();
            List<SapBinAllocations> sapBinAllocations = new List<SapBinAllocations>();

            try
            {
                BE_VentasCabecera value = valueVenta;

                int lineaDetalleLote = 0;

                var document = new SapDocument
                {
                    DocType = "dDocument_Items",
                    DocDate = value.fechaemision,
                    DocDueDate = value.fechaemision,
                    CardCode = value.cardcode,
                    DocCurrency = value.moneda,
                    TaxDate = value.fechaemision,
                    Comments = value.observacion,
                    DocObjectCode = "oCreditNotes",
                    DocumentLines = new List<SapDocumentLines>(),
                    //Campos de Usuario
                    U_SYP_EXTERNO = value.tipomovimiento + value.codventa,
                    Reference2 = value.tipomovimiento + value.codventa,
                    //U_SYP_MDTS = "TSI",
                    U_SYP_MDMT = "24",
                    U_SYP_TPOSALME = value.tipomovimiento,
                    U_SYP_ALMACENERO = value.usuario,
                    U_SYP_SOLICITANTE = value.usuario,
                    U_SYP_CS_PRESOTOR = value.codpresotor,
                    U_SYP_CS_OA = value.codatencion,
                    U_SYP_CS_DNI_PAC = value.docidentidad,
                    U_SYP_CS_NOM_PAC = value.nombre,
                    U_SYP_CS_PAC_HC = value.codpaciente,
                    U_SYP_CS_MOV_ORIGEN = value.codventadevolucion,
                    U_SBA_ORIG = "SBA"
                };

                foreach (BE_VentasDetalle item in value.listaVentaDetalle)
                {
                    var linea = new SapDocumentLines
                    {
                        ItemCode = item.codproducto,
                        ItemDescription = item.nombreproducto,
                        Quantity = item.cantsunat,
                        UnitPrice = item.preciounidad,
                        TaxCode = value.porcentajeimpuesto > 0 ? "IGV" : "EXO_IGV",
                        AccountCode = item.AccountCode,
                        WarehouseCode = item.codalmacen,
                        CostingCode = item.CostingCode,
                        CostingCode2 = item.CostingCode2,
                        CostingCode3 = item.CostingCode3,
                        CostingCode4 = item.CostingCode4,
                        BatchNumbers = new List<SapBatchNumbers>(),
                        U_SYP_EXT_LINEA = item.coddetalle,
                        DocumentLinesBinAllocations = new List<SapBinAllocations>()
                    };

                    sapBatchNumbers = new List<SapBatchNumbers>();
                    sapBinAllocations = new List<SapBinAllocations>();
                    lineaDetalleLote = 0;

                    if (item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);

                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion && xFila.SerialAndBatchNumbersBaseLine == lineaDetalleLote).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }

                                    lineaDetalleLote++;
                                }

                                linea.BatchNumbers = sapBatchNumbers;
                                linea.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    if (item.manBtchNum && !item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    var lineaLote = new SapBatchNumbers
                                    {
                                        Quantity = itemLote.cantidad,
                                        BatchNumber = itemLote.lote
                                    };

                                    sapBatchNumbers.Add(lineaLote);
                                }

                                linea.BatchNumbers = sapBatchNumbers;
                            }
                        }
                    }

                    if (!item.manBtchNum && item.binactivat)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            if (item.listVentasDetalleLotes.Count > 0)
                            {
                                foreach (BE_VentasDetalleLote itemLote in item.listVentasDetalleLotes)
                                {
                                    if (item.binactivat)
                                    {
                                        if (itemLote.ubicacion != 0)
                                        {

                                            int existe = sapBinAllocations.FindAll(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Count();

                                            if (existe == 0)
                                            {
                                                var lineaUbicacion = new SapBinAllocations
                                                {
                                                    BinAbsEntry = int.Parse(itemLote.ubicacion.ToString()),
                                                    Quantity = itemLote.cantidad,
                                                    SerialAndBatchNumbersBaseLine = lineaDetalleLote
                                                };
                                                sapBinAllocations.Add(lineaUbicacion);
                                            }
                                            else
                                            {
                                                sapBinAllocations.Find(xFila => xFila.BinAbsEntry == itemLote.ubicacion).Quantity += itemLote.cantidad;
                                            }
                                        }
                                    }
                                }
                                linea.DocumentLinesBinAllocations = sapBinAllocations;
                            }
                        }
                    }

                    document.DocumentLines.Add(linea);

                }

                var cadena = "CreditNotes";
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<SapDocument>>(cadena, document);

                if (data.DocEntry == 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje;
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
                vResultadoTransaccion.data = data;

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetReturnsDocumentNotaPago(BE_VentasCabecera valueVenta)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<SapDocumentLines> sapDocumentLines = new List<SapDocumentLines>();
            List<SapBatchNumbers> sapBatchNumbers = new List<SapBatchNumbers>();
            List<SapBinAllocations> sapBinAllocations = new List<SapBinAllocations>();

            try
            {
                BE_VentasCabecera value = valueVenta;

                var pagoEfectuado = new SapVendorPayments()
                {
                    DocType = "rCustomer",
                    DocDate = value.fechaemision,
                    CardCode = value.cardcode,
                    CashAccount = value.CuentaEfectivoPago,//"01111111",
                    DocCurrency = value.moneda,
                    CashSum = value.montototal,
                    TaxDate = value.fechaemision,
                    DocObjectCode = "bopot_OutgoingPayments",
                    DueDate = value.fechaemision,
                    CounterReference = "",
                    PaymentInvoices = new List<SapPaymentInvoices>()
                };

                var paymentInvoices = new SapPaymentInvoices()
                {
                    DocEntry = value.ide_docentrysap,
                    SumApplied = value.montototal,
                    InvoiceType = "it_CredItnote"
                };


                pagoEfectuado.PaymentInvoices.Add(paymentInvoices);


                var cadena = "VendorPayments";
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<SapDocument>>(cadena, pagoEfectuado);

                if (data.DocEntry == 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje;
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "PAGO DE NOTA DE CFREDITO EFECTUADA CORRECTAMENTE.";
                vResultadoTransaccion.data = data;

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetCancelDocumentNota(int docentry)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = string.Format("CreditNotes({0})/Cancel", docentry);
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostCancelAsyncSBA<SapBaseResponse<SapDocument>>(cadena);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetCancelDocumentNotaPago(int docentry)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = string.Format("VendorPayments({0})/Cancel", docentry);
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostCancelAsyncSBA<SapBaseResponse<SapDocument>>(cadena);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
    }
}
