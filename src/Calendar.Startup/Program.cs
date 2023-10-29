using Asp.Versioning;
using Calendar.Startup.Infra;
using Calendar.Startup.Infra.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddApiVersioning(setup =>
{
    setup.DefaultApiVersion = new ApiVersion(1, 0);
    setup.AssumeDefaultVersionWhenUnspecified = true;
    setup.ReportApiVersions = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
#if DEVELOPMENT
builder.Services.addApiVersioning();
builder.Services.ConfigureSwagger();
#endif

// Add database related items
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddIdentity();
builder.Services.AddRepositories();

// Add JWT token
builder.Services.ConfigureJWT(builder.Configuration);

// Add services
builder.Services.AddServices();

// Add mapping for entity and DTO
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Seed database
DatabaseSeeder.Seed(app);

// Add swagger only in build in debug
#if DEVELOPMENT
// Add swagger only if development env
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endif

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
