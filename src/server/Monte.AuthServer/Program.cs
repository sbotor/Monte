using Monte.AuthServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.RegisterServices(builder.Configuration);

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
