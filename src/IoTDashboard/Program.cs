using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services requis pour les API et fichiers statiques
builder.Services.AddControllers();

var app = builder.Build();

// Configurer l'application pour servir les fichiers statiques et les contrôleurs API
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseDefaultFiles();   // Rediriger automatiquement vers index.html
app.UseStaticFiles();    // Servir les fichiers statiques
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Configurer les routes des API
});

// Démarrer l'API
var taskApi = Task.Run(() => app.Run());

// Simulation des capteurs (devices)
var taskSimulators = Task.Run(async () =>
{
    var deviceService = new DeviceService();

    // Créer des simulateurs pour plusieurs appareils
    var simulator1 = new SensorSimulator(deviceService, "device-001");
    var simulator2 = new SensorSimulator(deviceService, "device-002");
    var simulator3 = new SensorSimulator(deviceService, "device-003");

    // Démarrer les simulateurs
    var tasks = new[]
    {
        simulator1.StartAsync(),
        simulator2.StartAsync(),
        simulator3.StartAsync(),
        MonitorDevicesAsync(deviceService) // Surveiller les appareils pour les déconnexions
    };

    await Task.WhenAll(tasks);
});

// Attendre que l'API et les simulateurs fonctionnent en parallèle
await Task.WhenAll(taskApi, taskSimulators);


// Fonction de surveillance pour détecter les déconnexions
async Task MonitorDevicesAsync(DeviceService deviceService)
{
    while (true)
    {
        var devices = deviceService.GetDeviceData();
        foreach (var device in devices)
        {
            if (deviceService.CheckIfDeviceDisconnected(device.Id))
            {
                Console.WriteLine($"Alert: Device {device.Id} is disconnected.");
            }
        }

        // Vérifier et expirer les appareils inactifs
        deviceService.CheckAndExpireInactiveDevices();

        await Task.Delay(5000); // Vérifier toutes les 5 secondes
    }
}
