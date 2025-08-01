using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace AnsitraPLC_QR_MIT
{
    public sealed partial class BancoPage : Page 
    {
        public string tipo_banco
        {
            get => (cmbx_tipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            set
            {
                foreach (ComboBoxItem item in cmbx_tipo.Items)
                {
                    if (item.Content?.ToString() == value)
                    {
                        cmbx_tipo.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        public string host_banco
        {
            get => txbx_host.Text;
            set => txbx_host.Text = value;
        }

        public string usuario_banco
        {
            get => txbx_usuario.Text;
            set => txbx_usuario.Text = value;
        }

        public string porta_banco
        {
            get => txbx_porta.Text;
            set => txbx_porta.Text = value;
        }

        public string exibicao_banco
        {
            get => (cmb_exibicao.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            set
            {
                foreach (ComboBoxItem item in cmb_exibicao.Items)
                {
                    if (item.Content?.ToString() == value)
                    {
                        cmb_exibicao.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        public string servidor_banco
        {
            get => txbx_servico.Text;
            set => txbx_servico.Text = value;
        }

        public string senha_banco
        {
            get => txbx_senha.Password;
            set => txbx_senha.Password = value;
        }

        public string servico_banco
        {
            get => txbx_servico.Text;
            set => txbx_servico.Text = value;
        }



        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public BancoPage()
        {
            this.InitializeComponent();
            CarregaValoresSalvos();
        }

        private void CarregaValoresSalvos()
        {
            // Recupera e atribui os valores salvos aos TextBoxes
            if (localSettings.Values.ContainsKey("Host"))
                txbx_host.Text = localSettings.Values["Host"] as string;

            if (localSettings.Values.ContainsKey("Usuario"))
                txbx_usuario.Text = localSettings.Values["Usuario"] as string;

            if (localSettings.Values.ContainsKey("Porta"))
                txbx_porta.Text = localSettings.Values["Porta"] as string;

            if (localSettings.Values.ContainsKey("Servico"))
                txbx_servico.Text = localSettings.Values["Servico"] as string;

            if (localSettings.Values.ContainsKey("Senha"))
                txbx_senha.Password = localSettings.Values["Senha"] as string;

            if (localSettings.Values.ContainsKey("ExibicaoIndex"))
            {
                int index = (int)localSettings.Values["ExibicaoIndex"];
                if (index >= 0 && index < cmb_exibicao.Items.Count)
                {
                    cmb_exibicao.SelectedIndex = index;
                }
            }

            if (localSettings.Values.ContainsKey("TipoIndex"))
            {
                int index = (int)localSettings.Values["TipoIndex"];
                if (index >= 0 && index < cmbx_tipo.Items.Count)
                {
                    cmbx_tipo.SelectedIndex = index;
                }
            }
        }

        private void SalvaValores()
        {
            // Salva os valores dos TextBoxes
            localSettings.Values["Host"] = txbx_host.Text;
            localSettings.Values["Usuario"] = txbx_usuario.Text;
            localSettings.Values["Porta"] = txbx_porta.Text;
            localSettings.Values["Servico"] = txbx_servico.Text;
            localSettings.Values["Senha"] = txbx_senha.Password;

            // Salva os índices selecionados nos ComboBoxes
            if (cmb_exibicao.SelectedIndex >= 0)
            {
                localSettings.Values["ExibicaoIndex"] = cmb_exibicao.SelectedIndex;
            }

            if (cmbx_tipo.SelectedIndex >= 0)
            {
                localSettings.Values["TipoIndex"] = cmbx_tipo.SelectedIndex;
            }
        }

        private void btn_gravar_Click(object sender, RoutedEventArgs e)
        {
            SalvaValores();
            MensagemDialogo("Informações salvas com sucesso.");
        }

        private async void MensagemDialogo(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Ansitra PLC - Informação do Banco de Dados",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void btn_limpar_Click(object sender, RoutedEventArgs e)
        {
            //Campos Texto
            txbx_host.Text = string.Empty;
            txbx_usuario.Text = string.Empty;
            txbx_porta.Text = string.Empty;
            txbx_servico.Text = string.Empty;
            txbx_senha.Password = string.Empty;

            //ComboBoxes
            cmb_exibicao.SelectedIndex = -1;
            cmbx_tipo.SelectedIndex = -1;

        }
    }
}
