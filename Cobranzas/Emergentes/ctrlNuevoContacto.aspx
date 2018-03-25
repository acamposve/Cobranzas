<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlNuevoContacto.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlNuevoContacto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Crear Nuevo Contacto</h2>
    <table class="TablaDatos">
        <tr>
            <th>
                Nombre:
            </th>
            <td>
                <asp:TextBox runat="server" ID="txtNombre" Width="300"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th>
                Cargo:
            </th>
            <td>
                <asp:TextBox runat="server" ID="txtCargo" Width="300"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th>
                Email:
            </th>
            <td>
                <asp:TextBox runat="server" ID="txtEmail" Width="300"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th>
                Teléfonos:
            </th>
            <td>
                Ingrese varios teléfonos separados por (;)
                <asp:TextBox runat="server" ID="txtTelefonos" Width="300"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button runat="server" ID="btnGuardar" Text="Guardar" onclick="btnGuardar_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
