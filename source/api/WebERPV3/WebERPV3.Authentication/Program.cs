using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using WebERPV3.Common.Interfaces;
using WebERPV3.Context;
using WebERPV3.Entities;
using WebERPV3.Entities.Model;
using WebERPV3.Repository;

using var log = new LoggerConfiguration()
    .WriteTo.File("logs/exceptions.txt")
    .CreateLogger();
Log.Logger = log;

var builder = WebApplication.CreateBuilder(args);

var connections = builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
builder.Services.AddDbContext<MainContext>(options =>
{
    var connection = new SqlConnection(connections.Main);
    options.UseSqlServer(connection);

});
// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddAuthentication().AddJwtBearer("Bearer", options => { 
//options.
//}).AddCookie();

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

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    var jwtConfig = configuration.GetSection("jwtConfig");
    var secretKey = jwtConfig["secret"];
    services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = jwtConfig["validIssuer"],
           ValidAudience = jwtConfig["validAudience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
       };
   });
    services.AddIdentity<User, Role>(o =>
    {
        o.Password.RequireDigit = true;
        o.Password.RequireLowercase = false;
        o.Password.RequireUppercase = true;
        o.Password.RequireNonAlphanumeric = true;
        o.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<MainContext>().AddDefaultTokenProviders();

 
   

    services.AddMemoryCache();
    services.AddCors(o => o.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }));

    var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

    services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddSingleton<IFileProvider>(
             new PhysicalFileProvider(
                 Path.Combine(Directory.GetCurrentDirectory(), "")));
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    //services.AddScoped<ICustomerService, CustomerService>();
    services.AddScoped<ITenantService, TenantService>();
    services.AddScoped<IDataRepositoryFactory, DataRepositoriesFactory>();//
    //services.AddScoped<IDataServiceFactory, DataServiceFactory>();
    services.AddTransient(typeof(IBase<>), typeof(Repository<>));
    //services.AddTransient(typeof(IBaseService<>), typeof(Service<>));
    //services.AddScoped<IInvoiceService, InvoiceService>();
    //services.AddScoped<IProductService, ProductService>();
    //services.AddScoped<IUserService, UserService>();//IBranchOfficeService
    //services.AddScoped<IUserAuthenticationRepository, UserAuthenticationRepository>();
    //services.AddScoped<IBranchOfficeService, BranchOfficeService>();
    //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    //services.AddAutoMapper(new CommonData().GetType().Assembly);
}
