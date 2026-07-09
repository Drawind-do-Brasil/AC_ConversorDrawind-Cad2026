using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class ConverterEditorExplosionTab : UserControl
    {
        private const string ExplodeLayerDragFormat = "ConversorDrawind.ExplodeLayerNames";
        private const string ExplodeLayerDragSourceFormat = "ConversorDrawind.ExplodeLayerSource";

        public ConverterEditorExplosionTab()
        {
            InitializeComponent();
        }

        public ListBox AllLayersListBox => AllExplodeLayersListBox;
        public ListBox SelectedLayersListBox => SelectedExplodeLayersListBox;

        public void ShowExplodeLayers(IEnumerable<string> baseLayers, IEnumerable<string> explodeLayers)
        {
            AllExplodeLayersListBox.ItemsSource = (baseLayers ?? Enumerable.Empty<string>())
                .Distinct(System.StringComparer.OrdinalIgnoreCase)
                .ToList();
            SelectedExplodeLayersListBox.ItemsSource = (explodeLayers ?? Enumerable.Empty<string>()).ToList();
        }

        public void AddExplodeLayers(global::ConversorDrawind.Configuration configuration)
        {
            foreach (string layer in AllExplodeLayersListBox.SelectedItems.OfType<string>())
            {
                if (!configuration.Layers.ExplodeLayers.Contains(layer))
                {
                    configuration.Layers.ExplodeLayers.Add(layer);
                }
            }

            Refresh(configuration);
        }

        public void RemoveSelectedExplodeLayers(global::ConversorDrawind.Configuration configuration)
        {
            foreach (string layer in SelectedExplodeLayersListBox.SelectedItems.OfType<string>().ToList())
            {
                configuration.Layers.ExplodeLayers.Remove(layer);
            }

            Refresh(configuration);
        }

        public void MoveExplodeLayers(global::ConversorDrawind.Configuration configuration, object sender, DragEventArgs e)
        {
            if (!(sender is ListBox targetListBox) || !e.Data.GetDataPresent(ExplodeLayerDragFormat))
            {
                return;
            }

            string sourceName = e.Data.GetDataPresent(ExplodeLayerDragSourceFormat)
                ? e.Data.GetData(ExplodeLayerDragSourceFormat) as string
                : string.Empty;
            if (sourceName == targetListBox.Name)
            {
                return;
            }

            List<string> layers = ((string[])e.Data.GetData(ExplodeLayerDragFormat)).ToList();
            if (targetListBox == SelectedExplodeLayersListBox)
            {
                foreach (string layer in layers)
                {
                    if (!configuration.Layers.ExplodeLayers.Contains(layer))
                    {
                        configuration.Layers.ExplodeLayers.Add(layer);
                    }
                }
            }
            else if (targetListBox == AllExplodeLayersListBox)
            {
                foreach (string layer in layers)
                {
                    configuration.Layers.ExplodeLayers.Remove(layer);
                }
            }

            Refresh(configuration);
            e.Handled = true;
        }

        private void Refresh(global::ConversorDrawind.Configuration configuration)
        {
            configuration.Layers.ExplodeLayers = configuration.Layers.ExplodeLayers
                .Where(layer => !string.IsNullOrWhiteSpace(layer))
                .Distinct(System.StringComparer.OrdinalIgnoreCase)
                .ToList();
            ShowExplodeLayers(configuration.Layers.BaseLayers, configuration.Layers.ExplodeLayers);
        }

        private MainWindow OwnerWindow => Window.GetWindow(this) as MainWindow;

        private void Forward(string action, object sender, RoutedEventArgs e) => OwnerWindow?.InvokeUiAction(action, sender, e);

        private void AddExplodeLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(AddExplodeLayerClick), sender, e);
        private void RemoveExplodeLayerClick(object sender, RoutedEventArgs e) => Forward(nameof(RemoveExplodeLayerClick), sender, e);

        private void ExplodeLayersMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !(sender is ListBox listBox) || listBox.SelectedItems.Count == 0)
            {
                return;
            }

            string[] selectedLayers = listBox.SelectedItems.OfType<string>().ToArray();
            if (selectedLayers.Length == 0)
            {
                return;
            }

            DataObject data = new DataObject();
            data.SetData(ExplodeLayerDragFormat, selectedLayers);
            data.SetData(ExplodeLayerDragSourceFormat, listBox.Name);
            DragDrop.DoDragDrop(listBox, data, DragDropEffects.Move);
        }

        private void ExplodeLayersDragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(ExplodeLayerDragFormat) ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        }

        private void ExplodeLayersDrop(object sender, DragEventArgs e) => Forward(nameof(ExplodeLayersDrop), sender, e);
    }
}
