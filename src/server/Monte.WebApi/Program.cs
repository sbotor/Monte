using Monte;
using Monte.Services;
using Monte.WebApi;
using Monte.WebApi.Auth;
using Monte.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureAuth(builder.Configuration);

builder.Services.AddMonte(builder.Configuration)
    .AddScoped<IUserContextProvider, HttpUserContextProvider>()
    .AddScoped<IAgentContextProvider, HttpAgentContextProvider>()
    .AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger(builder.Configuration);

builder.Services.AddCors(x =>
{
    var origins = builder.Configuration
        .GetSection("AllowedOrigins")
        .Get<string[]>()
        ?? throw new InvalidOperationException("No CORS config found.");
    
    x.AddDefaultPolicy(
        y => y.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(origins));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseConfiguredSwaggerUi();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

await app.MigrateDatabase();

app.Run();
