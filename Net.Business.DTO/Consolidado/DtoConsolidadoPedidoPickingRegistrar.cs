using Net.Business.Entities;
using System;
using System.Collections.Generic;

namespace Net.Business.DTO
{
    public class DtoConsolidadoPedidoPickingRegistrar: EntityBase
    {
        public int idconsolidado { get; set; }
        public string codpedido { get; set; }
        public string codproducto { get; set; }
        public decimal cantidad { get; set; }
        public decimal cantidadpicking { get; set; }
        public string lote { get; set; }
        public DateTime? fechavencimiento { get; set; }
        public string codalmacen { get; set; }
        public int? ubicacion { get; set; }
        public string codusuarioapu { get; set; }
        public int estado { get; set; }

        public BE_ConsolidadoPedidoPicking RetornaConsolidadoPedidoPicking()
        {
            return new BE_ConsolidadoPedidoPicking
            {
                idconsolidado = this.idconsolidado,
                codpedido = this.codpedido,
                codproducto = this.codproducto,
                cantidad = this.cantidad,
                cantidadpicking = this.cantidadpicking,
                lote = this.lote,
                fechavencimiento = this.fechavencimiento,
                codalmacen = this.codalmacen,
                ubicacion = this.ubicacion,
                codusuarioapu = this.codusuarioapu,
                estado = this.estado,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }

    public class DtoConsolidadoPedidoPickingRegistrarMasivo
    {
        public List<DtoConsolidadoPedidoPickingRegistrar> listConsolidadoPedidoPicking { get; set; }

        public List<BE_ConsolidadoPedidoPicking> RetornaConsolidadoPedidoPickingMasivo()
        {

            var list = new List<BE_ConsolidadoPedidoPicking>();

            foreach (DtoConsolidadoPedidoPickingRegistrar item in this.listConsolidadoPedidoPicking)
            {
                var itemnew = new BE_ConsolidadoPedidoPicking
                {
                    idconsolidado = item.idconsolidado,
                    codpedido = item.codpedido,
                    codproducto = item.codproducto,
                    cantidad = item.cantidad,
                    cantidadpicking = item.cantidadpicking,
                    lote = item.lote,
                    fechavencimiento = item.fechavencimiento,
                    codalmacen = item.codalmacen,
                    ubicacion = item.ubicacion,
                    codusuarioapu = item.codusuarioapu,
                    estado = item.estado,
                    RegIdUsuario = item.RegIdUsuario
                };

                list.Add(itemnew);
            }

            return list;
        }
    }
}
