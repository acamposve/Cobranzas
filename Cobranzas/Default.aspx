<%@ Page Language="C#" MasterPageFile="~/Site.master" CodeBehind="Default.aspx.cs" Inherits="Cobranzas._Default" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="Server">
<script type="text/javascript" >
    function setCookieCulture()
    {
        document.cookie = 'Culture=' + $('#ddlIdioma').val();
    }
</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="body" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server" />
    <div class="Login">
        <table>
            <tr>
                <td colspan="3">
                </td>
            </tr> 
            <tr>
                <td>
                </td>
                <td style="width: 300px; height: 300px;">
                    <div style="width: 100%; height: 100%; background-color: #000040; color: White; border-radius: 10px;box-shadow: 10px 10px 10px #000000">
                        <table style="width: 100%; height: 100%;">
                            <tr>
                                <td colspan="2">
                                    <img src="/Img/Logo.jpg" alt="Logo Veconinter" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <h2><asp:Literal ID="Literal1" Text="<%$ Resources:Recursos, titSystem %>" runat="server" /></h2>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Literal Text="<%$ Resources:Recursos, titLogin %>" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <th style="text-align: left;">
                                    <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Recursos, User %>"></asp:Label>
                                </th>
                                <td style="text-align: left;">
                                    <asp:TextBox ID="txtUsuario" runat="server" requerido="true" onblur="return blurText(this);" onkeypress="return kpText(event);" onfocus="return focusText(this);"></asp:TextBox>
                                </td>
                            </tr>
                            <!--tr>
                                <th style="text-align: left;">
                                    <asp:Label ID="Label2" runat="server" Text="<%$ Resources:Recursos, Password %>"></asp:Label>
                                </th>
                                <td style="text-align: left;">
                                    <asp:TextBox ID="txtContraseña" runat="server" TextMode="Password" requerido="true" onblur="return blurPass(this);" onkeypress="return kpPass(event);" onfocus="return focusPass(this);"></asp:TextBox>
                                </td>
                            </tr-->
                            <tr>
                                <th style="text-align: left;">
                                    <asp:Label ID="Label3" runat="server" Text="<%$ Resources:Recursos, Language %>"></asp:Label>
                                </th>
                                <td style="text-align: left;">
                                    <asp:DropDownList ID="ddlIdioma" runat="server" requerido="false" valido="true" AutoPostBack="True" onchange="setCookieCulture();">
                                        <asp:ListItem Text="<%$ Resources:Recursos, Spanish %>" Value="es-VE"></asp:ListItem>
                                        <asp:ListItem Text="<%$ Resources:Recursos, English %>" Value="en-US"></asp:ListItem>
                                        <asp:ListItem Text="<%$ Resources:Recursos, Portuguese %>" Value="pt-BR"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                                <td style="text-align:right;">
                                    <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="<%$ Resources:Recursos, btnOK %>" OnClientClick="setCookieCulture();" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">

    </script>
</asp:Content>
