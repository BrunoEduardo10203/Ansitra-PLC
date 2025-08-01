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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace AnsitraPLC_QR_MIT
{
    public sealed partial class LacosPage : Page
    {
        // Comprimento dos Laços
        public string CompLaco1
        {
            get => compLaco1?.Text;
            set => compLaco1.Text = value;
        }
        public string CompLaco2
        {
            get => compLaco2?.Text;
            set => compLaco2.Text = value;
        }
        public string CompLaco3
        {
            get => compLaco3?.Text;
            set => compLaco3.Text = value;
        }
        public string CompLaco4
        {
            get => compLaco4?.Text;
            set => compLaco4.Text = value;
        }
        public string CompLaco5
        {
            get => compLaco5?.Text;
            set => compLaco5.Text = value;
        }
        public string CompLaco6
        {
            get => compLaco6?.Text;
            set => compLaco6.Text = value;
        }
        public string CompLaco7
        {
            get => compLaco7?.Text;
            set => compLaco7.Text = value;
        }
        public string CompLaco8
        {
            get => compLaco8?.Text;
            set => compLaco8.Text = value;
        }
        public string CompLaco9
        {
            get => compLaco9?.Text;
            set => compLaco9.Text = value;
        }
        public string CompLaco10
        {
            get => compLaco10?.Text;
            set => compLaco10.Text = value;
        }
        public string CompLaco11
        {
            get => compLaco11?.Text;
            set => compLaco11.Text = value;
        }
        public string CompLaco12
        {
            get => compLaco12?.Text;
            set => compLaco12.Text = value;
        }
        public string CompLaco13
        {
            get => compLaco13?.Text;
            set => compLaco13.Text = value;
        }
        public string CompLaco14
        {
            get => compLaco14?.Text;
            set => compLaco14.Text = value;
        }
        public string CompLaco15
        {
            get => compLaco15?.Text;
            set => compLaco15.Text = value;
        }
        public string CompLaco16
        {
            get => compLaco16?.Text;
            set => compLaco16.Text = value;
        }

        // Distância dos Laços
        public string DistLaco1
        {
            get => distLaco1?.Text;
            set => distLaco1.Text = value;
        }
        public string DistLaco2
        {
            get => distLaco2?.Text;
            set => distLaco2.Text = value;
        }
        public string DistLaco3
        {
            get => distLaco3?.Text;
            set => distLaco3.Text = value;
        }
        public string DistLaco4
        {
            get => distLaco4?.Text;
            set => distLaco4.Text = value;
        }
        public string DistLaco5
        {
            get => distLaco5?.Text;
            set => distLaco5.Text = value;
        }
        public string DistLaco6
        {
            get => distLaco6?.Text;
            set => distLaco6.Text = value;
        }
        public string DistLaco7
        {
            get => distLaco7?.Text;
            set => distLaco7.Text = value;
        }
        public string DistLaco8
        {
            get => distLaco8?.Text;
            set => distLaco8.Text = value;
        }
        public string DistLaco9
        {
            get => distLaco9?.Text;
            set => distLaco9.Text = value;
        }
        public string DistLaco10
        {
            get => distLaco10?.Text;
            set => distLaco10.Text = value;
        }
        public string DistLaco11
        {
            get => distLaco11?.Text;
            set => distLaco11.Text = value;
        }
        public string DistLaco12
        {
            get => distLaco12?.Text;
            set => distLaco12.Text = value;
        }
        public string DistLaco13
        {
            get => distLaco13?.Text;
            set => distLaco13.Text = value;
        }
        public string DistLaco14
        {
            get => distLaco14?.Text;
            set => distLaco14.Text = value;
        }
        public string DistLaco15
        {
            get => distLaco15?.Text;
            set => distLaco15.Text = value;
        }
        public string DistLaco16
        {
            get => distLaco16?.Text;
            set => distLaco16.Text = value;
        }


        public LacosPage()
        {
            InitializeComponent();
            DataContext = App.MainViewModel;
        }
    }
}
