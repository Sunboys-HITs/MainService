namespace MainService.Db.Domain;

public class ClassRoomMember
{
    public Guid UserId { get; init; }
    public Guid ClassRoomId { get; init; }
    public ClassRoomMemberRole Role { get; init; }
}

public enum ClassRoomMemberRole
{
    Student,
    Teacher,
    Admin,
}
