namespace MainService.Db.DbModels;

public class ClassRoomMemberDbModel
{
    public Guid UserId { get; set; }
    public Guid ClassRoomId { get; set; }
    public ClassRoomMemberRoleDbModel Role { get; set; }

    public ClassRoomDbModel ClassRoom { get; set; } = null!;
}

public enum ClassRoomMemberRoleDbModel
{
    Student,
    Teacher,
    Admin,
}
