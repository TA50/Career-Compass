using System.Reflection;

var id = Guid.NewGuid();
var ctor = typeof(Test)
    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, [typeof(Guid), typeof(string)]);

var field = ctor.Invoke([ id, "Test" ]);



Console.WriteLine(field);

class Test
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private Test(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Test Create(string name)
    {
        return new Test(Guid.NewGuid(), name);
    }
}