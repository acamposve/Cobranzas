<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PagosPendientes.aspx.cs" Inherits="AccesoExterno.PagosPendientes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h1>Pagos Pendientes</h1>
    </div>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" DataKeyNames="idpago" DataSourceID="SqlDataSource1" GridLines="Vertical">
        <AlternatingRowStyle BackColor="#DCDCDC" />
        <Columns>
            <asp:HyperLinkField DataNavigateUrlFields="idPago,idPais" DataNavigateUrlFormatString="SoportesPagos.aspx?idPago={0}&amp;idPais={1}" DataTextFormatString="Detalles" Text="Detalles" />
            <asp:BoundField DataField="idpago" HeaderText="idpago" InsertVisible="False" ReadOnly="True" SortExpression="idpago" />
            <asp:BoundField DataField="idPais" HeaderText="idPais" SortExpression="idPais" />
            <asp:BoundField DataField="Codigo" HeaderText="Codigo" SortExpression="Codigo" />
            <asp:BoundField DataField="Nombre" HeaderText="Nombre" SortExpression="Nombre" />
            <asp:BoundField DataField="Nombre1" HeaderText="Nombre1" SortExpression="Nombre1" />
            <asp:BoundField DataField="NroCuenta" HeaderText="NroCuenta" SortExpression="NroCuenta" />
            <asp:BoundField DataField="MontoCheque" HeaderText="MontoCheque" SortExpression="MontoCheque" />
            <asp:BoundField DataField="MontoEfectivo" HeaderText="MontoEfectivo" SortExpression="MontoEfectivo" />
            <asp:BoundField DataField="MontoTotal" HeaderText="MontoTotal" ReadOnly="True" SortExpression="MontoTotal" />
            <asp:BoundField DataField="Referencia" HeaderText="Referencia" SortExpression="Referencia" />
            <asp:BoundField DataField="Descripcion" HeaderText="Descripcion" SortExpression="Descripcion" />
            <asp:BoundField DataField="idMoneda" HeaderText="idMoneda" SortExpression="idMoneda" />
        </Columns>
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#F1F1F1" />
        <SortedAscendingHeaderStyle BackColor="#0000A9" />
        <SortedDescendingCellStyle BackColor="#CAC9C9" />
        <SortedDescendingHeaderStyle BackColor="#000065" />
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CobranzasConnectionString %>" SelectCommand="SELECT p.idpago, PE.idPais, pe.Codigo,pe.Nombre , b.Nombre , bp.NroCuenta, p.MontoCheque ,p.MontoEfectivo , p.MontoTotal, p.Referencia , p.Descripcion , p.idMoneda FROM  Pagos as p inner join bancospropios as bp on p.idbancopropio = bp.idBancoPropio inner join Bancos as b on bp.idBanco=b.idBanco inner join personas as pe on p.idpersona=pe.idpersona where pe.idPais<>'VEN' and p.idStatusPago=1"></asp:SqlDataSource>
    </form>
</body>
</html>
