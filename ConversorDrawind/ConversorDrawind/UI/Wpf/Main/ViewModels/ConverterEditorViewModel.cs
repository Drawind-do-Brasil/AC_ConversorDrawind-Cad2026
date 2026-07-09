using System.Collections.ObjectModel;
using System.Linq;

namespace ConversorDrawind.UI.Wpf.Main.ViewModels
{
    public sealed class ConverterEditorViewModel : ViewModelBase
    {
        private global::ConversorDrawind.Configuration configuration = new global::ConversorDrawind.Configuration();
        private StatusConversorItem selectedStatus;
        private string selectedConverterName = string.Empty;
        private string templateName = string.Empty;
        private string extension = ApplicationRuntime.ExtensaoGeral;

        public ConverterEditorViewModel()
        {
            ExtensionOptions.Add("DWG");
            ExtensionOptions.Add("DXF");
        }

        public global::ConversorDrawind.Configuration Configuration
        {
            get { return configuration; }
            set
            {
                if (SetProperty(ref configuration, value ?? new global::ConversorDrawind.Configuration()))
                {
                    OnPropertyChanged(nameof(Comments));
                }
            }
        }

        public string Comments
        {
            get { return Configuration.Comments; }
            set
            {
                if (Configuration.Comments == value)
                {
                    return;
                }

                Configuration.Comments = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ConverterNames { get; } = new ObservableCollection<string>();
        public ObservableCollection<StatusConversorItem> StatusItems { get; } = new ObservableCollection<StatusConversorItem>();
        public ObservableCollection<string> ExtensionOptions { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> TeklaTextLayerOptions { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> FormatBlockLayerOptions { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> ScaleLayerOptions { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> DimensionLayerOptions { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> DimensionColorOptions { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> DimensionArrowTypeOptions { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> TextStyleOptions { get; } = new ObservableCollection<string>();
        public ObservableCollection<int> LinearPrecisionOptions { get; } = new ObservableCollection<int>();
        public ObservableCollection<int> AngularPrecisionOptions { get; } = new ObservableCollection<int>();
        public ObservableCollection<int> LinearUnitOptions { get; } = new ObservableCollection<int>();
        public ObservableCollection<int> AngularUnitOptions { get; } = new ObservableCollection<int>();
        public ObservableCollection<bool> BooleanOptions { get; } = new ObservableCollection<bool>();
        public ObservableCollection<int> TextPlacementOptions { get; } = new ObservableCollection<int>();
        public ObservableCollection<string> DimensionBaseLayerOptions { get; } = new ObservableCollection<string>();

        public StatusConversorItem SelectedStatus
        {
            get { return selectedStatus; }
            set { SetProperty(ref selectedStatus, value); }
        }

        public string SelectedConverterName
        {
            get { return selectedConverterName; }
            set { SetProperty(ref selectedConverterName, value ?? string.Empty); }
        }

        public string TemplateName
        {
            get { return templateName; }
            set { SetProperty(ref templateName, value ?? string.Empty); }
        }

        public string Extension
        {
            get { return extension; }
            set
            {
                if (SetProperty(ref extension, value ?? string.Empty))
                {
                    ApplicationRuntime.ExtensaoGeral = extension;
                }
            }
        }

        public void SetStatusItems(params StatusConversorItem[] items)
        {
            StatusItems.Clear();
            foreach (StatusConversorItem item in items ?? new StatusConversorItem[0])
            {
                StatusItems.Add(item);
            }

            if (SelectedStatus == null || !StatusItems.Contains(SelectedStatus))
            {
                SelectedStatus = StatusItems.FirstOrDefault();
            }
        }

        public void SetConverterNames(System.Collections.Generic.IEnumerable<string> names)
        {
            ConverterNames.Clear();
            foreach (string name in names ?? Enumerable.Empty<string>())
            {
                ConverterNames.Add(name);
            }
        }

        public void RefreshConfiguration()
        {
            OnPropertyChanged(nameof(Configuration));
            OnPropertyChanged(nameof(Comments));
        }

        public static void ReplaceOptions(ObservableCollection<string> target, System.Collections.Generic.IEnumerable<string> values, string selectedValue = null)
        {
            target.Clear();

            foreach (string value in (values ?? Enumerable.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(System.StringComparer.OrdinalIgnoreCase))
            {
                target.Add(value);
            }

            if (!string.IsNullOrWhiteSpace(selectedValue) &&
                !target.Contains(selectedValue, System.StringComparer.OrdinalIgnoreCase))
            {
                target.Add(selectedValue);
            }
        }

        public static void ReplaceOptions<T>(ObservableCollection<T> target, System.Collections.Generic.IEnumerable<T> values, T selectedValue)
        {
            target.Clear();

            foreach (T value in (values ?? Enumerable.Empty<T>()).Distinct())
            {
                target.Add(value);
            }

            if (!target.Contains(selectedValue))
            {
                target.Add(selectedValue);
            }
        }
    }
}
