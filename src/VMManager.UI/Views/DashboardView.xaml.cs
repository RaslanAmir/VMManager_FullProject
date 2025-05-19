using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace VMManager.UI.Views
{
    /// <summary>
    /// Code-behind for <see cref="DashboardView"/>, which displays grouped VM hosts and their statuses.
    /// This view acts as a UI container for monitoring and managing VM operations per host.
    /// </summary>
    public partial class DashboardView : UiPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardView"/> class and loads its components.
        /// </summary>
        public DashboardView()
        {
            InitializeComponent();
        }
    }
}
