using Wpf.Ui.Controls;

namespace VMManager.UI.Views
{
    /// <summary>
    /// Represents the UI page responsible for configuring and managing scheduled backup and restore jobs.
    /// Bound to the <see cref="ViewModels.ScheduleViewModel"/>.
    /// </summary>
    public partial class ScheduleView : UiPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleView"/> class and its components.
        /// </summary>
        public ScheduleView()
        {
            InitializeComponent();
        }
    }
}
