# Dashboard Wolverine - Monitoring Library

Library monitoring dan dashboard untuk Wolverine Framework dengan full CRUD operations menggunakan raw SQL queries.

## 🚀 Fitur

- ✅ **Dead Letters Management** - View, replay, dan delete dead letters
- ✅ **Bulk Replay** - Replay multiple dead letters sekaligus
- ✅ **Incoming Envelopes** - Monitor envelopes yang masuk
- ✅ **Nodes Management** - Monitor Wolverine nodes
- ✅ **Node Assignments** - Lihat assignment nodes
- ✅ **Real-time Stats** - Dashboard statistik real-time
- ✅ **Auto Refresh** - Refresh otomatis data
- ✅ **Search & Filter** - Cari data dengan mudah

## 📦 Instalasi

### 1. Install NuGet Package

```bash
dotnet add package Npgsql
```

### 2. Update Connection String

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "WolverineDb": "Host=localhost;Port=5432;Database=your_database;Username=your_username;Password=your_password"
  }
}
```

### 3. Register Service di Program.cs

```csharp
using DashboardWolverine;
using DashboardWolverine.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add Wolverine Repository
var connectionString = builder.Configuration.GetConnectionString("WolverineDb") 
    ?? throw new InvalidOperationException("Connection string 'WolverineDb' not found.");
builder.Services.AddSingleton(new WolverineRepository(connectionString));

builder.Services.AddControllers();

var app = builder.Build();

// Use Monitoring Dashboard
app.UseMonitoringDashboard(options =>
{
    options.RoutePrefix = "/monitoring";
    options.DashboardTitle = "Dashboard Wolverine - Monitoring";
    options.EnableAutoRefresh = true;
    options.AutoRefreshIntervalSeconds = 30;
});

app.MapControllers();
app.Run();
```

## 🎯 API Endpoints

### Dashboard Stats
```
GET /api/wolverine/stats
```

### Dead Letters
```
GET    /api/wolverine/dead-letters                    // List all dead letters
GET    /api/wolverine/dead-letters/{id}               // Get by ID (require receivedAt query param)
PUT    /api/wolverine/dead-letters/{id}/replay        // Set replayable status
PUT    /api/wolverine/dead-letters/replay-multiple    // Bulk replay
DELETE /api/wolverine/dead-letters/{id}               // Delete dead letter
```

### Incoming Envelopes
```
GET    /api/wolverine/incoming-envelopes              // List all incoming envelopes
GET    /api/wolverine/incoming-envelopes/{id}         // Get by ID
DELETE /api/wolverine/incoming-envelopes/{id}         // Delete envelope
```

### Nodes
```
GET    /api/wolverine/nodes                           // List all nodes
GET    /api/wolverine/nodes/{id}                      // Get by ID
DELETE /api/wolverine/nodes/{id}                      // Delete node
```

### Node Assignments
```
GET    /api/wolverine/node-assignments                // List all assignments
GET    /api/wolverine/node-assignments/{id}           // Get by ID
DELETE /api/wolverine/node-assignments/{id}           // Delete assignment
```

## 🖥️ Dashboard UI

Akses dashboard melalui browser:

```
https://localhost:5001/monitoring
```

atau untuk dashboard Wolverine khusus:

```
https://localhost:5001/monitoring/wolverine-dashboard.html
```

### Fitur Dashboard:

1. **Dead Letters Tab**
   - List semua dead letters
   - Select multiple items untuk bulk replay/unreplay
   - Search & filter
   - View detail message
   - Delete individual dead letter

2. **Incoming Envelopes Tab**
   - Monitor envelopes yang masuk
   - Filter by status dan message type
   - View attempts dan execution time

3. **Nodes Tab**
   - Monitor status nodes (Healthy/Unhealthy)
   - Health check monitoring
   - Node capabilities

4. **Node Assignments Tab**
   - View node assignments
   - Monitor started time

## 📊 Contoh Request

### Replay Single Dead Letter

```bash
curl -X PUT "https://localhost:5001/api/wolverine/dead-letters/{id}/replay?receivedAt=2024-01-01" \
  -H "Content-Type: application/json" \
  -d '{"replayable": true}'
```

### Bulk Replay Multiple Dead Letters

```bash
curl -X PUT "https://localhost:5001/api/wolverine/dead-letters/replay-multiple" \
  -H "Content-Type: application/json" \
  -d '{
    "deadLetters": [
      {"id": "guid-1", "receivedAt": "2024-01-01"},
      {"id": "guid-2", "receivedAt": "2024-01-02"}
    ],
    "replayable": true
  }'
```

### Get Dashboard Stats

```bash
curl -X GET "https://localhost:5001/api/wolverine/stats"
```

Response:
```json
{
  "totalDeadLetters": 10,
  "replayableDeadLetters": 5,
  "totalIncomingEnvelopes": 25,
  "activeNodes": 2,
  "timestamp": "2024-01-01T00:00:00Z"
}
```

## 🗄️ Database Tables

Library ini bekerja dengan tabel Wolverine standar:

- `wolverine_dead_letters`
- `wolverine_incoming_envelopes`
- `wolverine_nodes`
- `wolverine_node_assignments`

## ⚙️ Configuration Options

```csharp
app.UseMonitoringDashboard(options =>
{
    options.RoutePrefix = "/monitoring";                    // Dashboard URL prefix
    options.DashboardTitle = "My Dashboard";                // Dashboard title
    options.DefaultDataEndpoint = "/api/wolverine/stats";   // Default API endpoint
    options.EnableAutoRefresh = true;                       // Enable auto refresh
    options.AutoRefreshIntervalSeconds = 30;                // Refresh interval
});
```

## 🔒 Security

**PENTING**: Dashboard ini tidak memiliki autentikasi built-in. Untuk production:

1. Gunakan middleware authentication (JWT, Cookie, dll)
2. Tambahkan authorization pada controller
3. Restrict access by IP/Network
4. Gunakan HTTPS

Contoh menambahkan authorization:

```csharp
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/wolverine")]
public class WolverineController : ControllerBase
{
    // ... endpoints
}
```

## 🛠️ Development

### Project Structure

```
DashboardWolverine/
├── Models/
│   ├── WolverineDeadLetter.cs
│   ├── WolverineIncomingEnvelope.cs
│   ├── WolverineNode.cs
│   └── WolverineNodeAssignment.cs
├── Repositories/
│   └── WolverineRepository.cs
├── Controllers/
│   ├── WolverineController.cs
│   └── MonitoringController.cs
├── Extensions/
│   └── MonitoringDashboardExtensions.cs
├── Middleware/
│   └── MonitoringDashboardMiddleware.cs
├── Options/
│   └── MonitoringDashboardOptions.cs
└── wwwroot/
    └── monitoring/
        ├── dashboard.html
        └── wolverine-dashboard.html
```

## 📝 License

MIT

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## 📞 Support

Untuk pertanyaan atau issue, silakan buka GitHub issue.
