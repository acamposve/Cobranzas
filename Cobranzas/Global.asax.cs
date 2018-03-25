using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.DynamicData;
using System.Web.Routing;

using System.Threading;
using System.Globalization;
using System.Diagnostics;
using Entidades;

namespace Cobranzas
{
    public class Global : System.Web.HttpApplication
    {
        private static MetaModel s_defaultModel = new MetaModel();
        public static MetaModel DefaultModel
        {
            get
            {
                return s_defaultModel;
            }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            //                    IMPORTANTE: REGISTRO DEL MODELO DE DATOS 
            // Quite la marca de comentario de esta línea para registrar un modelo de ADO.NET Entity Framework para datos dinámicos de ASP.NET.
            // Establezca ScaffoldAllTables = true solo si está seguro de que desea que todas las tablas del
            // modelo de datos admitan una vista con scaffold (es decir, plantillas). Para controlar la técnica scaffolding para
            // tablas individuales, cree una clase parcial para la tabla y aplique el
            // atributo [ScaffoldTable(true)] a la clase parcial.
            // Nota: asegúrese de que cambia "YourDataContextType" al nombre de la clase del contexto de datos
            // en la aplicación.
            DefaultModel.RegisterContext(typeof(CobranzasDataContext), new ContextConfiguration() { ScaffoldAllTables = true });

            // La siguiente declaración admite el modo de páginas independientes, donde las tareas List, Detail, Insert y 
            // Update se realizan usando páginas independientes. Para habilitar este modo, quite las marcas de comentario de la siguiente 
            // definición del objeto route y marque como comentario las definiciones de route en la sección del modo combined-page siguiente.
            routes.Add(new DynamicDataRoute("{table}/{action}.aspx")
            {
                Constraints = new RouteValueDictionary(new { action = "List|Details|Edit|Insert" }),
                Model = DefaultModel
            });

            // Las siguientes declaraciones admiten el modo combined-page, donde las tareas List, Detail, Insert y
            // Update se realizan usando la misma página. Para habilitar este modo, quite las marcas de comentario de los siguientes objetos
            // routes y marque como comentario la definición del objeto route en la sección del modo de páginas independientes anterior.
            routes.Add(new DynamicDataRoute("{table}/ListDetails.aspx") {
                Action = PageAction.List,
                ViewName = "ListDetails",
                Model = DefaultModel
            });

            //routes.Add(new DynamicDataRoute("{table}/ListDetails.aspx") {
            //    Action = PageAction.Details,
            //    ViewName = "ListDetails",
            //    Model = DefaultModel
            //});
        }

        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            try
            {
                if ((DateTime.Now - Sesion.UltimaActividad).TotalMinutes > 10)
                {
                    Sesion.Actividad("OI", Tiempo: Sesion.UltimaActividad);
                    Sesion.Actividad("OO");
                }
                Sesion.UltimaActividad = DateTime.Now;
            }
            catch
            {

            }

        }
        protected void Application_EndRequest(object sender, EventArgs e)
        {
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
//            General.db = new CobranzasDataContext();

            HttpCookie cookie = Request.Cookies["Culture"];
            String Cultura = cookie == null ? "es-VE" : (cookie.Value ?? "es-VE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Cultura);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Cultura);
            Debug.Print(CultureInfo.CurrentCulture.ToString());
            Debug.Print(CultureInfo.CurrentUICulture.ToString());
            switch (Cultura)
            {
                case "en-US": 
                    General.FormatoFechaW = "ddd MM/dd/yyyy"; 
                    General.FormatoFecha = "MM/dd/yyyy"; 
                    General.FormatoFechaHora = "MM/dd/yyyy hh:mmtt"; 
                    //General.reFecha = "^$";
                    break;
                case "pt-BR": 
                    General.FormatoFechaW = "ddd MM/dd/yyyy"; 
                    General.FormatoFecha = "dd/MM/yyyy"; 
                    General.FormatoFechaHora = "MM/dd/yyyy hh:mmtt";
                    //General.reFecha = "^$";
                    break;
                default: 
                    General.FormatoFechaW = "ddd dd/MM/yyyy"; 
                    General.FormatoFecha = "dd/MM/yyyy"; 
                    General.FormatoFechaHora = "dd/MM/yyyy hh:mmtt";
                    //General.reFecha = @"^[0123]?\d[-/.][01]?\d$";
                    break;
            }

        }
        void Application_Error(object sender, EventArgs e)
        {
            Server.GetLastError().Informar();
        }

        void Session_Start(object sender, EventArgs e)
        {
        }

        void Session_End(object sender, EventArgs e)
        {
            try
            {
                Sesion.Actividad("LO", Session);
            }
            catch (Exception Ex)
            {
                Debug.Print("Qué pasó?");
            }
        }
    }
}
