using System.Web;
using System.Web.Mvc;

namespace Invest_Cloud_Technical
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
