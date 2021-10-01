using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;

namespace Net.Data
{
    public class AtencionRepository : RepositoryBase<BE_Atencion>, IAtencionRepository
    {
        private readonly string _cnx;
        private readonly string _cnx_clinica;
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_ListaAtencionPorFiltrosGet";
        const string SP_GET_PAQUETE = DB_ESQUEMA + "Sp_PaquetexAtencion_Consulta";

        public AtencionRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _cnx_clinica = configuration.GetConnectionString("cnnSqlClinica");
        }
        public async Task<IEnumerable<BE_Atencion>> GetListPacientePorFiltros(string opcion, string codpaciente, string nombres)
        {
            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@opcion", opcion));
                    cmd.Parameters.Add(new SqlParameter("@codpaciente", codpaciente));
                    cmd.Parameters.Add(new SqlParameter("@nombres", nombres));

                    var response = new List<BE_Atencion>();

                    conn.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(MapToValue(reader));
                        }
                    }

                    conn.Close();

                    return response;
                }
            }
        }

        public async Task<IEnumerable<BE_AtencionPaquete>> GetListPaquetePorCodAtencion(string codatencion)
        {
            using (SqlConnection conn = new SqlConnection(_cnx_clinica))
            {
                using (SqlCommand cmd = new SqlCommand(SP_GET_PAQUETE, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codatencion", codatencion));

                    var response = new List<BE_AtencionPaquete>();

                    conn.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(MapToValuePaquete(reader));
                        }
                    }

                    conn.Close();

                    return response;
                }
            }
        }

        public BE_Atencion MapToValue(SqlDataReader reader) => new BE_Atencion()
        {
            nombrepaciente = (string)reader["nombrepaciente"].ToString(),
            atencion = (string)reader["atencion"].ToString(),
            estadopaciente = (string)reader["estadopaciente"].ToString(),
            finicio = (string)reader["finicio"].ToString(),
            codpaciente = (string)reader["codpaciente"].ToString()
        };

        public BE_AtencionPaquete MapToValuePaquete(SqlDataReader reader) => new BE_AtencionPaquete()
        {
            idpaqueteclinico = (int)reader["idpaqueteclinico"],
            codatencion = (string)reader["codatencion"].ToString(),
            fecasignacion = (DateTime)reader["fecasignacion"],
            importepaquete = (decimal)reader["importepaquete"],
            idpaqueteclinico_bk = (int)reader["idpaqueteclinico_bk"],
            descripcion = (string)reader["descripcion"].ToString(),
        };
    }
}

