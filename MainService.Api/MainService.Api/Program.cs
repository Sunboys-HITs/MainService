using MainService.Db.Data;
using MainService.Db.Repositories;
using Microsoft.EntityFrameworkCore;
using MainService.Application.Features.ClassRooms.Commands;
using MainService.Application.Features.ClassRooms.Queries;
using MainService.Application.Features.Solutions.Commands;
using MainService.Application.Features.Solutions.Queries;
using MainService.Application.Features.Tasks.Commands;
using MainService.Application.Features.Tasks.Queries;
using MainService.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<MainServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IClassRoomRepository, ClassRoomRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ISolutionRepository, SolutionRepository>();
builder.Services.AddScoped<CreateClassRoomCommandHandler>();
builder.Services.AddScoped<UpdateClassRoomCommandHandler>();
builder.Services.AddScoped<DeleteClassRoomCommandHandler>();
builder.Services.AddScoped<GetClassRoomByIdQueryHandler>();
builder.Services.AddScoped<GetClassRoomsQueryHandler>();
builder.Services.AddScoped<CreateTaskCommandHandler>();
builder.Services.AddScoped<UpdateTaskCommandHandler>();
builder.Services.AddScoped<DeleteTaskCommandHandler>();
builder.Services.AddScoped<GetTaskByIdQueryHandler>();
builder.Services.AddScoped<GetClassRoomTasksQueryHandler>();
builder.Services.AddScoped<CreateSolutionCommandHandler>();
builder.Services.AddScoped<UpdateSolutionStatusCommandHandler>();
builder.Services.AddScoped<GetSolutionByIdQueryHandler>();
builder.Services.AddScoped<GetTaskSolutionsQueryHandler>();
builder.Services.AddScoped<GetUserSolutionsQueryHandler>();
builder.Services.AddRabbitMqCodeExecution(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
