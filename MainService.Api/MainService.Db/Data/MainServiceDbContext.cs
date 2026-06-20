using MainService.Db.DbModels;
using Microsoft.EntityFrameworkCore;

namespace MainService.Db.Data;

public class MainServiceDbContext(DbContextOptions<MainServiceDbContext> options) : DbContext(options)
{
    public DbSet<ClassRoomDbModel> ClassRooms => Set<ClassRoomDbModel>();
    public DbSet<ClassRoomMemberDbModel> ClassRoomMembers => Set<ClassRoomMemberDbModel>();
    public DbSet<TaskDbModel> Tasks => Set<TaskDbModel>();
    public DbSet<TestDbModel> Tests => Set<TestDbModel>();
    public DbSet<SolutionDbModel> Solutions => Set<SolutionDbModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClassRoomDbModel>(entity =>
        {
            entity.ToTable("class_rooms");

            entity.HasKey(classRoom => classRoom.Id);

            entity.Property(classRoom => classRoom.Title)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasMany(classRoom => classRoom.Members)
                .WithOne(member => member.ClassRoom)
                .HasForeignKey(member => member.ClassRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(classRoom => classRoom.Tasks)
                .WithOne(task => task.ClassRoom)
                .HasForeignKey(task => task.ClassRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClassRoomMemberDbModel>(entity =>
        {
            entity.ToTable("class_room_members");

            entity.HasKey(member => new { member.UserId, member.ClassRoomId });

            entity.Property(member => member.UserId)
                .IsRequired();

            entity.Property(member => member.ClassRoomId)
                .IsRequired();

            entity.Property(member => member.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(32);
        });

        modelBuilder.Entity<TaskDbModel>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(task => task.Id);

            entity.Property(task => task.Title)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(task => task.Description)
                .IsRequired();

            entity.Property(task => task.Tests)
                .IsRequired()
                .HasColumnType("jsonb");

            entity.Property(task => task.ClassRoomId)
                .IsRequired();

            entity.HasMany(task => task.Solutions)
                .WithOne(solution => solution.Task)
                .HasForeignKey(solution => solution.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(task => task.TestCases)
                .WithOne(test => test.Task)
                .HasForeignKey(test => test.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TestDbModel>(entity =>
        {
            entity.ToTable("tests");

            entity.HasKey(test => test.Id);

            entity.Property(test => test.Id)
                .ValueGeneratedOnAdd();

            entity.Property(test => test.TaskId)
                .HasColumnName("task_id")
                .IsRequired();

            entity.Property(test => test.InputData)
                .HasColumnName("input_data")
                .IsRequired()
                .HasMaxLength(4096);

            entity.Property(test => test.ExpectedOutput)
                .HasColumnName("expected_output")
                .IsRequired()
                .HasMaxLength(4096);
        });

        modelBuilder.Entity<SolutionDbModel>(entity =>
        {
            entity.ToTable("solutions");

            entity.HasKey(solution => solution.Id);

            entity.Property(solution => solution.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(32);

            entity.Property(solution => solution.CreatedAt)
                .IsRequired();

            entity.Property(solution => solution.TaskId)
                .IsRequired();

            entity.Property(solution => solution.UserId)
                .IsRequired();

            entity.Property(solution => solution.Language)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(32);

            entity.Property(solution => solution.Code)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
