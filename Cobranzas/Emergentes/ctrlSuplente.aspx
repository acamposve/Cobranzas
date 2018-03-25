<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlSuplente.aspx.cs" Inherits="Cobranzas.ctrlSuplente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlSuplente.aspx.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <input type="hidden" id="idPersona" runat="server" />
    <h2>Configurar nuevo Suplente:</h2>
    <table class="TablaDatos">
        <tr>
            <th>
                Fecha Inicio:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaInicio" />
            </td>
            <th>
                Fecha Fin:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaFin" />
            </td>
        </tr>
        <tr>
            <th>
                Operador a Suplantar:
            </th>
            <td>
                <select id="cboSuplantado">
                </select>
            </td>
            <th>
                Suplente:
            </th>
            <td>
                <select id="cboSuplente">
                </select>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="text-align:center;">
                <input type="button" id="Agregar()" value="Agregar" onclick="Agregar();" />
            </td>
        </tr>
    </table>
</asp:Content>
