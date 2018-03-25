/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Editor.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
function Inicializar() {
    Argumentos = searchToJSON();
    if ($("#PideParametros").val() == "1") return;
    LlamarServicio(_Reportes_sel, "Reportes_sel", { Reporte: $("#Origen").val(), idTipoReporte: Argumentos.idTipoReporte });
    //    if (Argumentos.idPersona)
    //    {
    //        Reportes_sel(_Reportes_sel, Argumentos.idPersona, Argumentos.idTipoReporte);
    //    }
    //    else if (Argumentos.idCuenta)
    //    {
    //        Reportes_sel(_Reportes_sel, Argumentos.idCuenta, Argumentos.idTipoReporte);
    //    }
}
function _Reportes_sel(msg) {
    TablaDataSet(msg.d, 'pnlResultado');
}
function LlenarClientes() {
    LlamarServicio(_Clientes_lst, 'Clientes_lst', {});
}
function _Clientes_lst(msg) {
    LlenarCombo({ Combo: "idCliente", Resultado: msg.d, CampoId: "id", CampoValor: "Nombre", TextoNull: "«Todos»", ValorNull: 0 })
}