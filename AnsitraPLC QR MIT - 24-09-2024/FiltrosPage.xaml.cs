using AnsitraPLC_QR_MIT.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AnsitraPLC_QR_MIT
{
    public sealed partial class FiltrosPage : Page
    {
        public string CompMaiorValor
        {
            get => compMaiorValor?.Text;
            set { if (compMaiorValor != null) compMaiorValor.Text = value; }
        }

        public string CompMaiorGravar
        {
            get => compMaiorGravar?.Text;
            set { if (compMaiorGravar != null) compMaiorGravar.Text = value; }
        }

        public string CompMenorValor
        {
            get => compMenorValor?.Text;
            set { if (compMenorValor != null) compMenorValor.Text = value; }
        }

        public string CompMenorGravar
        {
            get => compMenorGravar?.Text;
            set { if (compMenorGravar != null) compMenorGravar.Text = value; }
        }

        public string VeloMaiorValor
        {
            get => veloMaiorValor?.Text;
            set { if (veloMaiorValor != null) veloMaiorValor.Text = value; }
        }

        public string VeloMaiorGravar
        {
            get => veloMaiorGravar?.Text;
            set { if (veloMaiorGravar != null) veloMaiorGravar.Text = value; }
        }

        public string VeloMenorValor
        {
            get => veloMenorValor?.Text;
            set { if (veloMenorValor != null) veloMenorValor.Text = value; }
        }

        public string VeloMenorGravar
        {
            get => veloMenorGravar?.Text;
            set { if (veloMenorGravar != null) veloMenorGravar.Text = value; }
        }

        public FiltrosPage()
        {
            this.InitializeComponent();
        }

    }
}
