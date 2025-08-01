using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AnsitraPLC_QR_MIT.Model
{
    public class Estacao : INotifyPropertyChanged
    {
        private string _ipPlc;
        private string _ipGateWay;
        private string _ipMascara;
        private string _ipModulo;
        private string _gatewayModulo;
        private string _maskModulo;
        private string _tipo;
        private string _tipoCC;
        private string _nomeRodovia;
        private string _idEstacao;
        private string _nomeEstacao;
        private string _numEstacao;
        private string _rodovia;
        private string _km;
        private string _local;
        private string _ra;
        private string _estado;
        private string _comprimento;
        private string _compMaior;
        private string _compMaiorGravar;
        private string _compMenor;
        private string _compMenorGravar;
        private string _velocMaior;
        private string _velocMaiorGravar;
        private string _velocMenor;
        private string _velocMenorGravar;

        public List<string> NomesFaixas { get; set; } = new List<string>();
        public List<string> ComprimentoFaixas { get; set; } = new List<string>();
        public List<string> DistanciaFaixas { get; set; } = new List<string>();

        public string IpPlc
        {
            get => _ipPlc;
            set
            {
                _ipPlc = value;
                OnPropertyChanged(nameof(IpPlc));
            }
        }

        public string IpGateWay
        {
            get => _ipGateWay;
            set
            {
                _ipGateWay = value;
                OnPropertyChanged(nameof(IpGateWay));
            }
        }

        public string IpMascara
        {
            get => _ipMascara;
            set
            {
                _ipMascara = value;
                OnPropertyChanged(nameof(IpMascara));
            }
        }
        public string IpModulo
        {
            get => _ipModulo;
            set { _ipModulo = value; OnPropertyChanged(nameof(IpModulo)); }
        }

        public string MaskModulo
        {
            get => _maskModulo;
            set { _maskModulo = value; OnPropertyChanged(nameof(MaskModulo)); }
        }

        public string GatewayModulo
        {
            get => _gatewayModulo;
            set { _gatewayModulo = value; OnPropertyChanged(nameof(GatewayModulo)); }
        }


        public string Tipo
        {
            get => _tipo;
            set
            {
                _tipo = value;
                OnPropertyChanged(nameof(Tipo));
            }
        }

        public string Tipo_CC
        {
            get => _tipoCC;
            set
            {
                _tipoCC = value;
                OnPropertyChanged(nameof(Tipo_CC));
            }
        }

        public string NomeRodovia
        {
            get => _nomeRodovia;
            set
            {
                _nomeRodovia = value;
                OnPropertyChanged(nameof(NomeRodovia));
            }
        }

        public string IdEstacao
        {
            get => _idEstacao;
            set
            {
                _idEstacao = value;
                OnPropertyChanged(nameof(IdEstacao));
            }
        }

        public string NomeEstacao
        {
            get => _nomeEstacao;
            set
            {
                _nomeEstacao = value;
                OnPropertyChanged(nameof(NomeEstacao));
            }
        }

        public string NumEstacao
        {
            get => _numEstacao;
            set
            {
                _numEstacao = value;
                OnPropertyChanged(nameof(NumEstacao));
            }
        }

        public string Rodovia
        {
            get => _rodovia;
            set
            {
                _rodovia = value;
                OnPropertyChanged(nameof(Rodovia));
            }
        }

        public string Km
        {
            get => _km;
            set
            {
                _km = value;
                OnPropertyChanged(nameof(Km));
            }
        }

        public string Local
        {
            get => _local;
            set
            {
                _local = value;
                OnPropertyChanged(nameof(Local));
            }
        }

        public string Ra
        {
            get => _ra;
            set
            {
                _ra = value;
                OnPropertyChanged(nameof(Ra));
            }
        }

        public string Estado
        {
            get => _estado;
            set
            {
                _estado = value;
                OnPropertyChanged(nameof(Estado));
            }
        }

        public string CompMaior
        {
            get => _compMaior;
            set
            {
                _compMaior = value;
                OnPropertyChanged(nameof(CompMaior));
            }
        }

        public string CompMaiorGravar
        {
            get => _compMaiorGravar;
            set
            {
                _compMaiorGravar = value;
                OnPropertyChanged(nameof(CompMaiorGravar));
            }
        }

        public string CompMenor
        {
            get => _compMenor;
            set
            {
                _compMenor = value;
                OnPropertyChanged(nameof(CompMenor));
            }
        }

        public string CompMenorGravar
        {
            get => _compMenorGravar;
            set
            {
                _compMenorGravar = value;
                OnPropertyChanged(nameof(CompMenorGravar));
            }
        }

        public string VelocMaior
        {
            get => _velocMaior;
            set
            {
                _velocMaior = value;
                OnPropertyChanged(nameof(VelocMaior));
            }
        }

        public string VelocMaiorGravar
        {
            get => _velocMaiorGravar;
            set
            {
                _velocMaiorGravar = value;
                OnPropertyChanged(nameof(VelocMaiorGravar));
            }
        }

        public string VelocMenor
        {
            get => _velocMenor;
            set
            {
                _velocMenor = value;
                OnPropertyChanged(nameof(VelocMenor));
            }
        }

        public string VelocMenorGravar
        {
            get => _velocMenorGravar;
            set
            {
                _velocMenorGravar = value;
                OnPropertyChanged(nameof(VelocMenorGravar));
            }
        }

        private string _tipo_banco;
        private string _host_banco;
        private string _usuario_banco;
        private string _porta_banco;
        private string _exibicao_banco;
        private string _servidor_banco;
        private string _senha_banco;

        public string tipo_banco
        {
            get => _tipo_banco;
            set { _tipo_banco = value; OnPropertyChanged(nameof(tipo_banco)); }
        }

        public string host_banco
        {
            get => _host_banco;
            set { _host_banco = value; OnPropertyChanged(nameof(host_banco)); }
        }

        public string usuario_banco
        {
            get => _usuario_banco;
            set { _usuario_banco = value; OnPropertyChanged(nameof(usuario_banco)); }
        }

        public string porta_banco
        {
            get => _porta_banco;
            set { _porta_banco = value; OnPropertyChanged(nameof(porta_banco)); }
        }

        public string exibicao_banco
        {
            get => _exibicao_banco;
            set { _exibicao_banco = value; OnPropertyChanged(nameof(exibicao_banco)); }
        }

        public string servidor_banco
        {
            get => _servidor_banco;
            set
            {
                _servidor_banco = value;
                servico_banco = value;
                OnPropertyChanged(nameof(servidor_banco));
            }
        }

        public string senha_banco
        {
            get => _senha_banco;
            set { _senha_banco = value; OnPropertyChanged(nameof(senha_banco)); }
        }

        public string servico_banco
        {
            get => servidor_banco;
            set
            {
                if (servidor_banco != value)
                {
                    servidor_banco = value;
                    OnPropertyChanged(nameof(servico_banco));
                }
            }
        }

        // Implementação do PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
