using System;

namespace Net.Business.DTO
{
    public class DtoHospitalResponse
    {
        public string codatencion { get; set; }
        public string codpaciente { get; set; }
        public DateTime fechainicio { get; set; }
        public DateTime fechafin { get; set; }
        public string cama { get; set; }
        public string codmedico { get; set; }
        public string edad { get; set; }
        public string nombreaseguradora { get; set; }
        public string nombrecontratante { get; set; }
        public string nombrespaciente { get; set; }
        public string nombresmedico { get; set; }
        public string activo { get; set; }
        public string codaseguradora { get; set; }
        public string codcia { get; set; }
        public string restringido { get; set; }
        public string coddiagnostico { get; set; }
        public string nombrediagnostico { get; set; }
        public DateTime fechaaltamedica { get; set; }
        public string familiar { get; set; }
        public string fecini_112 { get; set; }
        public string paquete { get; set; }
    }
}
