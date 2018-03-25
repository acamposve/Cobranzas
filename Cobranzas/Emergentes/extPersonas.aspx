<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="extPersonas.aspx.cs" Inherits="Cobranzas.extPersonas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script>
function Importar(a){
    window.parent.Importar(a, 1,'VEN');
    window.parent.CerrarEmergente();
}
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Otras Personas</h2>
    <table class="TablaDatos">
        <tr>
            <th>
                Búsqueda por Código:
            </th>
            <td>
                <asp:TextBox runat="server" ID="txtCodigo"></asp:TextBox>
            </td>
            <td rowspan="2">
                <asp:Button runat="server" ID="btnBuscar" Text="Buscar" onclick="btnBuscar_Click" />
            </td>
        </tr>
        <tr>
            <th>
                Búsqueda por Nombre:
            </th>
            <td>
                <asp:TextBox runat="server" ID="txtNombre"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div runat="server" id="divContenido"></div>
</asp:Content>
