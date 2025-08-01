using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.StartScreen;

namespace AnsitraPLC_QR_MIT
{
    public partial class App : Application
    {
        public static Window m_window { get; private set; }
        public static MainViewModel MainViewModel { get; set; } = new MainViewModel();

        public App()
        {
            this.InitializeComponent();
            this.RequestedTheme = ApplicationTheme.Dark;
        }

        //Renderiza a janela
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        { 
            if(m_window == null)
            {
                m_window = new MainWindow();
            }

            m_window.Activate();
        }
    }
}
