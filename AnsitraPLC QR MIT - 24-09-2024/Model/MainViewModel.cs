using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Dispatching;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;
using AnsitraPLC_QR_MIT.Model;

namespace AnsitraPLC_QR_MIT
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private int numeroPassagem;

        public int contagemPassagem
        {
            get => numeroPassagem;
            set => numeroPassagem = value;
        }

        private Estacao _estacaoSelecionada = new Estacao();


        public Estacao EstacaoSelecionada
        {
            get => _estacaoSelecionada;
            set
            {
                _estacaoSelecionada = value;
                OnPropertyChanged(nameof(EstacaoSelecionada));
            }
        }

        public ObservableCollection<string> TiposBanco { get; } =
            new ObservableCollection<string> { "Oracle", "SQLServer" };

        public ObservableCollection<string> ModosExibicao { get; } =
            new ObservableCollection<string> { "Mod 1", "Mod 2", "Mod 3" };


        private ObservableCollection<string> _listaTipos = new ObservableCollection<string>
{
            "MOTO", "AUTO", "COMERCIAL"
        };

        public ObservableCollection<string> ListaTipos
        {
            get => _listaTipos;
            set
            {
                _listaTipos = value;
                OnPropertyChanged(nameof(ListaTipos));
            }
        }

        private ObservableCollection<Produto> produtos;
        public ObservableCollection<Produto> Produtos
        {
            get { return produtos; }
            set
            {
                produtos = value;
                OnPropertyChanged(nameof(Produtos));
            }
        }

        public ObservableCollection<string> RodoviasDisponiveis { get; set; }
        public ObservableCollection<string> LocalDisponiveis { get; set; }
        public ObservableCollection<string> Ufs { get; set; }

        public MainViewModel()
        {
            Produtos = new ObservableCollection<Produto> { };

            // Inicializa RodoviasDisponiveis
            RodoviasDisponiveis = new ObservableCollection<string>();
            for (int i = 1; i <= 99; i++)
            {
                RodoviasDisponiveis.Add(i.ToString("D2"));
            }

            LocalDisponiveis = new ObservableCollection<string>
            {
                "Pista Norte",
                "Pista Sul",
                "Pista Leste",
                "Portico",
                "Pista Oeste",
                "Pista Externa",
                "Pista Interna",
                "Canteiro Central"
            };

            Ufs = new ObservableCollection<string>
            {
                "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
                "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
                "RJ", "RN", "RS", "RO", "RR", "SC", "SE", "SP", "TO"
            };

        }

        public void LerConfigPLC(string dados)
        {
            Debug.WriteLine(dados);
            Debug.WriteLine("Leu dos dados");
        }

        private Dictionary<string, int> _quantidadePorClasse = new();

        private int _QuantidadeComercial = 0;
        private int _QuantidadeTotal = 0;

        public void ProcessarDadoMonitoramento(string dados)
        {
            try
            {
                string[] partes = dados.Split(';');

                string numero = contagemPassagem.ToString();
                string dataCompleta = partes[1];
                string horaStr = partes[2];
                string faixa = partes[6];
                string velocidade = partes[17];
                string comprimento = partes[18];
                string cc = partes[19];
                string ocupacaoM = partes[20];
                string gapM = partes[21];
                string gapS = partes[22];
                string headway = partes[23];
                string inter = partes[15];
                string ocupacaoL = partes[16];
                string acionamento = partes[24];
                string qtdEixos = partes[25];
                string grpEixos = "0";
                string classDNIT = "0";
                string pesoEx1 = "0";
                string pesoEx2 = "0";
                string pesoEx3 = "0";
                string pesoEx4 = "0";
                string pesoEx5 = "0";
                string pesoEx6 = "0";
                string pesoEx7 = "0";
                string pesoEx8 = "0";
                string pesoEx9 = "0";
                string pesoEx10 = "0";
                string pesototal = "0";
                
                if (!int.TryParse(horaStr.Split(':')[0], out int hora))
                {
                    Debug.WriteLine($"Erro ao converter hora: {horaStr}");
                    return;
                }

                //Formata a data
                DateTime.TryParse(dataCompleta, out DateTime dataConvertida);
                string dataFormatada = dataConvertida.ToString("dd/MM");

                //Formata a ocupacao
                if (ocupacaoM.Contains("."))
                {
                    string[] ocupacaoPartes = ocupacaoM.Split('.');
                    if (ocupacaoPartes.Length > 1 && ocupacaoPartes[1].Length >= 2)
                    {
                        ocupacaoM = ocupacaoPartes[0] + "." + ocupacaoPartes[1].Substring(0, 2);
                    }
                }

                //Adiciona o produto na tabela
                Produtos.Add(new Produto { Numero = numero, Data = dataFormatada, Hora = hora, Faixa = faixa, Velocidade = velocidade, Comprimento = comprimento, CC = cc, OcupacaoM = ocupacaoM, GapM = gapM, GapS = gapS, Headway = headway, Inter = inter, OcupacaoL = ocupacaoL, Acionamento = acionamento, QtdEixo = qtdEixos, GrpEixo = grpEixos, ClassDNIT = classDNIT, PesoEx1 = pesoEx1, PesoEx2 = pesoEx2, PesoEx3 = pesoEx3, PesoEx4 = pesoEx4, PesoEx5 = pesoEx5, PesoEx6 = pesoEx6, PesoEx7 = pesoEx7, PesoEx8 = pesoEx8, PesoEx9 = pesoEx9, PesoEx10 = pesoEx10, PesoTotal = pesototal, });
                contagemPassagem++;

                if (!string.IsNullOrWhiteSpace(cc) && int.TryParse(cc, out int classeInt) && classeInt >= 1 && classeInt <= 15)
                {
                    // Incrementa contagem
                    if (_quantidadePorClasse.ContainsKey(cc))
                        _quantidadePorClasse[cc]++;
                    else
                        _quantidadePorClasse[cc] = 1;

                    // Atualiza a propriedade correspondente
                    string propName = $"QuantidadeClass{classeInt}";
                    string valor = _quantidadePorClasse[cc].ToString();

                    // Usa reflexão para setar a propriedade correta
                    var prop = this.GetType().GetProperty(propName);
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(this, valor);
                    }
                }

                _QuantidadeTotal++;

                QuantidadeComercial = _QuantidadeComercial.ToString();
                QuantidadeTotal = _QuantidadeTotal.ToString();

                // Exibir a lista completa após adicionar um novo item
                Debug.WriteLine("Lista completa de produtos:");
                foreach (var produto in Produtos)
                {
                    Debug.WriteLine($"Número: {produto.Numero}, Data: {produto.Data}, Hora: {produto.Hora}, Faixa: {produto.Faixa}, Velocidade: {produto.Velocidade}");
                }
                Debug.WriteLine("---------------------------------");
                               
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Debug.WriteLine($"COMException: {ex.Message}, HRESULT: {ex.HResult}");
            }
        }


        //Altera campos do servidor
        private string _idServidor;

        public string IdServidor
        {
            get => _idServidor;
            set
            {
                _idServidor = value;
                OnPropertyChanged(nameof(IdServidor));
            }
        }

        private string _textoDoTextBox;
        public string TextoDoTextBox
        {
            get => _textoDoTextBox;
            set
            {
                _textoDoTextBox = value;
                OnPropertyChanged(nameof(TextoDoTextBox));
            }
        }
        

        // Propriedades do Sensor2
        private string _ipPlc2;
        private string _maskPlc2;
        private string _gatewayPlc2;
        private string _ipSensor2;
        private string _maskSensor2;
        private string _gatewaySensor2;

        // Propriedades do PLC2
        public string IpPlc2
        {
            get => _ipPlc2;
            set
            {
                if (_ipPlc2 != value)
                {
                    _ipPlc2 = value;
                    OnPropertyChanged(nameof(IpPlc2)); // Notifica a mudança
                }
            }
        }

        public string MaskPlc2
        {
            get => _maskPlc2;
            set
            {
                if (_maskPlc2 != value)
                {
                    _maskPlc2 = value;
                    OnPropertyChanged(nameof(MaskPlc2)); // Notifica a mudança
                }
            }
        }

        public string GatewayPlc2
        {
            get => _gatewayPlc2;
            set
            {
                if (_gatewayPlc2 != value)
                {
                    _gatewayPlc2 = value;
                    OnPropertyChanged(nameof(GatewayPlc2)); // Notifica a mudança
                }
            }
        }

        public string IpSensor2
        {
            get => _ipSensor2;
            set
            {
                if (_ipSensor2 != value)
                {
                    _ipSensor2 = value;
                    OnPropertyChanged(nameof(IpSensor2)); // Notifica a mudança
                }
            }
        }

        public string MaskSensor2
        {
            get => _maskSensor2;
            set
            {
                if (_maskSensor2 != value)
                {
                    _maskSensor2 = value;
                    OnPropertyChanged(nameof(MaskSensor2)); // Notifica a mudança
                }
            }
        }


        public string GatewaySensor2
        {
            get => _gatewaySensor2;
            set
            {
                if (_gatewaySensor2 != value)
                {
                    _gatewaySensor2 = value;
                    OnPropertyChanged(nameof(GatewaySensor2)); // Notifica a mudança
                }
            }
        }

        private string _nomeFaixa1;
        private string _nomeFaixa2;
        private string _nomeFaixa3;
        private string _nomeFaixa4;
        private string _nomeFaixa5;
        private string _nomeFaixa6;
        private string _nomeFaixa7;
        private string _nomeFaixa8;
        private string _nomeFaixa9;
        private string _nomeFaixa10;
        private string _nomeFaixa11;
        private string _nomeFaixa12;
        private string _nomeFaixa13;
        private string _nomeFaixa14;
        private string _nomeFaixa15;
        private string _nomeFaixa16;



        public string NomeFaixa1
        {
            get => _nomeFaixa1;
            set
            {
                _nomeFaixa1 = value;
                OnPropertyChanged(nameof(NomeFaixa1));
            }
        }
        public string NomeFaixa2
        {
            get => _nomeFaixa2;
            set
            {
                _nomeFaixa2 = value;
                OnPropertyChanged(nameof(NomeFaixa2));
            }
        }
        public string NomeFaixa3
        {
            get => _nomeFaixa3;
            set
            {
                _nomeFaixa3 = value;
                OnPropertyChanged(nameof(NomeFaixa3));
            }
        }
        public string NomeFaixa4
        {
            get => _nomeFaixa4;
            set
            {
                _nomeFaixa4 = value;
                OnPropertyChanged(nameof(NomeFaixa4));
            }
        }

        public string NomeFaixa5
        {
            get => _nomeFaixa5;
            set
            {
                _nomeFaixa5 = value;
                OnPropertyChanged(nameof(NomeFaixa5));
            }
        }

        public string NomeFaixa6
        {
            get => _nomeFaixa6;
            set
            {
                _nomeFaixa6 = value;
                OnPropertyChanged(nameof(NomeFaixa6));
            }
        }

        public string NomeFaixa7
        {
            get => _nomeFaixa7;
            set
            {
                _nomeFaixa7 = value;
                OnPropertyChanged(nameof(NomeFaixa7));
            }
        }

        public string NomeFaixa8
        {
            get => _nomeFaixa8;
            set
            {
                _nomeFaixa8 = value;
                OnPropertyChanged(nameof(NomeFaixa8));
            }
        }

        public string NomeFaixa9
        {
            get => _nomeFaixa9;
            set
            {
                _nomeFaixa9 = value;
                OnPropertyChanged(nameof(NomeFaixa9));
            }
        }

        public string NomeFaixa10
        {
            get => _nomeFaixa10;
            set
            {
                _nomeFaixa10= value;
                OnPropertyChanged(nameof(NomeFaixa10));
            }
        }

        public string NomeFaixa11
        {
            get => _nomeFaixa11;
            set
            {
                _nomeFaixa11 = value;
                OnPropertyChanged(nameof(NomeFaixa11));
            }
        }

        public string NomeFaixa12
        {
            get => _nomeFaixa12;
            set
            {
                _nomeFaixa12 = value;
                OnPropertyChanged(nameof(NomeFaixa12));
            }
        }

        public string NomeFaixa13
        {
            get => _nomeFaixa13;
            set
            {
                _nomeFaixa13 = value;
                OnPropertyChanged(nameof(NomeFaixa13));
            }
        }

        public string NomeFaixa14
        {
            get => _nomeFaixa14;
            set
            {
                _nomeFaixa14 = value;
                OnPropertyChanged(nameof(NomeFaixa14));
            }
        }

        public string NomeFaixa15
        {
            get => _nomeFaixa15;
            set
            {
                _nomeFaixa15 = value;
                OnPropertyChanged(nameof(NomeFaixa15));
            }
        }

        public string NomeFaixa16
        {
            get => _nomeFaixa16;
            set
            {
                _nomeFaixa16 = value;
                OnPropertyChanged(nameof(NomeFaixa16));
            }
        }


        private string _quantidadeClass1;
        private string _quantidadeClass2;
        private string _quantidadeClass3;
        private string _quantidadeClass4;
        private string _quantidadeClass5;
        private string _quantidadeClass6;
        private string _quantidadeClass7;
        private string _quantidadeClass8;
        private string _quantidadeClass9;
        private string _quantidadeClass10;
        private string _quantidadeClass11;
        private string _quantidadeClass12;
        private string _quantidadeClass13;
        private string _quantidadeClass14;
        private string _quantidadeClass15;

        private string _quantidadeComercial;
        private string _quantidadeTotal;


        public string QuantidadeClass1
        {
            get => _quantidadeClass1;
            set
            {
                _quantidadeClass1 = value;
                OnPropertyChanged(nameof(QuantidadeClass1));
            }
        }

        public string QuantidadeClass2
        {
            get => _quantidadeClass2;
            set
            {
                _quantidadeClass2 = value;
                OnPropertyChanged(nameof(QuantidadeClass2));
            }
        }

        public string QuantidadeClass3
        {
            get => _quantidadeClass3;
            set
            {
                _quantidadeClass3 = value;
                OnPropertyChanged(nameof(QuantidadeClass3));
            }
        }

        public string QuantidadeClass4
        {
            get => _quantidadeClass4;
            set
            {
                _quantidadeClass4 = value;
                OnPropertyChanged(nameof(QuantidadeClass4));
            }
        }

        public string QuantidadeClass5
        {
            get => _quantidadeClass5;
            set
            {
                _quantidadeClass5 = value;
                OnPropertyChanged(nameof(QuantidadeClass5));
            }
        }

        public string QuantidadeClass6
        {
            get => _quantidadeClass6;
            set
            {
                _quantidadeClass6 = value;
                OnPropertyChanged(nameof(QuantidadeClass6));
            }
        }

        public string QuantidadeClass7
        {
            get => _quantidadeClass7;
            set
            {
                _quantidadeClass7 = value;
                OnPropertyChanged(nameof(QuantidadeClass7));
            }
        }

        public string QuantidadeClass8
        {
            get => _quantidadeClass8;
            set
            {
                _quantidadeClass8 = value;
                OnPropertyChanged(nameof(QuantidadeClass8));
            }
        }

        public string QuantidadeClass9
        {
            get => _quantidadeClass9;
            set
            {
                _quantidadeClass9 = value;
                OnPropertyChanged(nameof(QuantidadeClass9));
            }
        }

        public string QuantidadeClass10
        {
            get => _quantidadeClass10;
            set
            {
                _quantidadeClass10 = value;
                OnPropertyChanged(nameof(QuantidadeClass10));
            }
        }

        public string QuantidadeClass11
        {
            get => _quantidadeClass11;
            set
            {
                _quantidadeClass11 = value;
                OnPropertyChanged(nameof(QuantidadeClass11));
            }
        }

        public string QuantidadeClass12
        {
            get => _quantidadeClass12;
            set
            {
                _quantidadeClass12 = value;
                OnPropertyChanged(nameof(QuantidadeClass12));
            }
        }

        public string QuantidadeClass13
        {
            get => _quantidadeClass13;
            set
            {
                _quantidadeClass13 = value;
                OnPropertyChanged(nameof(QuantidadeClass13));
            }
        }

        public string QuantidadeClass14
        {
            get => _quantidadeClass14;
            set
            {
                _quantidadeClass14 = value;
                OnPropertyChanged(nameof(QuantidadeClass14));
            }
        }

        public string QuantidadeClass15
        {
            get => _quantidadeClass15;
            set
            {
                _quantidadeClass15 = value;
                OnPropertyChanged(nameof(QuantidadeClass5));
            }
        }

        public string QuantidadeComercial
        {
            get => _quantidadeComercial;
            set
            {
                _quantidadeComercial = value;
                OnPropertyChanged(nameof(QuantidadeComercial));
            }
        }

        public string QuantidadeTotal
        {
            get => _quantidadeTotal;
            set
            {
                _quantidadeTotal = value;
                OnPropertyChanged(nameof(QuantidadeTotal));
            }
        }

        // Propriedade habilitar/desabilitar TextBox.

        private string _selectedItem1;
        private string _selectedItem2;
        private string _selectedItem3;
        private string _selectedItem4;
        private string _selectedItem5;
        private string _selectedItem6;
        private string _selectedItem7;
        private string _selectedItem8;
        private string _selectedItem9;
        private string _selectedItem10;
        private string _selectedItem11;
        private string _selectedItem12;
        private string _selectedItem13;
        private string _selectedItem14;
        private string _selectedItem15;
        private string _selectedItem16;
        public string SelectedItem1
        {
            get => _selectedItem1;
            set
            {
                _selectedItem1 = value;
                OnPropertyChanged(nameof(CompLaco1Enabled));
            }
        }

        public string SelectedItem2
        {
            get => _selectedItem2;
            set
            {
                _selectedItem2 = value;
                OnPropertyChanged(nameof(CompLaco2Enabled)); 
            }
        }

        public string SelectedItem3
        {
            get => _selectedItem3;
            set
            {
                _selectedItem3 = value;
                OnPropertyChanged(nameof(CompLaco3Enabled));
            }
        }        
        
        public string SelectedItem4
        {
            get => _selectedItem4;
            set
            {
                _selectedItem4 = value;
                OnPropertyChanged(nameof(CompLaco4Enabled));
            }
        }        
        
        public string SelectedItem5
        {
            get => _selectedItem5;
            set
            {
                _selectedItem5 = value;
                OnPropertyChanged(nameof(CompLaco5Enabled));
            }
        }        
        
        public string SelectedItem6
        {
            get => _selectedItem6;
            set
            {
                _selectedItem6 = value;
                OnPropertyChanged(nameof(CompLaco6Enabled));
            }
        }        
        
        public string SelectedItem7
        {
            get => _selectedItem7;
            set
            {
                _selectedItem7 = value;
                OnPropertyChanged(nameof(CompLaco7Enabled));
            }
        }        
        
        public string SelectedItem8
        {
            get => _selectedItem8;
            set
            {
                _selectedItem8 = value;
                OnPropertyChanged(nameof(CompLaco8Enabled));
            }
        }        
        
        public string SelectedItem9
        {
            get => _selectedItem9;
            set
            {
                _selectedItem9 = value;
                OnPropertyChanged(nameof(CompLaco9Enabled));
            }
        }        
        
        public string SelectedItem10
        {
            get => _selectedItem10;
            set
            {
                _selectedItem10 = value;
                OnPropertyChanged(nameof(CompLaco10Enabled));
            }
        }        
        
        public string SelectedItem11
        {
            get => _selectedItem11;
            set
            {
                _selectedItem11 = value;
                OnPropertyChanged(nameof(CompLaco11Enabled));
            }
        }        
        
        public string SelectedItem12
        {
            get => _selectedItem12;
            set
            {
                _selectedItem12 = value;
                OnPropertyChanged(nameof(CompLaco12Enabled));
            }
        }        
        
        public string SelectedItem13
        {
            get => _selectedItem13;
            set
            {
                _selectedItem13 = value;
                OnPropertyChanged(nameof(CompLaco13Enabled));
            }
        }        
        
        public string SelectedItem14
        {
            get => _selectedItem14;
            set
            {
                _selectedItem14 = value;
                OnPropertyChanged(nameof(CompLaco14Enabled));
            }
        }        
        
        public string SelectedItem15
        {
            get => _selectedItem15;
            set
            {
                _selectedItem15 = value;
                OnPropertyChanged(nameof(CompLaco15Enabled));
            }
        }        
        
        public string SelectedItem16
        {
            get => _selectedItem16;
            set
            {
                _selectedItem16 = value;
                OnPropertyChanged(nameof(CompLaco16Enabled));
            }
        }

        private string _distLaco1;
        private string _distLaco2;
        private string _distLaco3;
        private string _distLaco4;
        private string _distLaco5;
        private string _distLaco6;
        private string _distLaco7;
        private string _distLaco8;
        private string _distLaco9;
        private string _distLaco10;
        private string _distLaco11;
        private string _distLaco12;
        private string _distLaco13;
        private string _distLaco14;
        private string _distLaco15;
        private string _distLaco16;


        public string DistLaco1
        {
            get => _distLaco1;
            set
            {
                _distLaco1 = value;
                OnPropertyChanged(nameof(DistLaco1));
            }
        }

        public string DistLaco2
        {
            get => _distLaco2;
            set
            {
                _distLaco2= value;
                OnPropertyChanged(nameof(DistLaco2));
            }
        }

        public string DistLaco3
        {
            get => _distLaco3;
            set
            {
                _distLaco3 = value;
                OnPropertyChanged(nameof(DistLaco3));
            }
        }

        public string DistLaco4
        {
            get => _distLaco4;
            set
            {
                _distLaco4 = value;
                OnPropertyChanged(nameof(DistLaco4));
            }
        }

        public string DistLaco5
        {
            get => _distLaco5;
            set
            {
                _distLaco5 = value;
                OnPropertyChanged(nameof(DistLaco5));
            }
        }

        public string DistLaco6
        {
            get => _distLaco6;
            set
            {
                _distLaco6 = value;
                OnPropertyChanged(nameof(DistLaco6));
            }
        }

        public string DistLaco7
        {
            get => _distLaco7;
            set
            {
                _distLaco7 = value;
                OnPropertyChanged(nameof(DistLaco7));
            }
        }

        public string DistLaco8
        {
            get => _distLaco8;
            set
            {
                _distLaco8 = value;
                OnPropertyChanged(nameof(DistLaco8));
            }
        }

        public string DistLaco9
        {
            get => _distLaco9;
            set
            {
                _distLaco9 = value;
                OnPropertyChanged(nameof(DistLaco9));
            }
        }
        public string DistLaco10
        {
            get => _distLaco10;
            set
            {
                _distLaco10 = value;
                OnPropertyChanged(nameof(DistLaco10));
            }
        }

        public string DistLaco11
        {
            get => _distLaco11;
            set
            {
                _distLaco11 = value;
                OnPropertyChanged(nameof(DistLaco11));
            }
        }

        public string DistLaco12
        {
            get => _distLaco12;
            set
            {
                _distLaco12 = value;
                OnPropertyChanged(nameof(DistLaco12));
            }
        }

        public string DistLaco13
        {
            get => _distLaco13;
            set
            {
                _distLaco13 = value;
                OnPropertyChanged(nameof(DistLaco13));
            }
        }

        public string DistLaco14
        {
            get => _distLaco14;
            set
            {
                _distLaco14 = value;
                OnPropertyChanged(nameof(DistLaco14));
            }
        }

        public string DistLaco15
        {
            get => _distLaco15;
            set
            {
                _distLaco15 = value;
                OnPropertyChanged(nameof(DistLaco15));
            }
        }

        public string DistLaco16
        {
            get => _distLaco16;
            set
            {
                _distLaco16 = value;
                OnPropertyChanged(nameof(DistLaco16));
            }
        }

        private string _compLaco1;
        private string _compLaco2;
        private string _compLaco3;
        private string _compLaco4;
        private string _compLaco5;
        private string _compLaco6;
        private string _compLaco7;
        private string _compLaco8;
        private string _compLaco9;
        private string _compLaco10;
        private string _compLaco11;
        private string _compLaco12;
        private string _compLaco13;
        private string _compLaco14;
        private string _compLaco15;
        private string _compLaco16;

        public string CompLaco1
        {
            get => _compLaco1;
            set
            {
                _compLaco1 = value;
                OnPropertyChanged(nameof(CompLaco1));

            }
        }
        
        public string CompLaco2
        {
            get => _compLaco2;
            set
            {
                _compLaco2 = value;
                OnPropertyChanged(nameof(CompLaco2));
            }
        }
        
        public string CompLaco3
        {
            get => _compLaco3;
            set
            {
                _compLaco3 = value;
                OnPropertyChanged(nameof(CompLaco3));
            }
        }
        
        public string CompLaco4
        {
            get => _compLaco4;
            set
            {
                _compLaco4 = value;
                OnPropertyChanged(nameof(CompLaco4));
            }
        }

        public string CompLaco5
        {
            get => _compLaco5;
            set
            {
                _compLaco5 = value;
                OnPropertyChanged(nameof(CompLaco5));
            }
        }

        public string CompLaco6
        {
            get => _compLaco6;
            set
            {
                _compLaco6 = value;
                OnPropertyChanged(nameof(CompLaco6));
            }
        }

        public string CompLaco7
        {
            get => _compLaco7;
            set
            {
                _compLaco7 = value;
                OnPropertyChanged(nameof(CompLaco7));
            }
        }

        public string CompLaco8
        {
            get => _compLaco8;
            set
            {
                _compLaco8 = value;
                OnPropertyChanged(nameof(CompLaco8));
            }
        }

        public string CompLaco9
        {
            get => _compLaco9;
            set
            {
                _compLaco9 = value;
                OnPropertyChanged(nameof(CompLaco9));
            }
        }

        public string CompLaco10
        {
            get => _compLaco10;
            set
            {
                _compLaco10 = value;
                OnPropertyChanged(nameof(CompLaco10));
            }
        }

        public string CompLaco11
        {
            get => _compLaco11;
            set
            {
                _compLaco11 = value;
                OnPropertyChanged(nameof(CompLaco11));
            }
        }

        public string CompLaco12
        {
            get => _compLaco12;
            set
            {
                _compLaco12 = value;
                OnPropertyChanged(nameof(CompLaco12));
            }
        }

        public string CompLaco13
        {
            get => _compLaco13;
            set
            {
                _compLaco13 = value;
                OnPropertyChanged(nameof(CompLaco13));
            }
        }

        public string CompLaco14
        {
            get => _compLaco14;
            set
            {
                _compLaco14 = value;
                OnPropertyChanged(nameof(CompLaco14));
            }
        }

        public string CompLaco15
        {
            get => _compLaco15;
            set
            {
                _compLaco15 = value;
                OnPropertyChanged(nameof(CompLaco15));
            }
        }

        public string CompLaco16
        {
            get => _compLaco16;
            set
            {
                _compLaco16 = value;
                OnPropertyChanged(nameof(CompLaco16));
            }
        }

        private string _compMaior;
        private string _compMaiorGravar;
        private string _compMenor;
        private string _compMenorGravar;
        private string _velocMaior;
        private string _velocMaiorGravar;
        private string _velocMenor;
        private string _velocMenorGravar;

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
        //Pagina Classes

        private string _classTipo1;
        private string _classTipo2;
        private string _classTipo3;
        private string _classTipo4;
        private string _classTipo5;
        private string _classTipo6;
        private string _classTipo7;
        private string _classTipo8;
        private string _classTipo9;
        private string _classTipo10;
        private string _classTipo11;
        private string _classTipo12;
        private string _classTipo13;
        private string _classTipo14;
        private string _classTipo15;
        private string _classTipo16;

        public string ClassTipo1
        {
            get => _classTipo1;
            set
            {
                _classTipo1 = value;
                OnPropertyChanged(nameof(ClassTipo1));
            }
        }

        public string ClassTipo2
        {
            get => _classTipo2;
            set
            {
                _classTipo2 = value;
                OnPropertyChanged(nameof(ClassTipo2));
            }
        }

        public string ClassTipo3
        {
            get => _classTipo3;
            set
            {
                _classTipo3 = value;
                OnPropertyChanged(nameof(ClassTipo3));
            }
        }

        public string ClassTipo4
        {
            get => _classTipo4;
            set
            {
                _classTipo4 = value;
                OnPropertyChanged(nameof(ClassTipo4));
            }
        }
        public string ClassTipo5
        {
            get => _classTipo5;
            set
            {
                _classTipo5 = value;
                OnPropertyChanged(nameof(ClassTipo5));
            }
        }

        public string ClassTipo6
        {
            get => _classTipo6;
            set
            {
                _classTipo6 = value;
                OnPropertyChanged(nameof(ClassTipo6));
            }
        }

        public string ClassTipo7
        {
            get => _classTipo7;
            set
            {
                _classTipo7 = value;
                OnPropertyChanged(nameof(ClassTipo7));
            }
        }

        public string ClassTipo8
        {
            get => _classTipo8;
            set
            {
                _classTipo8 = value;
                OnPropertyChanged(nameof(ClassTipo8));
            }
        }

        public string ClassTipo9
        {
            get => _classTipo9;
            set
            {
                _classTipo9 = value;
                OnPropertyChanged(nameof(ClassTipo9));
            }
        }

        public string ClassTipo10
        {
            get => _classTipo10;
            set
            {
                _classTipo10 = value;
                OnPropertyChanged(nameof(ClassTipo10));
            }
        }

        public string ClassTipo11
        {
            get => _classTipo11;
            set
            {
                _classTipo11 = value;
                OnPropertyChanged(nameof(ClassTipo11));
            }
        }

        public string ClassTipo12
        {
            get => _classTipo12;
            set
            {
                _classTipo12 = value;
                OnPropertyChanged(nameof(ClassTipo12));
            }
        }

        public string ClassTipo13
        {
            get => _classTipo13;
            set
            {
                _classTipo13 = value;
                OnPropertyChanged(nameof(ClassTipo13));
            }
        }

        public string ClassTipo14
        {
            get => _classTipo14;
            set
            {
                _classTipo14 = value;
                OnPropertyChanged(nameof(ClassTipo14));
            }
        }

        public string ClassTipo15
        {
            get => _classTipo15;
            set
            {
                _classTipo15 = value;
                OnPropertyChanged(nameof(ClassTipo15));
            }
        }

        public string ClassTipo16
        {
            get => _classTipo16;
            set
            {
                _classTipo16 = value;
                OnPropertyChanged(nameof(ClassTipo16));
            }
        }

        private string _classNome1;
        private string _classNome2;
        private string _classNome3;
        private string _classNome4;
        private string _classNome5;
        private string _classNome6;
        private string _classNome7;
        private string _classNome8;
        private string _classNome9;
        private string _classNome10;
        private string _classNome11;
        private string _classNome12;
        private string _classNome13;
        private string _classNome14;
        private string _classNome15;


        public string ClassNome1
        {
            get => _classNome1;
            set
            {
                _classNome1 = value;
                OnPropertyChanged(nameof(ClassNome1));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo1 = null;
                }
            }
        }

        public string ClassNome2
        {
            get => _classNome2;
            set
            {
                _classNome2 = value;
                OnPropertyChanged(nameof(ClassNome2));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo2 = null;
                }
            }
        }
        
        public string ClassNome3
        {
            get => _classNome3;
            set
            {
                _classNome3 = value;
                OnPropertyChanged(nameof(ClassNome3));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo3 = null;
                }
            }
        }
        
        public string ClassNome4
        {
            get => _classNome4;
            set
            {
                _classNome4 = value;
                OnPropertyChanged(nameof(ClassNome4));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo4 = null;
                }
            }
        }
        
        public string ClassNome5
        {
            get => _classNome5;
            set
            {
                _classNome5 = value;
                OnPropertyChanged(nameof(ClassNome5));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo5 = null;
                }
            }
        }

        public string ClassNome6
        {
            get => _classNome6;
            set
            {
                _classNome6 = value;
                OnPropertyChanged(nameof(ClassNome6));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo6 = null;
                }
            }
        }

        public string ClassNome7
        {
            get => _classNome7;
            set
            {
                _classNome7 = value;
                OnPropertyChanged(nameof(ClassNome7));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo7 = null;
                }
            }
        }

        public string ClassNome8
        {
            get => _classNome8;
            set
            {
                _classNome8 = value;
                OnPropertyChanged(nameof(ClassNome8));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo8 = null;
                }
            }
        }

        public string ClassNome9
        {
            get => _classNome9;
            set
            {
                _classNome9 = value;
                OnPropertyChanged(nameof(ClassNome9));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo9 = null;
                }
            }
        }

        public string ClassNome10
        {
            get => _classNome10;
            set
            {
                _classNome10 = value;
                OnPropertyChanged(nameof(ClassNome10));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo10 = null;
                }
            }
        }

        public string ClassNome11
        {
            get => _classNome11;
            set
            {
                _classNome11 = value;
                OnPropertyChanged(nameof(ClassNome11));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo11 = null;
                }
            }
        }

        public string ClassNome12
        {
            get => _classNome12;
            set
            {
                _classNome12 = value;
                OnPropertyChanged(nameof(ClassNome12));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo12 = null;
                }
            }
        }

        public string ClassNome13
        {
            get => _classNome13;
            set
            {
                _classNome13 = value;
                OnPropertyChanged(nameof(ClassNome13));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo13 = null;
                }
            }
        }

        public string ClassNome14
        {
            get => _classNome14;
            set
            {
                _classNome14 = value;
                OnPropertyChanged(nameof(ClassNome14));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo14 = null;
                }
            }
        }

        public string ClassNome15
        {
            get => _classNome15;
            set
            {
                _classNome15 = value;
                OnPropertyChanged(nameof(ClassNome15));

                if (string.IsNullOrWhiteSpace(value))
                {
                    ClassTipo15 = null;
                }
            }
        }

        private string _classInicio1;
        private string _classInicio2;
        private string _classInicio3;
        private string _classInicio4;
        private string _classInicio5;
        private string _classInicio6;
        private string _classInicio7;
        private string _classInicio8;
        private string _classInicio9;
        private string _classInicio10;
        private string _classInicio11;
        private string _classInicio12;
        private string _classInicio13;
        private string _classInicio14;
        private string _classInicio15;

        public string ClassInicio1
        {
            get => _classInicio1;
            set
            {
                _classInicio1 = string.IsNullOrWhiteSpace(ClassNome1) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio1));
            }
        }

        public string ClassInicio2
        {
            get => _classInicio2;
            set
            {
                _classInicio2 = string.IsNullOrWhiteSpace(ClassNome2) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio2));
            }
        }

        public string ClassInicio3
        {
            get => _classInicio3;
            set
            {
                _classInicio3 = string.IsNullOrWhiteSpace(ClassNome3) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio3));
            }
        }

        public string ClassInicio4
        {
            get => _classInicio4;
            set
            {
                _classInicio4 = string.IsNullOrWhiteSpace(ClassNome4) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio4));
            }
        }

        public string ClassInicio5
        {
            get => _classInicio5;
            set
            {
                _classInicio5 = string.IsNullOrWhiteSpace(ClassNome5) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio5));
            }
        }

        public string ClassInicio6
        {
            get => _classInicio6;
            set
            {
                _classInicio6 = string.IsNullOrWhiteSpace(ClassNome6) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio6));
            }
        }

        public string ClassInicio7
        {
            get => _classInicio7;
            set
            {
                _classInicio7 = string.IsNullOrWhiteSpace(ClassNome7) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio7));
            }
        }

        public string ClassInicio8
        {
            get => _classInicio8;
            set
            {
                _classInicio8 = string.IsNullOrWhiteSpace(ClassNome8) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio8));
            }
        }

        public string ClassInicio9
        {
            get => _classInicio9;
            set
            {
                _classInicio9 = string.IsNullOrWhiteSpace(ClassNome9) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio9));
            }
        }

        public string ClassInicio10
        {
            get => _classInicio10;
            set
            {
                _classInicio10 = string.IsNullOrWhiteSpace(ClassNome10) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio10));
            }
        }

        public string ClassInicio11
        {
            get => _classInicio11;
            set
            {
                _classInicio11 = string.IsNullOrWhiteSpace(ClassNome11) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio11));
            }
        }

        public string ClassInicio12
        {
            get => _classInicio12;
            set
            {
                _classInicio12 = string.IsNullOrWhiteSpace(ClassNome12) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio12));
            }
        }

        public string ClassInicio13
        {
            get => _classInicio13;
            set
            {
                _classInicio13 = string.IsNullOrWhiteSpace(ClassNome13) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio13));
            }
        }

        public string ClassInicio14
        {
            get => _classInicio14;
            set
            {
                _classInicio14 = string.IsNullOrWhiteSpace(ClassNome14) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio14));
            }
        }

        public string ClassInicio15
        {
            get => _classInicio15;
            set
            {
                _classInicio15 = string.IsNullOrWhiteSpace(ClassNome15) ? "" : value;
                OnPropertyChanged(nameof(ClassInicio15));
            }
        }

        private string _classFim1;
        private string _classFim2;
        private string _classFim3;
        private string _classFim4;
        private string _classFim5;
        private string _classFim6;
        private string _classFim7;
        private string _classFim8;
        private string _classFim9;
        private string _classFim10;
        private string _classFim11;
        private string _classFim12;
        private string _classFim13;
        private string _classFim14;
        private string _classFim15;
        public string ClassFim1
        {
            get => _classFim1;
            set
            {
                _classFim1 = string.IsNullOrWhiteSpace(ClassNome1) ? "" : value;
                OnPropertyChanged(nameof(ClassFim1));
            }
        }

        public string ClassFim2
        {
            get => _classFim2;
            set
            {
                _classFim2 = string.IsNullOrWhiteSpace(ClassNome2) ? "" : value;
                OnPropertyChanged(nameof(ClassFim2));
            }
        }

        public string ClassFim3
        {
            get => _classFim3;
            set
            {
                _classFim3 = string.IsNullOrWhiteSpace(ClassNome3) ? "" : value;
                OnPropertyChanged(nameof(ClassFim3));
            }
        }

        public string ClassFim4
        {
            get => _classFim4;
            set
            {
                _classFim4 = string.IsNullOrWhiteSpace(ClassNome4) ? "" : value;
                OnPropertyChanged(nameof(ClassFim4));
            }
        }

        public string ClassFim5
        {
            get => _classFim5;
            set
            {
                _classFim5 = string.IsNullOrWhiteSpace(ClassNome5) ? "" : value;
                OnPropertyChanged(nameof(ClassFim5));
            }
        }

        public string ClassFim6
        {
            get => _classFim6;
            set
            {
                _classFim6 = string.IsNullOrWhiteSpace(ClassNome6) ? "" : value;
                OnPropertyChanged(nameof(ClassFim6));
            }
        }

        public string ClassFim7
        {
            get => _classFim7;
            set
            {
                _classFim7 = string.IsNullOrWhiteSpace(ClassNome7) ? "" : value;
                OnPropertyChanged(nameof(ClassFim7));
            }
        }

        public string ClassFim8
        {
            get => _classFim8;
            set
            {
                _classFim8 = string.IsNullOrWhiteSpace(ClassNome8) ? "" : value;
                OnPropertyChanged(nameof(ClassFim8));
            }
        }

        public string ClassFim9
        {
            get => _classFim9;
            set
            {
                _classFim9 = string.IsNullOrWhiteSpace(ClassNome9) ? "" : value;
                OnPropertyChanged(nameof(ClassFim9));
            }
        }

        public string ClassFim10
        {
            get => _classFim10;
            set
            {
                _classFim10 = string.IsNullOrWhiteSpace(ClassNome10) ? "" : value;
                OnPropertyChanged(nameof(ClassFim10));
            }
        }

        public string ClassFim11
        {
            get => _classFim11;
            set
            {
                _classFim11 = string.IsNullOrWhiteSpace(ClassNome11) ? "" : value;
                OnPropertyChanged(nameof(ClassFim11));
            }
        }

        public string ClassFim12
        {
            get => _classFim12;
            set
            {
                _classFim12 = string.IsNullOrWhiteSpace(ClassNome12) ? "" : value;
                OnPropertyChanged(nameof(ClassFim12));
            }
        }

        public string ClassFim13
        {
            get => _classFim13;
            set
            {
                _classFim13 = string.IsNullOrWhiteSpace(ClassNome13) ? "" : value;
                OnPropertyChanged(nameof(ClassFim13));
            }
        }

        public string ClassFim14
        {
            get => _classFim14;
            set
            {
                _classFim14 = string.IsNullOrWhiteSpace(ClassNome14) ? "" : value;
                OnPropertyChanged(nameof(ClassFim14));
            }
        }

        public string ClassFim15
        {
            get => _classFim15;
            set
            {
                _classFim15 = string.IsNullOrWhiteSpace(ClassNome15) ? "" : value;
                OnPropertyChanged(nameof(ClassFim15));
            }
        }

        private string _classQtdEixo1;
        private string _classQtdEixo2;
        private string _classQtdEixo3;
        private string _classQtdEixo4;
        private string _classQtdEixo5;
        private string _classQtdEixo6;
        private string _classQtdEixo7;
        private string _classQtdEixo8;
        private string _classQtdEixo9;
        private string _classQtdEixo10;
        private string _classQtdEixo11;
        private string _classQtdEixo12;
        private string _classQtdEixo13;
        private string _classQtdEixo14;
        private string _classQtdEixo15;

        public string ClassQtdEixo1
        {
            get => _classQtdEixo1;
            set
            {
                _classQtdEixo1 = string.IsNullOrWhiteSpace(ClassNome1) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo1));
            }
        }

        public string ClassQtdEixo2
        {
            get => _classQtdEixo2;
            set
            {
                _classQtdEixo2 = string.IsNullOrWhiteSpace(ClassNome2) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo2));
            }
        }

        public string ClassQtdEixo3
        {
            get => _classQtdEixo3;
            set
            {
                _classQtdEixo3 = string.IsNullOrWhiteSpace(ClassNome3) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo3));
            }
        }

        public string ClassQtdEixo4
        {
            get => _classQtdEixo4;
            set
            {
                _classQtdEixo4 = string.IsNullOrWhiteSpace(ClassNome4) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo4));
            }
        }

        public string ClassQtdEixo5
        {
            get => _classQtdEixo5;
            set
            {
                _classQtdEixo5 = string.IsNullOrWhiteSpace(ClassNome5) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo5));
            }
        }

        public string ClassQtdEixo6
        {
            get => _classQtdEixo6;
            set
            {
                _classQtdEixo6 = string.IsNullOrWhiteSpace(ClassNome6) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo6));
            }
        }

        public string ClassQtdEixo7
        {
            get => _classQtdEixo7;
            set
            {
                _classQtdEixo7 = string.IsNullOrWhiteSpace(ClassNome7) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo7));
            }
        }

        public string ClassQtdEixo8
        {
            get => _classQtdEixo8;
            set
            {
                _classQtdEixo8 = string.IsNullOrWhiteSpace(ClassNome8) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo8));
            }
        }

        public string ClassQtdEixo9
        {
            get => _classQtdEixo9;
            set
            {
                _classQtdEixo9 = string.IsNullOrWhiteSpace(ClassNome9) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo9));
            }
        }

        public string ClassQtdEixo10
        {
            get => _classQtdEixo10;
            set
            {
                _classQtdEixo10 = string.IsNullOrWhiteSpace(ClassNome10) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo10));
            }
        }

        public string ClassQtdEixo11
        {
            get => _classQtdEixo11;
            set
            {
                _classQtdEixo11 = string.IsNullOrWhiteSpace(ClassNome11) ? "" : value; ;
                OnPropertyChanged(nameof(ClassQtdEixo11));
            }
        }

        public string ClassQtdEixo12
        {
            get => _classQtdEixo12;
            set
            {
                _classQtdEixo12 = string.IsNullOrWhiteSpace(ClassNome12) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo12));
            }
        }

        public string ClassQtdEixo13
        {
            get => _classQtdEixo13;
            set
            {
                _classQtdEixo13 = string.IsNullOrWhiteSpace(ClassNome13) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo13));
            }
        }

        public string ClassQtdEixo14
        {
            get => _classQtdEixo14;
            set
            {
                _classQtdEixo14 = string.IsNullOrWhiteSpace(ClassNome14) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo14));
            }
        }

        public string ClassQtdEixo15
        {
            get => _classQtdEixo15;
            set
            {
                _classQtdEixo15 = string.IsNullOrWhiteSpace(ClassNome15) ? "" : value;
                OnPropertyChanged(nameof(ClassQtdEixo15));
            }
        }

        /// Função que habilita o Botao pistas da aba veículos.
        public bool CompLaco1Enabled => !string.IsNullOrEmpty(SelectedItem1);
        public bool CompLaco2Enabled => !string.IsNullOrEmpty(SelectedItem2);
        public bool CompLaco3Enabled => !string.IsNullOrEmpty(SelectedItem3);
        public bool CompLaco4Enabled => !string.IsNullOrEmpty(SelectedItem4);
        public bool CompLaco5Enabled => !string.IsNullOrEmpty(SelectedItem5);
        public bool CompLaco6Enabled => !string.IsNullOrEmpty(SelectedItem6);
        public bool CompLaco7Enabled => !string.IsNullOrEmpty(SelectedItem7);
        public bool CompLaco8Enabled => !string.IsNullOrEmpty(SelectedItem8);
        public bool CompLaco9Enabled => !string.IsNullOrEmpty(SelectedItem9);
        public bool CompLaco10Enabled => !string.IsNullOrEmpty(SelectedItem10);
        public bool CompLaco11Enabled => !string.IsNullOrEmpty(SelectedItem11);
        public bool CompLaco12Enabled => !string.IsNullOrEmpty(SelectedItem12);
        public bool CompLaco13Enabled => !string.IsNullOrEmpty(SelectedItem13);
        public bool CompLaco14Enabled => !string.IsNullOrEmpty(SelectedItem14);
        public bool CompLaco15Enabled => !string.IsNullOrEmpty(SelectedItem15);
        public bool CompLaco16Enabled => !string.IsNullOrEmpty(SelectedItem16);


        // Evento de notificação de mudança de propriedade, necessário para INotifyPropertyChanged.
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Produto
    {
        public string Numero { get; set; }
        public string Data { get; set; }
        public int Hora { get; set; }
        public string Faixa { get; set; }
        public string Velocidade { get; set; }
        public string Comprimento { get; set; }
        public string CC { get; set; }
        public string OcupacaoM { get; set; }
        public string GapM { get; set; }
        public string GapS { get; set; }
        public string Headway { get; set; }
        public string Inter { get; set; }
        public string OcupacaoL { get; set; }
        public string Acionamento { get; set; }
        public string QtdEixo { get; set; }
        public string GrpEixo { get; set; }
        public string ClassDNIT { get; set; }
        public string  PesoEx1 { get; set; }
        public string  PesoEx2 { get; set; }
        public string  PesoEx3 { get; set; }
        public string  PesoEx4 { get; set; }
        public string  PesoEx5 { get; set; }
        public string  PesoEx6 { get; set; }
        public string  PesoEx7 { get; set; }
        public string  PesoEx8 { get; set; }
        public string  PesoEx9 { get; set; }
        public string  PesoEx10 { get; set; }
        public string  PesoTotal { get; set; }
    }
}
