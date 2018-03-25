<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlCompromisos.aspx.cs" Inherits="Cobranzas.ctrlCompromisos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlCompromisos.aspx.js?ver=4" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
    <style type="text/css">
        .fecha
        {
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table>
        <tr>
            <td colspan="3">
                <h2>Compromisos de Pago</h2>
            </td>
            <td style="text-align: right;">
                <input type="button" value="Guardar" id="btnGuardar" onclick="Guardar();" />
            </td>
        </tr>
    </table>
    <table class="TablaDatos">
        <tr>
            <th>
                Descripción:
            </th>
            <td colspan="3">
                <textarea id="txtDescripcion" rows="2" cols="80"></textarea>
            </td>
            <td rowspan="3">
                <input type="button" value="Crear Equitativo" onclick="Crear();" /><input type="button" value="Crear Por Cuentas" onclick="Crear2();" />
            </td>
        </tr>
        <tr>
            <th>
                Fecha Inicial:
            </th>
            <td>
                <input type="text" class="fecha" value="" id="dtpFechaInicial" />
            </td>
            <th>
                Cantidad de Cuotas:
            </th>
            <td>
                <input type="text" id="txtCantidad" value="1" style="width: 80px" />
            </td>
        </tr>
        <tr>
            <th>
                Monto Total del Compromiso
            </th>
            <td>
                <input type="text" id="txtMontoTotal" value="1" style="width: 80px" />
            </td>
            <th>
                Monto de la Cuota
            </th>
            <td>
                <input type="text" id="txtMontoCuota" value="1" style="width: 80px" disabled="disabled" />
            </td>
        </tr>
    </table>
    <div id="pnlPrincipal">
    </div>
</asp:Content>
