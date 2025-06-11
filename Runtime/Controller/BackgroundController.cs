using System;
using Core.Entity;
using Frameworks.Utils;
using UnityEngine;

public class BackgroundController : Singleton<BackgroundController>
{
    public event Action<BackgroundEnum> OnBackgroundChanged;
    private BackgroundEnum _currentBackground;

    public BackgroundEnum CurrentBackgroundEnum => _currentBackground;

    public void OnChangeBackground(BackgroundEnum @enum)
    {
        _currentBackground = @enum;
        OnBackgroundChanged?.Invoke(@enum);
    }
}
