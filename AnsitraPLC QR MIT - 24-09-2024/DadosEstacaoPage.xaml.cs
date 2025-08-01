
using AnsitraPLC_QR_MIT.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AnsitraPLC_QR_MIT
{

    public sealed partial class DadosEstacaoPage : Page
    {

        public TextBlock txb_Rodovia
        {
            get => txb_rodovia;
            set => txb_rodovia = value;
        }

        public TextBlock txb_Estacao
        {
            get => txb_estacao;
            set => txb_estacao = value;
        }

        public TextBox txb_Numero
        {
            get => txb_numero;
            set => txb_numero = value;
        }

        public TextBox txb_Km
        {
            get => txb_km;
            set => txb_km = value;
        }

        public ComboBox cbx_Local
        {
            get => cbx_local;
            set => cbx_local = value;
        }

        public ComboBox cbx_Uf
        {
            get => cbx_uf;
            set => cbx_uf = value;
        }

        public ComboBox cbx_IdRodovia
        {
            get => cbx_rodovia;
            set => cbx_rodovia = value;
        }

        public DadosEstacaoPage()
        {
            this.InitializeComponent();
            this.DataContext = App.MainViewModel;
        }

        private void txb_numero_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.m_window is MainWindow mainWindow)
            {
                // Salva a posi��o do cursor utilizando SelectionStart
                int selectionStart = txb_numero.SelectionStart;

                // Filtra para manter apenas os d�gitos
                string somenteNumeros = new string(txb_numero.Text.Where(c => char.IsDigit(c)).ToArray());

                // Se houver caracteres n�o num�ricos, substitui pelo texto filtrado
                if (txb_numero.Text != somenteNumeros)
                {
                    txb_numero.Text = somenteNumeros;
                    // Ajusta a posi��o do cursor
                    txb_numero.SelectionStart = Math.Min(selectionStart, txb_numero.Text.Length);
                }

                if (string.IsNullOrEmpty(txb_numero.Text))
                {
                    mainWindow.AtualizarHeaderNumero("000");
                }
                else if (int.TryParse(txb_numero.Text, out int numero))
                {
                    // Formata o n�mero para 3 d�gitos com zeros � esquerda
                    mainWindow.AtualizarHeaderNumero(numero.ToString("D3"));
                }
            }
        }


        private void cbx_rodovia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && App.m_window is MainWindow mainWindow)
            {
                var selectedComboBoxItem = comboBox.SelectedItem as ComboBoxItem;
                var selectedValue = comboBox.SelectedItem?.ToString();

                mainWindow.AtualizarHeaderRodovia(selectedValue);
            }
        }

        private void txb_km_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Salva a posi��o do cursor
                int caretPosition = textBox.SelectionStart;

                string txtKM = textBox.Text;

                // Permite apenas n�meros e uma �nica v�rgula
                string textoNumerico = new string(txtKM.Where(c => char.IsDigit(c) || c == ',').ToArray());

                // Garante que haja no m�ximo uma v�rgula
                int indicePrimeiraVirgula = textoNumerico.IndexOf(',');

                // Remove qualquer v�rgula extra ap�s a primeira
                if (indicePrimeiraVirgula != -1)
                {
                    textoNumerico = textoNumerico.Substring(0, indicePrimeiraVirgula + 1) +
                                    textoNumerico.Substring(indicePrimeiraVirgula + 1).Replace(",", "");
                }

                string valorFormatado = textoNumerico;

                // Se houver v�rgula, garante no m�ximo 4 d�gitos antes da v�rgula e 3 d�gitos decimais
                if (indicePrimeiraVirgula != -1)
                {
                    string parteInteira = textoNumerico.Substring(0, indicePrimeiraVirgula);
                    string parteDecimal = textoNumerico.Substring(indicePrimeiraVirgula + 1);

                    // Limita a parte inteira a 4 d�gitos
                    if (parteInteira.Length > 4)
                    {
                        parteInteira = parteInteira.Substring(0, 4);
                    }

                    // Limita a parte decimal a 3 d�gitos
                    if (parteDecimal.Length > 3)
                    {
                        parteDecimal = parteDecimal.Substring(0, 3);
                    }
                    else
                    {
                        // Preenche com zeros
                        parteDecimal = parteDecimal.PadRight(3, '0'); 
                    }

                    valorFormatado = parteInteira + "," + parteDecimal;
                }
                // Se n�o houver v�rgula, limita a parte inteira a 4 d�gitos e adiciona ",000"
                else if (!string.IsNullOrEmpty(textoNumerico))
                {
                    textoNumerico = textoNumerico.Substring(0, Math.Min(4, textoNumerico.Length));
                    valorFormatado = textoNumerico + ",000";
                }

                // Atualiza o TextBox apenas se o texto foi alterado
                if (textBox.Text != valorFormatado)
                {
                    textBox.Text = valorFormatado;
                    textBox.SelectionStart = Math.Min(caretPosition, textBox.Text.Length); // Mant�m a posi��o do cursor
                }


                // Envia o valor formatado para atualiza��o
                if (App.m_window is MainWindow mainWindow)
                {
                    mainWindow.AtualizarHeaderKm(valorFormatado);
                }
            }
        }

        private void cbx_Uf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cbx_Uf.SelectedItem as string;

            if (selectedItem != null && selectedItem == "SP")
            {
                txbl_Ra.Visibility = Visibility.Visible;
                txb_RaValor.Visibility = Visibility.Visible;
            }
            else
            {
                txbl_Ra.Visibility = Visibility.Collapsed;
                txb_RaValor.Visibility = Visibility.Collapsed;
            }
        }

    }

}

