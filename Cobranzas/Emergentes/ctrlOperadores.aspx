<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlOperadores.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlOperadores" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlOperadores.aspx.js?ver=8" type="text/javascript"></script>
    <script src="/Scripts/Editor.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2><asp:Literal ID="Literal23" runat="server" Text="<%$ Resources:Recursos, OperatorsUnderSupervision %>"></asp:Literal></h2>
    <div id="tabs">
        <ul>
            <li><a href="#tabListado" id="pstListado"><asp:Literal ID="Literal24" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></a></li>
            <li><a href="#tabDetalles" id="pstDetalles"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Details %>"></asp:Literal></a></li>
        </ul>
        <div id="tabListado">
            <table style="width: 100%">
                <tr>
                    <td>
                        <h3><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></h3>
                    </td>
                    <td style="text-align: right;">
                        <input id="Button1" type="button" runat="server" class="btnAgregar32" title="<%$ Resources:Recursos, NewOperator %>" onclick="Nuevo();" />
                    </td>
                </tr>
            </table>
            <div id="pnlOperadores">
            </div>
        </div>
        <div id="tabDetalles">
            <input runat="server" type="hidden" id="idOperador" />
            <table style="width: 100%">
                <tr>
                    <td style="text-align: right;">
                        <input type="button" id="btnGuardar" onclick="Guardar();" class="btnGuardar32" runat="server" title="<%$ Resources:Recursos, Save %>" /><br />
                    </td>
                </tr>
            </table>
            <table class="TablaDatos">
                <tr>
                    <th>
                        <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Active1%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="checkbox" id="chkActivo" />
                    </td>
                    <th>
                        <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Code%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtCodigo" style="width: 80px" />
                    </td>
                </tr>
                <tr>
                    <th>
                        Login:
                    </th>
                    <td>
                        <input type="text" id="txtLogin" style="width: 100px" />
                    </td>
                    <th>
                        <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, Name%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtNombre" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, Email%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtCorreo" style="width: 200px" />
                    </td>
                    <th>
                        <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Charge%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtCargo" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, JoinDate%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtFechaIngreso" class="fecha" />
                    </td>
                    <th>
                        <asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:Recursos, DischargeDate%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtFechaEgreso" class="fecha" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal10" runat="server" Text="<%$ Resources:Recursos, EmailSignature%>"></asp:Literal>:
                    </th>
                    <td colspan="3" style="background-color: White;">
                        <textarea id="txtFirmaCorreos" rows="2" cols="90" style="width: 600px;"></textarea>
                        <span class="Comentario">Por favor dejar en blanco si quiere que el sistema Genere Automáticamente la firma.</span>
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal11" runat="server" Text="<%$ Resources:Recursos, Phones%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtTelefonos" style="width: 200px" />
                    </td>
                    <th>
                        <asp:Literal ID="Literal12" runat="server" Text="<%$ Resources:Recursos, Extension%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtExtension" style="width: 100px" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal13" runat="server" Text="<%$ Resources:Recursos, Country%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtPais" style="width: 100px" />
                    </td>
                    <th>
                        <asp:Literal ID="Literal14" runat="server" Text="<%$ Resources:Recursos, Office%>"></asp:Literal>:
                    </th>
                    <td>
                        <input type="text" id="txtZona" style="width: 100px" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal15" runat="server" Text="<%$ Resources:Recursos, Supervisor%>"></asp:Literal>:
                    </th>
                    <td>
                        <select id="cboSupervisor">
                        </select>
                    </td>
                    <th>
                        Permisos:</th>
                    <td>
                        <input type="checkbox" id="chkOperador" /> <span title="Un Operador Puede tener Cuentas Asignadas y Utilizar la Gestión Automática Asistida">Operador</span><br />
                        <input type="checkbox" id="chkSuperOperador" /> <span title="Un Operador Avanzado Puede Seleccionar las Cuentas a Gestionar">Operador Avanzado</span><br />
                        <input type="checkbox" id="chkBackOffice" /> <span title="Un BackOffice es un operador encargado de hacer tareas más complejas que pueden conllevar Tiempo">BackOffice</span><br />
                        <input type="checkbox" id="chkSupervisor" /> <span title="un Supervisor es alquel que tiene pesonal a su cargo y puede ver las pestañas de supervisión del sistema">Supervisor</span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
