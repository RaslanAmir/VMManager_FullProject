using System.Threading.Tasks;
using Moq;
using VMManager.Application.Services;
using VMManager.Services.Interfaces;
using VMManager.Common.Logging;
using VMManager.Core.Entities;
using Xunit;

namespace VMManager.Tests.Application
{
    public class BackupServiceTests
    {
        [Fact]
        public async Task RunBackupOnceAsync_TriggersLoggingAndExport()
        {
            // Arrange
            var hostRepo = new Mock<IHostRepository>();
            hostRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(new[] { new Host { HostName = "h1" } });

            var exportService = new Mock<IExportRestoreService>();
            exportService.Setup(e => e.ExportAllVMsOnHostAsync("h1"))
                         .Returns(Task.CompletedTask);

            var logger = new Mock<ILoggingService>();

            var svc = new BackupService(
                hostRepo.Object,
                exportService.Object,
                logger.Object);

            // Act
            await svc.RunBackupOnceAsync();

            // Assert
            logger.Verify(l => l.LogInformation(It.Is<string>(s => s.Contains("h1"))), Times.AtLeastOnce);
            exportService.Verify(e => e.ExportAllVMsOnHostAsync("h1"), Times.Once);
        }
    }
}
