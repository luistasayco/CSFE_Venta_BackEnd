namespace Net.Business.Entities
{
    public class ResultadoTransaccion
    {
        public int IdRegistro { get; set; }
        public int ResultadoCodigo { get; set; }
        public string ResultadoDescripcion { get; set; }
        public string ResultadoAplicacion { get; set; }
        public string ResultadoMetodo { get; set; }
        public string NombreEstacion { get => System.Environment.MachineName; set => NombreEstacion = System.Environment.MachineName; }
    }
}
