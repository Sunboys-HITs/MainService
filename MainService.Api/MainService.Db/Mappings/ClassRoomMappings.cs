using MainService.Db.DbModels;
using MainService.Db.Domain;

namespace MainService.Db.Mappings;

public static class ClassRoomMappings
{
    public static ClassRoom ToDomain(this ClassRoomDbModel dbModel)
    {
        return new ClassRoom
        {
            Id = dbModel.Id,
            Title = dbModel.Title,
            Members = dbModel.Members.Select(member => member.ToDomain()).ToArray(),
            Tasks = dbModel.Tasks.Select(task => task.ToDomain()).ToArray(),
        };
    }

    public static ClassRoomDbModel ToDbModel(this ClassRoom domain)
    {
        return new ClassRoomDbModel
        {
            Id = domain.Id,
            Title = domain.Title,
            Members = domain.Members.Select(member => member.ToDbModel()).ToArray(),
            Tasks = domain.Tasks.Select(task => task.ToDbModel()).ToArray(),
        };
    }

    public static ClassRoomMember ToDomain(this ClassRoomMemberDbModel dbModel)
    {
        return new ClassRoomMember
        {
            UserId = dbModel.UserId,
            ClassRoomId = dbModel.ClassRoomId,
            Role = (ClassRoomMemberRole)dbModel.Role,
        };
    }

    public static ClassRoomMemberDbModel ToDbModel(this ClassRoomMember domain)
    {
        return new ClassRoomMemberDbModel
        {
            UserId = domain.UserId,
            ClassRoomId = domain.ClassRoomId,
            Role = (ClassRoomMemberRoleDbModel)domain.Role,
        };
    }
}
