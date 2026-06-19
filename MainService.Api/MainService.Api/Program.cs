using MainService.Db.Data;
using MainService.Db.Repositories;
using Microsoft.EntityFrameworkCore;
using MainService.Application.Features.ClassRooms.Commands;
using MainService.Application.Features.ClassRooms.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<MainServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IClassRoomRepository, ClassRoomRepository>();
builder.Services.AddScoped<CreateClassRoomCommandHandler>();
builder.Services.AddScoped<UpdateClassRoomCommandHandler>();
builder.Services.AddScoped<DeleteClassRoomCommandHandler>();
builder.Services.AddScoped<GetClassRoomByIdQueryHandler>();
builder.Services.AddScoped<GetClassRoomsQueryHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
