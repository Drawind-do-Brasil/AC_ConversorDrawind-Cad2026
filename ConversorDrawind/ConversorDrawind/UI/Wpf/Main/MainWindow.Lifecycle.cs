using System;

namespace ConversorDrawind.UI.Wpf.Main
{
    public partial class MainWindow
    {
        protected override void OnClosed(EventArgs e)
        {
            DisposeTeklaDrawingBlock();
            DisposeScaleDrawing();
            base.OnClosed(e);
        }

    }
}
