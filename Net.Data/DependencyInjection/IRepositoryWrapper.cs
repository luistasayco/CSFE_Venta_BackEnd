namespace Net.Data
{
    public interface IRepositoryWrapper
    {
        IEmailSenderRepository EmailSender { get; }
        IVentaCabeceraRepository VentaCabecera { get; }
        ITipoCambioRepository TipoCambio { get; }
        IPlanesRepository Planes { get; }
        IWarehousesRepository Warehouses { get; }
        IAtencionRepository Atencion { get; }
        IClienteRepository Cliente { get; }
        IPersonalClinicaRepository PersonalClinica { get; }
        IMedicoRepository Medico { get; }
        IPacienteRepository Paciente { get;  }
    }
}
