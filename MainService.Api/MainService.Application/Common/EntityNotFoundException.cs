namespace MainService.Application.Common;

public sealed class EntityNotFoundException(string message) : Exception(message);
