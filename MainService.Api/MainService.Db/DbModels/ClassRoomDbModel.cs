namespace MainService.Db.DbModels;

public class ClassRoomDbModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public ICollection<ClassRoomMemberDbModel> Members { get; set; } = [];
    public ICollection<TaskDbModel> Tasks { get; set; } = [];
}
