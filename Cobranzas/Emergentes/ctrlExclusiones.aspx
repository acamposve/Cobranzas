<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlExclusiones.aspx.cs" Inherits="Cobranzas.ctrlExclusiones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlExclusiones.aspx.js?ver=6" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table style="width:100%">
        <tr>
            <td>
                <h2>Listado de exclusiones:</h2>
            </td>
            <td style="text-align: right;">
                <asp:Button runat="server" ID="btnExportar" OnClick="btnExportar_Click" Text="Exportar" />
            </td>
        </tr>
    </table>
    <div id="tabs">
        <ul>
            <li><a href="#tabPersonas" id="pstPersonas"><asp:Literal ID="Literal4" runat="server" Text="Personas" /></a></li>
            <li><a href="#tabCuentas" id="pstCuentas"><asp:Literal ID="Literal1" runat="server" Text="Cuentas" /></a></li>
        </ul>
        <div id="tabPersonas">
            <h3>Personas</h3>
            <div id="pnlPersonas">
            </div>
            <input type="button" value="Desincorporar de la lista" onclick="Desincorporar_Personas();" /><br />
            <input type="button" value="Aprobar Exclusión" onclick="Aprobar_Personas();" /><br />
            Código Persona: <input type="text" id="txtPersona" /><input type="button" value="Incorporar" onclick="Incorporar_Persona();" />
        </div>
        <div id="tabCuentas">
            <h3>Cuentas</h3>
            <div id="pnlCuentas">
            </div>
            <input type="button" value="Desincorporar de la lista" onclick="Desincorporar_Cuentas();" /><br />
            <input type="button" value="Aprobar Exclusión" onclick="Aprobar_Cuentas();" /><br />
            Código Cuenta: <input type="text" id="txtCuenta" /><input type="button" value="Incorporar" onclick="Incorporar_Cuenta();" />
        </div>
    </div>
</asp:Content>
