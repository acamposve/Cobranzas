using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class rptRankingEjecutivo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = new System.Data.DataTable();
            DateTime fecha1 = DateTime.Now;
            DateTime fecha2 = DateTime.Now;
            DateTime fecha3 = DateTime.Now;




            for (int i = 1; i < 12; i++)
            {


                string Mes = "";
                Mes = sMes(i);

                Table tablaOperadores = new Table();
                TableRow trTituloMes = new TableRow();
                TableRow trTitulos = new TableRow();
                TableHeaderCell tdTituloNombre = new TableHeaderCell();
                TableHeaderCell tdTituloMes = new TableHeaderCell();
                TableHeaderCell tdMontoRecaudado = new TableHeaderCell();
                TableHeaderCell tdMontoMeta = new TableHeaderCell();
                TableHeaderCell tdPorcentaje = new TableHeaderCell();

                tablaOperadores.BorderWidth = 1;
                tablaOperadores.BorderColor = System.Drawing.Color.Black;
                tablaOperadores.Width = new Unit("100%");



                tdTituloNombre.Text = "Nombre";
                tdMontoRecaudado.Text = "Monto Recaudado";
                tdMontoMeta.Text = "Monto Meta";
                tdPorcentaje.Text = "Porcentaje";
                tdTituloMes.ColumnSpan = 4;
                tdTituloMes.HorizontalAlign = HorizontalAlign.Center;
                tdTituloMes.Text = Mes;

                fecha1 = DateTime.Parse(DateTime.Now.Year + "/" + i + "/06");                                               
                fecha2 = fecha1.AddMonths(1);
                dt = Negocios.ListarOperadoresSUpervisor(Sesion.Operador.idOperador, fecha1, fecha2);

                
                trTituloMes.Controls.Add(tdTituloMes);
                trTitulos.Controls.Add(tdTituloNombre);
                trTitulos.Controls.Add(tdMontoRecaudado);
                trTitulos.Controls.Add(tdMontoMeta);
                trTitulos.Controls.Add(tdPorcentaje);
                tablaOperadores.Controls.Add(trTituloMes);
                tablaOperadores.Controls.Add(trTitulos);


                string[] datosOperador = new string[dt.Rows.Count];
                Decimal[] datosPorcentaje = new Decimal[dt.Rows.Count];
                string[] datosidOperador = new string[dt.Rows.Count];
                

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    

                    wsCobranzas ws = new wsCobranzas();
                    var Listado = ws.Ranking_Operador_lst(int.Parse(dt.Rows[j]["idOperador"].ToString()) , fecha1, fecha2);



                    datosOperador[j] = Listado[0].Operador.ToString();
                    datosPorcentaje[j] = Listado[0].Porc;
                    datosidOperador[j] = Listado[0].ToString();




                    TableRow trContenido = new TableRow();
                    TableCell tdContenidoNombre = new TableCell();
                    TableCell tdContenidoMontoRecaudado = new TableCell();
                    TableCell tdContenidoMontoMeta = new TableCell();
                    TableCell tdContenidoPorcentaje = new TableCell();
                    tdContenidoNombre.Text = Listado[0].Operador.ToString();
                    tdContenidoMontoRecaudado.Text = Listado[0].Real.ToString();
                    tdContenidoMontoMeta.Text = Listado[0].Meta.ToString();
                    tdContenidoPorcentaje.Text = Listado[0].Porc.ToString();
                    trContenido.Controls.Add(tdContenidoNombre);
                    trContenido.Controls.Add(tdContenidoMontoRecaudado);
                    trContenido.Controls.Add(tdContenidoMontoMeta);
                    trContenido.Controls.Add(tdContenidoPorcentaje);
                    tablaOperadores.Controls.Add(trContenido);
                }




                Image imgChart = new Image();
                imgChart.ImageUrl = Negocios.MostrarGraficoTotalPorcentajePorOperador(datosOperador, datosPorcentaje, datosidOperador, Mes);

                pnTablasOperadores.Controls.Add(tablaOperadores);
                pnTablasOperadores.Controls.Add(imgChart);




            }




            for (int i = 1; i < 12; i += 3)
            {



                string Mes2 = "";
                Mes2 = TriMes(i);

                Table tablaOperadores = new Table();
                TableRow trTituloMes = new TableRow();
                TableRow trTitulos = new TableRow();
                TableHeaderCell tdTituloNombre = new TableHeaderCell();
                TableHeaderCell tdTituloMes = new TableHeaderCell();
                TableHeaderCell tdMontoRecaudado = new TableHeaderCell();
                TableHeaderCell tdMontoMeta = new TableHeaderCell();
                TableHeaderCell tdPorcentaje = new TableHeaderCell();

                tablaOperadores.BorderWidth = 1;
                tablaOperadores.BorderColor = System.Drawing.Color.Black;
                tablaOperadores.Width = new Unit("100%");



                tdTituloNombre.Text = "Nombre";
                tdMontoRecaudado.Text = "Monto Recaudado";
                tdMontoMeta.Text = "Monto Meta";
                tdPorcentaje.Text = "Porcentaje";
                tdTituloMes.ColumnSpan = 4;
                tdTituloMes.HorizontalAlign = HorizontalAlign.Center;
                tdTituloMes.Text = Mes2;

                fecha1 = DateTime.Parse(DateTime.Now.Year + "/" + i + "/06");
                fecha2 = fecha1.AddMonths(3);
                dt = Negocios.ListarOperadoresSUpervisor(Sesion.Operador.idOperador, fecha1, fecha2);


                trTituloMes.Controls.Add(tdTituloMes);
                trTitulos.Controls.Add(tdTituloNombre);
                trTitulos.Controls.Add(tdMontoRecaudado);
                trTitulos.Controls.Add(tdMontoMeta);
                trTitulos.Controls.Add(tdPorcentaje);
                tablaOperadores.Controls.Add(trTituloMes);
                tablaOperadores.Controls.Add(trTitulos);

                string[] datosOperadorTri = new string[dt.Rows.Count];
                Decimal[] datosPorcentajeTri = new Decimal[dt.Rows.Count];
                string[] datosidOperadorTri = new string[dt.Rows.Count];

                for (int j = 0; j < dt.Rows.Count; j++)
                {


                    wsCobranzas ws = new wsCobranzas();
                    var Listado = ws.Ranking_Operador_lst(int.Parse(dt.Rows[j]["idOperador"].ToString()), fecha1, fecha2);



                    datosOperadorTri[j] = Listado[0].Operador.ToString();
                    datosPorcentajeTri[j] = Listado[0].Porc;
                    datosidOperadorTri[j] = Listado[0].ToString();




                    TableRow trContenido = new TableRow();
                    TableCell tdContenidoNombre = new TableCell();
                    TableCell tdContenidoMontoRecaudado = new TableCell();
                    TableCell tdContenidoMontoMeta = new TableCell();
                    TableCell tdContenidoPorcentaje = new TableCell();
                    tdContenidoNombre.Text = Listado[0].Operador.ToString();
                    tdContenidoMontoRecaudado.Text = Listado[0].Real.ToString();
                    tdContenidoMontoMeta.Text = Listado[0].Meta.ToString();
                    tdContenidoPorcentaje.Text = Listado[0].Porc.ToString();
                    trContenido.Controls.Add(tdContenidoNombre);
                    trContenido.Controls.Add(tdContenidoMontoRecaudado);
                    trContenido.Controls.Add(tdContenidoMontoMeta);
                    trContenido.Controls.Add(tdContenidoPorcentaje);
                    tablaOperadores.Controls.Add(trContenido);

                }
                Image imgChart = new Image();
                imgChart.ImageUrl = Negocios.MostrarGraficoTotalPorcentajePorOperador(datosOperadorTri, datosPorcentajeTri, datosidOperadorTri, Mes2);

                pnTablasOperadores.Controls.Add(tablaOperadores);
                pnTablasOperadores.Controls.Add(imgChart);



            }






            string Anu = "";
            Anu = Anual(1);


            Table tablaOperadores2 = new Table();
            TableRow trTituloMes2 = new TableRow();
            TableRow trTitulos2 = new TableRow();
            TableHeaderCell tdTituloNombre2 = new TableHeaderCell();
            TableHeaderCell tdTituloMes2 = new TableHeaderCell();
            TableHeaderCell tdMontoRecaudado2 = new TableHeaderCell();
            TableHeaderCell tdMontoMeta2 = new TableHeaderCell();
            TableHeaderCell tdPorcentaje2 = new TableHeaderCell();

            tablaOperadores2.BorderWidth = 1;
            tablaOperadores2.BorderColor = System.Drawing.Color.Black;
            tablaOperadores2.Width = new Unit("100%");



            tdTituloNombre2.Text = "Nombre";
            tdMontoRecaudado2.Text = "Monto Recaudado";
            tdMontoMeta2.Text = "Monto Meta";
            tdPorcentaje2.Text = "Porcentaje";
            tdTituloMes2.ColumnSpan = 4;
            tdTituloMes2.HorizontalAlign = HorizontalAlign.Center;
            tdTituloMes2.Text = Anu;

            fecha1 = DateTime.Parse(DateTime.Now.Year + "/" + "1" + "/06");
            fecha2 = DateTime.Parse(DateTime.Now.Year + "/" + "12" + "/06");
            dt = Negocios.ListarOperadoresSUpervisor(Sesion.Operador.idOperador, fecha1, fecha2);


            trTituloMes2.Controls.Add(tdTituloMes2);
            trTitulos2.Controls.Add(tdTituloNombre2);
            trTitulos2.Controls.Add(tdMontoRecaudado2);
            trTitulos2.Controls.Add(tdMontoMeta2);
            trTitulos2.Controls.Add(tdPorcentaje2);
            tablaOperadores2.Controls.Add(trTituloMes2);
            tablaOperadores2.Controls.Add(trTitulos2);

            string[] datosOperadorAnu = new string[dt.Rows.Count];
            Decimal[] datosPorcentajeAnu = new Decimal[dt.Rows.Count];
            string[] datosidOperadorAnu = new string[dt.Rows.Count];





            for (int j = 0; j < dt.Rows.Count; j++)
            {


                wsCobranzas ws = new wsCobranzas();
                var Listado = ws.Ranking_Operador_lst(int.Parse(dt.Rows[j]["idOperador"].ToString()), fecha1, fecha2);



                datosOperadorAnu[j] = Listado[0].Operador.ToString();
                datosPorcentajeAnu[j] = Listado[0].Porc;
                datosidOperadorAnu[j] = Listado[0].ToString();




                TableRow trContenido = new TableRow();
                TableCell tdContenidoNombre = new TableCell();
                TableCell tdContenidoMontoRecaudado = new TableCell();
                TableCell tdContenidoMontoMeta = new TableCell();
                TableCell tdContenidoPorcentaje = new TableCell();
                tdContenidoNombre.Text = Listado[0].Operador.ToString();
                tdContenidoMontoRecaudado.Text = Listado[0].Real.ToString();
                tdContenidoMontoMeta.Text = Listado[0].Meta.ToString();
                tdContenidoPorcentaje.Text = Listado[0].Porc.ToString();
                trContenido.Controls.Add(tdContenidoNombre);
                trContenido.Controls.Add(tdContenidoMontoRecaudado);
                trContenido.Controls.Add(tdContenidoMontoMeta);
                trContenido.Controls.Add(tdContenidoPorcentaje);
                tablaOperadores2.Controls.Add(trContenido);
            }

            Image imgChart2 = new Image();
            imgChart2.ImageUrl = Negocios.MostrarGraficoTotalPorcentajePorOperador(datosOperadorAnu, datosPorcentajeAnu, datosidOperadorAnu, Anu);

            pnTablasOperadores.Controls.Add(tablaOperadores2);
            pnTablasOperadores.Controls.Add(imgChart2);
            

        }


        private string sMes(int rMes) {

            string Mes = "";
            switch (rMes)
            {
                case 1:
                    Mes = "Enero-Febrero";
                    break;
                case 2:
                    Mes = "Febrero-Marzo";
                    break;
                case 3:
                    Mes = "Marzo-Abril";
                    break;
                case 4:
                    Mes = "Abril-Mayo";
                    break;
                case 5:
                    Mes = "Mayo-Junio";
                    break;
                case 6:
                    Mes = "Junio-Julio";
                    break;
                case 7:
                    Mes = "Julio-Agosto";
                    break;
                case 8:
                    Mes = "Agosto-Septiembre";
                    break;
                case 9:
                    Mes = "Septiembre-Octubre";
                    break;
                case 10:
                    Mes = "Octubre-Noviembre";
                    break;
                case 11:
                    Mes = "Noviembre-Diciembre";
                    break;
                case 12:
                    Mes = "Diciembre-Enero";
                    break;
                default:
                    Mes = "Enero-Febrero";
                    break;
            }



            return Mes;
        }




        private string TriMes(int rMes2)
        {

            string Mes2 = "";
            switch (rMes2)
            {
                case 1:
                    Mes2 = "Enero-Marzo";
                    break;
                case 4:
                    Mes2 = "Abril-Junio";
                    break;
                case 7:
                    Mes2 = "Julio-Septiembre";
                    break;
                case 10:
                    Mes2 = "Octubre-Diciembre";
                    break;

                default:
                    Mes2 = "Enero-Marzo";
                    break;
            }



            return Mes2;
        }

        private string Anual(int rAnual)
        {

            string sAnual = "";
            switch (rAnual)
            {
                case 1:
                    sAnual = "Enero-Diciembre";
                    break;



            }



            return sAnual;
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            pnlExportar.Visible = false;
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Ranking.xls\"");
            Response.AddHeader("content-type", "text/html");//application/xls
        }

        protected void btnExportarWord_Click(object sender, EventArgs e)
        {
            pnlExportar.Visible = false;
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Ranking.doc\"");
            Response.AddHeader("content-type", "text/html");//application/msword

        }
    }
}