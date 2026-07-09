using ConversorDrawind.UI.Wpf.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow
    {
        private void ConfigureAdvancedDimensionArrow()
        {
            using (ConfAvancadaDeCota f = new ConfAvancadaDeCota(configuration.Dimensions.FixArrow, configuration.Dimensions.FixArrowType, configuration.Dimensions.FixArrowFactor)) if (f.ShowDialog() == UiDialogResult.OK) { configuration.Dimensions.FixArrow = f.EXTDIMCorrigeSeta; configuration.Dimensions.FixArrowType = f.EXTDIMCorrigeSetaTipoSeta; configuration.Dimensions.FixArrowFactor = f.EXTDIMCorrigeSetaFactor; }
        }

        private void AddOtherDimensionColor(ComboBox targetComboBox)
        {
            using (GenericNewColor colorDialog = new GenericNewColor(targetComboBox.Text))
            {
                if (colorDialog.ShowDialog() != UiDialogResult.OK)
                {
                    return;
                }

                if (!configuration.Catalogs.Colors.Contains(colorDialog.colorClass))
                {
                    configuration.Catalogs.Colors.Add(colorDialog.colorClass);
                }

                RefreshDimensionOptions();
                targetComboBox.Text = colorDialog.colorClass;
            }
        }

        private void RefreshDimensionOptions()
        {
            ConverterEditorViewModel.ReplaceOptions(viewModel.DimensionLayerOptions, NewLayerNames(), configuration.Dimensions.Layer);
            ConverterEditorViewModel.ReplaceOptions(viewModel.DimensionColorOptions, configuration.Catalogs.Colors.Skip(1), configuration.Dimensions.LineColor);
            ConverterEditorViewModel.ReplaceOptions(viewModel.DimensionArrowTypeOptions, DimensionArrowTypes(), configuration.Dimensions.ArrowType);
            ConverterEditorViewModel.ReplaceOptions(viewModel.TextStyleOptions, TextStyleNames(), configuration.Text.DefaultStyleName);
            ConverterEditorViewModel.ReplaceOptions(viewModel.LinearPrecisionOptions, Enumerable.Range(0, 9), configuration.Dimensions.Precision);
            ConverterEditorViewModel.ReplaceOptions(viewModel.AngularPrecisionOptions, Enumerable.Range(0, 9), configuration.Dimensions.AngularPrecision);
            ConverterEditorViewModel.ReplaceOptions(viewModel.LinearUnitOptions, Enumerable.Range(1, 6), configuration.Dimensions.Unit);
            ConverterEditorViewModel.ReplaceOptions(viewModel.AngularUnitOptions, Enumerable.Range(1, 6), configuration.Dimensions.AngularUnit);
            ConverterEditorViewModel.ReplaceOptions(viewModel.BooleanOptions, BooleanOptions(), false);
            ConverterEditorViewModel.ReplaceOptions(viewModel.TextPlacementOptions, Enumerable.Range(0, 5), configuration.Dimensions.TextVerticalPosition);
            ConverterEditorViewModel.ReplaceOptions(viewModel.DimensionBaseLayerOptions, DimensionBaseLayers(), configuration.Dimensions.BaseLayer);
        }

        private IEnumerable<string> TextStyleNames()
        {
            configuration.EnsureDefaults();
            return configuration.Text.Styles.Select(style => style.Name);
        }

        private IEnumerable<string> DimensionBaseLayers()
        {
            yield return "DIMENSION";
            foreach (string layer in configuration.Layers.BaseLayers.Where(layer => !string.Equals(layer, "DIMENSION", StringComparison.OrdinalIgnoreCase)))
            {
                yield return layer;
            }
        }

        private IEnumerable<bool> BooleanOptions()
        {
            return new[] { true, false };
        }

        private IEnumerable<string> DimensionArrowTypes()
        {
            return new[]
            {
                "Architectural Tick",
                "Box",
                "Box Filled",
                "Closed",
                "Closed Blank",
                "Closed Filled",
                "Datum Triangle",
                "Datum Triangle Filled",
                "Dot",
                "Dot Blank",
                "Dot Small",
                "Integral",
                "None",
                "Oblique",
                "Open",
                "Origin Indicator",
                "Origin Indicator 2",
                "Right Angle"
            };
        }

        private void SelectScalePointsFromDrawing()
        {
            GetInfo drawing = OpenScaleDrawing();
            if (drawing == null)
            {
                return;
            }

            Point p1 = new Point();
            Point p2 = new Point();
            drawing.Get2Point(ref p1, ref p2);

            if (drawing.Status() == "ERROR")
            {
                return;
            }

            SetScalePointFields(p1, p2);
            Thread.Sleep(5);
            Activate();
        }

        private GetInfo OpenScaleDrawing()
        {
            if (scaleDrawing != null)
            {
                scaleDrawing.UpdateStatus();
            }

            if (scaleDrawing != null && scaleDrawing.Status() != "ERROR")
            {
                return scaleDrawing;
            }

            DisposeScaleDrawing();

            string fileName = BrowseDrawingFile(scaleDrawingPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            GetInfo drawing = OpenDrawingBlock(fileName);
            if (drawing == null)
            {
                scaleDrawingPath = string.Empty;
                return null;
            }

            scaleDrawing = drawing;
            scaleDrawingPath = fileName;
            return scaleDrawing;
        }

        private void SetScalePointFields(Point p1, Point p2)
        {
            configuration.Scale.Point1.X = p1.X;
            configuration.Scale.Point1.Y = p1.Y;
            configuration.Scale.Point1.Z = p1.Z;
            configuration.Scale.Point2.X = p2.X;
            configuration.Scale.Point2.Y = p2.Y;
            configuration.Scale.Point2.Z = p2.Z;
            viewModel.RefreshConfiguration();

            configuration.Scale.Point1.X = p1.X;
            configuration.Scale.Point1.Y = p1.Y;
            configuration.Scale.Point1.Z = p1.Z;
            configuration.Scale.Point2.X = p2.X;
            configuration.Scale.Point2.Y = p2.Y;
            configuration.Scale.Point2.Z = p2.Z;
        }

        private void DisposeScaleDrawing()
        {
            if (scaleDrawing == null)
            {
                return;
            }

            scaleDrawing.Dispose();
            scaleDrawing = null;
            scaleDrawingPath = string.Empty;
        }

    }
}
