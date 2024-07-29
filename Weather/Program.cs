using Microsoft.AspNetCore.Mvc.Formatters;
using Weather.Data;
using Weather.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<WeatherRepository>();
builder.Services.AddHttpClient<WeatherService>(client => client.Timeout = TimeSpan.FromSeconds(5));

builder.Services.AddControllers();
// NOTE: Replace this code with preceding line if you want null response instead of HTTP status 204.
// builder.Services.AddControllers(opt =>
// {
//     opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
// });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var repository = scope.ServiceProvider.GetRequiredService<WeatherRepository>();
    await repository.InitializeDatabaseAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
