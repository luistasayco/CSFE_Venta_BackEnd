namespace Net.Data
{
    public interface IRepositoryWrapper
    {
        IEmailSenderRepository EmailSender { get; }
        IVentaCabeceraRepository VentaCabecera { get; }
    }
}
