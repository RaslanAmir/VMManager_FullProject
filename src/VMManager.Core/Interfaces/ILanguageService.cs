using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
namespace VMManager.Core.Interfaces
{
    public interface ILanguageService
    {
        IEnumerable<CultureInfo> GetAvailableCultures();
        CultureInfo GetCurrentCulture();
        Task SetCultureAsync(CultureInfo culture);
    }
}