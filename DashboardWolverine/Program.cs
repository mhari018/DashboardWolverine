using DashboardWolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Monitoring Dashboard services
builder.Services.AddMonitoringDashboard(options =>
{
    options.RoutePrefix = "/wolverine-ui";
    options.DashboardTitle = "Dashboard - Monitoring";
    options.WolverineConnectionString = "Host=localhost;Port=5432;Database=wv_db;Username=postgres;Password=postgres";
    
   
    options.EnableAutoRefresh = true;
    options.AutoRefreshIntervalSeconds = 60;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Use Monitoring Dashboard middleware
app.UseMonitoringDashboard();

app.MapControllers();

app.Run();
