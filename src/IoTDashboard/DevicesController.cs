using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly DeviceService _deviceService;

    public DevicesController()
    {
        _deviceService = new DeviceService();
    }

    // Endpoint pour récupérer les données des capteurs
    [HttpGet]
    public ActionResult<IEnumerable<Device>> GetDevices()
    {
        var devices = _deviceService.GetDeviceData();
        return Ok(devices);
    }
}
