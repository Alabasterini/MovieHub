using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MovieHub.Client.ViewModels;

public partial class StarViewModel(int value, Action<int> onSelected) : ObservableObject
{
    public int Value { get; } = value;

    [ObservableProperty] private bool _isFilled;

    [RelayCommand]
    private void Select() => onSelected(Value);
}