using Net.Connection;

namespace Net.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private IConnectionSQL _repoContext;

        private IEmailSenderRepository _EmailSender;
        private IVentaCabeceraRepository _VentaCabecera;

        public RepositoryWrapper(IConnectionSQL repoContext)
        {
            _repoContext = repoContext;
        }
        public IEmailSenderRepository EmailSender
        {
            get
            {
                if (_EmailSender == null)
                {
                    _EmailSender = new EmailSenderRepository(_repoContext);
                }
                return _EmailSender;
            }
        }
        public IVentaCabeceraRepository VentaCabecera
        {
            get
            {
                if (_VentaCabecera == null)
                {
                    _VentaCabecera = new VentaCabeceraRepository(_repoContext);
                }
                return _VentaCabecera;
            }
        }
    }
}
