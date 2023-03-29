using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using TrainList.Data;
using TrainList.Service.Implementations;
using TrainList.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql("Host=localhost;Port=5432;Database=postgresdb;Username=admin;Password=admin"));
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<ITrainService, TrainService>();

builder.Services.AddMvc()
     .AddXmlSerializerFormatters()
     .AddMvcOptions(opts => {
         opts.FormatterMappings.SetMediaTypeMappingForFormat("xml", new MediaTypeHeaderValue("application/xml"));
     });

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();