using System.Globalization;
using System.Threading;

namespace VMManager.Core
{
    public static class LanguageManager
    {
        public static void ChangeLanguage(string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture   = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
