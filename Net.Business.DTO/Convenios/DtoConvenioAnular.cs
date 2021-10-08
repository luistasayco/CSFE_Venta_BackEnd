using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoConvenioAnular
    {
        public int idconvenio { get; set; }
        public int idUsuario { get; set; }

        public BE_ConveniosListaPrecio RetornaComprobanteDarBaja()
        {

            var obj = new BE_ConveniosListaPrecio();
            obj.idconvenio = idconvenio;
            obj.regcreateidusuario = idUsuario;
            return obj;
        }
    }
}
