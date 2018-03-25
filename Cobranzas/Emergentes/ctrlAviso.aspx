<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlAviso.aspx.cs" Inherits="Cobranzas.ctrlAviso" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlAviso.aspx.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <input type="hidden" id="idPersona" runat="server" />
    <h2>Recordatorio:</h2>
    <table class="TablaDatos">
        <tr>
            <th>
                Mensaje:
            </th>
            <td colspan="3">
                <asp:Label runat="server" ID="lblAviso" />
            </td>
        </tr>
        <tr>
            <th>
                Hora Creación:
            </th>
            <td>
                <asp:Label runat="server" ID="lblCreacion" />
            </td>
            <th>
                Hora Original:
            </th>
            <td>
                <asp:Label runat="server" ID="lblOriginal" />
            </td>
        </tr>
        <tr>
            <th>
                Hora:
            </th>
            <td>
                <asp:Label runat="server" ID="lblHora" />
            </td>
            <th>
                Usuario Creación:
            </th>
            <td>
                <asp:Label runat="server" ID="lblUsuario" />
            </td>
        </tr>
        <tr>
            <th>
                Persona:
            </th>
            <td colspan="3">
                <asp:Button runat="server" ID="btnCuenta" CssClass="Telefono" OnClick="btnCuenta_Click" />
            </td>
        </tr>
        <tr><th>Comentario de Cierre:</th><td colspan="3"> <asp:TextBox runat="server" ID="txtComentario" Width="90%" Rows="2" MaxLength ="500" TextMode="MultiLine"></asp:TextBox></td></tr>
        <tr>
            <td colspan="4" style="text-align: center;">

                <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" OnClick="btnAceptar_Click" />
                
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                <asp:Button ID="btnPosponer" runat="server" Text="Posponer" OnClick="btnPosponer_Click" />
                <asp:Button ID="btnIgnorar" runat="server" Text="Ignorar" OnClick="btnIgnorar_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="4" style="text-align: center;">
                <asp:Button ID="btnAsignarPrimero" runat="server" Text="Asignar de Primero" OnClick="btnAsignarPrimero_Click" />
                <asp:Button ID="btnAsignarUltimo" runat="server" Text="Asignar de Último" OnClick="btnAsignarUltimo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
