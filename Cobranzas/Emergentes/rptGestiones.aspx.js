/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar() {
    if (window.parent.PersonasTocadas == undefined) return;
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    Argumentos = searchToJSON();
    Clientes = [];
    $("#idOperador").val(Argumentos.idOperador);
    $("input.fecha").val(AFechaMuyCorta(new Date()));
    LlamarServicio(_Status_lst, "Status_lst2", { idOperador: Argumentos.idOperador });
    LlenarCombo({ Combo: "cboPais", Resultado: window.parent.Paises, DivResultado: "lstPais" });
    //$("#cboPais").text("«Todos»");
    $("#cboPais").val("");

    LlamarServicio(_Clientes_lst, "Clientes_lst"); //MIGRACIÓN
    _Personas_Tocadas_lst({ d: window.parent.PersonasTocadas });
}
function _Status_lst(msg) {
    Status = msg.d;
    LlenarStatus(Status);
}
function LlenarStatus(msg) {
    var StatusFiltrado = msg;
    $("#cboStatus").html("");

    var Option = document.createElement("OPTION");
    $(Option).text("«Seleccione para agregar»");
    $(Option).val("");
    $("#cboStatus")[0].appendChild(Option);

    Option = document.createElement("OPTION");
    $(Option).text("«Todos»");
    $(Option).val("*");
    $("#cboStatus")[0].appendChild(Option);
    for (var i = 0; i < StatusFiltrado.length; i++) {
        Option = document.createElement("OPTION");
        $(Option).text(StatusFiltrado[i].Nombre);
        $(Option).val(StatusFiltrado[i].idStatus);
        $(Option)[0].style.backgroundColor = Colores[StatusFiltrado[i].Tipo];
        $("#cboStatus")[0].appendChild(Option);
    }
}
//function StatusSeleccionados() { se usa la funcion ObtenerLista
//    var lista = $("#lstStatus")[0];
//    var Elementos = lista.getElementsByTagName("SPAN");
//    var Result = [];
//    for (var i = 0; i < Elementos.length; i++) {
//        Result.push(Elementos[i].title);
//    }
//    return Result;
//}

function SeleccionarStatus() {
    var lista = $("#lstStatus")[0];
    var idStatus = $("#cboStatus").val();
    if (idStatus == "") return;

    var Elementos = lista.getElementsByTagName("SPAN");
    if (idStatus == "*") {
        lista = [];
        for (var i = Elementos.length - 1; i >= 0; i--) {
            Elementos[i].parentNode.removeChild(Elementos[i]);
        }
        return;
    }
    $("#cboStatus").val("");
    var textStatus = Status.where(function (a) { return a.idStatus == idStatus })[0].Nombre;
    for (var i = 0; i < Elementos.length; i++) {
        if (Elementos[i].title == idStatus) {
            return;
        }
    }
    var span = document.createElement("SPAN");
    $(span).text(textStatus);
    $(span)[0].title = idStatus;
    $(span)[0].className = "Telefono";
    $(span).click(function () { Eliminar(this); });
    lista.appendChild(span);
}
function Eliminar(t) {
    //this.style.display = "none";
    t.parentNode.removeChild(t);
}
function _Clientes_lst(msg) {
    Clientes = msg.d
    LlenarCombo({ Combo: "cboCliente", Resultado: Clientes, CampoId: "id", CampoValor: "Nombre", TextoNull: "«Todos»" });
}
function _Personas_Tocadas_lst(msg) {
    PersonasTocadas = msg.d;

    LlenarCombo({ Combo: "cboPersona", Resultado: PersonasTocadas, CampoId: "id", CampoValor: "Nombre", TextoNull: "«Todas»" });
}
function ConsultarGestiones() {
    var FechaDesde = Convert.ToDateTime($("#dtpFechaDesde").val()).ToString("JSON");
    var FechaHasta = Convert.ToDateTime($("#dtpFechaHasta").val()).ToString("JSON");
    var idCliente = $("#cboCliente").val();
    var idPersona = $("#cboPersona").val();
    if (idCliente == "") idCliente = 0;
    if (idPersona == "") idPersona = 0;
    var lista = ObtenerLista("lstStatus");
    var paises = ObtenerLista("lstPais").join(",");
    //Gestiones_rpt(_Gestiones_rpt, FechaDesde, FechaHasta, Argumentos.idOperador, Argumentos.Supervisados == 1, idCliente, idPersona, lista);RA
    LlamarServicio(_Gestiones_rpt, "Gestiones_rpt", { FechaDesde: FechaDesde, FechaHasta: FechaHasta, idPais: paises ,idOperador: Argumentos.idOperador, Supervisados: Argumentos.Supervisados == 1, idCliente: idCliente, idPersona: idPersona, Status: lista });
}
function _Gestiones_rpt(msg) {
    Tabla({
        Contenedor: "pnlGestiones",
        Resultado: msg.d,
        TodosSeleccionados: false,
        idSeleccion: "idGestion",
        DivExportar: "btnExpor",
        NombreExportar: "Gestiones",
        Campos: [
            { Titulo: " ", Campo: "Img", Clase: "grdImagen", Imagen: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFechaHora", Ordenacion: true, Filtro: true },
            { Titulo: "Status", Campo: "Status", Clase: "grdTexto", Ordenacion: true, Filtro: true, Color: "Tipo" },
            { Titulo: "Type", Campo: "Tipo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            //{ Titulo: "Description", Campo: "Descripcion", Clase: "grdTexto", Tooltip: function (a) { return "Cuentas:\n" + a.Cuentas.join("\n"); }, Ordenacion: true, Filtro: true },
            { Titulo: "Description", Campo: "Descripcion", Clase: "grdTexto", Tooltip: "Cuentas", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Country", Campo: "idPais", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ],
        FuncEliminar: "Eliminar(«idGestion»)"
    });
}
function FiltrarPersona() {
    var filtro = $("#txtFiltroPersona").val().toUpperCase();
    if (filtro == "") {
        LlenarCombo({ Combo: "cboPersona", Resultado: PersonasTocadas, CampoId: "idPersona", CampoValor: "Nombre", TextoNull: "<<Niguno>>" });
    } else {
        LlenarCombo({ Combo: "cboPersona", Resultado: PersonasTocadas.where(function (a) { return a.Nombre.toUpperCase().indexOf(filtro) != -1; }), CampoId: "idPersona", CampoValor: "Nombre" });
    }
}
function Preparar() {
    $("#idCliente").val($("#cboCliente").val());
    $("#idPersona").val($("#cboPersona").val());
    var Status = "";
    var SS = StatusSeleccionados();
    for (var i = 0; i < SS.length; i++) {
        Status = Status + "," + SS(i);
    }
    if (Status.length > 0) Status = Status.substring(1);
    $("#idStatus").val(Status);
    
    $("#idPais").val(ObtenerLista("lstPais").join(","));
    
}
function SeleccionarPais() {
    var paises = ObtenerLista("lstPais");
    var CF = Clientes.where(function (a) { return paises.contains(a.idPais); });
    var PF = PersonasTocadas.where(function (a) { return paises.contains(a.idPais); });
    var SF = Status.where(function (a) { return paises.contains(a.idPais); });
    LlenarCombo({ Combo: "cboCliente", Resultado: CF, CampoId: "id", CampoValor: "Nombre", TextoNull: "«Todos»" });
    LlenarCombo({ Combo: "cboPersona", Resultado: PF, CampoId: "id", CampoValor: "Nombre", TextoNull: "«Todas»" });
    LlenarStatus(SF);
}
