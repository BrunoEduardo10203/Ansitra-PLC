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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AnsitraPLC_QR_MIT
{
    public sealed partial class ClassePage : Page
    {
        //Tipo
        public ComboBox Tipo1 { get => tipo1; set => tipo1 = value; }
        public ComboBox Tipo2{ get => tipo2; set => tipo2 = value; }
        public ComboBox Tipo3 { get => tipo3; set => tipo3 = value; }
        public ComboBox Tipo4 { get => tipo4; set => tipo4 = value; }
        public ComboBox Tipo5 { get => tipo5; set => tipo5 = value; }
        public ComboBox Tipo6 { get => tipo6; set => tipo6 = value; }
        public ComboBox Tipo7 { get => tipo7; set => tipo7 = value; }
        public ComboBox Tipo8 { get => tipo8; set => tipo8 = value; }
        public ComboBox Tipo9 { get => tipo9; set => tipo9 = value; }
        public ComboBox Tipo10 { get => tipo10; set => tipo10 = value; }
        public ComboBox Tipo11 { get => tipo11; set => tipo11 = value; }
        public ComboBox Tipo12 { get => tipo12; set => tipo12 = value; }
        public ComboBox Tipo13 { get => tipo13; set => tipo13 = value; }
        public ComboBox Tipo14 { get => tipo14; set => tipo14 = value; }
        public ComboBox Tipo15 { get => tipo15; set => tipo15 = value; }


        //Nome
        public TextBox Nome1 { get => txtbx_classe1; set => txtbx_classe1 = value; }
        public TextBox Nome2 { get => txtbx_classe2; set => txtbx_classe2 = value; }
        public TextBox Nome3 { get => txtbx_classe3; set => txtbx_classe3 = value; }
        public TextBox Nome4 { get => txtbx_classe4; set => txtbx_classe4 = value; }
        public TextBox Nome5 { get => txtbx_classe5; set => txtbx_classe5 = value; }
        public TextBox Nome6 { get => txtbx_classe6; set => txtbx_classe6 = value; }
        public TextBox Nome7 { get => txtbx_classe7; set => txtbx_classe7 = value; }
        public TextBox Nome8 { get => txtbx_classe8; set => txtbx_classe8 = value; }
        public TextBox Nome9 { get => txtbx_classe9; set => txtbx_classe9 = value; }
        public TextBox Nome10 { get => txtbx_classe10; set => txtbx_classe10 = value; }
        public TextBox Nome11 { get => txtbx_classe11; set => txtbx_classe11 = value; }
        public TextBox Nome12 { get => txtbx_classe12; set => txtbx_classe12 = value; }
        public TextBox Nome13 { get => txtbx_classe13; set => txtbx_classe13 = value; }
        public TextBox Nome14 { get => txtbx_classe14; set => txtbx_classe14 = value; }
        public TextBox Nome15 { get => txtbx_classe15; set => txtbx_classe15 = value; }



        //Inicio
        public TextBox Inicio1 { get => txtbx_inicio1; set => txtbx_inicio1 = value; }
        public TextBox Inicio2 { get => txtbx_inicio2; set => txtbx_inicio2 = value; }
        public TextBox Inicio3 { get => txtbx_inicio3; set => txtbx_inicio3 = value; }
        public TextBox Inicio4 { get => txtbx_inicio4; set => txtbx_inicio4 = value; }
        public TextBox Inicio5 { get => txtbx_inicio5; set => txtbx_inicio5 = value; }
        public TextBox Inicio6 { get => txtbx_inicio6; set => txtbx_inicio6 = value; }
        public TextBox Inicio7 { get => txtbx_inicio7; set => txtbx_inicio7 = value; }
        public TextBox Inicio8 { get => txtbx_inicio8; set => txtbx_inicio8 = value; }
        public TextBox Inicio9 { get => txtbx_inicio9; set => txtbx_inicio9 = value; }
        public TextBox Inicio10 { get => txtbx_inicio10; set => txtbx_inicio10 = value; }
        public TextBox Inicio11 { get => txtbx_inicio11; set => txtbx_inicio11 = value; }
        public TextBox Inicio12 { get => txtbx_inicio12; set => txtbx_inicio12 = value; }
        public TextBox Inicio13 { get => txtbx_inicio13; set => txtbx_inicio13 = value; }
        public TextBox Inicio14 { get => txtbx_inicio14; set => txtbx_inicio14 = value; }
        public TextBox Inicio15 { get => txtbx_inicio15; set => txtbx_inicio15 = value; }


        //Fim
        public TextBox Fim1 { get => txtbx_fim1; set => txtbx_fim1 = value; }
        public TextBox Fim2 { get => txtbx_fim2; set => txtbx_fim2 = value; }
        public TextBox Fim3 { get => txtbx_fim3; set => txtbx_fim3 = value; }
        public TextBox Fim4 { get => txtbx_fim4; set => txtbx_fim4 = value; }
        public TextBox Fim5 { get => txtbx_fim5; set => txtbx_fim5 = value; }
        public TextBox Fim6 { get => txtbx_fim6; set => txtbx_fim6 = value; }
        public TextBox Fim7 { get => txtbx_fim7; set => txtbx_fim7 = value; }
        public TextBox Fim8 { get => txtbx_fim8; set => txtbx_fim8 = value; }
        public TextBox Fim9 { get => txtbx_fim9; set => txtbx_fim9 = value; }
        public TextBox Fim10 { get => txtbx_fim10; set => txtbx_fim10 = value; }
        public TextBox Fim11 { get => txtbx_fim11; set => txtbx_fim11 = value; }
        public TextBox Fim12 { get => txtbx_fim12; set => txtbx_fim12 = value; }
        public TextBox Fim13 { get => txtbx_fim13; set => txtbx_fim13 = value; }
        public TextBox Fim14 { get => txtbx_fim14; set => txtbx_fim14 = value; }
        public TextBox Fim15 { get => txtbx_fim15; set => txtbx_fim15 = value; }


        //Quantidade de Eixos
        public TextBox QtdEixo1 { get => txtbx_qtdEixos1; set => txtbx_qtdEixos1 = value; }
        public TextBox QtdEixo2 { get => txtbx_qtdEixos2; set => txtbx_qtdEixos2 = value; }
        public TextBox QtdEixo3 { get => txtbx_qtdEixos3; set => txtbx_qtdEixos3 = value; }
        public TextBox QtdEixo4 { get => txtbx_qtdEixos4; set => txtbx_qtdEixos4 = value; }
        public TextBox QtdEixo5 { get => txtbx_qtdEixos5; set => txtbx_qtdEixos5 = value; }
        public TextBox QtdEixo6 { get => txtbx_qtdEixos6; set => txtbx_qtdEixos6 = value; }
        public TextBox QtdEixo7 { get => txtbx_qtdEixos7; set => txtbx_qtdEixos7 = value; }
        public TextBox QtdEixo8 { get => txtbx_qtdEixos8; set => txtbx_qtdEixos8 = value; }
        public TextBox QtdEixo9 { get => txtbx_qtdEixos9; set => txtbx_qtdEixos9 = value; }
        public TextBox QtdEixo10 { get => txtbx_qtdEixos10; set => txtbx_qtdEixos10 = value; }
        public TextBox QtdEixo11 { get => txtbx_qtdEixos11; set => txtbx_qtdEixos11 = value; }
        public TextBox QtdEixo12 { get => txtbx_qtdEixos12; set => txtbx_qtdEixos12 = value; }
        public TextBox QtdEixo13 { get => txtbx_qtdEixos13; set => txtbx_qtdEixos13 = value; }
        public TextBox QtdEixo14 { get => txtbx_qtdEixos14; set => txtbx_qtdEixos14 = value; }
        public TextBox QtdEixo15 { get => txtbx_qtdEixos15; set => txtbx_qtdEixos15 = value; }


        public ClassePage()
        {
            InitializeComponent();
            DataContext = App.MainViewModel;
        }

        private void btn_setarNomes_Click(object sender, RoutedEventArgs e)
        {
            string name = txtbx_nomeClasses.Text;

            if (int.TryParse(txtbx_quantidade.Text, out int quantity) && quantity <= 15 && quantity > 0)
            {
                if (!string.IsNullOrEmpty(name))
                {

                    TextBox[] textBoxes= { txtbx_classe1, txtbx_classe2, txtbx_classe3, txtbx_classe4, txtbx_classe5, txtbx_classe6, txtbx_classe7, txtbx_classe8, txtbx_classe9, txtbx_classe10, txtbx_classe11, txtbx_classe12, txtbx_classe13, txtbx_classe14, txtbx_classe15};

                    // Limita a quantidade para o número de CheckBoxes disponíveis
                    quantity = Math.Min(quantity, textBoxes.Length);


                    for (int i = 0; i < quantity; i++)
                    {
                        textBoxes[i].Text = name+(i+1);
                    }

                }
                else
                {
                    ShowDialog("Por favor, insira um nome válido.");
                }
            }
            else
            {
                ShowDialog("Por favor, insira um nome e um numero válido no campos de NOME e QUANTIDADE (De 1 a 15)");
            }
        }

        private async void ShowDialog(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Aviso",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task<bool> ShowDialog_exclusao(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Aviso",
                Content = message,
                PrimaryButtonText = "Sim",
                CloseButtonText = "Não",
                XamlRoot = this.Content.XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary; 
        }


        private async void btn_limparCampos_Click(object sender, RoutedEventArgs e)
        {
            bool confirmacao = await ShowDialog_exclusao("Todos os dados da coluna NOME DAS CLASSES serão EXCLUIDOS, deseja continuar?");

            if (confirmacao)
            {
                TextBox[] textBoxes = { txtbx_classe1, txtbx_classe2, txtbx_classe3, txtbx_classe4, txtbx_classe5, txtbx_classe6, txtbx_classe7, txtbx_classe8, txtbx_classe9, txtbx_classe10, txtbx_classe11, txtbx_classe12, txtbx_classe13, txtbx_classe14, txtbx_classe15, txtbx_nomeClasses, txtbx_quantidade };
                foreach (TextBox textBox in textBoxes)
                {
                    textBox.Text = "";
                }
            }
        }
        private async void btn_tipo_Click(object sender, RoutedEventArgs e)
        {
            ComboBox[] comboBoxes = { tipo1, tipo2, tipo3, tipo4, tipo5, tipo6, tipo7, tipo8, tipo9, tipo10, tipo11, tipo12, tipo13, tipo14, tipo15 };

            bool confirmacao = await ShowDialog_exclusao("Todos os dados da coluna TIPO serão EXCLUIDOS, deseja continuar?");

            if (confirmacao)
            {
                foreach (ComboBox comboBox in comboBoxes)
                {
                    comboBox.SelectedIndex = -1;
                }
            }
        
        }


        private async void btn_inicio_Click(object sender, RoutedEventArgs e)
        {
            TextBox[] textBoxes = { txtbx_inicio1, txtbx_inicio2, txtbx_inicio3, txtbx_inicio4, txtbx_inicio5, txtbx_inicio6, txtbx_inicio6, txtbx_inicio7, txtbx_inicio8, txtbx_inicio9, txtbx_inicio10, txtbx_inicio11, txtbx_inicio12, txtbx_inicio13, txtbx_inicio14, txtbx_inicio15 };
            
            bool confirmacao = await ShowDialog_exclusao("Todos os dados da coluna INICIO serão EXCLUIDOS, deseja continuar?");

            if (confirmacao)
            {
                foreach (TextBox textBox in textBoxes)
                {
                    textBox.Text = "";
                }
            }
        }

        private async void btn_fim_Click(object sender, RoutedEventArgs e)
        {
            TextBox[] textBoxes = { txtbx_fim1, txtbx_fim2, txtbx_fim3, txtbx_fim4, txtbx_fim5, txtbx_fim6, txtbx_fim7, txtbx_fim8, txtbx_fim9, txtbx_fim10, txtbx_fim11, txtbx_fim12, txtbx_fim13, txtbx_fim14, txtbx_fim15 };
            bool confirmacao = await ShowDialog_exclusao("Todos os dados da coluna FIM serão EXCLUIDOS, deseja continuar?");

            if (confirmacao)
            {
                foreach (TextBox textBox in textBoxes)
                {
                    textBox.Text = "";
                }
            }
        }

        private async void btn_qtdEixos_Click(object sender, RoutedEventArgs e)
        {

            TextBox[] textBoxes = {txtbx_qtdEixos1, txtbx_qtdEixos2, txtbx_qtdEixos3, txtbx_qtdEixos4, txtbx_qtdEixos5, txtbx_qtdEixos6, txtbx_qtdEixos7, txtbx_qtdEixos8, txtbx_qtdEixos9, txtbx_qtdEixos10, txtbx_qtdEixos11, txtbx_qtdEixos12, txtbx_qtdEixos13, txtbx_qtdEixos14, txtbx_qtdEixos15 };

            bool confirmacao = await ShowDialog_exclusao("Todos os dados da coluna QUANTIDADE DE EIXOS serão EXCLUIDOS, deseja continuar?");

            if (confirmacao)
            {
                foreach (TextBox textBox in textBoxes)
                {
                    textBox.Text = "";
                }
            }
        }
    }
}

