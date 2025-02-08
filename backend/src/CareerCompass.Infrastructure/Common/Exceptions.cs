namespace CareerCompass.Infrastructure.Common;

public class InfraStructureException(string message)
    : Exception(message)
{
}

public class EntityNotFoundException(string id, string entityName)
    : Exception($" Entity {entityName} with id {id} does not exists")
{
}

public class DatabaseOperationException(string message)
    : InfraStructureException(message)
{
}