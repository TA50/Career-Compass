using CareerCompass.Core.Users;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareerCompass.Infrastructure.Persistence.Converters;

public class UserIdConverter(ConverterMappingHints mappingHints = null) : ValueConverter<UserId, Guid>(
    id => id.Value,
    value => UserId.Create(value),
    mappingHints);