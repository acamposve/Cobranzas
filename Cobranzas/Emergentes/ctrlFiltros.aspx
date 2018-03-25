<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlFiltros.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlFiltros" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlFiltros.aspx.js?ver=1" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2><asp:Literal Text="<%$ Resources:Recursos, Filters %>" runat="server" /></h2>
    <input type="hidden" id="idFiltro" /> <input type="hidden" id="idOperador" />
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal1" Text="<%$ Resources:Recursos, MailFrom %>" runat="server" />:
            </th>
            <td>
                <input type="text" id="txtDe" />
            </td>
            <td rowspan="2" style="width: 0px;">
                <input type="button" id="btnAgregarFiltro" class="btnAgregar32" onclick="Agregar();" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal2" Text="<%$ Resources:Recursos, Person %>" runat="server" />:
            </th>
            <td>
                Filtro:<input type="text" id="txtFiltroPersona" onkeyup="FiltrarPersona()" />
                <select id="cboPersona" style="max-width: 300px; ">
                </select>
            </td>
        </tr>
    </table>
    <div id="pnlFiltros">
    </div>
</asp:Content>
