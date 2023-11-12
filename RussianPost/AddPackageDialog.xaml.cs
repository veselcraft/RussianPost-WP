using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Диалоговое окно содержимого" см. по адресу http://go.microsoft.com/fwlink/?LinkID=390556

namespace RussianPost
{
    public sealed partial class AddPackageDialog : ContentDialog
    {
        public string TrackID = null;
        internal ContentDialogResult Result;

        public AddPackageDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            TrackID = trackid.Text;
            Result = ContentDialogResult.Primary;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = ContentDialogResult.Secondary;
        }
    }
}
