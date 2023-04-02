using ZipService;

var builder = WebApplication.CreateBuilder(args);

AppConfiguration.ConfigureServices(builder.Services);

DependencyInjection.RegisterDependencies(builder);

var app = builder.Build();

AppConfiguration.Configure(app, app.Environment);

app.Run();

public partial class Program { }