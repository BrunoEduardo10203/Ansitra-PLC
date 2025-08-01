using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;


namespace AnsitraPLC_QR_MIT
{

    public sealed partial class AlarmesPage : Page
    {
        private Action<string> handlerDadoRecebido;


        public AlarmesPage()
        {
            this.InitializeComponent();
        }

        private async void btn_moniAlarme_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)App.m_window;
            bool estacaoCarregada = mainWindow.checkEstacaoCarregada();
            var uiDispatcher = DispatcherQueue.GetForCurrentThread();

            Ping ping = new Ping();

            if (estacaoCarregada)
            {
                bool confirmacao = await mainWindow.ShowDialog("Deseja inicar o verificação veículo a veículo?");
                var statusBotaoMonitoramento = btn_moniAlarme.Content.ToString();

                if (confirmacao)
                {
                    btn_moniAlarme.Visibility = Visibility.Collapsed;
                    btn_pararMoniAlarme.Visibility = Visibility.Visible;
                    mainWindow.manterConexao = true;

                    // Desabilita botões do Footer
                    mainWindow.BtnEnviarData.IsEnabled = false;
                    mainWindow.BtnResetar.IsEnabled = false;
                    mainWindow.BtnLerConfig.IsEnabled = false;
                    mainWindow.BtnEnviarConfig.IsEnabled = false;
                    mainWindow.BtnPararPLC.IsEnabled = false;
                    mainWindow.BtnFTP.IsEnabled = false;
                    mainWindow.BotaoConexao.IsEnabled = false;
                    mainWindow.BotaoEquipamentos.IsEnabled = false;

                    mainWindow.ExpanderNavItemArquivos.IsExpanded = false;
                    mainWindow.ExpanderNavItemBanco.IsExpanded = false;

                    mainWindow.NavItemArquivos.IsEnabled = false;
                    mainWindow.NavItemBanco.IsEnabled = false;
                    mainWindow.NavItemConfiguracao.IsEnabled = false;
                    mainWindow.NavItemConfiguracao.IsExpanded = false;
                    mainWindow.NavItemAfericao.IsEnabled = false;

                    PingReply resp = ping.Send(mainWindow.ip_conexao);

                    if (resp.Status == IPStatus.Success)
                    {
                        mainWindow.dadosPLC = "MALM";
                        mainWindow.flagSend = 1;
                        mainWindow.statusSend = 1;
                        mainWindow.nBytes = 4;



                        // Remove handler antigo, se houver
                        if (handlerDadoRecebido != null)
                        {
                            mainWindow.DadoRecebido -= handlerDadoRecebido;
                        }

                        handlerDadoRecebido = dado =>
                        {
                            uiDispatcher.TryEnqueue(() =>
                            {

                            });
                        };
                        mainWindow.DadoRecebido += handlerDadoRecebido;

                        Debug.WriteLine(mainWindow.dadosPLC);
                        mainWindow.Conect();
                    }
                }
            }
        }

        private async void btn_pararMoniAlarme_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)App.m_window;
            bool estacaoCarregada = mainWindow.checkEstacaoCarregada();
            var uiDispatcher = DispatcherQueue.GetForCurrentThread();

            mainWindow.manterConexao = false;

            Ping ping = new Ping();

            if (estacaoCarregada)
            {
                bool confirmacao = await mainWindow.ShowDialog("Deseja PARA o monitoramento de alarmes?");

                if (confirmacao)
                {
                    btn_pararMoniAlarme.Visibility = Visibility.Collapsed;
                    btn_moniAlarme.Visibility = Visibility.Visible;

                    mainWindow.manterConexao = true;

                    // Habilita botões do Footer
                    mainWindow.BtnEnviarData.IsEnabled = true;
                    mainWindow.BtnResetar.IsEnabled = true;
                    mainWindow.BtnLerConfig.IsEnabled = true;
                    mainWindow.BtnEnviarConfig.IsEnabled = true;
                    mainWindow.BtnPararPLC.IsEnabled = true;
                    mainWindow.BtnFTP.IsEnabled = true;
                    mainWindow.BotaoConexao.IsEnabled = true;
                    mainWindow.BotaoEquipamentos.IsEnabled = true;

                    mainWindow.ExpanderNavItemArquivos.IsExpanded = true;
                    mainWindow.ExpanderNavItemBanco.IsExpanded = true;

                    mainWindow.NavItemArquivos.IsEnabled = true;
                    mainWindow.NavItemBanco.IsEnabled = true;
                    mainWindow.NavItemConfiguracao.IsEnabled = true;
                    mainWindow.NavItemConfiguracao.IsExpanded = true;
                    mainWindow.NavItemAfericao.IsEnabled = true;


                    PingReply resp = ping.Send(mainWindow.ip_conexao);

                    if (resp.Status == IPStatus.Success)
                    {
                        mainWindow.dadosPLC = "FMON";
                        mainWindow.flagSend = 1;
                        mainWindow.statusSend = 1;
                        mainWindow.nBytes = 4;

                        Debug.WriteLine(mainWindow.dadosPLC);
                        mainWindow.Conect();
                    }
                    App.MainViewModel.Produtos.Clear();
                }
            }
        }

        private void nomeEquipamento_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtBox)
            {
                string numero = new string(txtBox.Name.Where(char.IsDigit).ToArray()); // pega o número do nome
                var border = this.FindName($"equipamento{numero}") as Border;

                if (border != null)
                {
                    if (!string.IsNullOrWhiteSpace(txtBox.Text))
                    {
                        border.Background = new SolidColorBrush(Color.FromArgb(255, 118, 152, 52));
                        gd_ntcip.Background = new SolidColorBrush(Color.FromArgb(255, 118, 152, 52));
                        gd_sdCard.Background = new SolidColorBrush(Color.FromArgb(255, 118, 152, 52));
                        gd_bateriaPlc.Background = new SolidColorBrush(Color.FromArgb(255, 118, 152, 52));
                        gd_moduloPlc.Background = new SolidColorBrush(Color.FromArgb(255, 118, 152, 52));
                        gd_energia.Background = new SolidColorBrush(Color.FromArgb(255, 118, 152, 52)); ;
                        gd_porta.Background = new SolidColorBrush(Color.FromArgb(255, 118, 152, 52));
                        gd_temperatura.Background = new SolidColorBrush(Color.FromArgb(255, 118, 152, 52));

                    }
                    else
                    {
                        border.Background = new SolidColorBrush(Colors.Transparent);
                    }
                }
            }
        }

        private async void cbx_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Confirmar ação",
                    Content = $"Você tem certeza que deseja desativar o filtro \"{cb.Content}\"?",
                    PrimaryButtonText = "Sim",
                    CloseButtonText = "Não",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot
                };

                var result = await confirmDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    Debug.WriteLine($"Filtro desmarcado: {cb.Content}");
                }
                else
                {
                    // Reverter: manter marcado
                    cb.Checked -= cbx_Checked;
                    cb.IsChecked = true;
                    cb.Checked += cbx_Checked;
                }
            }
        }

        private void cbx_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                Debug.WriteLine($"Filtro marcado: {cb.Content}");
            }
        }



    }
}
