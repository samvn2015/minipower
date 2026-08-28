using Hrm.Iam.Host.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.AddHostLayer();

var app = builder.Build();
app.UseHostLayer();
app.Run();
