using Net.Business.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Net.Connection.ServiceLayer;
using System.Text.RegularExpressions;
using System;
using Microsoft.Data.SqlClient;
using Net.Connection;
using System.Data;

namespace Net.Data
{
    public class ClienteRepository : RepositoryBase<BE_Cliente>, IClienteRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_LISTA_CLIENTE_POR_FILTRO = DB_ESQUEMA + "VEN_ListaClientePorFiltro";
        const string SP_GET_LISTA_CLIENTE_LOGISTICA_POR_FILTRO = DB_ESQUEMA + "VEN_ListaClienteLogisticaPorFiltro";
        const string SP_GET_LISTA_CLIENTE_LOGISTICA_POR_CLIENTE = DB_ESQUEMA + "VEN_ListaClienteLogisticaPorCliente";

        const string SP_INSERT = DB_ESQUEMA + "VEN_ClienteIns";

        const string SP_UPDATE = DB_ESQUEMA + "VEN_ClienteUpd";

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        public ClienteRepository(IConnectionSQL context, IHttpClientFactory clientFactory, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _configuration = configuration;
            _clientFactory = clientFactory;
            _aplicacionName = this.GetType().Name;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }
        public async Task<ResultadoTransaccion<BE_Cliente>> GetListClientePorFiltro(string opcion, string ruc, string nombre)
        {

            ResultadoTransaccion<BE_Cliente> vResultadoTransaccion = new ResultadoTransaccion<BE_Cliente>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = "BusinessPartners?$filter = CardType eq 'C'";
                var filter = "";
                var campos = "&$select= CardCode, CardName, FederalTaxID, MailAddress, Phone1, U_SYP_BPTP, U_SYP_BPAP, U_SYP_BPAM, U_SYP_BPNO, U_SYP_BPN2, U_SYP_BPTD";

                if (opcion == "RUC")
                {
                    filter = "&$filter = FederalTaxID eq '" + ruc + "'";
                }

                nombre = nombre == null ? string.Empty : nombre.ToUpper();

                if (opcion == "NOMBRE")
                {
                    filter = "&$filter=contains (CardName , '" + nombre + "' )";
                }

                cadena = cadena + filter + campos;

                List<BE_Cliente> data = await _connectServiceLayer.GetAsync<BE_Cliente>(cadena);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = data;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_Cliente>> GetListClienteLogisticaPorFiltro(string ruc, string nombre)
        {
            ResultadoTransaccion<BE_Cliente> vResultadoTransaccion = new ResultadoTransaccion<BE_Cliente>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LISTA_CLIENTE_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ruc", ruc));
                        cmd.Parameters.Add(new SqlParameter("@nombre", nombre));

                        conn.Open();

                        var response = new List<BE_Cliente>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Cliente>)context.ConvertTo<BE_Cliente>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        vResultadoTransaccion.dataList = response;

                    }
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<BE_ClienteLogistica>> GetListDataClienteLogisticaPorFiltro(string ruc, string nombre)
        {
            ResultadoTransaccion<BE_ClienteLogistica> vResultadoTransaccion = new ResultadoTransaccion<BE_ClienteLogistica>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LISTA_CLIENTE_LOGISTICA_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ruc", ruc));
                        cmd.Parameters.Add(new SqlParameter("@nombre", nombre));

                        conn.Open();

                        var response = new List<BE_ClienteLogistica>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ClienteLogistica>)context.ConvertTo<BE_ClienteLogistica>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        vResultadoTransaccion.dataList = response;

                    }
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }
        public async Task<ResultadoTransaccion<BE_ClienteLogistica>> GetListDataClienteLogisticaPorCliente(string codcliente)
        {
            ResultadoTransaccion<BE_ClienteLogistica> vResultadoTransaccion = new ResultadoTransaccion<BE_ClienteLogistica>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LISTA_CLIENTE_LOGISTICA_POR_CLIENTE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcliente", codcliente));

                        conn.Open();

                        var response = new List<BE_ClienteLogistica>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ClienteLogistica>)context.ConvertTo<BE_ClienteLogistica>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        vResultadoTransaccion.dataList = response;

                    }
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<BE_ClienteLogistica>> Registrar(BE_ClienteLogistica item)
        {
            ResultadoTransaccion<BE_ClienteLogistica> vResultadoTransaccion = new ResultadoTransaccion<BE_ClienteLogistica>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;


            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn, transaction))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter outputCodigoTransaccionParam = new SqlParameter("@codcliente", SqlDbType.Char, 8)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputCodigoTransaccionParam);

                        SqlParameter outputCardCodeTransaccionParam = new SqlParameter("@cardcode", SqlDbType.VarChar, 15)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputCardCodeTransaccionParam);

                        cmd.Parameters.Add(new SqlParameter("@cod_paciente", item.codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@dsc_primernombre", item.dsc_primernombre));
                        cmd.Parameters.Add(new SqlParameter("@dsc_segundonombre", item.dsc_segundonombre));
                        cmd.Parameters.Add(new SqlParameter("@dsc_appaterno", item.dsc_appaterno));
                        cmd.Parameters.Add(new SqlParameter("@dsc_apmaterno", item.dsc_apmaterno));
                        cmd.Parameters.Add(new SqlParameter("@docidentidad", item.docidentidad));
                        cmd.Parameters.Add(new SqlParameter("@tipdocidentidad", item.tipdocidentidad));
                        cmd.Parameters.Add(new SqlParameter("@direccion", item.direccion));
                        cmd.Parameters.Add(new SqlParameter("@telefono", item.telefono));
                        cmd.Parameters.Add(new SqlParameter("@sexo", item.sexo));
                        cmd.Parameters.Add(new SqlParameter("@correo", item.correo));
                        cmd.Parameters.Add(new SqlParameter("@cod_ubigeo", item.cod_ubigeo));
                        cmd.Parameters.Add(new SqlParameter("@fechanacimiento", item.fechanacimiento));
                        cmd.Parameters.Add(new SqlParameter("@ruc", item.ruc));
                        cmd.Parameters.Add(new SqlParameter("@cod_tipopersona", item.cod_tipopersona));
                        cmd.Parameters.Add(new SqlParameter("@observaciones", item.observaciones));
                        cmd.Parameters.Add(new SqlParameter("@codcivil", item.codcivil));

                        cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", item.RegIdUsuario));

                        SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdTransaccionParam);

                        SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputMsjTransaccionParam);

                        //await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        var businessPartners = new BusinessPartners
                        {
                            CardCode = (string)outputCardCodeTransaccionParam.Value,
                            CardName = item.nombre,
                            CardType = "cCustomer",
                            GroupCode = 100,
                            MailAddress = item.direccion,
                            MailZipCode = item.cod_ubigeo.Substring(2),
                            Phone1 = item.telefono,
                            FederalTaxID = item.ruc,
                            EmailAddress = item.correo,
                            FreeText = item.observaciones,
                            U_SYP_BPAP = item.dsc_appaterno,
                            U_SYP_BPAM = item.dsc_apmaterno,
                            U_SYP_BPNO = item.dsc_primernombre,
                            U_SYP_BPN2 = item.dsc_segundonombre,
                            U_SYP_BPTP = item.cod_tipopersona.Trim(),
                            U_SYP_BPTD = item.cod_tipopersona.Equals("TPJ") ? "6" : "1",
                            BPAddresses = new List<Addresses>()
                        };

                        var addresses = new Addresses
                        {
                            AddressName = item.direccion,
                            Street = item.direccion,
                            Block = item.direccion,
                            ZipCode = item.cod_ubigeo.Substring(2),
                            City = item.direccion,
                            County = item.direccion,
                            Country = item.cod_ubigeo.Substring(0,2),
                        };

                        businessPartners.BPAddresses.Add(addresses);

                        BusinessPartnersRepository businessPartnersRepository = new BusinessPartnersRepository(_clientFactory, _configuration, context);
                        ResultadoTransaccion<SapBaseResponse<BusinessPartners>> resultadoTransaccionBusiness = await businessPartnersRepository.SetCreateBusinessPartners(businessPartners);

                        if (resultadoTransaccionBusiness.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionBusiness.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                    }

                    transaction.Commit();
                    transaction.Dispose();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //conn.Close();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }

                conn.Close();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_ClienteLogistica>> Modificar(BE_ClienteLogistica item)
        {
            ResultadoTransaccion<BE_ClienteLogistica> vResultadoTransaccion = new ResultadoTransaccion<BE_ClienteLogistica>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;


            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE, conn, transaction))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@codcliente", item.codcliente));
                        cmd.Parameters.Add(new SqlParameter("@cod_paciente", item.codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@dsc_primernombre", item.dsc_primernombre));
                        cmd.Parameters.Add(new SqlParameter("@dsc_segundonombre", item.dsc_segundonombre));
                        cmd.Parameters.Add(new SqlParameter("@dsc_appaterno", item.dsc_appaterno));
                        cmd.Parameters.Add(new SqlParameter("@dsc_apmaterno", item.dsc_apmaterno));
                        cmd.Parameters.Add(new SqlParameter("@docidentidad", item.docidentidad));
                        cmd.Parameters.Add(new SqlParameter("@tipdocidentidad", item.tipdocidentidad));
                        cmd.Parameters.Add(new SqlParameter("@direccion", item.direccion));
                        cmd.Parameters.Add(new SqlParameter("@telefono", item.telefono));
                        cmd.Parameters.Add(new SqlParameter("@sexo", item.sexo));
                        cmd.Parameters.Add(new SqlParameter("@correo", item.correo));
                        cmd.Parameters.Add(new SqlParameter("@cod_ubigeo", item.cod_ubigeo));
                        cmd.Parameters.Add(new SqlParameter("@fechanacimiento", item.fechanacimiento));
                        cmd.Parameters.Add(new SqlParameter("@ruc", item.ruc));
                        cmd.Parameters.Add(new SqlParameter("@cod_tipopersona", item.cod_tipopersona));
                        cmd.Parameters.Add(new SqlParameter("@observaciones", item.observaciones));
                        cmd.Parameters.Add(new SqlParameter("@codcivil", item.codcivil));
                        cmd.Parameters.Add(new SqlParameter("@vip", item.vip));
                        cmd.Parameters.Add(new SqlParameter("@nombre", item.nombre));

                        cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", item.RegIdUsuario));

                        SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdTransaccionParam);

                        SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputMsjTransaccionParam);

                        await cmd.ExecuteNonQueryAsync();

                        var businessPartners = new BusinessPartners
                        {
                            CardCode = item.cardcode.Trim(),
                            CardName = item.nombre.Trim(),
                            CardType = "cCustomer",
                            GroupCode = 100,
                            MailAddress = item.direccion.Trim(),
                            MailZipCode = item.cod_ubigeo.Substring(2),
                            Phone1 = item.telefono.Trim(),
                            FederalTaxID = item.ruc.Trim(),
                            EmailAddress = item.correo.Trim(),
                            FreeText = item.observaciones.Trim(),
                            U_SYP_BPAP = item.dsc_appaterno.Trim(),
                            U_SYP_BPAM = item.dsc_apmaterno.Trim(),
                            U_SYP_BPNO = item.dsc_primernombre.Trim(),
                            U_SYP_BPN2 = item.dsc_segundonombre.Trim(),
                            U_SYP_BPTP = item.cod_tipopersona.Trim(),
                            U_SYP_BPTD = item.cod_tipopersona.Equals("TPJ") ? "6" : "1",
                            BPAddresses = new List<Addresses>()
                        };

                        //var addresses = new Addresses
                        //{
                        //    AddressName = item.direccion.Trim(),
                        //    Street = item.direccion.Trim(),
                        //    Block = item.direccion.Trim(),
                        //    ZipCode = item.cod_ubigeo.Substring(2),
                        //    City = item.direccion.Trim(),
                        //    County = item.direccion.Trim(),
                        //    Country = item.cod_ubigeo.Substring(0, 2),
                        //};

                        //businessPartners.BPAddresses.Add(addresses);

                        BusinessPartnersRepository businessPartnersRepository = new BusinessPartnersRepository(_clientFactory, _configuration, context);
                        ResultadoTransaccion<SapBaseResponse<BusinessPartners>> resultadoTransaccionBusiness = await businessPartnersRepository.SetUpdateBusinessPartners(businessPartners);

                        if (resultadoTransaccionBusiness.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionBusiness.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                    }

                    transaction.Commit();
                    transaction.Dispose();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }

                conn.Close();
            }

            return vResultadoTransaccion;
        }

    }
}
