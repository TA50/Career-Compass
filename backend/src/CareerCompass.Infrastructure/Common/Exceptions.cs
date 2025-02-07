namespace CareerCompass.Infrastructure.Common;

public class EntityNotFoundException(string id, string entityName)
    : Exception($" Entity {entityName} with id {id} does not exists")
{
}

public class DatabaseOperationException(string message)
    : Exception(message)
{
}