using Helpdesk.Application.Services;
using Helpdesk.Infrastructure.Impl;
using Helpdesk.Infrastructure.Impl.Options;
using Helpdesk.Infrastructure.Impl.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSO.DataAccess.Portgresql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<TelegramOptions>(options => builder.Configuration.GetSection(TelegramOptions.SectionName).Bind(options));
builder.Services.Configure<EmailOptions>(options => builder.Configuration.GetSection(EmailOptions.SectionName).Bind(options));
builder.Services.AddDataAccessPostgresql(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();


var telegramService = app.Services.GetRequiredService<TelegramService>();
await telegramService.StartReceivingAsync();

app.UseHttpsRedirection();
app.Run();