using Net.Connection;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Net.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly IConnectionSQL _repoContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        private IEmailSenderRepository _EmailSender;
        private IVentaCabeceraRepository _VentaCabecera;
        private ITipoCambioRepository _TipoCambio;
        private IPlanesRepository _Planes;
        private IWarehousesRepository _Warehouses;
        private IAtencionRepository _Atencion;
        private IClienteRepository _Cliente;
        private IPersonalClinicaRepository _PersonalClinica;
        private IMedicoRepository _Medico;
        private IPacienteRepository _Paciente;
        public RepositoryWrapper(IConnectionSQL repoContext, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _repoContext = repoContext;
            _configuration = configuration;
            _clientFactory = clientFactory;
        }
        public IEmailSenderRepository EmailSender
        {
            get
            {
                if (_EmailSender == null)
                {
                    _EmailSender = new EmailSenderRepository(_repoContext, _configuration);
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
                    _VentaCabecera = new VentaCabeceraRepository(_repoContext, _configuration);
                }
                return _VentaCabecera;
            }
        }

        public ITipoCambioRepository TipoCambio
        {
            get
            {
                if (_TipoCambio == null)
                {
                    _TipoCambio = new TipoCambioRepository(_repoContext);
                }
                return _TipoCambio;
            }
        }

        public IPlanesRepository Planes
        {
            get
            {
                if(_Planes == null)
                {
                    _Planes = new PlanesRepository(_repoContext, _configuration);
                }
                return _Planes;
            }
        }

        public IWarehousesRepository Warehouses
        {
            get
            {
                if (_Warehouses == null)
                {
                    _Warehouses = new WarehousesRepository(_clientFactory, _configuration);
                }
                return _Warehouses;
            }
        }
        public IAtencionRepository Atencion
        {
            get
            {
                if (_Atencion == null)
                {
                    _Atencion = new AtencionRepository(_repoContext, _configuration);
                }
                return _Atencion;
            }
        }
        public IClienteRepository Cliente
        {
            get
            {
                if (_Cliente == null)
                {
                    _Cliente = new ClienteRepository(_clientFactory, _configuration);
                }
                return _Cliente;
            }
        }
        public IPersonalClinicaRepository PersonalClinica
        {
            get
            {
                if (_PersonalClinica == null)
                {
                    _PersonalClinica = new PersonalClinicaRepository(_repoContext, _configuration);
                }
                return _PersonalClinica;
            }
        }
        public IMedicoRepository Medico
        {
            get
            {
                if (_Medico == null)
                {
                    _Medico = new MedicoRepository(_repoContext, _configuration);
                }
                return _Medico;
            }
        }
        public IPacienteRepository Paciente
        {
            get
            {
                if (_Paciente == null)
                {
                    _Paciente = new PacienteRepository(_repoContext, _configuration);
                }
                return _Paciente;
            }
        }
    }
}
