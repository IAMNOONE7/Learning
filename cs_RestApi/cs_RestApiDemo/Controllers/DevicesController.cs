using cs_RestApiDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cs_RestApiDemo.Controllers
{
    /*
      ===========================================================
      REST API – Industrial Devices Example (C# ASP.NET Core)
      ===========================================================

      PURPOSE
      -----------
      This controller simulates a simple REST API for managing
      industrial devices — such as PLCs, HMIs, or drives — 
      that you might find in a production line.

      You can:
      - View all devices (GET)
      - View one device by ID (GET {id})
      - Add a new device (POST)
      - Update an existing device (PUT)
      - Delete a device (DELETE)

      This example shows the standard CRUD operations that
      every REST API implements, using JSON for communication.

      -----------------------------------------------------------
      HOW ASP.NET CORE MAKES IT WORK
      -----------------------------------------------------------

      1️)  [ApiController]
          - Tells ASP.NET Core that this class is a REST controller.
          - It automatically:
            - Validates JSON input (based on model properties)
            - Returns 400 Bad Request when the model is invalid
            - Infers [FromBody] and [FromRoute] automatically
              so you don’t have to write them explicitly.

      2️)  [Route("api/[controller]")]
          - Defines the base URL for all endpoints in this class.
          - [controller] is replaced by the controller’s name
            (minus the word “Controller”).
            - DevicesController - /api/devices
          - So each method route becomes:
            - /api/devices for [HttpGet], [HttpPost]
            - /api/devices/{id} for [HttpGet("{id}")], etc.

      3️)  [HttpGet], [HttpPost], [HttpPut], [HttpDelete]
          - Map each method to an HTTP verb:
            - GET - retrieve data
            - POST - create new data
            - PUT - update existing data
            - DELETE - remove data
          - Swagger (via Swashbuckle) automatically reads these
            and generates the interactive documentation you see.

      4️)  ActionResult<T> and IActionResult
          - Allow returning both a result object (like Device)
            and an HTTP status code (e.g., 200 OK, 404 NotFound).
          - Example:
              return Ok(device);        - 200 OK + JSON body
              return NotFound();        - 404 Not Found
              return NoContent();       - 204 No Content
              return CreatedAtAction(); - 201 Created + Location

      -----------------------------------------------------------
      HOW THE "DATABASE" WORKS HERE
      -----------------------------------------------------------

      For simplicity, we use an in-memory list:

          private static readonly List<Device> _devices = new();

      This acts as a fake database. In a real project you’d use:
      - Entity Framework Core with a DbContext
      - A SQL or PostgreSQL database
      - Or even an OPC UA / factory data source

      When you restart the app, this list resets (it’s in memory).

      -----------------------------------------------------------
      EXAMPLE ENDPOINTS
      -----------------------------------------------------------

      GET     /api/devices
          - Returns all devices as JSON.

      GET     /api/devices/2
          - Returns one device by ID.

      POST    /api/devices
          - Accepts JSON body:
            {
              "name": "Main PLC",
              "ipAddress": "192.168.0.10",
              "location": "Line 1",
              "isOnline": true
            }

      PUT     /api/devices/2
          - Updates an existing device (full object).

      DELETE  /api/devices/2
          - Removes device from list.

      -----------------------------------------------------------
      HOW THE REQUEST–RESPONSE FLOW WORKS
      -----------------------------------------------------------

      Client (e.g., browser or WPF app) - sends HTTP request
      - ASP.NET Core router - finds correct controller/method
      - Executes method - returns ActionResult
      - ASP.NET serializes result object to JSON automatically

      Example:
          Request:
              GET /api/devices
          Response:
              200 OK
              [
                {
                  "id": 1,
                  "name": "Main PLC",
                  "ipAddress": "192.168.0.10",
                  "location": "Line 1",
                  "isOnline": true
                }
              ]    
      */



    [Route("api/[controller]")] // -> api/devices
    [ApiController]
    public class DevicesController : ControllerBase
    {
        // For demo purposes we use an in-memory list.
        // In real life this would be a database or an OPC UA source.
        private static readonly List<Device> _devices = new()
        {
            new Device { Id = 1, Name = "Main PLC", IpAddress = "192.168.0.10", Location = "Line 1", IsOnline = true },
            new Device { Id = 2, Name = "HMI Panel", IpAddress = "192.168.0.11", Location = "Line 1", IsOnline = false },
            new Device { Id = 3, Name = "Drive Cabinet", IpAddress = "192.168.0.12", Location = "Line 2", IsOnline = true }
        };

        // ============================================================
        // GET: api/devices
        // Returns all devices.
        // ============================================================
        [HttpGet]
        public ActionResult<IEnumerable<Device>> GetAll()
        {
            return Ok(_devices);
        }

        // ============================================================
        // GET: api/devices/3
        // Returns single device by id.
        // ============================================================
        [HttpGet("{id}")]
        public ActionResult<Device> GetById(int id)
        {
            var device = _devices.FirstOrDefault(d => d.Id == id);
            if (device == null)
                return NotFound(); // 404
            return Ok(device);     // 200 + JSON
        }

        // ============================================================
        // POST: api/devices
        // Create a new device.
        // Body (JSON): { "name": "...", "ipAddress": "...", "location": "...", "isOnline": false }
        // ============================================================
        [HttpPost]
        public ActionResult<Device> Create(Device newDevice)
        {
            // naive id generator
            newDevice.Id = _devices.Count == 0 ? 1 : _devices.Max(d => d.Id) + 1;
            _devices.Add(newDevice);

            // Returns 201 + Location header + new object
            return CreatedAtAction(nameof(GetById), new { id = newDevice.Id }, newDevice);
        }

        // ============================================================
        // PUT: api/devices/2
        // Full update of existing device.
        // ============================================================
        [HttpPut("{id}")]
        public IActionResult Update(int id, Device updated)
        {
            var device = _devices.FirstOrDefault(d => d.Id == id);
            if (device == null)
                return NotFound();

            device.Name = updated.Name;
            device.IpAddress = updated.IpAddress;
            device.Location = updated.Location;
            device.IsOnline = updated.IsOnline;

            return NoContent(); // 204
        }

        // ============================================================
        // DELETE: api/devices/2
        // Delete device by id.
        // ============================================================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var device = _devices.FirstOrDefault(d => d.Id == id);
            if (device == null)
                return NotFound();

            _devices.Remove(device);
            return Ok(); // or NoContent()
        }
    }
}
