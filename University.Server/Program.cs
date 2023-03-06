using AutoMapper;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using University.Server.Domain.Models;
using University.Server.Domain.Persistence.Entities;
using University.Server.Domain.Persistence.Repositories;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services;
using University.Server.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
                      builder =>
                      {
                          builder.WithOrigins("https://localhost",
                                              "http://localhost");
                      });
});

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    var enumConverter = new JsonStringEnumConverter();
    opts.JsonSerializerOptions.Converters.Add(enumConverter);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "University Administration API",
        Description = "An ASP.NET Core Web API for university administration.",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICosmosDbRepository<User, UserEntity>, CosmosDbRepository<User, UserEntity>>();

builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<ICosmosDbRepository<University.Server.Domain.Models.Module, ModuleEntity>, CosmosDbRepository<University.Server.Domain.Models.Module, ModuleEntity>>();

builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ICosmosDbRepository<Location, LocationEntity>, CosmosDbRepository<Location, LocationEntity>>();

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICosmosDbRepository<Course, CourseEntity>, CosmosDbRepository<Course, CourseEntity>>();

builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ICosmosDbRepository<Semester, SemesterEntity>, CosmosDbRepository<Semester, SemesterEntity>>();

//builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
//{
//    cfg.AddProfile(new ModelToEntityProfile());
//    cfg.AddProfile(new EntityToModelProfile(
//        provider.CreateScope().ServiceProvider.GetService<IUserService>(),
//        provider.CreateScope().ServiceProvider.GetService<IModuleService>(),
//        provider.CreateScope().ServiceProvider.GetService<ILocationService>()));
//    cfg.AddProfile(new ResourceToModelProfile(
//        provider.CreateScope().ServiceProvider.GetService<IUserService>()));
//    cfg.AddProfile(new ModelToResourceProfile());

//}).CreateMapper());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("_myAllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
