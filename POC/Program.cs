using POC.DataAccess;
using POC.Repositories;
using POC.Services;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
.WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

// Add services to the container.
builder.Host.UseSerilog();

builder.Services.AddControllers();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngularApp", policy =>
//    {
//        policy.WithOrigins("http://localhost:4200")
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://resource-tracker-app-chi.vercel.app"
              )
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});




builder.Services.AddScoped<IDbHelper, DbHelper>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IDbHelper, DbHelper>();


//builder.Services.AddSingleton<EmployeeService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





//var app = builder.Build();


//app.UseCors("AllowAngularApp");

////app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

//app.UseSwagger();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{ 
//    app.UseSwaggerUI();
//}


//app.UseHttpsRedirection();

//app.UseAuthentication();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

var app = builder.Build();

// Use Swagger (optional)
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

// Ensure routing comes before CORS
app.UseRouting();

// Apply CORS after routing, before auth
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Endpoint mapping comes last
app.MapControllers();

app.Run();



