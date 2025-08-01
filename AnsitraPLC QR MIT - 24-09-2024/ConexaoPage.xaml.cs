using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// ...
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AnsitraPLC_QR_MIT
{
    public sealed partial class ConexaoPage : Page
    {
        public string nomeCaixa1
        {
            get => txb_nomeCaixa1.Text;
            set => txb_nomeCaixa1.Text = value;
        }

        public string nomeCaixa2
        {
            get => txb_nomeCaixa2.Text;
            set => txb_nomeCaixa2.Text = value;
        }

        public string altera_estacao
        {
            get => txbx_AltearIP.Text;
            set => txbx_AltearIP.Text = value;
        }
        public string ip_estacao
        {
            get => ip_PLC.Text;
            set => ip_PLC.Text = value;
        }
        public string mask_estacao
        {
            get => mask_PLC.Text;
            set => mask_PLC.Text = value;
        }
        public string gateway_estacao
        {
            get => gateway_PLC.Text;
            set => gateway_PLC.Text = value;
        }
        public string ip_modulo
        {
            get => txb_ipModulo.Text;
            set => txb_ipModulo.Text = value;
        }
        public string mask_modulo
        {
            get => txb_maskModulo.Text;
            set => txb_maskModulo.Text = value;
        }
        public string gateway_modulo
        {
            get => txb_gatewayModulo.Text;
            set => txb_gatewayModulo.Text = value;
        }

        public string Ip_plc2
        {
            get => ip_plc2.Text;
            set => ip_plc2.Text = value;
        }
        public string Mask_plc2
        {
            get => mask_plc2.Text;
            set => mask_plc2.Text = value;
        }
        public string Gateway_plc2
        {
            get => gateway_plc2.Text;
            set => gateway_plc2.Text = value;
        }

        public string Ip_sensor2
        {
            get => ip_sensor2.Text;
            set => ip_sensor2.Text = value;
        }
        public string Mask_sensor2
        {
            get => mask_sensor2.Text;
            set => mask_sensor2.Text = value;
        }
        public string Gateway_sensor2
        {
            get => gateway_sensor2.Text;
            set => gateway_sensor2.Text = value;
        }

        public Grid Box3
        {
            get => box3;
            set => box3 = value;
        }

        public Grid Box4
        {
            get => box4;
            set => box4 = value;
        }

        public ConexaoPage()
        {
            this.InitializeComponent();

            var mainViewModel = App.MainViewModel;
            DataContext = mainViewModel;
        }

        private async void btn_GravarPLC_Click(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();
            MainWindow mainWindow = (MainWindow)App.m_window;

            if (string.IsNullOrWhiteSpace(ip_PLC.Text) || string.IsNullOrWhiteSpace(mask_PLC.Text) || string.IsNullOrWhiteSpace(gateway_PLC.Text))
            {
                await ShowMessageDialog("Por favor, preencha todos os campos antes de enviar.");
                return;
            }

            if (!IsValidIP(ip_PLC.Text) || !IsValidIP(mask_PLC.Text) || !IsValidIP(gateway_PLC.Text))
            {
                await ShowDialog("Por favor, insira um endereço IP válido.", this);
                return;
            }

            if (mainWindow.checkEstacaoCarregada())
            {
                bool confirmacao = await ShowDialog("Deseja enviar as configurações de rede para o PLC?", this);

                if (confirmacao)
                {
                    PingReply resp = ping.Send(mainWindow.ip_conexao);

                    if (resp.Status == IPStatus.Success)
                    {
                        // Envia a configuração de rede
                        if (mainWindow.BotaoEquipamentos.Content.ToString() == "Ansitra QR")
                        {
                            var dado = $"NPLC{{\"ip\": \"{ip_PLC.Text}\", \"mask\": \"{mask_PLC.Text}\", \"gateway\": \"{gateway_PLC.Text}\"}}";
                            var tamanho = dado.Length;
                            mainWindow.dadosPLC = $"Caracteres:{tamanho} // {dado}";
                        }
                        else
                        {
                            var dado = $"PLC1{{\"ip\": \"{ip_PLC.Text}\", \"mask\": \"{mask_PLC.Text}\", \"gateway\": \"{gateway_PLC.Text}\"}}";
                            var tamanho = dado.Length;
                            mainWindow.dadosPLC = $"Caracteres:{tamanho} // {dado}";
                        }
                        mainWindow.flagSend = 1;
                        mainWindow.statusSend = 1;
                        mainWindow.nBytes = 4;

                        // Conecta ao PLC
                        mainWindow.Conect();
                    }
                }
            }
        }



        public async Task<bool> ShowDialog(string message, FrameworkElement context)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Ansitra PLC",
                Content = message,
                PrimaryButtonText = "Sim",
                CloseButtonText = "Não",
                XamlRoot = context.XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        private async Task ShowMessageDialog(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Ansitra PLC",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private async void btn_GravarModulo_Click(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();
            MainWindow mainWindow = (MainWindow)App.m_window;

            if (string.IsNullOrWhiteSpace(txb_ipModulo.Text) || string.IsNullOrWhiteSpace(txb_maskModulo.Text) || string.IsNullOrWhiteSpace(txb_gatewayModulo.Text))
            {
                await ShowMessageDialog("Por favor, preencha todos os campos antes de enviar.");
                return;
            }

            if (!IsValidIP(txb_ipModulo.Text) || !IsValidIP(txb_maskModulo.Text) || !IsValidIP(txb_gatewayModulo.Text))
            {
                await ShowDialog("Por favor, insira um endereço IP válido.", this);
                return;
            }

            if (mainWindow.checkEstacaoCarregada())
            {
                bool confirmacao = await ShowDialog("Deseja enviar as configurações do módulo para o PLC?", this);

                if (confirmacao)
                {
                    PingReply resp = ping.Send(mainWindow.ip_conexao);

                    if (resp.Status == IPStatus.Success)
                    {

                        // Envia a configuração de rede
                        if (mainWindow.BotaoEquipamentos.Content.ToString() == "Ansitra QR")
                        {
                            var dado = $"NPLC{{\"ip\": \"{ip_PLC.Text}\", \"mask\": \"{mask_PLC.Text}\", \"gateway\": \"{gateway_PLC.Text}\"}}";
                            var tamanho = dado.Length;

                            mainWindow.dadosPLC = $"Caracteres:{tamanho} // {dado}";
                        }
                        else
                        {
                            var dado = $"SSW1{{\"ip\": \"{ip_PLC.Text}\", \"mask\": \"{mask_PLC.Text}\", \"gateway\": \"{gateway_PLC.Text}\"}}";
                            var tamanho = dado.Length;

                            mainWindow.dadosPLC = $"Caracteres:{tamanho} // {dado}";
                        }
                        mainWindow.flagSend = 1;
                        mainWindow.statusSend = 1;
                        mainWindow.nBytes = 4;

                        // Conecta ao PLC
                        mainWindow.Conect();
                    }
                }
            }
        }

        private async void btn_AlteraIP_Click(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();
            MainWindow mainWindow = (MainWindow)App.m_window;

            if (string.IsNullOrWhiteSpace(txbx_AltearIP.Text))
            {
                await ShowMessageDialog("Por favor, preencha o campo de IP antes de enviar.");
                return;
            }

            if (!IsValidIP(txbx_AltearIP.Text))
            {
                await ShowDialog("Por favor, insira um endereço IP válido.", this);
                return;
            }

            if (mainWindow.checkEstacaoCarregada())
            {
                bool confirmacao = await ShowDialog("Deseja ALTERAR O ENDEREÇO DE IP DO SERVIDOR?", this);

                if (confirmacao)
                {
                    PingReply resp = ping.Send(mainWindow.ip_conexao);

                    if (resp.Status == IPStatus.Success)
                    {
                        var dado = $"IPSV{txbx_AltearIP.Text}";
                        var tamanho = dado.Length;

                        // Envia a configuração de rede
                        mainWindow.dadosPLC = $"Caracteres:{tamanho} // {dado}";
                        mainWindow.flagSend = 1;
                        mainWindow.statusSend = 1;
                        mainWindow.nBytes = 4;

                        // Conecta ao PLC
                        mainWindow.Conect();
                    }
                }
            }
        }
        private bool IsValidIP(string ip)
        {
            // Expressão regular para validar o formato de um IP (IPv4)
            var regex = new System.Text.RegularExpressions.Regex(@"^(\d{1,3}\.){3}\d{1,3}$");
            return regex.IsMatch(ip) && ip.Split('.').All(part => int.TryParse(part, out int num) && num >= 0 && num <= 255);
        }

        private async void btn_Sensor1_Click(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();
            MainWindow mainWindow = (MainWindow)App.m_window;

            if (string.IsNullOrWhiteSpace(txb_ipModulo.Text) || string.IsNullOrWhiteSpace(txb_maskModulo.Text) || string.IsNullOrWhiteSpace(txb_gatewayModulo.Text))
            {
                await ShowMessageDialog("Por favor, preencha todos os campos antes de enviar.");
                return;
            }

            if (!IsValidIP(txb_ipModulo.Text) || !IsValidIP(txb_maskModulo.Text) || !IsValidIP(txb_gatewayModulo.Text))
            {
                await ShowDialog("Por favor, insira um endereço IP válido.", this);
                return;
            }

            if (mainWindow.checkEstacaoCarregada())
            {
                bool confirmacao = await ShowDialog("Deseja enviar as configurações do módulo para o PLC?", this);

                if (confirmacao)
                {
                    PingReply resp = ping.Send(mainWindow.ip_conexao);

                    if (resp.Status == IPStatus.Success)
                    {
                        var dado = $"PLC2{{\"ip\": \"{ip_PLC.Text}\", \"mask\": \"{mask_PLC.Text}\", \"gateway\": \"{gateway_PLC.Text}\"}}";
                        var tamanho = dado.Length;

                        // Envia a configuração de rede
                        mainWindow.dadosPLC = $"Caracteres:{tamanho} // {dado}";
                        mainWindow.flagSend = 1;
                        mainWindow.statusSend = 1;
                        mainWindow.nBytes = 4;

                        // Conecta ao PLC
                        mainWindow.Conect();
                    }
                }
            }
        }

        private async void btn_Sensor2_Click(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();
            MainWindow mainWindow = (MainWindow)App.m_window;

            if (string.IsNullOrWhiteSpace(txb_ipModulo.Text) || string.IsNullOrWhiteSpace(txb_maskModulo.Text) || string.IsNullOrWhiteSpace(txb_gatewayModulo.Text))
            {
                await ShowMessageDialog("Por favor, preencha todos os campos antes de enviar.");
                return;
            }

            if (!IsValidIP(txb_ipModulo.Text) || !IsValidIP(txb_maskModulo.Text) || !IsValidIP(txb_gatewayModulo.Text))
            {
                await ShowDialog("Por favor, insira um endereço IP válido.", this);
                return;
            }

            if (mainWindow.checkEstacaoCarregada())
            {
                bool confirmacao = await ShowDialog("Deseja enviar as configurações do módulo para o PLC?", this);

                if (confirmacao)
                {
                    PingReply resp = ping.Send(mainWindow.ip_conexao);

                    if (resp.Status == IPStatus.Success)
                    {
                        var dado = $"SSW2{{\"ip\": \"{ip_PLC.Text}\", \"mask\": \"{mask_PLC.Text}\", \"gateway\": \"{gateway_PLC.Text}\"}}";
                        var tamanho = dado.Length;

                        // Envia a configuração de rede
                        mainWindow.dadosPLC = $"Caracteres:{tamanho} // {dado}";
                        mainWindow.flagSend = 1;
                        mainWindow.statusSend = 1;
                        mainWindow.nBytes = 4;

                        // Conecta ao PLC
                        mainWindow.Conect();
                    }
                }
            }
        }
    }
}
