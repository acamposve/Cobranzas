using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Web.Hosting;
using Cobranzas.OT;
using System.IO;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using Entidades;

namespace Cobranzas
{
    public static class Negocios
    {
        public static Color Positivo = Color.FromArgb(0x00, 0xFF, 0x00);
        public static Color Negativo = Color.FromArgb(0xFF, 0x00, 0x00);
        public static Color Neutral = Color.FromArgb(0xFF, 0xFF, 0x00);
        public static Color Total = Color.FromArgb(0x00, 0x00, 0xFF);
        public static Color Fondo = Color.FromArgb(0xFF, 0xFF, 0xFF);
        public static Color LineaMayor = Color.FromArgb(0x00, 0x00, 0x00);
        public static Color LineaMenor = Color.FromArgb(0x80, 0x80, 0x80);

        public static String MostrarGraficoSupervision(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Chart Grafico = new Chart();
                    Grafico.Height = 300;
                    Grafico.Width = 600;

                    var Operadores = Comunes.OperadoresSupervisados(idOperador, db);
                    var Datos = (from c in db.Operadores
                                 where Operadores.Contains(c.idOperador)
                                 select new
                                 {
                                     Nombre = c.Nombre,
                                     Positivas = c.Gestiones.Count(x => x.Fecha.Date == DateTime.Now.Date && x.Status.Tipo == "Positivo"),
                                     Negativas = c.Gestiones.Count(x => x.Fecha.Date == DateTime.Now.Date && x.Status.Tipo == "Negativo"),
                                     Neutrales = c.Gestiones.Count(x => x.Fecha.Date == DateTime.Now.Date && x.Status.Tipo == "Neutral"),
                                     Totales = c.Gestiones.Count(x => x.Fecha.Date == DateTime.Now.Date),
                                 }).ToList();
                    if (Datos.Count == 0) return "/Img/Temp/" + Resources.Recursos.New + ".png";
                    ChartArea Area = new ChartArea("Area");
                    Grafico.ChartAreas.Add(Area);
                    Area.BackColor = Fondo;

                    Area.AxisY.MajorGrid.Interval = 5;
                    Area.AxisY.MajorGrid.LineColor = LineaMayor;
                    Area.AxisY.MajorGrid.LineWidth = 1;
                    Area.AxisY.MinorGrid.Interval = 1;
                    Area.AxisY.MinorGrid.LineColor = LineaMenor;
                    Area.AxisY.MinorGrid.LineWidth = 1;
                    Area.AxisY.MinorGrid.Enabled = true;
                    Area.AxisX.MajorGrid.Interval = 5;
                    Area.AxisX.MajorGrid.LineColor = LineaMayor;
                    Area.AxisX.MajorGrid.LineWidth = 1;
                    Area.AxisX.MinorGrid.Interval = 1;
                    Area.AxisX.MinorGrid.LineColor = LineaMenor;
                    Area.AxisX.MinorGrid.Enabled = true;
                    Area.AxisX.MinorGrid.LineWidth = 1;
                    Area.AxisX.LabelStyle.Interval = 1;
                    Area.AxisX.LabelStyle.Angle = -90;
                    Series Negativas = new Series()
                    {
                        Name = Resources.Recursos.Negatives,
                        Color = Negativo,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Column
                    };
                    Negativas.Points.DataBindXY(Datos.Select(x => x.Nombre).ToList(), Datos.Select(x => x.Negativas).ToList());
                    Series Neutrales = new Series()
                    {
                        Name = Resources.Recursos.Neutrals,
                        Color = Neutral,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Column
                    };
                    Neutrales.Points.DataBindXY(Datos.Select(x => x.Nombre).ToList(), Datos.Select(x => x.Neutrales).ToList());
                    Series Positivas = new Series()
                    {
                        Name = Resources.Recursos.Positives,
                        Color = Positivo,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Column

                    };
                    Positivas.Points.DataBindXY(Datos.Select(x => x.Nombre).ToList(), Datos.Select(x => x.Positivas).ToList());
                    Series Totales = new Series()
                    {
                        Name = Resources.Recursos.Totals,
                        Color = Total,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Column
                    };
                    Totales.Points.DataBindXY(Datos.Select(x => x.Nombre).ToList(), Datos.Select(x => x.Totales).ToList());
                    Grafico.Series.Add(Positivas);
                    Grafico.Series.Add(Neutrales);
                    Grafico.Series.Add(Negativas);
                    Grafico.Series.Add(Totales);
                    Grafico.Legends.Add(new Legend("leyenda"));
                    Grafico.DataBind();
                    String ruta = "/Img/Temp/SUsuario" + idOperador.ToString() + ".png";
                    Grafico.SaveImage(HostingEnvironment.MapPath(ruta), ChartImageFormat.Png);
                    return ruta;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        public static String MostrarGraficoPorSemana(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Chart Grafico = new Chart();
                    Grafico.Height = 400;
                    Grafico.Width = 600;

                    var Operadores = Comunes.OperadoresSupervisados(idOperador, db);
                    var Datos = (from c in db.Operadores
                                 where Operadores.Contains(c.idOperador)
                                 select new
                                 {
                                     Nombre = c.Nombre,
                                     Positivas = c.Gestiones.Count(x => x.Fecha.Date <= DateTime.Now.Date && x.Fecha.Date >= DateTime.Now.Date.AddDays(-6) && x.Status.Tipo == "Positivo"),
                                     Negativas = c.Gestiones.Count(x => x.Fecha.Date <= DateTime.Now.Date && x.Fecha.Date >= DateTime.Now.Date.AddDays(-6) && x.Status.Tipo == "Negativo"),
                                     Neutrales = c.Gestiones.Count(x => x.Fecha.Date <= DateTime.Now.Date && x.Fecha.Date >= DateTime.Now.Date.AddDays(-6) && x.Status.Tipo == "Neutral"),
                                     Totales = c.Gestiones.Count(x => x.Fecha.Date <= DateTime.Now.Date && x.Fecha.Date >= DateTime.Now.Date.AddDays(-6)),
                                 }).ToList();
                    if (Datos.Count == 0) return "/Img/Temp/" + Resources.Recursos.New + ".png";
                    ChartArea Area = new ChartArea("Area");
                    Grafico.ChartAreas.Add(Area);
                    Area.BackColor = Fondo;

                    Area.AxisY.MajorGrid.Interval = 10;
                    Area.AxisY.MajorGrid.LineColor = LineaMayor;
                    Area.AxisY.MajorGrid.LineWidth = 1;
                    Area.AxisY.MinorGrid.Interval = 2;
                    Area.AxisY.MinorGrid.LineColor = LineaMenor;
                    Area.AxisY.MinorGrid.LineWidth = 1;
                    Area.AxisY.MinorGrid.Enabled = true;
                    Area.AxisX.MajorGrid.Interval = 5;
                    Area.AxisX.MajorGrid.LineColor = LineaMayor;
                    Area.AxisX.MajorGrid.LineWidth = 1;
                    Area.AxisX.MinorGrid.Interval = 1;
                    Area.AxisX.MinorGrid.LineColor = LineaMenor;
                    Area.AxisX.MinorGrid.Enabled = true;
                    Area.AxisX.MinorGrid.LineWidth = 1;
                    Area.AxisX.LabelStyle.Interval = 1;
                    Area.AxisX.LabelStyle.Angle = -90;
                    Series Negativas = new Series()
                    {
                        Name = Resources.Recursos.Negatives,
                        Color = Negativo,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Column
                    };
                    Negativas.Points.DataBindXY(Datos.Select(x => x.Nombre).ToList(), Datos.Select(x => x.Negativas).ToList());
                    Series Neutrales = new Series()
                    {
                        Name = Resources.Recursos.Neutrals,
                        Color = Neutral,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Column
                    };
                    Neutrales.Points.DataBindXY(Datos.Select(x => x.Nombre).ToList(), Datos.Select(x => x.Neutrales).ToList());
                    Series Positivas = new Series()
                    {
                        Name = Resources.Recursos.Positives,
                        Color = Positivo,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Column

                    };
                    Positivas.Points.DataBindXY(Datos.Select(x => x.Nombre).ToList(), Datos.Select(x => x.Positivas).ToList());
                    Series Totales = new Series()
                    {
                        Name = Resources.Recursos.Totals,
                        Color = Total,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Column
                    };
                    Totales.Points.DataBindXY(Datos.Select(x => x.Nombre).ToList(), Datos.Select(x => x.Totales).ToList());
                    Grafico.Series.Add(Positivas);
                    Grafico.Series.Add(Neutrales);
                    Grafico.Series.Add(Negativas);
                    Grafico.Series.Add(Totales);
                    Grafico.Legends.Add(new Legend("leyenda"));
                    Grafico.DataBind();
                    String ruta = "/Img/Temp/SSUsuario" + idOperador.ToString() + ".png";
                    Grafico.SaveImage(HostingEnvironment.MapPath(ruta), ChartImageFormat.Png);
                    return ruta;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        public static DataTable ListarOperadoresSUpervisor(int idOperador, DateTime fecha1, DateTime fecha2)
        {
            try
            {
                DataAccessLayer ObjDatos = new DataAccessLayer();
                SqlParameter[] SqlParametros = new SqlParameter[3];
                DataTable dtOperadores = new DataTable();
                SqlParametros[0] = new SqlParameter("@idOperador", SqlDbType.Int);
                SqlParametros[0].Value = idOperador;


                SqlParametros[1] = new SqlParameter("@fecha1", SqlDbType.DateTime);
                SqlParametros[1].Value = fecha1;


                SqlParametros[2] = new SqlParameter("@fecha2", SqlDbType.DateTime);
                SqlParametros[2].Value = fecha2;
                dtOperadores = ObjDatos.EjecutarConsulta("OperadoresSupervisor", SqlParametros);
                return dtOperadores;
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        public static DataTable ListarCorreos(int idOperador, int idPersona)
        {
            try
            {
                DataAccessLayer ObjDatos = new DataAccessLayer();
                SqlParameter[] SqlParametros = new SqlParameter[2];
                DataTable dtCorreos = new DataTable();

                SqlParametros[0] = new SqlParameter("@idPersona", SqlDbType.Int);
                SqlParametros[0].Value = idPersona;


                SqlParametros[1] = new SqlParameter("@idOperador", SqlDbType.Int);
                SqlParametros[1].Value = idOperador;



                dtCorreos = ObjDatos.EjecutarConsulta("ListaCorreos", SqlParametros);


                return dtCorreos;



            }
            catch (Exception ex)
            {

                throw ex;
            }










        }

        public static String MostrarGraficoTotalPorcentajePorOperador(string[] datosOp, Decimal[] datosPorc, string[] datosIdOp, string mes)
        {
            
            Chart Grafico = new Chart();
            Grafico.Height = 400;
            Grafico.Width = 600;

            ChartArea Area = new ChartArea("Area");
            Grafico.ChartAreas.Add(Area);
            Area.BackColor = Fondo;

            Area.AxisY.MajorGrid.Interval = 10;
            Area.AxisY.MajorGrid.LineColor = LineaMayor;
            Area.AxisY.MajorGrid.LineWidth = 1;
            Area.AxisY.MinorGrid.Interval = 2;
            Area.AxisY.MinorGrid.LineColor = LineaMenor;
            Area.AxisY.MinorGrid.LineWidth = 1;
            Area.AxisY.MinorGrid.Enabled = true;
            Area.AxisX.MajorGrid.Interval = 5;
            Area.AxisX.MajorGrid.LineColor = LineaMayor;
            Area.AxisX.MajorGrid.LineWidth = 1;
            Area.AxisX.MinorGrid.Interval = 1;
            Area.AxisX.MinorGrid.LineColor = LineaMenor;
            Area.AxisX.MinorGrid.Enabled = true;
            Area.AxisX.MinorGrid.LineWidth = 1;
            Area.AxisX.LabelStyle.Interval = 1;
            Area.AxisX.LabelStyle.Angle = -90;

            // asignar los valores apra colocarlos en la grafica en la seccion Series



            Series Negativas = new Series()
            {
                Name = "Porcentajes",

                ChartArea = "Area",
                ChartType = SeriesChartType.Column
            };
            Negativas.Points.DataBindXY(datosOp.ToList(), datosPorc.ToList());



            Grafico.Series.Add(Negativas);
            Grafico.Legends.Add(new Legend("leyenda"));
            Grafico.DataBind();
            String ruta = "/Img/Temp/SSPorcentaje2" + datosIdOp.ToString() + mes.ToString() + ".png";
            Grafico.SaveImage(HostingEnvironment.MapPath(ruta), ChartImageFormat.Png);
            return ruta;






        }

        public static String MostrarGraficoTotalPorcentajePorSupervisor(int idOperador, DateTime fecha1, DateTime fecha2)
        {


            DataAccessLayer ObjDatos = new DataAccessLayer();
            SqlParameter[] SqlParametros = new SqlParameter[3];
            DataTable dt = new DataTable();



            SqlParametros[0] = new SqlParameter("@idOperador", SqlDbType.Int);
            SqlParametros[0].Value = idOperador;


            SqlParametros[1] = new SqlParameter("@fecha1", SqlDbType.DateTime);
            SqlParametros[1].Value = fecha1;



            SqlParametros[2] = new SqlParameter("@fecha2", SqlDbType.DateTime);
            SqlParametros[2].Value = fecha2;

            dt = ObjDatos.EjecutarConsulta("pa_totales_porcentaje_por_supervisor", SqlParametros);

            int j = dt.Rows.Count;


            int datosPorcentaje = 0;


            if (dt.Rows[0]["Porcentaje"].ToString() == "")
            {
                string texto = "/Img/Temp/Nuevo.png";
                return texto;
            }
            else
            {
                datosPorcentaje = int.Parse(dt.Rows[0]["Porcentaje"].ToString());


                // dimensaionar una matirz string[]

                Chart Grafico = new Chart();
                Grafico.Height = 400;
                Grafico.Width = 600;

                ChartArea Area = new ChartArea("Area");
                Grafico.ChartAreas.Add(Area);
                Area.BackColor = Fondo;

                Area.AxisY.MajorGrid.Interval = 10;
                Area.AxisY.MajorGrid.LineColor = LineaMayor;
                Area.AxisY.MajorGrid.LineWidth = 1;
                Area.AxisY.MinorGrid.Interval = 2;
                Area.AxisY.MinorGrid.LineColor = LineaMenor;
                Area.AxisY.MinorGrid.LineWidth = 1;
                Area.AxisY.MinorGrid.Enabled = true;
                Area.AxisX.MajorGrid.Interval = 5;
                Area.AxisX.MajorGrid.LineColor = LineaMayor;
                Area.AxisX.MajorGrid.LineWidth = 1;
                Area.AxisX.MinorGrid.Interval = 1;
                Area.AxisX.MinorGrid.LineColor = LineaMenor;
                Area.AxisX.MinorGrid.Enabled = true;
                Area.AxisX.MinorGrid.LineWidth = 1;
                Area.AxisX.LabelStyle.Interval = 1;
                Area.AxisX.LabelStyle.Angle = -90;
                // asignar los valores apra colocarlos en la grafica en la seccion Series



                Series PorcentajeSup = new Series()
                {
                    Name = Resources.Recursos.PorcentajeSup,
                    Color = Positivo,
                    ChartArea = "Area",
                    ChartType = SeriesChartType.Column
                };

                int[] valoresY = { datosPorcentaje };
                PorcentajeSup.Points.DataBindY(valoresY);




                Grafico.Series.Add(PorcentajeSup);
                Grafico.Legends.Add(new Legend("leyenda"));
                Grafico.DataBind();
                String ruta = "/Img/Temp/SSPorcentajeSupervisor" + idOperador.ToString() + ".png";
                Grafico.SaveImage(HostingEnvironment.MapPath(ruta), ChartImageFormat.Png);
                return ruta;
            }




        }

        public static String MostrarGraficoTotalPorcentajePorFechaOperador(int idOperador, DateTime fecha1, DateTime fecha2)
        {


            DataAccessLayer ObjDatos = new DataAccessLayer();
            SqlParameter[] SqlParametros = new SqlParameter[3];
            DataTable dt = new DataTable();


            SqlParametros[0] = new SqlParameter("@idOperador", SqlDbType.Int);
            SqlParametros[0].Value = idOperador;


            SqlParametros[1] = new SqlParameter("@fecha1", SqlDbType.DateTime);
            SqlParametros[1].Value = fecha1;



            SqlParametros[2] = new SqlParameter("@fecha2", SqlDbType.DateTime);
            SqlParametros[2].Value = fecha2;

            dt = ObjDatos.EjecutarConsulta("pa_totales_porcentaje_por_fecha_operador ", SqlParametros);



            int j = dt.Rows.Count;

            double Monto = 0;
            double cambio = 0;
            double porcentaje = 0;
            double montototal = 0;
            double suma = 0;

            for (int i = 0; i < j; i++)
            {

                Monto = (double.Parse(dt.Rows[i]["Monto"].ToString()) / double.Parse(dt.Rows[i]["Cambiolocal"].ToString()));
                suma = suma + Monto;

            }

            montototal = suma * 100;
            porcentaje = montototal / double.Parse(dt.Rows[0]["MontoMeta"].ToString());


            if (dt.Rows.Count == 0 || dt.Rows[0]["Porcentaje"].ToString() == "")
            {

                string texto = "/Img/Temp/Nuevo.png";
                return texto;
            }
            else
            {





                double datosPorcentaje = double.Parse(dt.Rows[0]["porcentaje"].ToString());


                // dimensaionar una matirz string[]

                Chart Grafico = new Chart();
                Grafico.Height = 400;
                Grafico.Width = 600;

                ChartArea Area = new ChartArea("Area");
                Grafico.ChartAreas.Add(Area);
                Area.BackColor = Fondo;

                Area.AxisY.MajorGrid.Interval = 10;
                Area.AxisY.MajorGrid.LineColor = LineaMayor;
                Area.AxisY.MajorGrid.LineWidth = 1;
                Area.AxisY.MinorGrid.Interval = 2;
                Area.AxisY.MinorGrid.LineColor = LineaMenor;
                Area.AxisY.MinorGrid.LineWidth = 1;
                Area.AxisY.MinorGrid.Enabled = true;
                Area.AxisX.MajorGrid.Interval = 5;
                Area.AxisX.MajorGrid.LineColor = LineaMayor;
                Area.AxisX.MajorGrid.LineWidth = 1;
                Area.AxisX.MinorGrid.Interval = 1;
                Area.AxisX.MinorGrid.LineColor = LineaMenor;
                Area.AxisX.MinorGrid.Enabled = true;
                Area.AxisX.MinorGrid.LineWidth = 1;
                Area.AxisX.LabelStyle.Interval = 1;
                Area.AxisX.LabelStyle.Angle = -90;
                // asignar los valores apra colocarlos en la grafica en la seccion Series



                Series PorcentajeSup = new Series()
                {
                    Name = Resources.Recursos.PorcentajeSup,
                    Color = Positivo,
                    ChartArea = "Area",
                    ChartType = SeriesChartType.Column
                };

                double[] valoresY = { datosPorcentaje };
                PorcentajeSup.Points.DataBindY(valoresY);


                Grafico.Series.Add(PorcentajeSup);
                Grafico.Legends.Add(new Legend("leyenda"));
                Grafico.DataBind();
                String ruta = "/Img/Temp/SSPorcentajeporFechaOperador" + idOperador.ToString() + ".png";
                Grafico.SaveImage(HostingEnvironment.MapPath(ruta), ChartImageFormat.Png);
                return ruta;
            }




        }

        public static String MostrarGrafico(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Chart Grafico = new Chart();
                    Grafico.Height = 300;
                    Grafico.Width = 500;
                    List<Int32> lstOperadores;
                    if (idOperador == Sesion.idOperador)
                    {
                        lstOperadores = new List<int>();
                        lstOperadores.Add(idOperador);
                    }
                    else
                    {
                        lstOperadores = Comunes.Clones(idOperador, db);
                    }
                    //                        (from c in db.Gestiones where c.idOperador == idOperador group c by c.Fecha.Date into d orderby d.Key descending select new { Fecha = d.Key, Positivas = d.Count(x => x.Status.Tipo == "Positivo"), Negativas = d.Count(x => x.Status.Tipo == "Negativo"), Neutrales = d.Count(x => x.Status.Tipo == "Neutral"), Totales = d.Count() }).Take(10).OrderBy(x => x.Fecha).ToList()
                    var Datos = (from c in db.Gestiones where lstOperadores.Contains(c.idOperador) group c by c.Fecha.Date into d orderby d.Key descending select new { Fecha = d.Key, Positivas = d.Count(x => x.Status.Tipo == "Positivo"), Negativas = d.Count(x => x.Status.Tipo == "Negativo"), Neutrales = d.Count(x => x.Status.Tipo == "Neutral"), Totales = d.Count() }).Take(10).OrderBy(x => x.Fecha).ToList();

                    if (Datos.Count == 0) return "/Img/Temp/" + Resources.Recursos.New + ".png";
                    ChartArea Area = new ChartArea("Area");
                    Grafico.ChartAreas.Add(Area);
                    Area.BackColor = Fondo;

                    Area.AxisY.MajorGrid.Interval = 5;
                    Area.AxisY.MajorGrid.LineColor = LineaMayor;
                    Area.AxisY.MajorGrid.LineWidth = 1;
                    Area.AxisY.MinorGrid.Interval = 1;
                    Area.AxisY.MinorGrid.LineColor = LineaMenor;
                    Area.AxisY.MinorGrid.LineWidth = 1;
                    Area.AxisY.MinorGrid.Enabled = true;
                    Area.AxisX.MajorGrid.Interval = 5;
                    Area.AxisX.MajorGrid.LineColor = LineaMayor;
                    Area.AxisX.MajorGrid.LineWidth = 1;
                    Area.AxisX.MinorGrid.Interval = 1;
                    Area.AxisX.MinorGrid.LineColor = LineaMenor;
                    Area.AxisX.MinorGrid.Enabled = true;
                    Area.AxisX.MinorGrid.LineWidth = 1;
                    Area.AxisX.LabelStyle.Interval = 1;
                    Area.AxisX.LabelStyle.Angle = -90;
                    Series Negativas = new Series()
                    {
                        Name = Resources.Recursos.Negatives,
                        MarkerStyle = MarkerStyle.Circle,
                        MarkerSize = 8,
                        MarkerColor = Negativo,
                        Color = Negativo,
                        BorderWidth = 2,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Line
                    };
                    Negativas.Points.DataBindXY(Datos.Select(x => x.Fecha.AFechaCorta()).ToList(), Datos.Select(x => x.Negativas).ToList());
                    Series Neutrales = new Series()
                    {
                        Name = Resources.Recursos.Neutrals,
                        MarkerStyle = MarkerStyle.Square,
                        MarkerSize = 8,
                        MarkerColor = Neutral,
                        Color = Neutral,
                        BorderWidth = 2,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Line
                    };
                    Neutrales.Points.DataBindXY(Datos.Select(x => x.Fecha.AFechaCorta()).ToList(), Datos.Select(x => x.Neutrales).ToList());
                    Series Positivas = new Series()
                    {
                        Name = Resources.Recursos.Positives,
                        MarkerStyle = MarkerStyle.Star5,
                        MarkerSize = 8,
                        MarkerColor = Positivo,
                        Color = Positivo,
                        BorderWidth = 2,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Line

                    };
                    Positivas.Points.DataBindXY(Datos.Select(x => x.Fecha.AFechaCorta()).ToList(), Datos.Select(x => x.Positivas).ToList());
                    Series Totales = new Series()
                    {
                        Name = Resources.Recursos.Totals,
                        MarkerStyle = MarkerStyle.Triangle,
                        MarkerSize = 8,
                        MarkerColor = Total,
                        Color = Total,
                        BorderWidth = 2,
                        ChartArea = "Area",
                        ChartType = SeriesChartType.Line
                    };
                    Totales.Points.DataBindXY(Datos.Select(x => x.Fecha.AFechaCorta()).ToList(), Datos.Select(x => x.Totales).ToList());
                    Grafico.Series.Add(Positivas);
                    Grafico.Series.Add(Neutrales);
                    Grafico.Series.Add(Negativas);
                    Grafico.Series.Add(Totales);
                    Grafico.Legends.Add(new Legend("leyenda"));
                    Grafico.DataBind();
                    String ruta = "/Img/Temp/Usuario" + idOperador.ToString() + ".png";
                    Grafico.SaveImage(HostingEnvironment.MapPath(ruta), ChartImageFormat.Png);
                    return ruta;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        public class IndDin
        {
            public Int32 id;
            public Decimal Monto;
            public Int32 Cantidad;
        }

        public static void DistribuirOperadoresACuentas(ref List<otCuenta> Cuentas, Int32 idModoDistOperadores, List<Int32> Operadores)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (Operadores.Count == 0) return;//Si no hay operadores, no se puede distribuir nada...
                    switch (idModoDistOperadores)
                    {
                        case 1: //No Asignar
                            break;
                        case 2: //Experiencia con la Cuenta
                                //Busca cual de los operadores seleccionados tiene más experiencia con la cuenta y se la asigna
                            foreach (otCuenta cuenta in Cuentas)
                            {
                                int idCuenta = cuenta.idCuenta;
                                var q = from c in db.Gestiones
                                        where c.Cuentas_Gestiones.Any(x => x.idCuenta == idCuenta)
                                        && Operadores.Contains(c.idOperador)
                                        group c by c.idOperador into g
                                        orderby g.Count()
                                        select g.Key;
                                if (q.Count() == 0)
                                {
                                    cuenta.idOperador = null;
                                }
                                else
                                {
                                    cuenta.idOperador = q.First();
                                }
                            }
                            break;
                        case 3: //Último Contacto
                                //Busca cual de los operadores seleccionados fue el último en tocar la cuenta y se la asigna
                            foreach (otCuenta Cuenta in Cuentas)
                            {
                                int idCuenta = Cuenta.idCuenta;
                                var q = from c in db.Gestiones
                                        where c.Cuentas_Gestiones.Any(x => x.idCuenta == idCuenta)
                                        && Operadores.Contains(c.idOperador)
                                        orderby c.Fecha descending
                                        select c.idOperador;
                                if (q.Count() == 0)
                                {
                                    Cuenta.idOperador = null;
                                }
                                else
                                {
                                    Cuenta.idOperador = q.First();
                                }
                            }
                            break;
                        case 4: //Equitativo(Dinero-Campaña)
                                //Trata de Repartir Equitativamente Las cuentas a los operadores, según el monto asociado
                            {
                                Cuentas = Cuentas.OrderByDescending(x => x.DeudaDolar).ToList();
                                List<IndDin> OperadoresD = Operadores.Select(x => new IndDin { id = x, Monto = 0, Cantidad = 0 }).ToList();
                                foreach (otCuenta Cuenta in Cuentas)
                                {
                                    OperadoresD = OperadoresD.OrderBy(x => x.Monto).ToList();
                                    Cuenta.idOperador = OperadoresD[0].id;
                                    OperadoresD[0].Monto += Cuenta.DeudaDolar;
                                }
                            }
                            break;
                        case 5: //Equitativo(Cantidad-Campaña)
                            {
                                //Trata de Repartir Equitativamente Las cuentas a los operadores, según la cantidad de cuentas, pero tratando de balancear el monto
                                int ind = 0;
                                Cuentas = Cuentas.OrderByDescending(x => x.DeudaDolar).ToList();
                                List<IndDin> OperadoresD = Operadores.Select(x => new IndDin { id = x, Monto = 0, Cantidad = 0 }).ToList();
                                foreach (otCuenta Cuenta in Cuentas)
                                {
                                    if (ind == OperadoresD.Count)
                                    {
                                        OperadoresD = OperadoresD.OrderBy(x => x.Monto).ToList();
                                        ind = 0;
                                    }
                                    Cuenta.idOperador = OperadoresD[ind].id;
                                    OperadoresD[ind].Monto += Cuenta.DeudaDolar;
                                    ind++;
                                }
                            }
                            break;
                        case 6: //Equitativo(Dinero-Total)
                                //Trata de Repartir Equitativamente Las cuentas a los operadores, según el monto asociado
                            {
                                Cuentas = Cuentas.OrderByDescending(x => x.DeudaDolar).ToList();
                                List<IndDin> OperadoresD = Operadores.Select(x => new IndDin { id = x, Monto = db.Cuentas_Operadores.Where(y => y.FechaFin == null && y.idOperador == x).Sum(y => y.Cuentas.MontoRestante), Cantidad = 0 }).ToList();
                                foreach (otCuenta Cuenta in Cuentas)
                                {
                                    OperadoresD = OperadoresD.OrderBy(x => x.Monto).ToList();
                                    Cuenta.idOperador = OperadoresD[0].id;
                                    OperadoresD[0].Monto += Cuenta.DeudaDolar;
                                }
                            }
                            break;
                        case 7: //Equitativo(Cantidad-Total)
                            {
                                //Trata de Repartir Equitativamente Las cuentas a los operadores, según la cantidad de cuentas, pero tratando de balancear el monto
                                Cuentas = Cuentas.OrderByDescending(x => x.DeudaDolar).ToList();
                                List<IndDin> OperadoresD = Operadores.Select(x => new IndDin { id = x, Monto = db.Cuentas_Operadores.Where(y => y.FechaFin == null && y.idOperador == x).Sum(y => (decimal?)y.Cuentas.MontoRestante) ?? 0.00M, Cantidad = db.Cuentas_Operadores.Where(y => y.FechaFin == null && y.idOperador == x).Count() }).ToList();
                                foreach (otCuenta Cuenta in Cuentas)
                                {
                                    OperadoresD = OperadoresD.OrderBy(x => x.Cantidad).ThenBy(x => x.Monto).ToList();
                                    Cuenta.idOperador = OperadoresD[0].id;
                                    OperadoresD[0].Monto += Cuenta.DeudaDolar;
                                    OperadoresD[0].Cantidad++;
                                }
                            }
                            break;
                        case 8: //+Dinero>+Antigüedad (Suave)
                            {
                                Cuentas = Cuentas.OrderByDescending(x => x.DeudaDolar).ToList();
                                List<IndDin> OperadoresD = Operadores.Select(x => new IndDin { id = x, Monto = (DateTime.Now.Date - db.Operadores.Single(y => y.idOperador == x).FechaIngreso).Days }).OrderByDescending(x => x.Monto).ToList();
                                int ind = 0;
                                foreach (otCuenta Cuenta in Cuentas) Cuenta.idOperador = OperadoresD[ind++ % OperadoresD.Count].id;
                            }
                            break;
                        case 9: //+Dinero>+Efectividad (Suave)
                            {
                                Cuentas = Cuentas.OrderByDescending(x => x.DeudaDolar).ToList();
                                List<IndDin> OperadoresD = Operadores.Select(x => new IndDin { id = x, Monto = db.Efectividad(x) ?? 0 }).OrderByDescending(x => x.Monto).ToList();
                                int ind = 0;
                                foreach (otCuenta Cuenta in Cuentas) Cuenta.idOperador = OperadoresD[ind++ % OperadoresD.Count].id;
                            }
                            break;
                        case 10://+Dinero>+Antigüedad (Fuerte)
                            {
                                Cuentas = Cuentas.OrderByDescending(x => x.DeudaDolar).ToList();
                                List<IndDin> OperadoresD = Operadores.Select(x => new IndDin { id = x, Monto = (DateTime.Now.Date - db.Operadores.Single(y => y.idOperador == x).FechaIngreso).Days }).OrderByDescending(x => x.Monto).ToList();
                                int TotalCuentas = Cuentas.Count;
                                int TotalOperadores = OperadoresD.Count;
                                int MayorCantidad = TotalCuentas % TotalOperadores;
                                int CantidadMenor = TotalCuentas / TotalOperadores;

                                int indC = 0;//Indice Cuentas
                                int indOp = 0;//Indice Operador
                                foreach (otCuenta Cuenta in Cuentas)
                                {
                                    if ((indOp < MayorCantidad && indC > CantidadMenor) || (indOp >= MayorCantidad && indC == CantidadMenor))
                                    {
                                        indOp++;
                                        indC = 0;
                                    }
                                    Cuenta.idOperador = OperadoresD[indOp].id;
                                    indC++;
                                }
                            }
                            break;
                        case 11://+Dinero>+Efectividad (Fuerte)
                            {
                                Cuentas = Cuentas.OrderByDescending(x => x.DeudaDolar).ToList();
                                List<IndDin> OperadoresD = Operadores.Select(x => new IndDin { id = x, Monto = db.Efectividad(x) ?? 0 }).OrderByDescending(x => x.Monto).ToList();
                                int TotalCuentas = Cuentas.Count;
                                int TotalOperadores = OperadoresD.Count;
                                int MayorCantidad = TotalCuentas % TotalOperadores;
                                int CantidadMenor = TotalCuentas / TotalOperadores;

                                int indC = 0;//Indice Cuentas
                                int indOp = 0;//Indice Operador
                                foreach (otCuenta Cuenta in Cuentas)
                                {
                                    if ((indOp <= MayorCantidad && indC > CantidadMenor) || (indOp > MayorCantidad && indC == CantidadMenor))
                                    {
                                        indOp++;
                                        indC = 0;
                                    }
                                    Cuenta.idOperador = OperadoresD[indOp].id;
                                    indC++;
                                }
                            }
                            break;
                        case 12://Aleatorio            
                                //Asigna Aleatoriamente los Operadores a las cuentas
                            Random al = new Random();
                            foreach (otCuenta deuda in Cuentas) deuda.idOperador = Operadores[al.Next(Operadores.Count)];
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }

        }

        public static otIndicadores ActualizarIndicadores(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.OperadoresMetas OM;
                    OM = db.OperadoresMetas.SingleOrDefault(x => x.idOperador == idOperador && x.Fecha == DateTime.Now.Date);
                    otIndicadores I = new otIndicadores();
                    if (OM == null)
                    {
                        Entidades.OperadoresMetas OM2 = db.OperadoresMetas.Where(x => x.idOperador == idOperador).OrderByDescending(x => x.Fecha).FirstOrDefault();
                        OM = new Entidades.OperadoresMetas();
                        db.OperadoresMetas.InsertOnSubmit(OM);
                        if (OM2 == null)
                        {
                            OM.MetaGestiones = 0;
                            OM.MetaMonto = 0;
                            OM.MetaTiempo = 0;
                            OM.MetaTiempoInactivo = 0;
                            OM.MetaTMO = 0;
                        }
                        else
                        {
                            OM.MetaGestiones = OM2.MetaGestiones;
                            OM.MetaMonto = OM2.MetaMonto;
                            OM.MetaTiempo = OM2.MetaTiempo;
                            OM.MetaTiempoInactivo = OM2.MetaTiempoInactivo;
                            OM.MetaTMO = OM2.MetaTMO;
                        }
                        OM.Fecha = DateTime.Now.Date;
                        OM.idOperador = idOperador;
                    }

                    I.MetaMonto = OM.MetaMonto;
                    I.MetaGestiones = OM.MetaGestiones;
                    I.MetaGestionesPersonas = OM.MetaGestionesPersonas;
                    I.MetaTiempo = OM.MetaTiempo;
                    I.MetaTiempoInactivo = OM.MetaTiempoInactivo;
                    I.CuentasAsignadas = db.Cuentas_Operadores.Count(x => x.idOperador == idOperador && x.FechaFin == null);
                    I.PersonasAsignadas = db.Personas.Count(x => x.Cuentas.Any(y => y.Cuentas_Operadores.Any(z => z.FechaFin == null && z.idOperador == idOperador)));
                    I.MetaTMO = OM.MetaTMO;
                    I.RealMonto = db.Pagos.Where(x => x.idOperador == idOperador && (x.Aprobado ?? false) && (x.Confirmado ?? false) && x.Fecha == DateTime.Now.Date).Sum(x => (decimal?)x.MontoTotal) ?? 0;
                    var Gestiones = db.Gestiones.Where(x => x.idOperador == idOperador && x.Fecha.Date == DateTime.Now.Date);
                    I.RealGestiones = Gestiones.Count();
                    I.RealGestionesPersonas = Gestiones.Select(x => x.idPersona).Distinct().Count();

                    I.TiempoEnLlamada = db.Llamadas.Where(x => x.Fecha.Date == DateTime.Now.Date && x.idOperador == idOperador).Sum(x => (int?)x.Duracion) ?? 0;
                    I.LlamadasRealizadas = db.Llamadas.Where(x => x.Fecha.Date == DateTime.Now.Date && x.idOperador == idOperador).Count();
                    I.LlamadasContestadas = db.Llamadas.Where(x => x.Fecha.Date == DateTime.Now.Date && x.StatusPrimario == "Contestado" && x.idOperador == idOperador).Count();
                    I.LlamadasNoContestadas = I.LlamadasRealizadas - I.LlamadasContestadas;

                    CalcularTiempos(ref I, idOperador);
                    OM.RealGestiones = I.RealGestiones;
                    OM.RealMonto = I.RealMonto;
                    OM.RealTiempo = I.RealTiempo;
                    OM.RealTMO = I.RealTMO;
                    db.SubmitChanges();
                    return I;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        private static void CalcularTiempos(ref otIndicadores I, int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    var ACS = db.Acciones.Where(x => x.idOperador == idOperador && x.Fecha.Date == DateTime.Now.Date).OrderBy(x => x.Fecha);
                    I.RealTiempo = 0;
                    I.RealTiempoInactivo = 0;
                    I.RealTMO = 0;
                    Dictionary<Char, DateTime?> Fechas = new Dictionary<char, DateTime?>();
                    DateTime? FechaLogOut = null;
                    foreach (Entidades.Acciones AC in ACS)
                    {
                        if (AC.Accion[1] == 'I')
                        {
                            if (AC.Accion == "LI" && FechaLogOut.HasValue)
                            {
                                I.RealTiempoInactivo += Convert.ToInt32((AC.Fecha - FechaLogOut.Value).TotalMinutes);
                                I.RealTiempo += Convert.ToInt32((AC.Fecha - FechaLogOut.Value).TotalMinutes);
                                FechaLogOut = null;
                            }
                            if (!Fechas.ContainsKey(AC.Accion[0]))
                            {
                                Fechas.Add(AC.Accion[0], AC.Fecha);
                            }
                        }
                        if (AC.Accion[1] == 'O')
                        {
                            if (AC.Accion == "LO")
                            {
                                FechaLogOut = AC.Fecha;
                            }
                            if (Fechas.ContainsKey(AC.Accion[0]))
                            {
                                if (AC.Accion[0] == 'L')
                                {
                                    I.RealTiempo += Convert.ToInt32((AC.Fecha - Fechas[AC.Accion[0]].Value).TotalMinutes);
                                    Fechas.Remove(AC.Accion[0]);
                                    foreach (var Fecha in Fechas)
                                    {
                                        I.RealTiempoInactivo += Convert.ToInt32((AC.Fecha - Fecha.Value.Value).TotalMinutes);
                                        //Fechas.Remove(Fecha.Key);
                                    }
                                    Fechas.Clear();
                                }
                                else
                                {
                                    I.RealTiempoInactivo += Convert.ToInt32((AC.Fecha - Fechas[AC.Accion[0]].Value).TotalMinutes);
                                    Fechas.Remove(AC.Accion[0]);
                                }
                            }
                        }
                    }
                    foreach (var Fecha in Fechas)
                    {
                        if (Fecha.Key == 'L')
                        {
                            I.RealTiempo += Convert.ToInt32((DateTime.Now - Fecha.Value.Value).TotalMinutes);
                        }
                        else
                        {
                            I.RealTiempoInactivo += Convert.ToInt32((DateTime.Now - Fecha.Value.Value).TotalMinutes);
                        }
                    }
                    I.RealTMO = I.RealGestiones == 0 ? 0 : (Decimal)(I.RealTiempo - I.RealTiempoInactivo) / I.RealGestiones;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }

        }

        public static int BuscarSiguienteGestion(int idOperador)
        {
            //try
            //{
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 Result = db.BuscarSiguienteGestion(idOperador) ?? 0;
                if (Result == 0) { return 0; }
                db.Colas.DeleteAllOnSubmit(db.Colas.Where(x => x.idOperador == idOperador && x.idPersona == Result));
                db.SubmitChanges();
#warning Colocar en BD.
                //foreach (Entidades.Cuentas Cuenta in db.Personas.Single(x => x.idPersona == Result).Cuentas)
                //{
                //    if (Cuenta.Campanas_Cuentas.Any(x => x.Campanas.TipoCampana == 1))
                //    {
                //        Negocios.AsignarCuenta(Cuenta.idCuenta, idOperador);
                //    }
                //}
                return Result;
            }
            //}
            //catch (Exception Ex)
            //{
            //    throw Ex.Informar();
            //}

        }

        private static void AsignarCuenta(int idCuenta, int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Cuentas Cuenta = db.Cuentas.Single(x => x.idCuenta == idCuenta);
                    if (Cuenta.Cuentas_Operadores.Any(x => x.idOperador == idOperador && x.FechaFin == null)) return; //si ya lo tengo asignado, no le pares.
                    foreach (Entidades.Cuentas_Operadores CO in Cuenta.Cuentas_Operadores.Where(x => x.FechaFin == null))
                    {
                        CO.FechaFin = DateTime.Now;
                    }
                    Cuenta.Cuentas_Operadores.Add(new Entidades.Cuentas_Operadores { idOperador = idOperador, FechaInicio = DateTime.Now });
                    db.SubmitChanges();

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        public static void BajarArchivo(String Ruta, HttpResponse Response)
        {
            String Disposition = "attachment";
            if (Ruta.ToLower().EndsWith(".pdf")) { Response.ContentType = "application/pdf"; Disposition = "inline"; }
            if (Ruta.ToLower().EndsWith(".jpg")) { Response.ContentType = "image/jpeg"; Disposition = "inline"; }
            if (Ruta.ToLower().EndsWith(".gif")) { Response.ContentType = "image/gif"; Disposition = "inline"; }
            if (Ruta.ToLower().EndsWith(".bmp")) { Response.ContentType = "image/bmp"; Disposition = "inline"; }
            if (Ruta.ToLower().EndsWith(".png")) { Response.ContentType = "image/png"; Disposition = "inline"; }

            String NombreArchivo = "";
            String Archivo = "";
            if (Ruta.IndexOf("\\") != -1)//Ruta UNC
            {
                Archivo = Ruta;
                NombreArchivo = Ruta.Substring(Ruta.LastIndexOf("\\") + 1);
            }
            else//URL
            {
                NombreArchivo = Ruta.Substring(Ruta.LastIndexOf("/") + 1);
                String Ruta2 = "";
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Ruta2 = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                }
                Archivo = Ruta2 + DateTime.Now.ToString("yyyyMMddHHmmssfff") + NombreArchivo;
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(Ruta, Archivo);
                }
            }
            Response.AddHeader("content-disposition", Disposition + ";filename=\"" + NombreArchivo + "\"");
            Response.WriteFile(Archivo);
            Response.Flush();
            Response.End();
        }

        public static void Agregar_cola(Int32 idpersona, Int32 lugar)
        {
            DataAccessLayer ObjDatos = new DataAccessLayer();
            SqlParameter[] SqlParametros = new SqlParameter[2];

            SqlParametros[0] = new SqlParameter("@idPersona", SqlDbType.Int);
            SqlParametros[0].Value = idpersona;

            SqlParametros[1] = new SqlParameter("@Lugar", SqlDbType.Int);
            SqlParametros[1].Value = lugar;
            bool resultado = false;
            resultado = ObjDatos.EjecutarAccion("AgregarColaSupervisor", SqlParametros);
        }




        public static DataTable ListaCorreoGrupoEmpresa(int idPersona)
        {
            try
            {
                DataAccessLayer ObjDatos = new DataAccessLayer();
                SqlParameter[] SqlParametros = new SqlParameter[1];
                DataTable dtCorreosGruposEmpresa = new DataTable();

                SqlParametros[0] = new SqlParameter("@idPersona", SqlDbType.Int);
                SqlParametros[0].Value = idPersona;


                dtCorreosGruposEmpresa = ObjDatos.EjecutarConsulta("CorreoGrupoEmpresa", SqlParametros);


                return dtCorreosGruposEmpresa;



            }
            catch (Exception ex)
            {

                throw ex;
            }













        }
        public static DataTable GestionGrupoEmpresa(int idPersona)
        {
            try
            {
                DataAccessLayer ObjDatos = new DataAccessLayer();
                SqlParameter[] SqlParametros = new SqlParameter[1];
                DataTable dtGestionGrupoEmpresa = new DataTable();

                SqlParametros[0] = new SqlParameter("@idPersona", SqlDbType.Int);
                SqlParametros[0].Value = idPersona;


                dtGestionGrupoEmpresa = ObjDatos.EjecutarConsulta("GestionGrupoEmpresa", SqlParametros);


                return dtGestionGrupoEmpresa;



            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static DataTable ListarClientes()
        {
            try
            {
                DataAccessLayer ObjDatos = new DataAccessLayer();
                SqlParameter[] SqlParametros = new SqlParameter[0];
                DataTable dtListarClientes = new DataTable();

                dtListarClientes = ObjDatos.EjecutarConsulta("ListarCliente", SqlParametros);
                return dtListarClientes;
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }


    }

    public class CobranzasException : Exception
    {
        public CobranzasException(String Descripcion) : base(Descripcion) { }
    }
    

}