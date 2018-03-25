<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlNuevoReclamo.aspx.cs" Inherits="Cobranzas.ctrlNuevoReclamo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlNuevoReclamo.aspx.js?ver=4" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Crear Nuevo Reclamo </h2>
    <table style="width: 100%;">
        <tr>
            <th>
                Motivo:
            </th>
            <td>
                <select id="cboMotivosReclamo" style="width: 300px;">
                </select>
            </td>
            <th>
                Fecha:
            </th>
            <td>
                <span id="lblFechaReclamo"/>
            </td>
        </tr>
        <tr>
            <th>
                Descripción:
            </th>
            <td colspan="3">
                <textarea id="txtDescripcionReclamo" cols="80" name="S1" rows="3" style="width: 90%;" runat="server" onblur="return blurText(this);" onfocus="return focusText(this);" onkeydown="return kdText(event);" onkeypress="return kpText(event);"></textarea>
            </td>
        </tr>
        <tr><!--th>Asignar a Dpto:</th>
        <td><select id="cboDepartamento"></select></td-->
        <th>Soportes:</th>
        <td colspan="3">
            <input id="Button1" type="button" runat="server" value="Archivo desde el Equipo..." onclick="Adjuntar();" />
            <div id="Adjuntos">
            </div>
        </td>
        </tr>
        <tr>
            <th colspan="4">
                Cuentas:
            </th>
        </tr>
        <tr>
            <td colspan="4">
                <div id="pnlPeCuentasReclamo">
                </div>
            </td>
        </tr>
    </table>
    <p>
        <input type="button" id="btnCrear" value="Crear" onclick="Insertar_Reclamo()" />
    </p>
</asp:Content>
