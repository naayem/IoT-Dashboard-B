using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IoTDashboard.Tests
{
    public class DeviceServiceTests
    {
        [Fact]
        public void Should_Display_Device_Data_When_Devices_Exist()
        {
            // Arrange : Simuler un service d'appareils IoT avec des données
            var deviceService = new DeviceService();
            deviceService.AddDevice(new Device { Id = "device-001", Temperature = 25.5, Humidity = 60 });

            // Act : Récupérer les données des appareils
            var result = deviceService.GetDeviceData();

            // Assert : Vérifier que le tableau contient les données correctes
            Assert.NotEmpty(result);
            Assert.Equal("device-001", result.First().Id);
            Assert.Equal(25.5, result.First().Temperature);
            Assert.Equal(60, result.First().Humidity);
        }

        [Fact]
        public void Should_Display_Empty_When_No_Devices_Exist()
        {
            // Arrange : Créer un service d'appareils IoT vide
            var deviceService = new DeviceService();

            // Act : Récupérer les données
            var result = deviceService.GetDeviceData();

            // Assert : Vérifier que le tableau est vide
            Assert.Empty(result);
        }

        [Fact]
        public async Task Should_Update_Data_Every_5_Seconds()
        {
            // Arrange : Simuler un service d'appareils
            var deviceService = new DeviceService();
            deviceService.AddDevice(new Device { Id = "device-001", Temperature = 20.0, Humidity = 50 });

            // Act : Récupérer les données initiales
            var initialData = deviceService.GetDeviceData();

            // Simuler une mise à jour toutes les 5 secondes
            await Task.Delay(5000);
            deviceService.UpdateDeviceData("device-001", 22.0, 55);

            // Récupérer les nouvelles données
            var updatedData = deviceService.GetDeviceData();

            // Assert : Vérifier que les données ont été mises à jour après 5 secondes
            Assert.NotEqual(initialData.First().Temperature, updatedData.First().Temperature);
            Assert.Equal(22.0, updatedData.First().Temperature);
        }

        [Fact]
        public void Should_Not_Allow_Duplicate_Device_Ids()
        {
            // Arrange
            var deviceService = new DeviceService();
            deviceService.AddDevice(new Device { Id = "device-001", Temperature = 25.5, Humidity = 60 });

            // Act : Tenter d'ajouter un appareil avec un ID existant
            var result = deviceService.AddDevice(new Device { Id = "device-001", Temperature = 30.0, Humidity = 70 });

            // Assert : Vérifier que l'ajout du deuxième appareil échoue
            Assert.False(result);
            Assert.Single(deviceService.GetDeviceData());
        }

        [Fact]
        public void Should_Remove_Device_By_Id()
        {
            // Arrange
            var deviceService = new DeviceService();
            deviceService.AddDevice(new Device { Id = "device-001", Temperature = 25.5, Humidity = 60 });
            deviceService.AddDevice(new Device { Id = "device-002", Temperature = 22.0, Humidity = 50 });

            // Act : Supprimer un appareil
            deviceService.RemoveDevice("device-001");

            // Assert : Vérifier que l'appareil a été supprimé
            var remainingDevices = deviceService.GetDeviceData();
            Assert.Single(remainingDevices);
            Assert.Equal("device-002", remainingDevices.First().Id);
        }

        [Fact]
        public void Should_Reset_All_Devices_Data()
        {
            // Arrange
            var deviceService = new DeviceService();
            deviceService.AddDevice(new Device { Id = "device-001", Temperature = 25.5, Humidity = 60 });
            deviceService.AddDevice(new Device { Id = "device-002", Temperature = 22.0, Humidity = 50 });

            // Act : Réinitialiser les données de tous les appareils
            deviceService.ResetAllDevices();

            // Assert : Vérifier que les données de tous les appareils ont été réinitialisées
            var devices = deviceService.GetDeviceData();
            Assert.All(devices, d =>
            {
                Assert.Equal(0, d.Temperature);
                Assert.Equal(0, d.Humidity);
            });
        }

        [Fact]
        public async Task Should_Expire_Inactive_Device_After_One_Minute()
        {
            // Arrange
            var deviceService = new DeviceService();
            deviceService.AddDevice(new Device { Id = "device-001", Temperature = 25.5, Humidity = 60 });

            // Act : Attendre plus d'une minute sans mettre à jour l'appareil
            await Task.Delay(60000);
            deviceService.CheckAndExpireInactiveDevices();

            // Assert : Vérifier que l'appareil a été supprimé pour inactivité
            var devices = deviceService.GetDeviceData();
            Assert.Empty(devices);
        }

        [Fact]
        public void Should_Throw_Exception_When_Updating_Nonexistent_Device()
        {
            // Arrange
            var deviceService = new DeviceService();

            // Act & Assert : Vérifier qu'une exception est levée lorsqu'on essaie de mettre à jour un appareil inexistant
            Assert.Throws<DeviceNotFoundException>(() => deviceService.UpdateDeviceData("nonexistent-device", 30.0, 70));
        }
    }
}
