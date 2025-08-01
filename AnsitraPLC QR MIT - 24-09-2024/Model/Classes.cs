using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace AnsitraPLC_QR_MIT.Model
{
    public class ClasseVeiculo : INotifyPropertyChanged
    {
        private string _tipo;
        private string _veiculo;
        private string _descricao;
        private string _rangeInicial;
        private string _rangeFinal;
        private string _qtdEixos;

        public string Tipo
        {
            get => _tipo;
            set
            {
                _tipo = value;
                OnPropertyChanged(nameof(Tipo));
            }
        }

        public string Veiculo
        {
            get => _veiculo;
            set
            {
                _veiculo = value;
                OnPropertyChanged(nameof(Veiculo));
            }
        }

        public string Descricao
        {
            get => _descricao;
            set
            {
                _descricao = value;
                OnPropertyChanged(nameof(Descricao));
            }
        }

        public string RangeInicial
        {
            get => _rangeInicial;
            set
            {
                _rangeInicial = value;
                OnPropertyChanged(nameof(RangeInicial));
            }
        }

        public string RangeFinal
        {
            get => _rangeFinal;
            set
            {
                _rangeFinal = value;
                OnPropertyChanged(nameof(RangeFinal));
            }
        }

        public string QtdEixos
        {
            get => _qtdEixos;
            set
            {
                _qtdEixos = value;
                OnPropertyChanged(nameof(QtdEixos));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
