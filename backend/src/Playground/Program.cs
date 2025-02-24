using ErrorOr;

var error = Error.Custom(121, "1", "description");

Console.WriteLine($"{error.Type}, {error.NumericType}");