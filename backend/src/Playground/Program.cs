using System.ComponentModel;
using System.Runtime.CompilerServices;

var src = new Source();
src.PropertyChanged += (sender, args) => Console.WriteLine(args.PropertyName);

src.Prop = "Hello";

Console.ReadLine();

class Source : INotifyPropertyChanged

{
    private string _prop;

    public string Prop
    {
        get { return _prop; }
        set
        {
            if (SetField(ref _prop, value))
            {
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}