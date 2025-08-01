using Microsoft.UI;
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
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace AnsitraPLC_QR_MIT
{
    public sealed partial class ContagemPage : Page
    {
        public ContagemPage()
        {
            this.InitializeComponent();

            var mainViewModel = App.MainViewModel;
            DataContext = mainViewModel;
        }

        // Método chamado quando o CheckBox "Select All" é marcado
        private void SelectAllPistas_Checked(object sender, RoutedEventArgs e)
        {
            nome_pista1.IsChecked = true;
            nome_pista2.IsChecked = true;
            nome_pista3.IsChecked = true;
            nome_pista4.IsChecked = true;
            nome_pista5.IsChecked = true;
            nome_pista6.IsChecked = true;
            nome_pista7.IsChecked = true;
            nome_pista8.IsChecked = true;
            nome_pista9.IsChecked = true;
            nome_pista10.IsChecked = true;
            nome_pista11.IsChecked = true;
            nome_pista12.IsChecked = true;
            nome_pista13.IsChecked = true;
            nome_pista14.IsChecked = true;
            nome_pista15.IsChecked = true;
            nome_pista16.IsChecked = true;
        }

        // Método chamado quando o CheckBox "Select All" é desmarcado
        private void SelectAllPistas_Unchecked(object sender, RoutedEventArgs e)
        {
            nome_pista1.IsChecked = false;
            nome_pista2.IsChecked = false;
            nome_pista3.IsChecked = false;
            nome_pista4.IsChecked = false;
            nome_pista5.IsChecked = false;
            nome_pista6.IsChecked = false;
            nome_pista7.IsChecked = false;
            nome_pista8.IsChecked = false;
            nome_pista9.IsChecked = false;
            nome_pista10.IsChecked = false;
            nome_pista11.IsChecked = false;
            nome_pista12.IsChecked = false;
            nome_pista13.IsChecked = false;
            nome_pista14.IsChecked = false;
            nome_pista15.IsChecked = false;
            nome_pista16.IsChecked = false;
        }
        private void btn_selecPistas_Click(object sender, RoutedEventArgs e)
        {
            VerificarCheckBox();
        }
        private void VerificarCheckBox()
        {
            CheckBox[] checkBoxes = { nome_pista1, nome_pista2, nome_pista3, nome_pista4, nome_pista5, nome_pista6, nome_pista7, nome_pista8, nome_pista9, nome_pista10, nome_pista11, nome_pista12, nome_pista13, nome_pista14, nome_pista15, nome_pista16};

            foreach (var checkBox in checkBoxes)
            {
                checkBox.Visibility = string.IsNullOrEmpty(checkBox.Content?.ToString())
                                      ? Visibility.Collapsed
                                      : Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtbx_pista1.Text = string.Empty;
            txtbx_pista2.Text = string.Empty;
            txtbx_pista3.Text = string.Empty;
            txtbx_pista4.Text = string.Empty;
            txtbx_pista5.Text = string.Empty;
            txtbx_pista6.Text = string.Empty;
            txtbx_pista7.Text = string.Empty;
            txtbx_pista8.Text = string.Empty;
            txtbx_pista9.Text = string.Empty;
            txtbx_pista10.Text = string.Empty;
            txtbx_pista11.Text = string.Empty;
            txtbx_pista12.Text = string.Empty;
            txtbx_pista13.Text = string.Empty;
            txtbx_pista14.Text = string.Empty;
            txtbx_pista15.Text = string.Empty;
            txtbl_caminhao.Text = string.Empty;
            txtbl_carros.Text = string.Empty;
            txtbl_motos.Text = string.Empty;
            txtbl_total.Text = string.Empty;
        }
    }
}
