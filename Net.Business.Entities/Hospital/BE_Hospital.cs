using Net.Connection.Attributes;
using System;
using System.Data;

namespace Net.Business.Entities
{
    public class BE_Hospital
    {
        //[DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        //public string quirofano { get; set; }
        //[DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        //public string recuperacion { get; set; }
        //[DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        //public DateTime fechaaltamedica { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string cama { get; set; }
        //[DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        //public string cama_alias { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codatencion { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechainicio { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codpaciente { get; set; }
        public string nombrespaciente { get; set; }
        //[DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        //public string codmedico { get; set; }
        [DBParameter(SqlDbType.Char, 20, ActionType.Everything)]
        public string codpoliza { get; set; }
        [DBParameter(SqlDbType.Char, 4, ActionType.Everything)]
        public string codaseguradora { get; set; }
        //[DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        //public string familiar { get; set; }
        [DBParameter(SqlDbType.Char, 10, ActionType.Everything)]
        public string planpoliza { get; set; }
        public string polizaplan { get => string.Format("{0}-{1}", string.IsNullOrEmpty(codpoliza) ? string.Empty : codpoliza.Trim(), string.IsNullOrEmpty(planpoliza) ? string.Empty: planpoliza.Trim()); }
        //[DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        //public string piso { get; set; }
        //[DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        //public string salaoperacion { get; set; }
        //[DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        //public string preoperatorio { get; set; }
        //[DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        //public string ucuidados { get; set; }
        //[DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        //public string cAmb { get; set; }
        //[DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        //public int hisRN { get; set; }
        [DBParameter(SqlDbType.VarChar, 300, ActionType.Everything)]
        public string nombres { get; set; }
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string edad { get; set; }
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string sexo { get; set; }
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string nombremedico { get; set; }
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string nombreaseguradora { get; set; }
        //[DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        //public string estadopaciente { get; set; }
        //[DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        //public string paquete { get; set; }
        //[DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        //public string diagnostico { get; set; }

        //I - Hospital detalla
        public string tipoingreso { get; set; }
        public string tipoegreso { get; set; }
        public string activo { get; set; }
        public string coddiagnostico { get; set; }
        public DateTime fechafin { get; set; }
        public string codmedico { get; set; }
        public string tipomedico { get; set; }
        public string titular { get; set; }
        public string quirofano { get; set; }
        public string codcia { get; set; }
        public string quiencreoregistro { get; set; }
        public string quienmodificoregistro { get; set; }
        public string estado { get; set; }
        public string familiar { get; set; }
        public string gestante { get; set; }
        public string fur { get; set; }
        public DateTime fechaaltamedica { get; set; }
        public decimal vaseguradora { get; set; }
        public decimal vpaciente { get; set; }
        public decimal vcontratante { get; set; }
        public string quieneliminoregistro { get; set; }
        public int usr_altamedica { get; set; }
        public int usr_altaadministrativa { get; set; }
        public DateTime fec_altaadministrativa { get; set; }
        public string descripcioncama { get; set; }
        public string nombresmedico { get; set; }
        public string nombresmedicoemergencia { get; set; }
        public string nombretipomedico { get; set; }
        public string nombretipoingreso { get; set; }
        public string nombretipoegreso { get; set; }
        public string nombrecontratante { get; set; }
        public string nombrediagnostico { get; set; }
        public string restringido { get; set; }
        public int planmamis { get; set; }
        public int partonormal { get; set; }
        public int reingresohospitalario { get; set; }
        public int infeccionintrahospitalaria { get; set; }
        public string observaciones { get; set; }
        public int deduciblexservicio { get; set; }
        public int pagodeducible { get; set; }
        public DateTime fechagenerada { get; set; }
        public DateTime? fechamodifica { get; set; }
        public string tipodocumentoatencion { get; set; }
        public string numerodocumentoatencion { get; set; }
        public string numeroadicionalatencion { get; set; }
        public string codespecialidad { get; set; }
        public string nivelatencion { get; set; }
        public string tipohospitalizacion { get; set; }
        public string segtipodocumentoatencion { get; set; }
        public string segnumerodocumentoatencion { get; set; }
        public string tipomedicoatiende { get; set; }
        public string codplanesp { get; set; }
        public string desplanesp { get; set; }
        public string codespecialidadatencion { get; set; }
        public string tipodestino { get; set; }
        public string nombreespecialidadatencion { get; set; }
        public string nombredestino { get; set; }
        public string codespecialidadtriage { get; set; }
        public string ruc_ipress { get; set; }
        public string nombre_ipress { get; set; }
        public string flg_quemadura { get; set; }
        public string codmedicoanamnesis { get; set; }
        public string fecini_112 { get; set; }
        public string paquete { get; set; }
        //F| - Hospital detalla
    }
}
