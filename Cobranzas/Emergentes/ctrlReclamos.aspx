<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlReclamos.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlReclamos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlReclamos.aspx.js?ver=1" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Reclamo </h2>
    <table class="TablaDatos">
        <tr>
            <th>
                Codigo:
            </th>
            <td>
                <span id="lblCodigo" />
            </td>
            <th>
                Fecha:
            </th>
            <td>
                <span id="lblFecha"></span>
            </td>
        </tr>
        <tr>
            <th>
                Motivo:
            </th>
            <td colspan="3">
                <span id="lblMotivo" />
            </td>
        </tr>
        <tr>
            <th>
                Descripción:
            </th>
            <td colspan="3">
                <span runat="server" id="lblDescripcion"></span>
            </td>
        </tr>
        <tr>
            <th>
                Departamento Asignado:
            </th>
            <td>
                <span runat="server" id="lblDepartamento"></span>
            </td>
            <th>
                Status
            </th>
            <td>
                <span runat="server" id="lblStatus"></span>
            </td>
        </tr>
        <tr>
            <th>
                Soportes:
            </th>
            <td colspan="4">
                <div id="pnlSoportes">
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
    <input type="button" value="Mostrar Historial" onclick="MostrarHistorial();" />
</asp:Content>
