using ConversorDrawind.UI.Wpf.Layers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ConversorDrawind
{
    public class ConfigurarLayers : IDisposable
    {
        public Arranjos arranjos = new Arranjos();

        public ConfigurarLayers(Arranjos arranjos)
        {
            this.arranjos = arranjos;
        }

        public UiDialogResult ShowDialog()
        {
            NewLayersConfigurationControl control = new NewLayersConfigurationControl(arranjos);
            Window window = CreateWindow(control);
            bool? result = window.ShowDialog();
            return result == true ? UiDialogResult.OK : UiDialogResult.Cancel;
        }

        public void CheckLines()
        {
            NewLayersConfigurationControl.CheckLines(arranjos);
        }

        public void OpenAcadLoadLayerExterno()
        {
            NewLayersConfigurationControl control = new NewLayersConfigurationControl(arranjos);
            control.OpenAcadLoadLayerExterno();
        }

        public void Dispose()
        {
        }

        private Window CreateWindow(NewLayersConfigurationControl control)
        {
            Window window = new Window
            {
                Title = Localization.TitleLayerConfiguration,
                Width = 714,
                Height = 403,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false
            };

            DockPanel container = new DockPanel();
            StackPanel buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 6, 12, 12)
            };

            Button continueButton = new Button
            {
                Content = Localization.ButtonContinue,
                IsDefault = true
            };
            Button cancelButton = new Button
            {
                Content = Localization.ButtonCancel,
                IsCancel = true,
                Margin = new Thickness(6, 0, 0, 0)
            };

            continueButton.Click += delegate
            {
                if (control.ApplyRowsToArranjos())
                {
                    window.DialogResult = true;
                }
            };
            cancelButton.Click += delegate { window.DialogResult = false; };

            buttons.Children.Add(continueButton);
            buttons.Children.Add(cancelButton);
            DockPanel.SetDock(buttons, Dock.Bottom);
            container.Children.Add(buttons);
            container.Children.Add(control);
            window.Content = container;

            return window;
        }
    }
}
