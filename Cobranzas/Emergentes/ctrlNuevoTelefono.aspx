<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlNuevoTelefono.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlNuevoTelefono" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h3>Por Favor Escriba el Nuevo Número de Teléfono:</h3>
    <table class="TablaDatos">
        <tr>
            <td>
                Código de Área
            </td>
            <td>
                Teléfono
            </td>
            <td rowspan="2">
                <asp:Button runat="server" ID="btnAgregar" Text="Agregar" OnClick="btnAgregar_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox runat="server" ID="txtCodigoArea" Width="80px"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtTelefono"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
