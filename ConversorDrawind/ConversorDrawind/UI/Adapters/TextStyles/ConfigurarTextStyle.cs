using ConversorDrawind.UI.Wpf.TextStyles;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind
{
    public sealed class ConfigurarTextStyle : IDisposable
    {
        public Arranjos arranjos = new Arranjos();

        public ConfigurarTextStyle(Arranjos arranjos)
        {
            this.arranjos = arranjos;
        }

        public UiDialogResult ShowDialog()
        {
            TextStyleConfigurationControl control = new TextStyleConfigurationControl(arranjos);
            Window window = CreateWindow(control);
            bool? result = window.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }

        private Window CreateWindow(TextStyleConfigurationControl control)
        {
            Window window = new Window
            {
                Title = Localization.TitleTextStylesConfiguration,
                Width = 740,
                Height = 420,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false
            };

            DockPanel container = new DockPanel();
            StackPanel buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 8, 12, 12)
            };

            Button saveButton = new Button
            {
                Content = Localization.ButtonSave,
                Margin = new Thickness(0, 0, 6, 0)
            };
            Button cancelButton = new Button
            {
                Content = Localization.ButtonCancel,
                IsCancel = true
            };

            saveButton.Click += delegate
            {
                if (control.ApplyRowsToArranjos())
                {
                    window.DialogResult = true;
                }
            };
            cancelButton.Click += delegate { window.DialogResult = false; };

            buttons.Children.Add(saveButton);
            buttons.Children.Add(cancelButton);
            DockPanel.SetDock(buttons, Dock.Bottom);
            container.Children.Add(buttons);
            container.Children.Add(control);
            window.Content = container;

            return window;
        }
    }
}
