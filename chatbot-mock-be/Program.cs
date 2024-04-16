using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});
builder.Services.AddCors(optionts =>
{
    optionts.AddDefaultPolicy(corsPolicyBuilder =>
        corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
});
app.UseCors();
app.MapGet("/{question}", (string question) =>
    {
        switch (question)
        {
            case "Hello":
                return new { Message = "Hello, how can I assist you?" };
        }
        return null;
    }).WithDescription("Some Method Description")
    .WithOpenApi();

app.Run();