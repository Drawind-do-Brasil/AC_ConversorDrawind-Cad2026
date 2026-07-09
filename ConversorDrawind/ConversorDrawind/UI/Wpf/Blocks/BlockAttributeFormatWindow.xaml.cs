using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ConversorDrawind.UI.Wpf.Blocks
{
    public partial class BlockAttributeFormatWindow : Window
    {
        private readonly Block block;
        private readonly global::ConversorDrawind.Configuration configuration;
        private GetInfo drawingBlock;
        private bool isContinueOp1 = true;

        public BlockAttributeFormatWindow(Block block, global::ConversorDrawind.Configuration configuration, GetInfo drawingBlock)
        {
            InitializeComponent();
            this.block = block;
            this.configuration = configuration ?? new global::ConversorDrawind.Configuration();
            this.drawingBlock = drawingBlock;
            Rows = new ObservableCollection<BlockAttributeRow>();
            LoadRows();
            DataContext = this;
        }

        public ObservableCollection<BlockAttributeRow> Rows { get; }
        public GetInfo DrawingBlock
        {
            get { return drawingBlock; }
        }

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void LoadRows()
        {
            foreach (TagBlock item in block.listTags)
            {
                Rows.Add(new BlockAttributeRow(
                    item.tag,
                    item.modify,
                    FormatPoints(item.p1, item.p2),
                    item.filtro.layerBase + ";" + item.filtro.GetConjunto(),
                    item.widthfactor));
            }
        }

        private static string FormatPoints(Point p1, Point p2)
        {
            return p1.X.ToString().Replace(',', '.') + "," +
                   p1.Y.ToString().Replace(',', '.') + "," +
                   p1.Z.ToString().Replace(',', '.') + ";" +
                   p2.X.ToString().Replace(',', '.') + "," +
                   p2.Y.ToString().Replace(',', '.') + "," +
                   p2.Z.ToString().Replace(',', '.');
        }

        private void LoadDrawingBlock()
        {
            if (drawingBlock != null)
            {
                drawingBlock.UpdateStatus();
            }

            if (drawingBlock == null || drawingBlock.Status() == "ERROR")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    if (File.Exists(openFileDialog.FileName))
                    {
                        Thread statusThread = new Thread(new ThreadStart(ApplicationRuntime.ThreadMethodAbrindoCad));
                        statusThread.SetApartmentState(ApartmentState.STA);
                        statusThread.Start();
                        drawingBlock = new GetInfo(openFileDialog.FileName);
                        ApplicationRuntime.StopStatusThread(statusThread);
                        isContinueOp1 = true;
                    }
                }
                else
                {
                    isContinueOp1 = false;
                }
            }
            else
            {
                isContinueOp1 = true;
            }
        }

        private void TagsGridPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TagsGrid.CurrentItem is BlockAttributeRow row && CurrentColumnIndex() == 1)
            {
                row.Modify = !row.Modify;
            }
        }

        private void TagsGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TagsGrid.CurrentItem is not BlockAttributeRow row)
            {
                return;
            }

            EditCell(row, CurrentColumnIndex());
        }

        private void TagsGridMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TagsGrid.CurrentItem is not BlockAttributeRow row)
            {
                return;
            }

            int columnIndex = CurrentColumnIndex();
            if (columnIndex == 1)
            {
                AttFormatChangeCheckBox modifierDialog = new AttFormatChangeCheckBox();
                modifierDialog.ShowDialog();
                foreach (BlockAttributeRow selectedRow in SelectedRows())
                {
                    selectedRow.Modify = modifierDialog.modificar;
                }
            }
            else
            {
                EditCell(row, columnIndex);
            }
        }

        private void EditCell(BlockAttributeRow row, int columnIndex)
        {
            if (columnIndex == 2)
            {
                LoadDrawingBlock();
                if (isContinueOp1 && drawingBlock != null)
                {
                    Point p1 = new Point();
                    Point p2 = new Point();
                    drawingBlock.Get2Point(ref p1, ref p2);
                    string points = FormatPoints(p1, p2);
                    foreach (BlockAttributeRow selectedRow in SelectedRows())
                    {
                        selectedRow.Xyz = points;
                    }

                    Thread.Sleep(5);
                    SetForegroundWindow(new WindowInteropHelper(this).Handle);
                    Activate();
                }
            }
            else if (columnIndex == 3)
            {
                string[] layer = row.Filter.Split(';');
                AttFormatFilter filterDialog = new AttFormatFilter(layer[1], configuration, layer[0]);
                filterDialog.ShowDialog();
                string filter = filterDialog.filtro.layerBase + ";" + filterDialog.filtro.GetConjunto();
                foreach (BlockAttributeRow selectedRow in SelectedRows())
                {
                    selectedRow.Filter = filter;
                }

                filterDialog.Dispose();
            }
            else if (columnIndex == 4)
            {
                AttFormatWidthFactor widthDialog = new AttFormatWidthFactor(row.WidthFactor);
                widthDialog.ShowDialog();
                foreach (BlockAttributeRow selectedRow in SelectedRows())
                {
                    selectedRow.WidthFactor = widthDialog.WicthFactor;
                }

                widthDialog.Dispose();
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            block.listTags.Clear();
            foreach (BlockAttributeRow row in Rows)
            {
                TagBlock tag = new TagBlock
                {
                    tag = row.Tag,
                    modify = row.Modify
                };

                string[] pts = row.Xyz.Split(';');
                string[] pts1 = pts[0].Split(',');
                string[] pts2 = pts[1].Split(',');
                tag.p1 = new Point(Convert.ToDouble(pts1[0].Replace('.', ',')), Convert.ToDouble(pts1[1].Replace('.', ',')), Convert.ToDouble(pts1[2].Replace('.', ',')));
                tag.p2 = new Point(Convert.ToDouble(pts2[0].Replace('.', ',')), Convert.ToDouble(pts2[1].Replace('.', ',')), Convert.ToDouble(pts2[2].Replace('.', ',')));

                string[] layer = row.Filter.Split(';');
                tag.filtro.SetConjunto(layer[1]);
                tag.filtro.layerBase = layer[0];
                tag.widthfactor = row.WidthFactor;
                block.listTags.Add(tag);
            }

            DialogResult = true;
        }

        private int CurrentColumnIndex()
        {
            return TagsGrid.CurrentColumn == null ? -1 : TagsGrid.Columns.IndexOf(TagsGrid.CurrentColumn);
        }

        private IEnumerable<BlockAttributeRow> SelectedRows()
        {
            List<BlockAttributeRow> selectedRows = TagsGrid.SelectedItems
                .OfType<BlockAttributeRow>()
                .ToList();

            if (selectedRows.Count == 0 && TagsGrid.CurrentItem is BlockAttributeRow row)
            {
                selectedRows.Add(row);
            }

            return selectedRows;
        }

        public class BlockAttributeRow : INotifyPropertyChanged
        {
            private bool modify;
            private string xyz;
            private string filter;
            private string widthFactor;

            public BlockAttributeRow(string tag, bool modify, string xyz, string filter, string widthFactor)
            {
                Tag = tag;
                this.modify = modify;
                this.xyz = xyz;
                this.filter = filter;
                this.widthFactor = widthFactor;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Tag { get; }

            public bool Modify
            {
                get { return modify; }
                set
                {
                    modify = value;
                    OnPropertyChanged();
                }
            }

            public string Xyz
            {
                get { return xyz; }
                set
                {
                    xyz = value;
                    OnPropertyChanged();
                }
            }

            public string Filter
            {
                get { return filter; }
                set
                {
                    filter = value;
                    OnPropertyChanged();
                }
            }

            public string WidthFactor
            {
                get { return widthFactor; }
                set
                {
                    widthFactor = value;
                    OnPropertyChanged();
                }
            }

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}




