using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cobranzas
{
    public static class Sesion
    {
        public static void Actividad(String Accion, System.Web.SessionState.HttpSessionState sesion = null, DateTime? Tiempo = null)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (sesion == null) sesion = HttpContext.Current.Session;
                    Entidades.Acciones Ac = new Entidades.Acciones() { Accion = Accion, Fecha = Tiempo ?? DateTime.Now, idOperador = Sesion.idOperador };
                    db.Acciones.InsertOnSubmit(Ac);
                    db.SubmitChanges();
                }
            }
            catch (Exception Ex)
            {
                //Ex.Registrar();
            }
        }
        public static void SelPersona(int? idPersona)
        {
            if (Sesion.idPersona == idPersona) return; //misma persona, no hacer nada... (null,null) (no null, no null);
            if (Sesion.idPersona != null) //cambio de persona o finalizar sesión (null,no null) (no null,no null);
            {
                if (Sesion.TiempoGestion.ContainsKey(Sesion.idPersona.Value))
                {
                    Sesion.TiempoGestion[Sesion.idPersona.Value] += Convert.ToInt32((DateTime.Now - Sesion.InicioGestion.Value).TotalMinutes);
                }
                else
                {
                    Sesion.TiempoGestion.Add(Sesion.idPersona.Value, Convert.ToInt32((DateTime.Now - Sesion.InicioGestion.Value).TotalMinutes));
                }
            }
            Sesion.InicioGestion = DateTime.Now;
            Sesion.idPersona = idPersona;
        }
        public static DateTime UltimaActividad
        {
            get
            {
                DateTime? Temp = (DateTime?)HttpContext.Current.Session["UltimaActividad"];
                if (Temp != null) return Temp.Value;
                Temp = DateTime.Now;
                HttpContext.Current.Session["UltimaActividad"] = Temp;
                return Temp.Value;
            }
            set { HttpContext.Current.Session["UltimaActividad"] = value; }
        }
        public static string Cultura
        {
            get { return ((string)HttpContext.Current.Session["Cultura"] ?? "es"); }
            set { HttpContext.Current.Session["Cultura"] = value; }
        }
        public static Entidades.Operadores Operador
        {
            get { return (Entidades.Operadores)HttpContext.Current.Session["Operador"]; }
            set { HttpContext.Current.Session["Operador"] = value; }
        }
        public static string LoginOperador
        {
            get { return ((string)HttpContext.Current.Session["LoginOperador"] ?? "Anónimo"); }
            set { HttpContext.Current.Session["LoginOperador"] = value; }
        }
        public static string NombreOperador
        {
            get { return ((string)HttpContext.Current.Session["NombreOperador"] ?? "Anónimo"); }
            set { HttpContext.Current.Session["NombreOperador"] = value; }
        }
        public static int idOperador
        {
            get { return ((int?)HttpContext.Current.Session["idOperador"] ?? 0); }
            set { HttpContext.Current.Session["idOperador"] = value; }
        }
        public static int? idSupervisor
        {
            get { return ((int?)HttpContext.Current.Session["idSupervisor"]); }
            set { HttpContext.Current.Session["idSupervisor"] = value; }
        }

        public static int? idPersona
        {
            get { return ((int?)HttpContext.Current.Session["idPersona"]); }
            set { HttpContext.Current.Session["idPersona"] = value; }
        }
        public static DateTime? InicioGestion
        {
            get { return ((DateTime?)HttpContext.Current.Session["InicioGestion"]); }
            set { HttpContext.Current.Session["InicioGestion"] = value; }
        }
        public static Dictionary<int, int> TiempoGestion
        {
            get { return ((Dictionary<int, int>)HttpContext.Current.Session["TiempoGestion"]) ?? new Dictionary<int, int>(); }
            set { HttpContext.Current.Session["TiempoGestion"] = value; }
        }

        public static Boolean Impersonando
        {
            get { return ((Boolean?)HttpContext.Current.Session["Impersonando"]) ?? false; }
            set { HttpContext.Current.Session["Impersonando"] = value; }
        }

        public static void Llenar(Entidades.Operadores Op)
        {
            Operador = Op;
            idOperador = Op.idOperador;
            idSupervisor = Op.idSupervisor;
            LoginOperador = Op.Login;
            NombreOperador = Op.Nombre;
        }
    }
}