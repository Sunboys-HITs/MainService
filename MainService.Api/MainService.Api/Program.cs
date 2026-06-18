using MainService.Api.Features.ClassRooms;
using MainService.Application.Common;
using MainService.Application.Features.ClassRooms.Commands;
using MainService.Application.Features.ClassRooms.Queries;
using MainService.Db.Data;
using MainService.Db.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

var classRooms = app.MapGroup("/classrooms")
    .WithTags("ClassRooms");

classRooms.MapPost("/", async (
        CreateClassRoomRequest request,
        CreateClassRoomCommandHandler handler,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var classRoom = await handler.Handle(
                new CreateClassRoomCommand(request.Title, request.AdminId),
                cancellationToken);

            return Results.Created($"/classrooms/{classRoom.Id}", classRoom);
        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(new { exception.Message });
        }
    })
    .WithName("CreateClassRoom");

classRooms.MapGet("/", async (
        GetClassRoomsQueryHandler handler,
        CancellationToken cancellationToken) =>
    {
        var classRoomsResult = await handler.Handle(new GetClassRoomsQuery(), cancellationToken);

        return Results.Ok(classRoomsResult);
    })
    .WithName("GetClassRooms");

classRooms.MapGet("/{id:guid}", async (
        Guid id,
        GetClassRoomByIdQueryHandler handler,
        CancellationToken cancellationToken) =>
    {
        var classRoom = await handler.Handle(new GetClassRoomByIdQuery(id), cancellationToken);

        return classRoom is null
            ? Results.NotFound()
            : Results.Ok(classRoom);
    })
    .WithName("GetClassRoomById");

classRooms.MapPut("/{id:guid}", async (
        Guid id,
        UpdateClassRoomRequest request,
        UpdateClassRoomCommandHandler handler,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var classRoom = await handler.Handle(
                new UpdateClassRoomCommand(id, request.Title, request.AdminId),
                cancellationToken);

            return Results.Ok(classRoom);
        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(new { exception.Message });
        }
        catch (EntityNotFoundException exception)
        {
            return Results.NotFound(new { exception.Message });
        }
        catch (ForbiddenAccessException exception)
        {
            return Results.Problem(exception.Message, statusCode: StatusCodes.Status403Forbidden);
        }
    })
    .WithName("UpdateClassRoom");

classRooms.MapDelete("/{id:guid}", async (
        Guid id,
        Guid adminId,
        DeleteClassRoomCommandHandler handler,
        CancellationToken cancellationToken) =>
    {
        try
        {
            await handler.Handle(new DeleteClassRoomCommand(id, adminId), cancellationToken);

            return Results.NoContent();
        }
        catch (EntityNotFoundException exception)
        {
            return Results.NotFound(new { exception.Message });
        }
        catch (ForbiddenAccessException)
        {
            return Results.Problem("Only classroom admins can delete classroom.", statusCode: StatusCodes.Status403Forbidden);
        }
    })
    .WithName("DeleteClassRoom");

app.Run();
