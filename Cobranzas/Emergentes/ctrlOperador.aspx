<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlOperador.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlOperador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Actualizar Datos</h2>
    <p>Para empezar a Recibir los Correos en la Bandeja del sistema, necesitamos que confirme su contraseña de Correo.</p>
    <table class="TablaDatos">
        <tr>
            <th>
                Contraseña de Correo:
            </th>
            <td>
                <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnEnviar" runat="server" value="Enviar" OnClick="btnEnviar_Click" Text="Enviar" />
            </td>
        </tr>
    </table>
</asp:Content>
