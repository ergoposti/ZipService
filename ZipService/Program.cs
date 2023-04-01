using ZipService;

var builder = WebApplication.CreateBuilder(args);

AppConfiguration.ConfigureServices(builder.Services);

var app = builder.Build();

AppConfiguration.Configure(app, app.Environment);

app.Run();