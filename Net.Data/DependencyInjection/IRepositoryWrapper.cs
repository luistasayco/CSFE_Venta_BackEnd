namespace Net.Data
{
    public interface IRepositoryWrapper
    {
        IEmailSenderRepository EmailSender { get; }
        IVentaRepository Venta { get; }
        ITipoCambioRepository TipoCambio { get; }
        IPlanesRepository Planes { get; }
        IWarehousesRepository Warehouses { get; }
        IAtencionRepository Atencion { get; }
        IClienteRepository Cliente { get; }
        IPersonalClinicaRepository PersonalClinica { get; }
        IMedicoRepository Medico { get; }
        IPacienteRepository Paciente { get;  }
        IComprobanteRepository Comprobante { get; }
        IPedidoRepository Pedido { get; }
        IProductoRepository Producto { get; }
        ISeriePorMaquinaRepository SeriePorMaquina { get; }
        IVentaConfiguracion VentaConfiguracion { get; }
        ITablaRepository Tabla { get; }
        IHospitalRepository Hospital { get; }
        IRecetaRepository Receta { get; }
        ISerieRepository Serie { get; }
        ICentroRepository CentroCosto { get; }
        IPickingRepository Picking { get; }
        IConsolidadoRepository Consolidado { get; }
        IConveniosRepository Convenios { get; }
        IAseguradoraxProductoRepository AseguradoraxProducto { get; }
    }
}
