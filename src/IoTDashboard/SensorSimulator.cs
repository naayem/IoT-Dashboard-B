using System;
using System.Threading.Tasks;

public class SensorSimulator
{
    private readonly DeviceService _deviceService;
    private readonly string _deviceId;
    private readonly Random _random;

    public SensorSimulator(DeviceService deviceService, string deviceId)
    {
        _deviceService = deviceService;
        _deviceId = deviceId;
        _random = new Random();

        // Ajouter l'appareil lors de l'initialisation
        var deviceAdded = _deviceService.AddDevice(new Device
        {
            Id = _deviceId,
            Temperature = 0,
            Humidity = 0,
            LastUpdate = DateTime.Now
        });

        if (!deviceAdded)
        {
            Console.WriteLine($"Device {_deviceId} already exists.");
        }
    }

    public async Task StartAsync()
    {
        while (true)
        {
            double temperature = _random.NextDouble() * 40; // Température entre 0 et 40
            int humidity = _random.Next(20, 80); // Humidité entre 20% et 80%

            _deviceService.UpdateDeviceData(_deviceId, temperature, humidity);

            Console.WriteLine($"Device {_deviceId}: Temp={temperature:F2}°C, Humidity={humidity}%");

            await Task.Delay(5000); // Attendre 5 secondes
        }
    }
}
