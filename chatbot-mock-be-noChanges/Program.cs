using chatbot_mock_be.Data;
using chatbot_mock_be.Data.Concretes;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

builder.Services.AddScoped<IGetStreamClient, GetStreamClient>();
builder.Services.AddScoped<Channel, WebChannel>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// This method is not executed by ASP.NET Core runtime
// Use CreateHostBuilder method to configure the web application and host
// static void Main()
// {
//     Type type = typeof(MessageClient); // Replace IRestClient with the type you're interested in
// 
//     Assembly assembly = type.Assembly;
//     string assemblyName = assembly.GetName().Name;
// 
//     Console.WriteLine($"The assembly name for type {type.FullName} is: {assemblyName}");
// }

app.UseCors(); // Apply CORS policy

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
});

app.UseRouting(); // Enable routing

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Map controllers routes
});

app.Run();