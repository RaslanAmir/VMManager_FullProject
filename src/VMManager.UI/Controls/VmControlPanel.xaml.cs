using System.Windows;
using System.Windows.Controls;
using VMManager.Models; // ✅ Use unified DTO location

namespace VMManager.UI.Controls
{
    /// <summary>
    /// A reusable control panel for VM operations.
    /// </summary>
    public partial class VmControlPanel : UserControl
    {
        public VmControlPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DependencyProperty for binding a VM instance to this control.
        /// </summary>
        public static readonly DependencyProperty VMProperty = DependencyProperty.Register(
            nameof(VM),
            typeof(VMDto),
            typeof(VmControlPanel),
            new PropertyMetadata(default(VMDto)));

        /// <summary>
        /// Gets or sets the VM associated with this control panel.
        /// </summary>
        public VMDto VM
        {
            get => (VMDto)GetValue(VMProperty);
            set => SetValue(VMProperty, value);
        }
    }
}
