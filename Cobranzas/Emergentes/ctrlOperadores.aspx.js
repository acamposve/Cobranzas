/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar()
{
    $("#tabs").tabs({ show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui)
        {
            if (ui.newPanel[0].id == "tabDetalles" && $('#idOperador').val() == "")
            {
                Mensaje({ mensaje: "No ha seleccionado ningún ítem para ver" });
                return false;
            }
        }
    });
    new nicEditor().panelInstance('txtFirmaCorreos');
    Argumentos = searchToJSON();
    Refrescar();
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    $("#idOperador").val(Argumentos.idOperador);
}
function Refrescar()
{
    LlamarServicio(_OperadoresSupervisadosCompleto_lst, "OperadoresSupervisadosCompleto_lst", { idOperador: Argumentos.idOperador });
    LlamarServicio(_OperadoresSupervisadosConSupervisor_lst, "OperadoresSupervisadosConSupervisor_lst", { idOperador: Argumentos.idOperador });
}
function _OperadoresSupervisadosConSupervisor_lst(msg)
{
    LlenarCombo({ Combo: "cboSupervisor", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre", TextoNull: "«Seleccione»", ValorNull: "" });
}
function _OperadoresSupervisadosCompleto_lst(msg)
{

    Tabla({
        Contenedor: "pnlOperadores",
        Resultado: msg.d,
        TodosSeleccionados: false,
        //idSeleccion: "idOperador",
        LimitarAltura: 400,
        Campos: [
            { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Group", Campo: "Grupo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Email", Campo: "Correo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "StartDate", Campo: "FechaIngreso", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "EndDate", Campo: "FechaFin", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Position", Campo: "Cargo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Supervisor", Campo: "Supervisor", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Active", Campo: "Activo", Clase: "grdBooleano", Ordenacion: true, Filtro: true }
        ],
        FuncSeleccionar: "SeleccionarOperador(«idOperador»);",
        FuncEliminar: "EliminarOperador(«idOperador»);"
    });
}
function SeleccionarOperador(idOperador)
{
    LlamarServicio(_Operadores_sel, "Operadores_sel", { idOperador: idOperador });
}

function EliminarOperador(idOperador)
{
    LlamarServicio(_Operadores_del, "Operadores_del", { idOperador: idOperador });
}

function _Operadores_del(msg)
{
    Mensaje({ mensaje: "El Operador ha sido eliminado Satisfactoriamente" });
    Refrescar();
}

function _Operadores_sel(msg)
{
    var Result = msg.d;
    $("#chkActivo")[0].checked = Result.Activo;
    $("#txtCodigo").val(Result.Codigo);
    $("#txtLogin").val(Result.Login);
    $("#txtNombre").val(Result.Nombre);
    $("#txtCorreo").val(Result.Correo);
    $("#txtCargo").val(Result.Cargo);
    $("#txtFechaIngreso").val(AFechaMuyCorta(Result.FechaIngreso));
    $("#txtFechaEgreso").val(AFechaMuyCorta(Result.FechaEgreso));
    nicEditors.findEditor('txtFirmaCorreos').setContent(Result.FirmaCorreo);
    $("#txtTelefonos").val(Result.Telefonos);
    $("#txtExtension").val(Result.Extension);
    $("#txtPais").val(Result.Pais);
    $("#txtZona").val(Result.Zona);
    $("#cboSupervisor").val(Result.idSupervisor);
    $("#idOperador").val(Result.idOperador);
    var Tipo = Result.Tipo;
    $("#chkOperador")[0].checked = Tipo.indexOf("OP") != -1;
    $("#chkSuperOperador")[0].checked = Tipo.indexOf("SO") != -1;
    $("#chkBackOffice")[0].checked = Tipo.indexOf("BO") != -1;
    $("#chkSupervisor")[0].checked = Tipo.indexOf("SU") != -1;
    $('#pstDetalles').click();
}


function Nuevo()
{
    $("#cboSupervisor").val(Argumentos.idOperador);
    $("#chkActivo")[0].checked = true;
    $("#txtCodigo").val("");
    $("#txtLogin").val("");
    $("#txtNombre").val("");
    $("#txtCorreo").val("");
    $("#txtCargo").val("");
    $("#txtFechaIngreso").val("");
    $("#txtFechaEgreso").val("");
    nicEditors.findEditor('txtFirmaCorreos').setContent("");
    $("#txtTelefonos").val("");
    $("#txtExtension").val("");
    $("#txtPais").val("");
    $("#txtZona").val("");
    $("#idOperador").val(0);
    $("#chkOperador")[0].checked = true;
    $("#chkSuperOperador")[0].checked = true;
    $("#chkBackOffice")[0].checked = true;
    $("#chkSupervisor")[0].checked = true;

    $('#pstDetalles').click();
}
function Guardar()
{
    if ($("#txtFechaIngreso").val()=="")
    {
        Mensaje({ mensaje: "Debe asignar una fecha de ingreso." });
        return false;
    }

    var Result = {};
    Result.Activo = $("#chkActivo")[0].checked;
    Result.Codigo = $("#txtCodigo").val();
    Result.Login = $("#txtLogin").val();
    Result.Nombre = $("#txtNombre").val();
    Result.Correo = $("#txtCorreo").val();
    Result.Cargo = $("#txtCargo").val();
    Result.FechaIngreso = aJSON($("#txtFechaIngreso").val());
    Result.FechaEgreso = aJSON($("#txtFechaEgreso").val());
    nicEditors.findEditor("txtFirmaCorreos").saveContent();
    Result.FirmaCorreo = $("#txtFirmaCorreos").val();
    Result.Telefonos = $("#txtTelefonos").val();
    Result.Extension = $("#txtExtension").val();
    Result.Pais = $("#txtPais").val();
    Result.Zona = $("#txtZona").val();
    Result.idSupervisor = $("#cboSupervisor").val();
    Result.idOperador = $("#idOperador").val();
    Result.Tipo = "";
    if ($("#chkOperador")[0].checked) Result.Tipo += ",OP";
    if ($("#chkSuperOperador")[0].checked) Result.Tipo += ",SO";
    if ($("#chkBackOffice")[0].checked) Result.Tipo += ",BO";
    if ($("#chkSupervisor")[0].checked) Result.Tipo += ",SU";

    if (Result.Tipo.length > 0) Result.Tipo = Result.Tipo.substring(1);
    LlamarServicio(_Operadores_sav, "Operadores_sav", { Operador: Result });
}

function _Operadores_sav(msg)
{
    Mensaje({ mensaje: "El Operador ha sido Guardado Satisfactoriamente" });
    Refrescar();
}