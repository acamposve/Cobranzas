using System.Threading;
using System.Globalization;
using System.Web;
namespace Cobranzas
{
    public class Pagina : System.Web.UI.Page
    {
        protected override void InitializeCulture()
        {
            /*HttpCookie cookie = Request.Cookies["Culture"];
            string Culture=cookie==null?"es":cookie.Value;
            this.Culture = Culture;
            this.UICulture = Culture;
            // Establecer cultura.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(Culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Culture);
            // Guardar valores actuales para poder restaurar en futuras peticiones.
            base.InitializeCulture();*/
        }
    }
    public class PaginaMaestra : System.Web.UI.MasterPage
    {
    }
}