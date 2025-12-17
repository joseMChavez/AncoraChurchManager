using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AncoraChurchManager.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// ViewModek base con soporte para PropertyChanged (binding)
/// </summary>
/// <summary>
/// Base ViewModel with support for PropertyChanged (binding)
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool _isLoading;
    private string _title;

    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    protected void SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
    }

    public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
 