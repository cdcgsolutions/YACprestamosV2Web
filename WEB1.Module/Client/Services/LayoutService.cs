using System;

namespace WEB1.Module.Client.Services
{
    public class LayoutService
    {
        public string CurrentTitle { get; private set; } = "Inicio";
        public event Action? OnTitleChanged;

        public void SetTitle(string title)
        {
            if (CurrentTitle == title) return;
            CurrentTitle = title;
            OnTitleChanged?.Invoke();
        }
    }
}
