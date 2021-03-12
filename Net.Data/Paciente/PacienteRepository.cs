using Net.Business.Entities;
using Net.Connection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
namespace Net.Data
{
    class PacienteRepository : RepositoryBase<BE_Paciente>, IPacienteRepository
    {
        private readonly string _cnx;
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "Fa_Pacientes_Info_Farma";

        public PacienteRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlClinica");
        }
        public async Task<BE_Paciente> GetPacientePorAtencion(string codAtencion)
        {
            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codatencion", codAtencion));

                    var response = new BE_Paciente();

                    conn.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        response = context.Convert<BE_Paciente>(reader);
                    }

                    conn.Close();

                    return response;
                }
            }
        }
    }
}
