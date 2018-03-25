<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Personas.aspx.cs" Inherits="SCIInterfaz.Personas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Estilos.css" rel="stylesheet" type="text/css" />
<script>
function Importar(a){
    window.parent.Importar(a, 1,'VEN');
    window.parent.CerrarEmergente();
}
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Personas sin Deuda</h2>
    <table class="TablaDatos">
        <tr>
            <th>País:</th>
            <td colspan="2">
                <asp:DropDownList runat="server" ID="cboPais">
                    <asp:ListItem Text="Venezuela" Value="VEN" Selected="True" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <th>
                Búsqueda por Código:
            </th>
            <td>
                <asp:TextBox runat="server" ID="txtCodigo"></asp:TextBox>
            </td>
            <td>
                <asp:Button runat="server" ID="btnBuscarPorCódigo" Text="Buscar por Código" onclick="btnBuscarPorCódigo_Click" />
            </td>
        </tr>
        <tr>
            <th>
                Búsqueda por Nombre:
            </th>
            <td>
                <asp:TextBox runat="server" ID="txtNombre"></asp:TextBox>
            </td>
            <td>
                <asp:Button runat="server" ID="btnBuscarPorNombre" Text="Buscar por Nombre" onclick="btnBuscarPorNombre_Click" />
            </td>
        </tr>
    </table>
    <div runat="server" id="divContenido"></div>
</asp:Content>
