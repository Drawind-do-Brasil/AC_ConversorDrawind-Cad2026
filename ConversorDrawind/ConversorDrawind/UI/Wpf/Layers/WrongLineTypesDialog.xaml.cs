using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ConversorDrawind.UI.Wpf.Layers
{
    public partial class WrongLineTypesDialog : Window
    {
        private readonly List<CorrecaoLinhas> wrongLines;
        private readonly Arranjos arranjos;
        private readonly ObservableCollection<WrongLineRow> rows = new ObservableCollection<WrongLineRow>();

        public WrongLineTypesDialog(List<CorrecaoLinhas> linhasErradas, Arranjos arranjos)
        {
            InitializeComponent();
            wrongLines = linhasErradas;
            this.arranjos = arranjos;
            LoadRows();
            WrongLinesDataGrid.ItemsSource = rows;
        }

        public bool CheckLines()
        {
            for (int i = 0; i < wrongLines.Count; i++)
            {
                bool linhaOK = false;
                foreach (string line in arranjos.allLineType2)
                {
                    string[] lineTemp = line.Split(',');
                    if (wrongLines[i].newLinha.ToUpper() == lineTemp.First().ToUpper())
                    {
                        linhaOK = true;
                    }
                }

                if (!linhaOK)
                {
                    return false;
                }
            }

            return true;
        }

        private void LoadRows()
        {
            rows.Clear();
            foreach (CorrecaoLinhas line in wrongLines)
            {
                rows.Add(new WrongLineRow
                {
                    NomeLayer = line.nomeLayer,
                    LinhaLayer = line.oldLinha
                });
            }
        }

        private void WrongLinesDataGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WrongLinesDataGrid.SelectedItem is not WrongLineRow selectedRow)
            {
                return;
            }

            int rowIndex = rows.IndexOf(selectedRow);
            LayerLineTypeDialog dialog = new LayerLineTypeDialog(selectedRow.LinhaLayer, arranjos)
            {
                Owner = this
            };

            dialog.ShowDialog();
            string linha = selectedRow.LinhaLayer;
            try
            {
                linha = dialog.LineType.Split(',').First().ToUpper();
            }
            catch
            {
            }

            selectedRow.LinhaLayer = linha;
            WrongLinesDataGrid.Items.Refresh();
            wrongLines[rowIndex].newLinha = linha;
            ContinueButton.IsEnabled = CheckLines();
        }

        private void ContinueButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (CorrecaoLinhas item in wrongLines)
            {
                arranjos.allNewLayerComposition[item.posLinha] = item.GetNewLinha();
            }

            DialogResult = true;
        }

        private sealed class WrongLineRow
        {
            public string NomeLayer { get; set; }
            public string LinhaLayer { get; set; }
        }
    }
}



