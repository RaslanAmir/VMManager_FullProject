using System.Collections.Generic;
using System.Threading.Tasks;
namespace VMManager.Core.Interfaces
{
    public interface IThemeService
    {
        IEnumerable<string> GetAvailableThemes();
        string GetCurrentTheme();
        Task SetThemeAsync(string theme);
    }
}