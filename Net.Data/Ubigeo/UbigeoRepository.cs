using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using Net.CrossCotting;

namespace Net.Data
{
    public class UbigeoRepository : RepositoryBase<BE_Pais>, IUbigeoRepository
    {
        private readonly string _cnx;

        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_PAIS = DB_ESQUEMA + "Sp_Pais_Consulta";
        const string SP_GET_DEPARTAMENTO = DB_ESQUEMA + "Sp_Departamento_Consulta";
        const string SP_GET_PROVINCIA_POR_FILTRO = DB_ESQUEMA + "Sp_Provincias_Consulta1";
        const string SP_GET_DISTRITO_POR_FILTRO = DB_ESQUEMA + "Sp_DistritoMae_Consulta";

        public UbigeoRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _cnx = configuration.GetConnectionString("cnnSqlClinica");
        }

        public async Task<ResultadoTransaccion<BE_Pais>> GetListPais()
        {
            ResultadoTransaccion<BE_Pais> vResultadoTransaccion = new ResultadoTransaccion<BE_Pais>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Pais>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_PAIS, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@orden", 1));
                        cmd.Parameters.Add(new SqlParameter("@codpais", string.Empty));
                        cmd.Parameters.Add(new SqlParameter("@nombre", string.Empty));
                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Pais>)context.ConvertTo<BE_Pais>(reader);
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
        public async Task<ResultadoTransaccion<BE_Departamento>> GetListDepartamento()
        {
            ResultadoTransaccion<BE_Departamento> vResultadoTransaccion = new ResultadoTransaccion<BE_Departamento>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Departamento>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_DEPARTAMENTO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Departamento>)context.ConvertTo<BE_Departamento>(reader);
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
        public async Task<ResultadoTransaccion<BE_Provincia>> GetListProvinciaPorFiltro(string coddepartamento)
        {
            ResultadoTransaccion<BE_Provincia> vResultadoTransaccion = new ResultadoTransaccion<BE_Provincia>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Provincia>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_PROVINCIA_POR_FILTRO, conn))
                    {
                        int cero = 0;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@coddepartamento", string.Empty));
                        cmd.Parameters.Add(new SqlParameter("@buscar", coddepartamento));
                        cmd.Parameters.Add(new SqlParameter("@key", cero));
                        cmd.Parameters.Add(new SqlParameter("@numerolineas", cero));
                        cmd.Parameters.Add(new SqlParameter("@orden", 4));
                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Provincia>)context.ConvertTo<BE_Provincia>(reader);
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
        public async Task<ResultadoTransaccion<BE_Distrito>> GetListDistritoPorFiltro(string coddepartamento, string codprovincia)
        {
            ResultadoTransaccion<BE_Distrito> vResultadoTransaccion = new ResultadoTransaccion<BE_Distrito>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Distrito>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_DISTRITO_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_departamento", coddepartamento));
                        cmd.Parameters.Add(new SqlParameter("@cod_provincia", codprovincia));
                        cmd.Parameters.Add(new SqlParameter("@cod_distrito", string.Empty));
                        cmd.Parameters.Add(new SqlParameter("@flg_estado", 1));
                        cmd.Parameters.Add(new SqlParameter("@orden", 1));
                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Distrito>)context.ConvertTo<BE_Distrito>(reader);
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
    }
}
