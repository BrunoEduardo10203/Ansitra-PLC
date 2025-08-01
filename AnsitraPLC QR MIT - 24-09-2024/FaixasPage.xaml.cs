using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using AnsitraPLC_QR_MIT;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using AnsitraPLC_QR_MIT.Models;

namespace AnsitraPLC_QR_MIT
{
    public sealed partial class FaixasPage : Page
    {
        public string NomeFaixa1 => txbxNomePD1?.Text;
        public string NomeFaixa2 => txbxNomePD2?.Text;
        public string NomeFaixa3 => txbxNomePD3?.Text;
        public string NomeFaixa4 => txbxNomePD4?.Text;
        public string NomeFaixa5 => txbxNomePD5?.Text;
        public string NomeFaixa6 => txbxNomePD6?.Text;
        public string NomeFaixa7 => txbxNomePD7?.Text;
        public string NomeFaixa8 => txbxNomePD8?.Text;
        public string NomeFaixa9 => txbxNomePD9?.Text;
        public string NomeFaixa10 => txbxNomePD10?.Text;
        public string NomeFaixa11 => txbxNomePD11?.Text;
        public string NomeFaixa12 => txbxNomePD12?.Text;
        public string NomeFaixa13 => txbxNomePD13?.Text;
        public string NomeFaixa14 => txbxNomePD14?.Text;
        public string NomeFaixa15 => txbxNomePD15?.Text;
        public string NomeFaixa16 => txbxNomePD16?.Text;

        public CheckBox canal1 => ckbxHabilitaCanal1_1_1;
        public CheckBox canal2 => ckbxHabilitaCanal2_1_2;
        public CheckBox canal3 => ckbxHabilitaCanal3_1_3;
        public CheckBox canal4 => ckbxHabilitaCanal4_1_4;
        public CheckBox canal5 => ckbxHabilitaCanal5_2_1;
        public CheckBox canal6 => ckbxHabilitaCanal6_2_2;
        public CheckBox canal7 => ckbxHabilitaCanal7_2_3;
        public CheckBox canal8 => ckbxHabilitaCanal8_2_4;
        public CheckBox canal9 => ckbxHabilitaCanal9_3_1;
        public CheckBox canal10 => ckbxHabilitaCanal10_3_2;
        public CheckBox canal11 => ckbxHabilitaCanal11_3_3;
        public CheckBox canal12 => ckbxHabilitaCanal12_3_4;
        public CheckBox canal13 => ckbxHabilitaCanal13_4_1;
        public CheckBox canal14 => ckbxHabilitaCanal14_4_2;
        public CheckBox canal15 => ckbxHabilitaCanal15_4_3;
        public CheckBox canal16 => ckbxHabilitaCanal16_4_4;



        public FaixasPage()
        {
            InitializeComponent();
            DataContext = App.MainViewModel;
        }
        public void ckbx_HabilitaCanal_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;

            if (chkBox.IsChecked == true)
            {
                setaCamposFaixas(chkBox);
            }
            else if (chkBox.IsChecked == false)
            {
                setaCamposFaixas(chkBox);
            }
        }

        public void setaCamposFaixas(CheckBox chkBox)
        {
            int pos1 = chkBox.Name.IndexOf("_");
            int tam = pos1 - 17;

            string numero = chkBox.Name.Substring(17, tam);
            string detector = chkBox.Name.Substring(pos1 + 1, 1);
            string numElem = chkBox.Name.Substring(pos1 + 3);

            TextBlock txtBlockNome = (TextBlock)this.FindName("txbxNomePD" + numero);
            ComboBox cbBoxTipo = (ComboBox)this.FindName("cbbxTipoPD" + numero + "_" + detector + "_" + numElem);
            ComboBox cbBoxSent = (ComboBox)this.FindName("cbbxSentPD" + numero + "_" + detector + "_" + numElem);
            ComboBox cbBoxNume = (ComboBox)this.FindName("cbbxNumePD" + numero + "_" + detector + "_" + numElem);
            TextBlock txtBlockPort = (TextBlock)this.FindName("txtbx_IDPorta" + numero);
            CheckBox chkBxCont = (CheckBox)this.FindName("ckbx_contagemP" + numero);
            TextBox txtBoxCont = (TextBox)this.FindName("txtbx_contagemP" + numero);

            if (chkBox.IsChecked == true)
            {
                cbBoxSent.IsEnabled = true;
            }
            
            if(chkBox.IsChecked == false)
            {
                txtBlockNome.Text = "";
                cbBoxTipo.SelectedIndex = -1;
                cbBoxTipo.IsEnabled = false;
                cbBoxSent.SelectedIndex = -1;
                cbBoxSent.IsEnabled = false;
                cbBoxNume.SelectedIndex = -1;
                cbBoxNume.IsEnabled = false;
                txtBlockPort.Text = "ID: ";
            }
        }

        public void cbbxSentPD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            int pos1 = comboBox.Name.IndexOf("_");
            int tam = pos1 - 10;
            string numero = comboBox.Name.Substring(10, tam);
            string detector = comboBox.Name.Substring(pos1 + 1, 1);
            string numElem = comboBox.Name.Substring(pos1 + 3);

            TextBlock txtBlockNome = (TextBlock)this.FindName("txbxNomePD" + numero);
            ComboBox cbBoxTipo = (ComboBox)this.FindName("cbbxTipoPD" + numero + "_" + detector + "_" + numElem);
            ComboBox cbBoxSent = (ComboBox)this.FindName("cbbxSentPD" + numero + "_" + detector + "_" + numElem);
            ComboBox cbBoxNume = (ComboBox)this.FindName("cbbxNumePD" + numero + "_" + detector + "_" + numElem);
            TextBlock txtBlockPort = (TextBlock)this.FindName("txtbx_IDPorta" + numero);

            //Habilita e desabilita o sentido, tipo e numero.
            if (cbBoxSent.SelectedIndex > -1)
            {
                cbBoxTipo.IsEnabled = true;
            }

            if (cbBoxTipo.SelectedIndex > -1)
            {
                cbBoxNume.IsEnabled = true;
            }

            if (!string.IsNullOrEmpty(cbBoxTipo.SelectedValue?.ToString()) &&
                !string.IsNullOrEmpty(cbBoxSent.SelectedValue?.ToString()) &&
                !string.IsNullOrEmpty(cbBoxNume.SelectedValue?.ToString()))
            {
                string nome = cbBoxSent.SelectedValue.ToString().Substring(0, 1) +
                              cbBoxTipo.SelectedValue.ToString().Substring(0, 1) +                              
                              cbBoxNume.SelectedValue.ToString();
                string id = numero +
                            (cbBoxSent.SelectedIndex + 1).ToString() +
                            (cbBoxTipo.SelectedIndex + 1).ToString() +
                            (cbBoxNume.SelectedIndex + 1).ToString();

                txtBlockNome.Text = nome;
                txtBlockPort.Text = $"ID: {id}";
            }
        }

        public void PreencherComboBoxes(string nome, string numero, string detector, string numElem)
        {
            // Gerar o nome dinâmico para os ComboBoxes
            string nomeSent = "cbbxSentPD" + numero + "_" + detector + "_" + numElem;
            string nomeTipo = "cbbxTipoPD" + numero + "_" + detector + "_" + numElem;
            string nomeNume = "cbbxNumePD" + numero + "_" + detector + "_" + numElem;

            // Usar reflexão para acessar os ComboBoxes dinamicamente
            ComboBox cbbxSent = (ComboBox)this.FindName(nomeSent);
            ComboBox cbbxTipo = (ComboBox)this.FindName(nomeTipo);
            ComboBox cbbxNume = (ComboBox)this.FindName(nomeNume);

            if (cbbxSent != null && cbbxTipo != null && cbbxNume != null)
            {
                string letraSent = nome.Substring(0, 1);
                string letraTipo = nome.Substring(1, 1);
                string numeroBusca = nome.Substring(2);

                // Preencher os ComboBoxes de acordo com o nome
                PreencherComboBoxDinamico(cbbxSent, letraSent);
                PreencherComboBoxDinamico(cbbxTipo, letraTipo);
                PreencherComboBoxDinamico(cbbxNume, numeroBusca);
            }
        }


        private void PreencherComboBoxDinamico(ComboBox comboBox, string valor)
        {
            foreach (var item in comboBox.Items)
            {
                if (item.ToString().StartsWith(valor, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void ckbx_unchecked(object sender, RoutedEventArgs e)
        {
            setaCamposFaixas((CheckBox)sender);
        }
    }
}

