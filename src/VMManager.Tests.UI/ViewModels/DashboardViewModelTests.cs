// 📍 File: src/VMManager.Tests.UI/ViewModels/DashboardViewModelTests.cs
using System.Threading.Tasks;
using Moq;
using VMManager.Services.Interfaces;
using VMManager.UI.ViewModels;
using Xunit;

namespace VMManager.Tests.UI.ViewModels
{
    public class DashboardViewModelTests
    {
        [Fact]
        public async Task ShowWelcomeMessageCommand_CallsDialogService()
        {
            // Arrange
            var mockDialogService = new Mock<IDialogService>();
            var vm = new DashboardViewModel(mockDialogService.Object);

            // Act
            await vm.ShowWelcomeMessageCommand.ExecuteAsync(null);

            // Assert
            mockDialogService.Verify(x => x.ShowMessageAsync("Welcome to the dashboard!", "Information"), Times.Once);
        }
    }
}
