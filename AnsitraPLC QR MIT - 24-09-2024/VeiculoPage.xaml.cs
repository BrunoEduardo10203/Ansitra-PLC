using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using System;
using System.IO;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Microsoft.UI.Dispatching;

namespace AnsitraPLC_QR_MIT
{
    public partial class VeiculoPage : Page
    {
        private Action<string> handlerDadoRecebido;

        int controlarContagem = 0, statusConexao = 2, statusMostraDados = 0, statusSend = 0, nBytes = 0, flagSend = 0;
        public Button BotaoIniciar => btn_iniciar;
        public Button BotaoFechar => btn_fechar;
        public Button BotaoGravar => btn_gravar;


        public VeiculoPage()
        {
            this.InitializeComponent();
            this.DataContext = App.MainViewModel;
        }
        private void VerificarCheckBox()
        {
            CheckBox[] checkBoxes= { pista1, pista2, pista3, pista4, pista5, pista6, pista7, pista8, pista9, pista10, pista11, pista12, pista13, pista14, pista15, pista16, };

            foreach (var checkBox in checkBoxes)
            {
                checkBox.Visibility = !string.IsNullOrEmpty(checkBox.Content?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Método chamado quando o CheckBox "Select All" é marcado
        private void SelectAllPistas_Checked(object sender, RoutedEventArgs e)
        {
            pista1.IsChecked = true;
            pista2.IsChecked = true;
            pista3.IsChecked = true;
            pista4.IsChecked = true;
            pista5.IsChecked = true;
            pista6.IsChecked = true;
            pista7.IsChecked = true;
            pista8.IsChecked = true;
            pista9.IsChecked = true;
            pista10.IsChecked = true;
            pista11.IsChecked = true;
            pista12.IsChecked = true;
            pista13.IsChecked = true;
            pista14.IsChecked = true;
            pista15.IsChecked = true;
            pista16.IsChecked = true;

        }

        private async void btn_limpar_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)App.m_window;

            Ping ping = new Ping();

            bool confirmacao = await mainWindow.ShowDialog("Deseja inicar o verificação veículo a veículo?");

            if (confirmacao)
            {
                MainViewModel vm = new MainViewModel();
                vm.contagemPassagem = 5;

                App.MainViewModel.Produtos.Clear();
            }

        }
        

        // Método chamado quando o CheckBox "Select All" é desmarcado
        private void SelectAllPistas_Unchecked(object sender, RoutedEventArgs e)
        {
            pista1.IsChecked = false;
            pista2.IsChecked = false;
            pista3.IsChecked = false;
            pista4.IsChecked = false;
            pista5.IsChecked = false;
            pista6.IsChecked = false;
            pista7.IsChecked = false;
            pista8.IsChecked = false;
            pista9.IsChecked = false;
            pista10.IsChecked = false;
            pista11.IsChecked = false;
            pista12.IsChecked = false;
            pista13.IsChecked = false;
            pista14.IsChecked = false;
            pista15.IsChecked = false;
            pista16.IsChecked = false;


        }

        private async void btn_gravar_Click(object sender, RoutedEventArgs e)
        {
            var botao = (Button)sender;
            string textoAtual = botao.Content.ToString();
            var mainWindow = (MainWindow)App.m_window;


            if (textoAtual == "Iniciar Gravação")
            {
                var dialog = new ContentDialog
                {
                    Title = "Confirmação",
                    Content = "Deseja mesmo iniciar a gravação?",
                    PrimaryButtonText = "Sim",
                    CloseButtonText = "Não",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.Content.XamlRoot // Importante no WinUI 3
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    botao.Content = "Parar Gravação";
                }
            }
            else if (textoAtual == "Parar Gravação")
            {
                var dialog = new ContentDialog
                {
                    Title = "Confirmação",
                    Content = "Deseja mesmo parar a gravação?",
                    PrimaryButtonText = "Sim",
                    CloseButtonText = "Não",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.Content.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    botao.Content = "Iniciar Gravação";
                }
            }
        }


        // Método chamado quando o CheckBox "Selecionar todas as colunas" é marcado
        private void SelectAllColunas_Checked(object sender, RoutedEventArgs e)
        {
            numeroCbx.IsChecked = true;
            dataCbx.IsChecked = true;
            horaCbx.IsChecked = true;
            faixaCbx.IsChecked = true;
            velocCbx.IsChecked = true;
            compriCbx.IsChecked = true;
            ccCbx.IsChecked = true;
            ocupCbx.IsChecked = true;
            gapMtCbx.IsChecked = true;
            gapSegCbx.IsChecked = true;
            headwayCbx.IsChecked = true;
            interCbx.IsChecked = true;
            ocupacaoCbx.IsChecked = true;
            acionamCbx.IsChecked = true;
            qtdEixoCbx.IsChecked = true;
            grpEixoCbx.IsChecked = true;
            classDNITCbx.IsChecked = true;
            pesoEixo01Cbx.IsChecked = true;
            pesoEixo02Cbx.IsChecked = true;
            pesoEixo03Cbx.IsChecked = true;
            pesoEixo04Cbx.IsChecked = true;
            pesoEixo05Cbx.IsChecked = true;
            pesoEixo06Cbx.IsChecked = true;
            pesoEixo07Cbx.IsChecked = true;
            pesoEixo08Cbx.IsChecked = true;
            pesoEixo09Cbx.IsChecked = true;
            pesoEixo10Cbx.IsChecked = true;
            pesototalCbx.IsChecked = true;

        }

        // Método chamado quando o CheckBox "Selecionar todas as colunas" é desmarcado
        private void SelectAllColunas_Unchecked(object sender, RoutedEventArgs e)
        {
            numeroCbx.IsChecked = false;
            dataCbx.IsChecked = false;
            horaCbx.IsChecked = false;
            faixaCbx.IsChecked = false;
            velocCbx.IsChecked = false;
            compriCbx.IsChecked = false;
            ccCbx.IsChecked = false;
            ocupCbx.IsChecked = false;
            gapMtCbx.IsChecked = false;
            gapSegCbx.IsChecked = false;
            headwayCbx.IsChecked = false;
            interCbx.IsChecked = false;
            ocupacaoCbx.IsChecked = false;
            acionamCbx.IsChecked = false;
            qtdEixoCbx.IsChecked = false;
            grpEixoCbx.IsChecked = false;
            classDNITCbx.IsChecked = false;
            pesoEixo01Cbx.IsChecked = false;
            pesoEixo02Cbx.IsChecked = false;
            pesoEixo03Cbx.IsChecked = false;
            pesoEixo04Cbx.IsChecked = false;
            pesoEixo05Cbx.IsChecked = false;
            pesoEixo06Cbx.IsChecked = false;
            pesoEixo07Cbx.IsChecked = false;
            pesoEixo08Cbx.IsChecked = false;
            pesoEixo09Cbx.IsChecked = false;
            pesoEixo10Cbx.IsChecked = false;
            pesototalCbx.IsChecked = false;


        }

        private void btn_pistas_Click(object sender, RoutedEventArgs e)
        {
            VerificarCheckBox();
        }

        public void SalvarDadoEmArquivo(string dado)
        {
            string caminhoArquivo = @"C:\Users\Stine\OneDrive - STN\Aplicativos\AnsitraPLC - Mit. QR\AnsitraPLC_QR_Beta\AnsitraPLC QR MIT - 24-09-2024\dadosDePassagem\dados_recebidos.txt";

            try
            {
                if (dado.StartsWith("IVBV;"))
                    dado = dado.Substring("IVBV;".Length);

                if (dado.EndsWith(";FVBV"))
                    dado = dado.Substring(0, dado.Length - ";FVBV".Length);

                // Salva somente o conteúdo limpo no arquivo
                File.AppendAllText(caminhoArquivo, dado + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro ao salvar dado: " + ex.Message);
            }
        }




        private async void btn_iniciar_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)App.m_window;
            bool estacaoCarregada = mainWindow.checkEstacaoCarregada();
            var uiDispatcher = DispatcherQueue.GetForCurrentThread();

            Ping ping = new Ping();

            if (estacaoCarregada)
            {
                bool confirmacao = await mainWindow.ShowDialog("Deseja inicar o verificação veículo a veículo?");

                if (confirmacao)
                {
                    mainWindow.manterConexao = true;
                    btn_iniciar.Visibility = Visibility.Collapsed;
                    btn_fechar.Visibility = Visibility.Visible;

                    // Desabilita botões do Footer
                    mainWindow.BtnEnviarData.IsEnabled = false;
                    mainWindow.BtnResetar.IsEnabled = false;
                    mainWindow.BtnLerConfig.IsEnabled = false;
                    mainWindow.BtnEnviarConfig.IsEnabled = false;
                    mainWindow.BtnPararPLC.IsEnabled = false;
                    mainWindow.BtnFTP.IsEnabled = false;
                    mainWindow.BotaoEquipamentos.IsEnabled = false;

                    mainWindow.ExpanderNavItemArquivos.IsExpanded = false;
                    mainWindow.ExpanderNavItemBanco.IsExpanded = false;

                    mainWindow.NavItemArquivos.IsEnabled = false;
                    mainWindow.NavItemBanco.IsEnabled = false;
                    mainWindow.NavItemAlarmes.IsEnabled = false;
                    mainWindow.NavItemConfiguracao.IsEnabled = false;
                    mainWindow.NavItemConfiguracao.IsExpanded = false;

                    PingReply resp = ping.Send(mainWindow.ip_conexao);

                    if (resp.Status == IPStatus.Success)
                    {
                        mainWindow.dadosPLC = "MVBV";
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
                                App.MainViewModel.ProcessarDadoMonitoramento(dado);
                                if(btn_gravar.Content.ToString() == "Parar Gravação")
                                {
                                    SalvarDadoEmArquivo(dado);
                                }
                            });
                        };
                        mainWindow.DadoRecebido += handlerDadoRecebido;

                        Debug.WriteLine(mainWindow.dadosPLC);
                        mainWindow.Conect();
                    }
                }
            }
        }

        private async void btn_fechar_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)App.m_window;
            bool estacaoCarregada = mainWindow.checkEstacaoCarregada();
            var uiDispatcher = DispatcherQueue.GetForCurrentThread();

            mainWindow.manterConexao = false;

            Ping ping = new Ping();

            if (estacaoCarregada)
            {
                bool confirmacao = await mainWindow.ShowDialog("Deseja PARA a verificação veículo a veículo?");

                if (confirmacao)
                {
                    mainWindow.manterConexao = true;
                    btn_iniciar.Visibility = Visibility.Visible;
                    btn_fechar.Visibility = Visibility.Collapsed;

                    // Habilita botões do Footer
                    mainWindow.BtnEnviarData.IsEnabled = true;
                    mainWindow.BtnResetar.IsEnabled = true;
                    mainWindow.BtnLerConfig.IsEnabled = true;
                    mainWindow.BtnEnviarConfig.IsEnabled = true;
                    mainWindow.BtnPararPLC.IsEnabled = true;
                    mainWindow.BtnFTP.IsEnabled = true;
                    mainWindow.BotaoEquipamentos.IsEnabled = true;

                    mainWindow.NavItemArquivos.IsEnabled = true;
                    mainWindow.NavItemBanco.IsEnabled = true;
                    mainWindow.NavItemAlarmes.IsEnabled = true;
                    mainWindow.NavItemConfiguracao.IsEnabled = true;


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

    }

    public class BooleanToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isChecked = (bool)value;

            // Converte o parâmetro em um valor de largura
            if (parameter is string widthStr && double.TryParse(widthStr, out double width))
            {
                return isChecked ? new GridLength(width) : new GridLength(0);
            }

            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
    

}
