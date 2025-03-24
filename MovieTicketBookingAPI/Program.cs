using DataAccessLayers.UnitOfWork;
using DataAccessLayers;
using Services.Interface;
using Services.Service;

using Microsoft.OpenApi.Models;
using BusinessObjects;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<MovieRepository>();
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<TicketRepository>();
builder.Services.AddScoped<TransactionRepository>();
builder.Services.AddScoped<TransactionHistoryRepository>();
builder.Services.AddScoped<TransactionTypeRepository>();
builder.Services.AddScoped<ShowTimeRepository>();
builder.Services.AddScoped<SeatRepository>();
builder.Services.AddScoped<CinemaRoomRepository>();
builder.Services.AddScoped<BillRepository>();

//UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddDbContext<MovieprojectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DB")));

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionHistoryService, TransactionHIstoryService>();
builder.Services.AddScoped<ITransactionTypeService, TransactionTypeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IShowTimeService, ShowTimeService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<ICinemaRoomService, CinemaRoomService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider.GetRequiredService<MovieprojectContext>();
    context.Database.Migrate();
}

app.Run();
