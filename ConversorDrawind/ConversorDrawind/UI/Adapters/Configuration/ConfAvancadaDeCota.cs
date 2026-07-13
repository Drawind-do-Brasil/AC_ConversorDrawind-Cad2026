using ConversorDrawind.UI.Wpf.Configuration;
using System;
namespace ConversorDrawind
{
    public sealed class ConfAvancadaDeCota : IDisposable
    {
        public bool EXTDIMCorrigeSeta = false;
        public string EXTDIMCorrigeSetaTipoSeta = "Oblique";
        public double EXTDIMCorrigeSetaFactor = 7.23;

        public ConfAvancadaDeCota(bool concerta, string tipoSeta, double distancia)
        {
            EXTDIMCorrigeSeta = concerta;
            EXTDIMCorrigeSetaTipoSeta = tipoSeta;
            EXTDIMCorrigeSetaFactor = distancia;
        }

        public UiDialogResult ShowDialog()
        {
            DimensionAdvancedDialog dialog = new DimensionAdvancedDialog(EXTDIMCorrigeSeta, EXTDIMCorrigeSetaTipoSeta, EXTDIMCorrigeSetaFactor);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                EXTDIMCorrigeSeta = dialog.EXTDIMCorrigeSeta;
                EXTDIMCorrigeSetaTipoSeta = dialog.EXTDIMCorrigeSetaTipoSeta;
                EXTDIMCorrigeSetaFactor = dialog.EXTDIMCorrigeSetaFactor;
                return UiDialogResult.OK;
            }

            return UiDialogResult.Cancel;
        }

        public void Dispose()
        {
        }
    }
}



