/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar() {
    if (window.parent.idOp == undefined) return;
    Clientes = {};
    $("#idPais").val(window.parent.Paises.join(","));
    Argumentos = searchToJSON();
    if (Argumentos.General != undefined) {
        $("#filaOperador").hide();
    }
    else {
        if (Argumentos.Supervisados == 0) {
            $("#filaOperador").hide();
        }
        //OperadoresSupervisados_lst(_OperadoresSupervisados_lst, Argumentos.idOperador);RA
        LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisados_lst", { idOperador: Argumentos.idOperador });
    }
    //Clientes_lst(_Clientes_lst);RA
    LlenarCombo({ Combo: "cboPais", Resultado: window.parent.Paises, TextoNull: "Todos", ValorNull: "%%%", DivResultado: "lstPais" });
    LlamarServicio(_Clientes_lst, "Clientes_lst");
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    $("input.fecha").val(new Date().ToString(Regional.FormatoFecha));
}
function _OperadoresSupervisados_lst(msg) {
    LlenarCombo({ Combo: "cboOperador", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre", TextoNull: "Todos", ValorNull: Argumentos.idOperador })
    $("#cboOperador").val(Argumentos.idOperador);
}
function _Clientes_lst(msg) {
    Clientes = msg.d;
    FiltrarPorPais();
}
function Ejecutar() {
    var Paises = ObtenerLista("lstPais").join(",");
    if (Paises=="") {
        var Paises = window.parent.Paises.join(",");
    }
    var idOperador = Argumentos.General != undefined ? Argumentos.idOperador : $("#cboOperador").val();
    LlamarServicio(_AnalisisPorCliente_rpt, "AnalisisPorCliente_rpt", {idPais:Paises, idCliente: Convert.ToInt32($("#cboCliente").val()), FechaIni: Convert.ToDateTime($("#dtpFechaIni").val()).ToString("JSON"), FechaFin: Convert.ToDateTime($("#dtpFechaFin").val()).ToString("JSON"), idOperador: idOperador, Supervisados: Argumentos.Supervisados == 1, General: Argumentos.General == 1 });
}
function _AnalisisPorCliente_rpt(msg) {
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Country", Campo: "Pais", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "PersonType", Campo: "TipoPersona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Total", Campo: "Total", Clase: "grdMonto", Ordenacion: true, Filtro: true },
            { Titulo: "0-30", Campo: "M0a30", Clase: "grdMonto", Ordenacion: true, Filtro: true },
            { Titulo: "30-45", Campo: "M30a45", Clase: "grdMonto", Ordenacion: true, Filtro: true },
            { Titulo: "45-60", Campo: "M45a60", Clase: "grdMonto", Ordenacion: true, Filtro: true },
            { Titulo: "60-90", Campo: "M60a90", Clase: "grdMonto", Ordenacion: true, Filtro: true },
            { Titulo: "90-120", Campo: "M90a120", Clase: "grdMonto", Ordenacion: true, Filtro: true },
            { Titulo: "120-180", Campo: "M120a180", Clase: "grdMonto", Ordenacion: true, Filtro: true },
            { Titulo: ">180", Campo: "M180ainf", Clase: "grdMonto", Ordenacion: true, Filtro: true },
            { Titulo: "LastManagement", Campo: "UltimaGestion", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "LastManagement", Campo: "FechaUltimaGestion", Clase: "grdFecha", Ordenacion: true, Filtro: true }
        ]
    });
}
function FiltrarPorPais() {
    var Pais = $("#cboPais").val();

    var ListaClientes = Pais == "%%%" ? Clientes : Clientes.where(function (a) { return a.Nombre.indexOf("(" + Pais + ")") != -1 });
    LlenarCombo({ Combo: "cboCliente", Resultado: ListaClientes, CampoId: "id", CampoValor: "Nombre", TextoNull: "Todos", ValorNull: "0" });
}

function SeleccionarPais() {
    var paises = ObtenerLista("lstPais");
    $("#idPais").val(paises);
    var CF = Clientes.where(function (a) { return paises.contains(a.idPais); });
    LlenarCombo({ Combo: "cboCliente", Resultado: CF, CampoId: "id", CampoValor: "Nombre", TextoNull: "«Todos»" });
}