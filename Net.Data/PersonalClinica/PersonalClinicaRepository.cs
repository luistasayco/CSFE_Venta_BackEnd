using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Net.Data
{
    class PersonalClinicaRepository : RepositoryBase<BE_PersonalClinica>, IPersonalClinicaRepository
    {
        private readonly string _cnx;
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "Fa_Personal_Consulta";

        public PersonalClinicaRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlPlanilla");
        }
        public async Task<IEnumerable<BE_PersonalClinica>> GetListPersonalClinicaPorNombre(string nombre)
        {
            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@buscar", nombre == null ? "": nombre.ToUpper()));
                    cmd.Parameters.Add(new SqlParameter("@key", 1));
                    cmd.Parameters.Add(new SqlParameter("@numerolineas", 1));
                    cmd.Parameters.Add(new SqlParameter("@orden", 5));

                    var response = new List<BE_PersonalClinica>();

                    conn.Open();

                    using (IDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        response = (List<BE_PersonalClinica>)context.ConvertTo<BE_PersonalClinica>(reader);
                    }

                    conn.Close();

                    return response;
                }
            }
        }
    }
}
