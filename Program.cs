using Microsoft.EntityFrameworkCore;
using ShareUp.API.Data;
using ShareUp.API.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ShareUpDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ISettlementService, SettlementService>();
/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins("http://localhost:4200", "http://localhost:58379")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
*/
builder.Services.AddCors(options =>
{
  options.AddPolicy("AngularLocalPolicy",
      policy =>
      {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost") // يسمح بأي بورت من localhost
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
//app.UseCors("AllowAngular");
app.UseCors("AngularLocalPolicy");

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{*/
  app.UseSwagger();
  app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
