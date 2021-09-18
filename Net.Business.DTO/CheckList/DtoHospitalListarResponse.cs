using Net.Business.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Net.Business.DTO
{
    public class DtoHospitalListarResponse
    {
        public IEnumerable<DtoHospitalResponse> ListaHospital { get; set; }

        public DtoHospitalListarResponse RetornarHospitalListar(IEnumerable<BE_Hospital> listaHospital)
        {
            IEnumerable<DtoHospitalResponse> lista = (
                from value in listaHospital
                select new DtoHospitalResponse
                {
                    codatencion = value.codatencion,
                    codpaciente = value.codpaciente,
                    fechainicio = value.fechainicio,
                    fechafin = value.fechafin,
                    cama = value.cama,
                    codmedico = value.codmedico,
                    edad = value.edad,
                    nombreaseguradora = value.nombreaseguradora,
                    nombrecontratante = value.nombrecontratante,
                    nombrespaciente = value.nombrespaciente,
                    nombresmedico = value.nombresmedico,
                    activo = value.activo,
                    codaseguradora = value.codaseguradora,
                    codcia = value.codcia,
                    restringido = value.restringido,
                    coddiagnostico = value.coddiagnostico,
                    nombrediagnostico = value.nombrediagnostico,
                    fechaaltamedica = value.fechaaltamedica,
                    familiar = value.familiar,
                    fecini_112 = value.fecini_112,
                    paquete = value.paquete,
                }
            );

            return new DtoHospitalListarResponse() { ListaHospital = lista };
        }
    }
}
