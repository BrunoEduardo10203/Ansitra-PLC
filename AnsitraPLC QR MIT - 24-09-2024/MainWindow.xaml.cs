using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AnsitraPLC_QR_MIT.Model;
using AnsitraPLC_QR_MIT.Models;
using AnsitraPLC_QR_MIT.Services;
using AnsitraPLC_QR_MIT.Services.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Azure.Core.HttpHeader;

namespace AnsitraPLC_QR_MIT
{
    [Microsoft.UI.Xaml.CustomAttributes.MUXPropertyChangedCallback(enable = true)]
    [Microsoft.UI.Xaml.CustomAttributes.MUXPropertyChangedCallbackMethodName(value = "OnPropertyChanged")]
    [Windows.Foundation.Metadata.MarshalingBehavior(Windows.Foundation.Metadata.MarshalingType.Agile)]
    [Windows.Foundation.Metadata.Threading(Windows.Foundation.Metadata.ThreadingModel.Both)]
    [Windows.Foundation.Metadata.ContractVersion(typeof(Microsoft.UI.Xaml.XamlContract), 65536)]

    public partial class MainWindow : Window
    {
        public Button BotaoEquipamentos => btn_Equipamento;
        public ToggleButton BotaoConexao => btn_Conexao;

        public Button BtnEnviarData
        {
            get => btn_enviarData;
            set => btn_enviarData = value;
        }

        public Button BtnResetar
        {
            get => btn_resetar;
            set => btn_resetar = value;
        }

        public Button BtnLerConfig
        {
            get => btn_lerConfig;
            set => btn_lerConfig = value;
        }

        public Button BtnEnviarConfig
        {
            get => btnEnviar;
            set => btnEnviar = value;
        }

        public Button BtnPararPLC
        {
            get => btn_pararPLC;
            set => btn_pararPLC = value;
        }

        public Button BtnFTP
        {
            get => btn_FTP;
            set => btn_FTP = value;
        }

        public NavigationViewItem NavItemArquivos => nav_item_arquivos;
        public NavigationViewItem NavItemBanco => nav_item_banco;
        public NavigationViewItem NavItemConfiguracao => nav_item_configuracao;
        public NavigationViewItem NavItemAlarmes => nav_item_alarmes;
        public NavigationViewItem NavItemAfericao => nav_item_afericao;
        public Expander ExpanderNavItemArquivos => Arquivos;
        public Expander ExpanderNavItemBanco => Banco;
        public string ip_conexao => txb_ip.Text;
        public string numero_estacao => txt_numeroValor.Text;

        public event Action<string> DadoRecebido;
        private Action<string> handlerLerConfig;



        public int controlarContagem = 0, statusConexao = 2, statusMostraDados = 0, statusSend = 0, nBytes = 0, flagSend = 0;
        public string dadosPLC;
        public bool manterConexao = false;

        bool isConnected = false;

        IPEndPoint serverEndPoint;
        TcpListener tcpListener;
        NetworkStream networkStream;
        Thread listenThread;
        TcpClient tcpClient;
        Thread clientThread;
        List<int> listStatusNoReply;

        private Window m_window;
        private FaixasPage faixasPage;
        private LacosPage lacosPage;
        private FiltrosPage filtrosPage;
        private ClassePage classePage;
        private ConexaoPage conexaoPage;
        private BancoPage bancoPage;
        private VeiculoPage veiculoPage;
        private DadosEstacaoPage dadosEstacaoPage;
        private readonly DispatcherQueue _dispatcher;

        public MainWindow()
        {               
            this.InitializeComponent();

            _dispatcher = DispatcherQueue.GetForCurrentThread();

            RootGrid.DataContext = App.MainViewModel;

            IntPtr Window = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var WindowID = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(Window);
            var app = AppWindow.GetFromWindowId(WindowID);

            app.SetIcon("Assets/app_logo.ico");

            // Define o tamanho inicial da janela
            var appWindow = GetAppWindowForCurrentWindow();


            var titulo_barra = appWindow.TitleBar;

            // Define cores personalizadas
            titulo_barra.ForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255); // Branco
            titulo_barra.BackgroundColor = Windows.UI.Color.FromArgb(255, 47, 79, 79);//DarkSlateGray
            titulo_barra.InactiveForegroundColor = Windows.UI.Color.FromArgb(255, 200, 200, 200); // Cinza claro
            titulo_barra.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 37, 63, 63); // Cinza escuro
            titulo_barra.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 37, 63, 63);

            //// Cores dos botões
            titulo_barra.ButtonForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255); // Branco
            titulo_barra.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 47, 79, 79); // DarkSlateGray
            titulo_barra.ButtonHoverForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255); // Branco
            titulo_barra.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 0, 150, 245); // Azul mais claro
            titulo_barra.ButtonPressedForegroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0); // Preto
            titulo_barra.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 200, 200, 200); // Cinza claro

            if (appWindow != null)
            {
                //Devine o tamanhaho e a localização que vai abrir a janela
                appWindow.Resize(new SizeInt32(1780, 920));
                var displayArea = DisplayArea.GetFromWindowId(WindowID, DisplayAreaFallback.Primary);
                var centerX = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
                var centerY = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;

                appWindow.Move(new PointInt32(centerX, centerY));

            }

            bool isExpanded = nav_control.IsPaneOpen;

            InitializarApp();
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            // Obtenha o identificador da janela atual
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(windowId);
        }

        public void AtualizarHeaderNumero(string texto)
        {            
            txt_numeroValor.Text = texto;
        }
        public void AtualizarHeaderRodovia(string texto)
        {
            txt_rodoviaValor.Text = texto;
        }

        public void AtualizarHeaderKm(string texto) 
        {
            txt_kmValor.Text = texto;
        }


        //-------------------------------Inicio do servidor-------------------------------------------------//
        private async void InitializarApp()
        {
            await Task.Delay(500);
            bool dlay = true;

            IniciarServer();
            lacosPage = new LacosPage();
            faixasPage = new FaixasPage();
            filtrosPage = new FiltrosPage();
            classePage = new ClassePage();
            conexaoPage = new ConexaoPage();
            bancoPage = new BancoPage();
            veiculoPage = new VeiculoPage();
            dadosEstacaoPage = new DadosEstacaoPage();

            CarregarArquivosJson();

            if (dlay)
            {
                await AtualizarEquipamentoAsync();
                controle_Equipamentos();
            }            
        }

        public void IniciarServer()
        {
            try
            {
                // Obtém o nome da máquina
                string hostName = Dns.GetHostName();

                // Obtém todos os endereços IP associados
                IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);

                IPAddress ipServer = ipAddresses.FirstOrDefault(ip =>
                    ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip));

                // Iniciar o TcpListener escutando em qualquer IP disponível na porta 3050
                this.tcpListener = new TcpListener(ipServer, 3050);

                this.tcpListener.Start();

                // Criar e iniciar uma nova thread para escutar os clientes
                this.listenThread = new Thread(new ThreadStart(ListenForClients));
                this.listenThread.IsBackground = true;
                this.listenThread.Start();
                manterConexao = true;
                Debug.WriteLine("TCP rodando na porta: 3050");

            }
            catch (Exception err)
            {
                Debug.WriteLine($"Error:{ err.Message}{ Environment.NewLine}Local do erro:{err.StackTrace}");
            }

        }

        private void ListenForClients()
        {
            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();
                client.ToString();

                // Exibir uma mensagem indicando que um cliente se conectou
                Debug.WriteLine("Um cliente se conectou!");

                // Criar uma nova thread para lidar com a comunicação com o cliente
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
        }

        private string caminhoPastaSelecionada;
        private void HandleClientComm(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;

            try
            {
                NetworkStream clientStream = client.GetStream();
                byte[] buffer = new byte[8192];

                while (manterConexao)
                {
                    if (!client.Connected) break; //Sai do loop

                    // Verifica se há dados
                    if (clientStream.DataAvailable)
                    {
                        int bytesRead = clientStream.Read(buffer, 0, buffer.Length);

                        if (bytesRead == 0)
                        {
                            Debug.WriteLine("Cliente desconectado.");
                            break;
                        }

                        string dadoRecebido = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        //string dadoRecebido = "IVBV;2025-05-08;03:35:57;1;18528300;28;1111;2025;05;08;03;35;57;999;0;400;800;36;6,0;2;600;29;2;3;1;0;FVBV";

                        if (dadoRecebido.StartsWith("IVBV") && dadoRecebido.EndsWith("FVBV"))
                        {
                            Debug.WriteLine($"Esse é o dado: {dadoRecebido}");
                            DadoRecebido?.Invoke(dadoRecebido);
                        }

                        if (!string.IsNullOrEmpty(caminhoPastaSelecionada))
                        {
                            // salva o arquivo no caminho
                            File.WriteAllText(Path.Combine(caminhoPastaSelecionada, "arquivo.txt"), dadoRecebido);
                            caminhoPastaSelecionada = null;
                        }
                        else
                            _dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () =>
                            {
                                try
                                {
                                    var dados = JsonSerializer.Deserialize<DadosJson>(dadoRecebido);
                                    if (dados != null)
                                    {
                                        CarregarConfiguracao(dados);
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Falha ao desserializar dadoRecebido.");
                                    }
                                }
                                catch (JsonException ex)
                                {
                                    Debug.WriteLine("Erro de JSON: " + ex.Message);
                                }
                            });

                    }

                    Thread.Sleep(10); //Evitar consumo excessivo de CPU
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro: " + ex.Message);
            }
            finally
            {
                client.Close();
                Debug.WriteLine("Conexão encerrada.");
            }
        }

        //-------------------------------Fim do servidor-------------------------------------------------//

        private async Task ShowMessageDialog(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Ansitra PLC - Informação do Servidor",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot    
            };

            await dialog.ShowAsync();
        }

        public async Task<bool> ShowDialog(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Ansitra PLC",
                Content = message,
                PrimaryButtonText = "Sim",
                CloseButtonText = "Não",    
                XamlRoot = this.Content.XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        private async Task<ContentDialogResult> AbrirEquipamentosPage()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Ansitra PLC - Equipamentos",
                Content = "Selecione o equipamento desejado.",
                PrimaryButtonText = "Ansitra QR",
                SecondaryButtonText = "Ansitra WT",
                XamlRoot = this.Content.XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();
            return result;
        }

        //-------------------------------Inicio da conexão como cliente-------------------------------------------------//
        public async void ToggleButton_Click(object sender, RoutedEventArgs e)
        { 
            Ping ping = new Ping();
            bool isExpanded = Conexao.IsExpanded;

            try { 
                if (!isConnected)
                {
                    if (txb_ip.Text.Length > 6 && txb_porta.Text.Length > 1)
                    {
                        PingReply resp = ping.Send(txb_ip.Text);

                        if (resp.Status == IPStatus.Success)
                        {
                            IniciarTCPServer();

                            dadosPLC = "LDCF";
                            statusSend = 6;
                            flagSend = 1;
                            nBytes = 22;

                            Conect();

                            if (statusConexao == 1)
                            {
                                {
                                    isConnected = true;
                                    Conexao.IsExpanded = !isExpanded;
                                    string ip = txb_ip.Text;
                                    string porta = txb_porta.Text;
                                    txt_ipValor.Text = ip;
                                    txt_portaValor.Text = porta;
                                    txb_ip.IsEnabled = false;
                                    txb_porta.IsEnabled = false;
                                    btn_Conexao.IsEnabled = true;
                                    btn_Conexao.Content = "Desconectar";
                                }
                            }
                        }
                        else
                        {
                            ShowMessageDialog("IP não responde...");
                        }
                    }
                    else
                    {
                        ShowMessageDialog("Digite o IP da Estação e a Porta de comunicação TCP.");
                    }
                }
                else
                {
                    bool confirmacao = await ShowDialog("Deseja realmente fechar a conexão com o PLC?");

                    if (confirmacao)
                    {
                        FecharConexao();
                    }
                }
            }
            catch
            {
                ShowMessageDialog("Por favor, digite um número de IP e uma Porta válida.");
                Conexao.IsExpanded = true;
                isConnected = false;
                txb_ip.IsEnabled = true;
                txb_ip.Text = "";
                txb_porta.IsEnabled = true;
                txb_porta.Text = "";
                btn_Conexao.Content = "Conectar";
            }
        }

        private void IniciarTCPServer()
        {
            //Verificação se a thread já foi criada

            if (clientThread == null)
            {
                // statusSend = 1: Enviar Configuração para o PLC
                // statusSend = 2: Ler Configuração do PLC
                // statusSend = 3: Monitoramento
                // statusSend = 4: Finaliza o Monitoramento
                // statusSend = 5: Reset MDV8
                // statusSend = 6: Recebe a Data/Hora do PLC
                // statusSend = 7: Envia Data/Hora
                // statusSend = 8: Reset BBB
                listStatusNoReply = new List<int> { 4, 5, 7, 8 };

                statusConexao = 2;

                clientThread = new Thread(new ThreadStart(HandleTCP));
                clientThread.IsBackground = true;
                clientThread.Start();
            }
        }

        public void Conect()
        {
            try
            {
                // Verificar se a conexão já existe e está ativa
                if (tcpClient != null && tcpClient.Connected && statusConexao != 0)
                {
                    Debug.WriteLine("Conexão já está aberta. Reutilizando a conexão existente.");
                    return;
                }

                statusConexao = 2; // Indica que estamos tentando conectar

                // Validar IP
                IPAddress ipAddressOut = null;
                bool isValidIp = IPAddress.TryParse(txb_ip.Text, out ipAddressOut);

                // Se o IP for válido, conectar com IPEndPoint
                if (isValidIp)
                {
                    tcpClient = new TcpClient();
                    serverEndPoint = new IPEndPoint(ipAddressOut, Convert.ToInt32(txb_porta.Text));
                    tcpClient.Connect(serverEndPoint);
                }
                else
                {
                    tcpClient = new TcpClient();
                    tcpClient.Connect(txb_ip.Text, Convert.ToInt32(txb_porta.Text));
                }

                // Definir o tamanho do buffer de recepção após a conexão
                tcpClient.ReceiveBufferSize = 1024;

                // Verificar se a conexão foi bem-sucedida
                if (tcpClient.Connected)
                {
                    statusConexao = 1; // Conexão bem-sucedida
                    Debug.WriteLine($"Conectado ao IP {txb_ip.Text}. Porta {txb_porta.Text}");
                }
                else
                {
                    throw new Exception("Não foi possível conectar no PLC");
                }
            }
            catch (Exception ex)
            {
                statusConexao = 0; // Indicar falha na conexão
                Debug.WriteLine($"Erro ao conectar: {ex.Message}");
                ShowMessageDialog($"Não foi possível conectar. Verifique se IP {txb_ip.Text} e Porta {txb_porta.Text} estão corretos.");
            }
        }


        public void FecharConexao()
        {
            try
            {
                if (networkStream != null)
                {
                    networkStream.Close();
                }

                if (tcpClient != null && tcpClient.Connected)
                {
                    tcpClient.Close();
                    tcpClient = null;
                }

                statusConexao = 0;
                ShowMessageDialog("Conexão TCP fechada com sucesso");

                // Código adicional após fechar a conexão
                Conexao.IsExpanded = true;
                isConnected = false;
                txb_ip.IsEnabled = true;
                txt_ipValor.Text = "";
                txb_porta.IsEnabled = true;
                txt_portaValor.Text = "";
                btn_Conexao.Content = "Conectar";
                veiculoPage.BotaoIniciar.Visibility = Visibility.Visible;
                veiculoPage.BotaoFechar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro ao fechar a conexão TCP: " + ex.Message);
                ShowMessageDialog($"Erro ao fechar a conexão TCP: {ex.Message}");
            }
        }



        private void HandleTCP()
        {
            bool tcpAtivo = true;

            while (tcpAtivo)
            {
                try
                {
                    //Verifica de o servido caiu ou foi encerrado
                    if (tcpClient != null && !IsSocketConnected(tcpClient.Client))
                    {
                        FecharConexao();
                    }

                    if (statusConexao == 1 && tcpClient != null)
                    {
                        if (flagSend == 1)
                        {
                            networkStream = tcpClient.GetStream();

                            ASCIIEncoding encoder = new ASCIIEncoding();
                            byte[] buffer = encoder.GetBytes(dadosPLC);

                            networkStream.Write(buffer, 0, buffer.Length);
                            networkStream.Flush();

                            flagSend = 0;

                            if (listStatusNoReply.Contains(statusSend))
                            {
                                statusConexao = 0;
                            }
                        }

                        if (!listStatusNoReply.Contains(statusSend) && networkStream != null && networkStream.CanRead)
                        {
                            //GetDadosTCP(networkStream);     // Lê dados enviados pelo PLC
                        }
                    }
                    else if (statusConexao == 0 && tcpClient != null)
                    {
                        tcpClient.Close();
                        tcpClient = null;
                        if (btn_Conexao.IsEnabled == false)
                        {
                            Debug.WriteLine("Status: TCP Fechado.");
                            ShowMessageDialog("Conexão TCP Fechado");

                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Tcp inativo");
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        statusConexao = 0;
                        ShowMessageDialog("A conexão com o servidor foi perdida. Verifique sua conexão e tente novamente.");
                        FecharConexao();
                    });
                }
                Thread.Sleep(200);
            }
        }

        private bool IsSocketConnected(Socket s)
        {
            try
            {
                bool part1 = s.Poll(3000, SelectMode.SelectRead);
                bool part2 = (s.Available == 0);
                return !(part1 && part2);
            }
            catch
            {
                return false;
            }
        }

        //-------------------------------Fim da conexão como cliente-------------------------------------------------//


        //-------------------------------Inicio da conexão como o banco de dados-------------------------------------------------//

        private List<Estacao> dadosCache = new List<Estacao>();

        private void cbx_bancos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemSelec = (ComboBoxItem)cbx_bancos.SelectedItem;

            if (itemSelec.Content.ToString() == "SQL Server")
            {
                banco_SQL();
            }
        }

        private List<Estacao> banco_SQL()
        {
            if (dadosCache.Count > 0)
            {
                return dadosCache;
            }

            List<Estacao> resultados = new List<Estacao>();
            List<ClasseVeiculo> resultadoClass = new List<ClasseVeiculo>();

            string servicoBD = bancoPage.servico_banco;
            string nomeBanco = bancoPage.host_banco;
            string usuarioBD = bancoPage.usuario_banco;
            string senhaBD = bancoPage.senha_banco;

            SqlConnection conexao = DAOConexao.ConectarSQL(servicoBD, nomeBanco, usuarioBD, senhaBD);

            if (conexao != null)
            {
                txb_banco.Text = "Conectado ao Banco";

                SqlCommand estacaoBD = new SqlCommand(@"
                                                        SELECT 
                                                            E.NUM_ESTACAO,
                                                            E.NOME_ESTACAO, 
                                                            E.KM, 
                                                            E.LOCAL, 
                                                            E.RA, 
                                                            E.ESTADO, 
                                                            E.RODOVIA, 
                                                            E.IP, 
                                                            E.IP_GATEWAY, 
                                                            E.IP_MASCARA, 
                                                            E.TIPO,
                                                            E.TIPO_CC,
                                                            L.NOME_FAIXA,
                                                            L.COMPRIMENTO,
                                                            L.DISTANCIA,
                                                            F.COMP_MAIOR,
                                                            F.COMP_MAIOR_GRAVAR,
                                                            F.COMP_MENOR,
                                                            F.COMP_MENOR_GRAVAR,
                                                            F.VELOC_MAIOR,
                                                            F.VELOC_MAIOR_GRAVAR,
                                                            F.VELOC_MENOR,
                                                            F.VELOC_MENOR_GRAVAR
                                                        FROM 
                                                            ESTACAO E
                                                        LEFT JOIN 
                                                            LACO L ON E.NUM_ESTACAO = L.NUM_ESTACAO
                                                        LEFT JOIN 
                                                            FILTROS F ON E.NUM_ESTACAO = F.NUM_ESTACAO", conexao);

                SqlDataReader estacao_reader = estacaoBD.ExecuteReader();

                Dictionary<string, Estacao> estacoesDict = new Dictionary<string, Estacao>();

                string tipoCC = "";
                while (estacao_reader.Read())
                {

                    if (estacao_reader["NUM_ESTACAO"] != DBNull.Value)
                    {
                        string numEstacao = estacao_reader["NUM_ESTACAO"].ToString();

                        if (!estacoesDict.ContainsKey(numEstacao))
                        {
                            string ipPlc = estacao_reader["IP"].ToString();
                            string ipGateWay = estacao_reader["IP_GATEWAY"].ToString();
                            string ipMascara = estacao_reader["IP_MASCARA"].ToString();
                            string nomeRodovia = estacao_reader["RODOVIA"].ToString();
                            string nomeEstacao = estacao_reader["NOME_ESTACAO"].ToString();
                            string km = estacao_reader["KM"].ToString();
                            string tipo = estacao_reader["TIPO"].ToString();
                            tipoCC = estacao_reader["TIPO_CC"].ToString();
                            string local = estacao_reader["LOCAL"].ToString();
                            string ra = estacao_reader["RA"].ToString();
                            string estado = estacao_reader["ESTADO"].ToString();
                            string comprimento = estacao_reader["COMPRIMENTO"].ToString();
                            string comp_maior = estacao_reader["COMP_MAIOR"].ToString();
                            string comp_maior_gravar = estacao_reader["COMP_MAIOR_GRAVAR"].ToString();
                            string comp_menor = estacao_reader["COMP_MENOR"].ToString();
                            string comp_menor_gravar = estacao_reader["COMP_MENOR_GRAVAR"].ToString();
                            string veloc_maior = estacao_reader["VELOC_MAIOR"].ToString();
                            string veloc_maior_gravar = estacao_reader["VELOC_MAIOR_GRAVAR"].ToString();
                            string veloc_menor = estacao_reader["VELOC_MENOR"].ToString();
                            string veloc_menor_gravar = estacao_reader["VELOC_MENOR_GRAVAR"].ToString();

                            var estacao = new Estacao
                            {
                                IpPlc = ipPlc,
                                IpGateWay = ipGateWay,
                                IpMascara = ipMascara,
                                Tipo = tipo,
                                Tipo_CC = tipoCC,
                                NomeRodovia = nomeRodovia,
                                IdEstacao = numEstacao,
                                NumEstacao = numEstacao,
                                NomeEstacao = nomeEstacao,
                                Km = km,
                                Local = local,
                                Estado = estado,
                                Ra = ra,
                                CompMaior = comp_maior,
                                CompMaiorGravar = comp_maior_gravar,
                                CompMenor = comp_menor,
                                CompMenorGravar = comp_menor_gravar,
                                VelocMaior = veloc_maior,
                                VelocMaiorGravar = veloc_maior_gravar,
                                VelocMenor = veloc_menor,
                                VelocMenorGravar = veloc_menor_gravar,
                            };

                            estacoesDict[numEstacao] = estacao;

                            // Adiciona no ComboBox
                            if (!cbx_equipamentos.Items.Contains(numEstacao))
                            {
                                cbx_equipamentos.Items.Add(numEstacao);
                            }
                        }

                        // Adiciona a faixa se houver
                        if (estacao_reader["NOME_FAIXA"] != DBNull.Value)
                        {
                            string nomeFaixa = estacao_reader["NOME_FAIXA"].ToString();

                            if (!estacoesDict[numEstacao].NomesFaixas.Contains(nomeFaixa))
                            {
                                estacoesDict[numEstacao].NomesFaixas.Add(nomeFaixa);
                            }
                        }

                        // Adiciona o Comprimento se houver
                        if (estacao_reader["COMPRIMENTO"] != DBNull.Value)
                        {
                            string comprimentoFaixa = estacao_reader["COMPRIMENTO"].ToString();

                            estacoesDict[numEstacao].ComprimentoFaixas.Add(comprimentoFaixa);
                        }

                        // Adiciona o Distacia se houver
                        if (estacao_reader["DISTANCIA"] != DBNull.Value)
                        {
                            string distanciaFaixa = estacao_reader["DISTANCIA"].ToString();

                            estacoesDict[numEstacao].DistanciaFaixas.Add(distanciaFaixa);
                        }
                    }
                }
                estacao_reader.Close();
                CarregarClasses(conexao, tipoCC);
                resultados = estacoesDict.Values.ToList();
                conexao.Close();

                dadosCache = resultados;
            }
            else
            {
                txb_banco.Text = "Erro ao conectar";
                txb_banco.Foreground = new SolidColorBrush(Colors.Red);
            }

            return resultados;
        }

        public void CarregarClasses(SqlConnection conexao, string tipocc)
        {
            // Consulta da tabela CLASSES
            SqlCommand classesBD = new SqlCommand(@"
                                                SELECT 
                                                    TIPO,
                                                    VEICULO,
                                                    DESCRICAO,
                                                    RANGE_INICIAL,
                                                    RANGE_FINAL,
                                                    QTD_EIXOS
                                                FROM CLASSES", conexao);

            SqlDataReader classes_reader = classesBD.ExecuteReader();
            int contador = 1;

            for (int i = 1; i <= 16; i++)
            {
                typeof(MainViewModel).GetProperty($"ClassTipo{i}")?.SetValue(App.MainViewModel, "");
                typeof(MainViewModel).GetProperty($"ClassNome{i}")?.SetValue(App.MainViewModel, string.Empty);
                typeof(MainViewModel).GetProperty($"ClassInicio{i}")?.SetValue(App.MainViewModel, string.Empty);
                typeof(MainViewModel).GetProperty($"ClassFim{i}")?.SetValue(App.MainViewModel, string.Empty);
                typeof(MainViewModel).GetProperty($"ClassQtdEixo{i}")?.SetValue(App.MainViewModel, string.Empty);
            }

            while (classes_reader.Read())
            {
                if (classes_reader["TIPO"].ToString() == tipocc)
                {
                    string tipoEquipamento = classes_reader["VEICULO"].ToString();
                    string descricao = classes_reader["DESCRICAO"].ToString();
                    string inicio = classes_reader["RANGE_INICIAL"].ToString();
                    string fim = classes_reader["RANGE_FINAL"].ToString();
                    string qtdEixo = classes_reader["QTD_EIXOS"].ToString();

                    // Use reflexão para reduzir o código repetitivo
                    typeof(MainViewModel).GetProperty($"ClassTipo{contador}")?.SetValue(App.MainViewModel, tipoEquipamento);
                    typeof(MainViewModel).GetProperty($"ClassNome{contador}")?.SetValue(App.MainViewModel, descricao);
                    typeof(MainViewModel).GetProperty($"ClassInicio{contador}")?.SetValue(App.MainViewModel, inicio);
                    typeof(MainViewModel).GetProperty($"ClassFim{contador}")?.SetValue(App.MainViewModel, fim);
                    typeof(MainViewModel).GetProperty($"ClassQtdEixo{contador}")?.SetValue(App.MainViewModel, qtdEixo);

                    contador++;
                    if (contador > 16) break;

                }
                else if (classes_reader["TIPO"].ToString() == tipocc)
                {
                    string tipoEquipamento = classes_reader["VEICULO"].ToString();
                    string descricao = classes_reader["DESCRICAO"].ToString();
                    string inicio = classes_reader["RANGE_INICIAL"].ToString();
                    string fim = classes_reader["RANGE_FINAL"].ToString();
                    string qtdEixo = classes_reader["QTD_EIXOS"].ToString();

                    // Use reflexão para reduzir o código repetitivo
                    typeof(MainViewModel).GetProperty($"ClassTipo{contador}")?.SetValue(App.MainViewModel, tipoEquipamento);
                    typeof(MainViewModel).GetProperty($"ClassNome{contador}")?.SetValue(App.MainViewModel, descricao);
                    typeof(MainViewModel).GetProperty($"ClassInicio{contador}")?.SetValue(App.MainViewModel, inicio);
                    typeof(MainViewModel).GetProperty($"ClassFim{contador}")?.SetValue(App.MainViewModel, fim);
                    typeof(MainViewModel).GetProperty($"ClassQtdEixo{contador}")?.SetValue(App.MainViewModel, qtdEixo);

                    contador++;
                    if (contador > 16) break;
                }
            }

            classes_reader.Close();
        }


        private void cbx_equipamentos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string servicoBD = bancoPage.servico_banco;
            string nomeBanco = bancoPage.host_banco;
            string usuarioBD = bancoPage.usuario_banco;
            string senhaBD = bancoPage.senha_banco;
            SqlConnection conexao = DAOConexao.ConectarSQL(servicoBD, nomeBanco, usuarioBD, senhaBD);

            string selecionado = cbx_equipamentos.SelectedItem?.ToString();


            List<Estacao> dadosDoBanco = banco_SQL();

            // Procura uma estação com o NumEstacao igual ao selecionado
            Estacao estacaoSelecionada = dadosDoBanco.FirstOrDefault(est => est.NumEstacao == selecionado);

            if (estacaoSelecionada != null)
            {
                CarregarClasses(conexao, estacaoSelecionada.Tipo_CC);

                Debug.WriteLine($"Você selecionou a estação: {estacaoSelecionada.NomeEstacao} - Classe: ");

                // Extrai número e rodovia da NomeEstacao
                string numero = estacaoSelecionada.NomeEstacao.Substring(1, 3);
                string rodovia = estacaoSelecionada.NomeEstacao.Substring(4, 2);

                // Atualiza a ViewModel principal
                App.MainViewModel.EstacaoSelecionada.IpPlc = estacaoSelecionada.IpPlc;
                App.MainViewModel.EstacaoSelecionada.IpGateWay = estacaoSelecionada.IpGateWay;
                App.MainViewModel.EstacaoSelecionada.IpMascara = estacaoSelecionada.IpMascara;
                App.MainViewModel.EstacaoSelecionada.Tipo = estacaoSelecionada.Tipo;
                App.MainViewModel.EstacaoSelecionada.NomeRodovia = estacaoSelecionada.NomeRodovia;
                App.MainViewModel.EstacaoSelecionada.IdEstacao = estacaoSelecionada.NomeEstacao;
                App.MainViewModel.EstacaoSelecionada.NumEstacao = estacaoSelecionada.NumEstacao;
                App.MainViewModel.EstacaoSelecionada.Rodovia = rodovia;
                App.MainViewModel.EstacaoSelecionada.Km = estacaoSelecionada.Km;
                App.MainViewModel.EstacaoSelecionada.Local = estacaoSelecionada.Local;
                App.MainViewModel.EstacaoSelecionada.Estado = estacaoSelecionada.Estado;
                App.MainViewModel.EstacaoSelecionada.Ra = estacaoSelecionada.Ra;

                //Filtros
                App.MainViewModel.CompMaior = estacaoSelecionada.CompMaior;
                App.MainViewModel.CompMaiorGravar = estacaoSelecionada.CompMaiorGravar;
                App.MainViewModel.CompMenor = estacaoSelecionada.CompMenor;
                App.MainViewModel.CompMenorGravar = estacaoSelecionada.CompMenorGravar;
                App.MainViewModel.VelocMaior = estacaoSelecionada.CompMaior;
                App.MainViewModel.VelocMaiorGravar = estacaoSelecionada.CompMaiorGravar;
                App.MainViewModel.VelocMenor = estacaoSelecionada.CompMenor;
                App.MainViewModel.VelocMenorGravar = estacaoSelecionada.CompMenorGravar;

                // Atualiza campos de texto
                txt_numeroValor.Text = numero;
                txt_rodoviaValor.Text = rodovia;
                txt_kmValor.Text = estacaoSelecionada.Km;

                // Atualiza campos de Faixa
                int totalFaixas = 16;

                for (int i = 0; i < totalFaixas; i++)
                {
                    var prop = App.MainViewModel.GetType().GetProperty($"NomeFaixa{i + 1}");
                    var comp = App.MainViewModel.GetType().GetProperty($"CompLaco{i + 1}");
                    var dist = App.MainViewModel.GetType().GetProperty($"DistLaco{i + 1}");

                    if (prop != null && prop.CanWrite && comp != null && comp.CanWrite && dist != null && dist.CanWrite)
                    {
                        prop.SetValue(App.MainViewModel, string.Empty);
                        comp.SetValue(App.MainViewModel, string.Empty);
                        dist.SetValue(App.MainViewModel, string.Empty);
                    }

                    // Limpa o estado dos checkboxes: canal1, canal2, etc.
                    var checkBoxProp = faixasPage.GetType().GetProperty($"canal{i + 1}");

                    if (checkBoxProp != null)
                    {
                        var checkBox = checkBoxProp.GetValue(faixasPage) as CheckBox;
                        if (checkBox != null)
                        {
                            checkBox.IsChecked = false;
                        }
                    }
                }

                if (estacaoSelecionada.NomesFaixas != null && estacaoSelecionada.NomesFaixas.Count > 0)
                {
                    for (int i = 0; i < estacaoSelecionada.NomesFaixas.Count; i++)
                    {
                        var faixa = estacaoSelecionada.NomesFaixas[i];
                        var comprimento = estacaoSelecionada.ComprimentoFaixas[i];
                        var distancia = estacaoSelecionada.DistanciaFaixas[i];

                        switch (i)
                        {
                            case 0:
                                App.MainViewModel.NomeFaixa1 = faixa;
                                App.MainViewModel.CompLaco1 = comprimento;
                                App.MainViewModel.DistLaco1 = distancia;
                                faixasPage.canal1.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "1", "1", "1");
                                faixasPage.setaCamposFaixas(faixasPage.canal1);
                                break;
                            case 1:
                                App.MainViewModel.NomeFaixa2 = faixa;
                                App.MainViewModel.CompLaco2 = comprimento;
                                App.MainViewModel.DistLaco2 = distancia;
                                faixasPage.canal2.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "2", "1", "2");
                                faixasPage.setaCamposFaixas(faixasPage.canal2);
                                break;
                            case 2:
                                App.MainViewModel.NomeFaixa3 = faixa;
                                App.MainViewModel.CompLaco3 = comprimento;
                                App.MainViewModel.DistLaco3 = distancia;
                                faixasPage.canal3.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "3", "1", "3");
                                faixasPage.setaCamposFaixas(faixasPage.canal3);
                                break;
                            case 3:
                                App.MainViewModel.NomeFaixa4 = faixa;
                                App.MainViewModel.CompLaco4 = comprimento;
                                App.MainViewModel.DistLaco4 = distancia;
                                faixasPage.canal4.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "4", "1", "4");
                                faixasPage.setaCamposFaixas(faixasPage.canal4);
                                break;
                            case 4:
                                App.MainViewModel.NomeFaixa5 = faixa;
                                App.MainViewModel.CompLaco5 = comprimento;
                                App.MainViewModel.DistLaco5 = distancia;
                                faixasPage.canal5.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "5", "2", "1");
                                faixasPage.setaCamposFaixas(faixasPage.canal5);
                                break;
                            case 5:
                                App.MainViewModel.NomeFaixa6 = faixa;
                                App.MainViewModel.CompLaco6 = comprimento;
                                App.MainViewModel.DistLaco6 = distancia;
                                faixasPage.canal6.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "6", "2", "2");
                                faixasPage.setaCamposFaixas(faixasPage.canal6);
                                break;
                            case 6:
                                App.MainViewModel.NomeFaixa7 = faixa;
                                App.MainViewModel.CompLaco7 = comprimento;
                                App.MainViewModel.DistLaco7 = distancia;
                                faixasPage.canal7.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "7", "2", "3");
                                faixasPage.setaCamposFaixas(faixasPage.canal7);
                                break;
                            case 7:
                                App.MainViewModel.NomeFaixa8 = faixa;
                                App.MainViewModel.CompLaco8 = comprimento;
                                App.MainViewModel.DistLaco8 = distancia;
                                faixasPage.canal8.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "8", "2", "4");
                                faixasPage.setaCamposFaixas(faixasPage.canal8);
                                break;
                            case 8:
                                App.MainViewModel.NomeFaixa9 = faixa;
                                App.MainViewModel.CompLaco9 = comprimento;
                                App.MainViewModel.DistLaco9 = distancia;
                                faixasPage.canal9.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "9", "3", "1");
                                faixasPage.setaCamposFaixas(faixasPage.canal9);
                                break;
                            case 9:
                                App.MainViewModel.NomeFaixa10 = faixa;
                                App.MainViewModel.CompLaco10 = comprimento;
                                App.MainViewModel.DistLaco10 = distancia;
                                faixasPage.canal10.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "10", "3", "2");
                                faixasPage.setaCamposFaixas(faixasPage.canal10);
                                break;
                            case 10:
                                App.MainViewModel.NomeFaixa11 = faixa;
                                App.MainViewModel.CompLaco11 = comprimento;
                                App.MainViewModel.DistLaco11 = distancia;
                                faixasPage.canal11.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "11", "3", "3");
                                faixasPage.setaCamposFaixas(faixasPage.canal11);
                                break;
                            case 11:
                                App.MainViewModel.NomeFaixa12 = faixa;
                                App.MainViewModel.CompLaco12 = comprimento;
                                App.MainViewModel.DistLaco12 = distancia;
                                faixasPage.canal12.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "12", "3", "4");
                                faixasPage.setaCamposFaixas(faixasPage.canal12);
                                break;
                            case 12:
                                App.MainViewModel.NomeFaixa13 = faixa;
                                App.MainViewModel.CompLaco13 = comprimento;
                                App.MainViewModel.DistLaco13 = distancia;
                                faixasPage.canal13.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "13", "4", "1");
                                faixasPage.setaCamposFaixas(faixasPage.canal13);
                                break;
                            case 13:
                                App.MainViewModel.NomeFaixa14 = faixa;
                                App.MainViewModel.CompLaco14 = comprimento;
                                App.MainViewModel.DistLaco14 = distancia;
                                faixasPage.canal14.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "14", "4", "2");
                                faixasPage.setaCamposFaixas(faixasPage.canal14);
                                break;
                            case 14:
                                App.MainViewModel.NomeFaixa15 = faixa;
                                App.MainViewModel.CompLaco15 = comprimento;
                                App.MainViewModel.DistLaco15 = distancia;
                                faixasPage.canal15.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "15", "4", "3");
                                faixasPage.setaCamposFaixas(faixasPage.canal15);
                                break;
                            case 15:
                                App.MainViewModel.NomeFaixa16 = faixa;
                                App.MainViewModel.CompLaco16 = comprimento;
                                App.MainViewModel.DistLaco16 = distancia;
                                faixasPage.canal16.IsChecked = true;
                                faixasPage.PreencherComboBoxes(faixa, "16", "4", "4");
                                faixasPage.setaCamposFaixas(faixasPage.canal16);
                                break;
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Nenhuma faixa associada a esta estação.");
                }

            }
        }
        //-------------------------------Fim da conexão como o banco de dados-------------------------------------------------//


        private void controle_Equipamentos()
        {
            var conexaoPage = mainFrame.Content as ConexaoPage;

            if (btn_Equipamento.Content.ToString() == "Ansitra WT")
            {
                nav_item_lacos.Visibility = Visibility.Collapsed;
                nav_item_resetMDV8.Visibility = Visibility.Collapsed;
                conexaoPage.Box3.Visibility = Visibility.Visible;
                conexaoPage.Box4.Visibility = Visibility.Visible;
                conexaoPage.nomeCaixa1 = "PLC 1";
                conexaoPage.nomeCaixa2 = "Sensor 1";
            }
            else if (btn_Equipamento.Content.ToString() == "Ansitra QR")
            {
                if (conexaoPage != null)
                {
                    if (conexaoPage.Box3 != null)
                        conexaoPage.Box3.Visibility = Visibility.Collapsed;

                    if (conexaoPage.Box4 != null)
                        conexaoPage.Box4.Visibility = Visibility.Collapsed;

                    conexaoPage.nomeCaixa1 = "PLC";
                    conexaoPage.nomeCaixa2 = "Módulo RD 55";
                }
            }
        }

        private void navi_control_PaneClosed(NavigationView sender, NavigationViewPaneClosingEventArgs args)
        {
            Conexao.IsExpanded = false;
            Banco.IsExpanded = false;            
            Arquivos.IsExpanded = false;            
        }

        private void navi_control_PaneOpening(NavigationView sender, object args)
        {
            Conexao.IsExpanded = true;
            Banco.IsExpanded = true;
            Arquivos.IsExpanded = true;
        }


        private async void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ping ping = new Ping();

                if (checkEstacaoCarregada())
                {
                    bool confirmacao = await ShowDialog("Confirmar envio de configuração para o PLC?");

                    if (confirmacao)
                    {
                        // Usar SendPingAsync para ser assíncrono
                        PingReply resp = await ping.SendPingAsync(txb_ip.Text);

                        if (resp.Status == IPStatus.Success)
                        {
                            if (btn_Equipamento.Content.ToString() == "Ansitra QR")
                            {
                                var dado = GerarDadosPLC();

                                int tamanho = dado.Length;

                                dadosPLC = $"Caracteres:{tamanho} // EDCF{dado}";
                                flagSend = 1;
                                statusSend = 1;
                                nBytes = 6;

                                Debug.WriteLine(dado);

                                Conect();
                            }
                            else
                            {
                                var dado = GerarDadosWT();

                                dadosPLC = "EDCF" + dado;
                                flagSend = 1;
                                statusSend = 1;
                                nBytes = 6;

                                Debug.WriteLine(dado);

                                Conect();
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Falha ao pingar o PLC.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao enviar configuração: {ex.Message}");
            }
        }


        private string GerarDadosWT()
        {
            //PLC 1
            string ipc_plc1 = conexaoPage.ip_estacao;
            string gateway_plc1 = conexaoPage.gateway_estacao;
            string mask_plc1 = conexaoPage.mask_estacao;
            //Sensor1
            string ip_sensor1 = conexaoPage.ip_modulo;
            string gateway_sensor1 = conexaoPage.mask_modulo;
            string mask_sensor1 = conexaoPage.gateway_modulo;
            //PLC 2
            string ip_plc2 = conexaoPage.Ip_plc2;
            string gateway_plc2 = conexaoPage.Gateway_plc2;
            string mask_plc2 = conexaoPage.Mask_plc2;
            //SENSOR 2
            string ip_sensor2 = conexaoPage.ip_estacao;
            string gateway_sensor2 = conexaoPage.gateway_estacao;
            string mask_sensor2 = conexaoPage.mask_estacao;

            string tipo = bancoPage.tipo_banco;
            string host = bancoPage.host_banco;
            string usuario = bancoPage.usuario_banco;
            string porta = bancoPage.porta_banco;
            string exibicao = bancoPage.exibicao_banco;
            string servidor = bancoPage.servidor_banco;
            string senha = bancoPage.senha_banco;

            string local = dadosEstacaoPage.cbx_Local.SelectedItem?.ToString();
            string uf = dadosEstacaoPage.cbx_Uf.SelectedItem?.ToString();


            string nomeFaixa1Text = faixasPage.NomeFaixa1;
            string nomeFaixa2Text = faixasPage.NomeFaixa2;
            string nomeFaixa3Text = faixasPage.NomeFaixa3;
            string nomeFaixa4Text = faixasPage.NomeFaixa4;
            string nomeFaixa5Text = faixasPage.NomeFaixa5;
            string nomeFaixa6Text = faixasPage.NomeFaixa6;
            string nomeFaixa7Text = faixasPage.NomeFaixa7;
            string nomeFaixa8Text = faixasPage.NomeFaixa8;
            string nomeFaixa9Text = faixasPage.NomeFaixa9;
            string nomeFaixa10Text = faixasPage.NomeFaixa10;
            string nomeFaixa11Text = faixasPage.NomeFaixa11;
            string nomeFaixa12Text = faixasPage.NomeFaixa12;
            string nomeFaixa13Text = faixasPage.NomeFaixa13;
            string nomeFaixa14Text = faixasPage.NomeFaixa14;
            string nomeFaixa15Text = faixasPage.NomeFaixa15;
            string nomeFaixa16Text = faixasPage.NomeFaixa16;

            string compLaco1Text = App.MainViewModel.CompLaco1;
            string compLaco2Text = App.MainViewModel.CompLaco2;
            string compLaco3Text = App.MainViewModel.CompLaco3;
            string compLaco4Text = App.MainViewModel.CompLaco4;
            string compLaco5Text = App.MainViewModel.CompLaco5;
            string compLaco6Text = App.MainViewModel.CompLaco6;
            string compLaco7Text = App.MainViewModel.CompLaco7;
            string compLaco8Text = App.MainViewModel.CompLaco8;
            string compLaco9Text = App.MainViewModel.CompLaco9;
            string compLaco10Text = App.MainViewModel.CompLaco10;
            string compLaco11Text = App.MainViewModel.CompLaco11;
            string compLaco12Text = App.MainViewModel.CompLaco12;
            string compLaco13Text = App.MainViewModel.CompLaco13;
            string compLaco14Text = App.MainViewModel.CompLaco14;
            string compLaco15Text = App.MainViewModel.CompLaco15;
            string compLaco16Text = App.MainViewModel.CompLaco16;

            string disLaco1Text = App.MainViewModel.DistLaco1;
            string disLaco2Text = App.MainViewModel.DistLaco2;
            string disLaco3Text = App.MainViewModel.DistLaco3;
            string disLaco4Text = App.MainViewModel.DistLaco4;
            string disLaco5Text = App.MainViewModel.DistLaco5;
            string disLaco6Text = App.MainViewModel.DistLaco6;
            string disLaco7Text = App.MainViewModel.DistLaco7;
            string disLaco8Text = App.MainViewModel.DistLaco8;
            string disLaco9Text = App.MainViewModel.DistLaco9;
            string disLaco10Text = App.MainViewModel.DistLaco10;
            string disLaco11Text = App.MainViewModel.DistLaco11;
            string disLaco12Text = App.MainViewModel.DistLaco12;
            string disLaco13Text = App.MainViewModel.DistLaco13;
            string disLaco14Text = App.MainViewModel.DistLaco14;
            string disLaco15Text = App.MainViewModel.DistLaco15;
            string disLaco16Text = App.MainViewModel.DistLaco16;

            string compMaiorValText = App.MainViewModel.CompMaior;
            string compMaiorGraText = App.MainViewModel.CompMaiorGravar;
            string compMenorValText = App.MainViewModel.CompMenor;
            string compMenorGraText = App.MainViewModel.CompMenorGravar;
            string veloMaiorValText = App.MainViewModel.VelocMaior;
            string veloMaiorGraText = App.MainViewModel.VelocMaiorGravar;
            string veloMenorValText = App.MainViewModel.VelocMenor;
            string valoMenorGraText = App.MainViewModel.VelocMenorGravar;

            string tipo1 = App.MainViewModel.ClassTipo1;
            string tipo2 = App.MainViewModel.ClassTipo2;
            string tipo3 = App.MainViewModel.ClassTipo3;
            string tipo4 = App.MainViewModel.ClassTipo4;
            string tipo5 = App.MainViewModel.ClassTipo5;
            string tipo6 = App.MainViewModel.ClassTipo6;
            string tipo7 = App.MainViewModel.ClassTipo7;
            string tipo8 = App.MainViewModel.ClassTipo8;
            string tipo9 = App.MainViewModel.ClassTipo9;
            string tipo10 = App.MainViewModel.ClassTipo10;
            string tipo11 = App.MainViewModel.ClassTipo11;
            string tipo12 = App.MainViewModel.ClassTipo12;
            string tipo13 = App.MainViewModel.ClassTipo13;
            string tipo14 = App.MainViewModel.ClassTipo14;
            string tipo15 = App.MainViewModel.ClassTipo15;

            string nome1 = App.MainViewModel.ClassNome1;
            string nome2 = App.MainViewModel.ClassNome2;
            string nome3 = App.MainViewModel.ClassNome3;
            string nome4 = App.MainViewModel.ClassNome4;
            string nome5 = App.MainViewModel.ClassNome5;
            string nome6 = App.MainViewModel.ClassNome6;
            string nome7 = App.MainViewModel.ClassNome7;
            string nome8 = App.MainViewModel.ClassNome8;
            string nome9 = App.MainViewModel.ClassNome9;
            string nome10 = App.MainViewModel.ClassQtdEixo10;
            string nome11 = App.MainViewModel.ClassQtdEixo11;
            string nome12 = App.MainViewModel.ClassQtdEixo12;
            string nome13 = App.MainViewModel.ClassQtdEixo13;
            string nome14 = App.MainViewModel.ClassQtdEixo14;
            string nome15 = App.MainViewModel.ClassQtdEixo15;

            string inicio1 = App.MainViewModel.ClassInicio1;
            string inicio2 = App.MainViewModel.ClassInicio2;
            string inicio3 = App.MainViewModel.ClassInicio3;
            string inicio4 = App.MainViewModel.ClassInicio4;
            string inicio5 = App.MainViewModel.ClassInicio5;
            string inicio6 = App.MainViewModel.ClassInicio6;
            string inicio7 = App.MainViewModel.ClassInicio7;
            string inicio8 = App.MainViewModel.ClassInicio8;
            string inicio9 = App.MainViewModel.ClassInicio9;
            string inicio10 = App.MainViewModel.ClassInicio10;
            string inicio11 = App.MainViewModel.ClassInicio11;
            string inicio12 = App.MainViewModel.ClassInicio12;
            string inicio13 = App.MainViewModel.ClassInicio13;
            string inicio14 = App.MainViewModel.ClassInicio14;
            string inicio15 = App.MainViewModel.ClassInicio15;

            string fim1 = App.MainViewModel.ClassFim1;
            string fim2 = App.MainViewModel.ClassFim2;
            string fim3 = App.MainViewModel.ClassFim3;
            string fim4 = App.MainViewModel.ClassFim4;
            string fim5 = App.MainViewModel.ClassFim5;
            string fim6 = App.MainViewModel.ClassFim6;
            string fim7 = App.MainViewModel.ClassFim7;
            string fim8 = App.MainViewModel.ClassFim8;
            string fim9 = App.MainViewModel.ClassFim9;
            string fim10 = App.MainViewModel.ClassFim10;
            string fim11 = App.MainViewModel.ClassFim11;
            string fim12 = App.MainViewModel.ClassFim12;
            string fim13 = App.MainViewModel.ClassFim13;
            string fim14 = App.MainViewModel.ClassFim14;
            string fim15 = App.MainViewModel.ClassFim15;

            string qtdEixo1 = App.MainViewModel.ClassQtdEixo1;
            string qtdEixo2 = App.MainViewModel.ClassQtdEixo2;
            string qtdEixo3 = App.MainViewModel.ClassQtdEixo3;
            string qtdEixo4 = App.MainViewModel.ClassQtdEixo4;
            string qtdEixo5 = App.MainViewModel.ClassQtdEixo5;
            string qtdEixo6 = App.MainViewModel.ClassQtdEixo6;
            string qtdEixo7 = App.MainViewModel.ClassQtdEixo7;
            string qtdEixo8 = App.MainViewModel.ClassQtdEixo8;
            string qtdEixo9 = App.MainViewModel.ClassQtdEixo9;
            string qtdEixo10 = App.MainViewModel.ClassQtdEixo10;
            string qtdEixo11 = App.MainViewModel.ClassQtdEixo11;
            string qtdEixo12 = App.MainViewModel.ClassQtdEixo12;
            string qtdEixo13 = App.MainViewModel.ClassQtdEixo13;
            string qtdEixo14 = App.MainViewModel.ClassQtdEixo14;
            string qtdEixo15 = App.MainViewModel.ClassQtdEixo15;



            // Converte para JSON
            var objeto = new DadosJson
            {
                ip_servidor = txt_ipValor.Text,

                rede_plc1 = new RedeEstacao
                {
                    ip = string.IsNullOrWhiteSpace(ipc_plc1) ? "" : ipc_plc1,
                    mask = string.IsNullOrWhiteSpace(mask_plc1) ? "" : mask_plc1,
                    gateway = string.IsNullOrWhiteSpace(gateway_plc1) ? "" : gateway_plc1,
                },
                rede_sensor1 = new RedeModulo
                {
                    ip = string.IsNullOrWhiteSpace(ip_sensor1) ? "" : ip_sensor1,
                    mask = string.IsNullOrWhiteSpace(mask_sensor1) ? "" : mask_sensor1,
                    gateway = string.IsNullOrWhiteSpace(gateway_sensor1) ? "" : gateway_sensor1,
                },
                rede_plc2 = new RedeEstacao
                {
                    ip = string.IsNullOrWhiteSpace(ip_plc2) ? "" : ip_plc2,
                    mask = string.IsNullOrWhiteSpace(mask_plc2) ? "" : mask_plc2,
                    gateway = string.IsNullOrWhiteSpace(gateway_sensor1) ? "" : gateway_sensor1,
                },
                rede_sensor2 = new RedeModulo
                {
                    ip = string.IsNullOrWhiteSpace(ip_sensor2) ? "" : ip_sensor2,
                    mask = string.IsNullOrWhiteSpace(mask_sensor2) ? "" : mask_sensor2,
                    gateway = string.IsNullOrWhiteSpace(gateway_sensor2) ? "" : gateway_sensor2,
                },
                banco_dados = new BancoDados
                {
                    tipo = string.IsNullOrWhiteSpace(tipo) ? "" : tipo,
                    host_banco = string.IsNullOrWhiteSpace(host) ? "" : host,
                    usuario = string.IsNullOrWhiteSpace(usuario) ? "" : usuario,
                    porta = string.IsNullOrWhiteSpace(porta) ? "" : porta,
                    exibicao = string.IsNullOrWhiteSpace(exibicao) ? "" : exibicao,
                    servidor = string.IsNullOrWhiteSpace(servidor) ? "" : servidor,
                    senha = string.IsNullOrWhiteSpace(senha) ? "" : Convert.ToBase64String(Encoding.UTF8.GetBytes(senha)),
                },
                num_estacao = string.IsNullOrWhiteSpace(txt_numeroValor.Text) ? "" : txt_numeroValor.Text,
                km_rodovia = string.IsNullOrWhiteSpace(txt_kmValor.Text) ? "" : txt_kmValor.Text,
                id_rodovia = string.IsNullOrWhiteSpace(txt_rodoviaValor.Text) ? "" : txt_rodoviaValor.Text,
                local_rodovia = string.IsNullOrWhiteSpace(local) ? "" : local,
                uf_rodovia = string.IsNullOrWhiteSpace(uf) ? "" : uf,

                id_faixa = new IdFaixa
                {
                    FX1 = string.IsNullOrWhiteSpace(nomeFaixa1Text) ? "" : nomeFaixa1Text,
                    FX2 = string.IsNullOrWhiteSpace(nomeFaixa2Text) ? "" : nomeFaixa2Text,
                    FX3 = string.IsNullOrWhiteSpace(nomeFaixa3Text) ? "" : nomeFaixa3Text,
                    FX4 = string.IsNullOrWhiteSpace(nomeFaixa4Text) ? "" : nomeFaixa4Text,
                    FX5 = string.IsNullOrWhiteSpace(nomeFaixa5Text) ? "" : nomeFaixa5Text,
                    FX6 = string.IsNullOrWhiteSpace(nomeFaixa6Text) ? "" : nomeFaixa6Text,
                    FX7 = string.IsNullOrWhiteSpace(nomeFaixa7Text) ? "" : nomeFaixa7Text,
                    FX8 = string.IsNullOrWhiteSpace(nomeFaixa8Text) ? "" : nomeFaixa8Text,
                    FX9 = string.IsNullOrWhiteSpace(nomeFaixa9Text) ? "" : nomeFaixa9Text,
                    FX10 = string.IsNullOrWhiteSpace(nomeFaixa10Text) ? "" : nomeFaixa10Text,
                    FX11 = string.IsNullOrWhiteSpace(nomeFaixa11Text) ? "" : nomeFaixa11Text,
                    FX12 = string.IsNullOrWhiteSpace(nomeFaixa12Text) ? "" : nomeFaixa12Text,
                    FX13 = string.IsNullOrWhiteSpace(nomeFaixa13Text) ? "" : nomeFaixa13Text,
                    FX14 = string.IsNullOrWhiteSpace(nomeFaixa14Text) ? "" : nomeFaixa14Text,
                    FX15 = string.IsNullOrWhiteSpace(nomeFaixa15Text) ? "" : nomeFaixa15Text,
                    FX16 = string.IsNullOrWhiteSpace(nomeFaixa16Text) ? "" : nomeFaixa16Text,
                },
                dist_laco = new DistLaco
                {
                    FX1 = ParseToIntOrZero(disLaco1Text),
                    FX2 = ParseToIntOrZero(disLaco2Text),
                    FX3 = ParseToIntOrZero(disLaco3Text),
                    FX4 = ParseToIntOrZero(disLaco4Text),
                    FX5 = ParseToIntOrZero(disLaco5Text),
                    FX6 = ParseToIntOrZero(disLaco6Text),
                    FX7 = ParseToIntOrZero(disLaco7Text),
                    FX8 = ParseToIntOrZero(disLaco8Text),
                    FX9 = ParseToIntOrZero(disLaco9Text),
                    FX10 = ParseToIntOrZero(disLaco10Text),
                    FX11 = ParseToIntOrZero(disLaco11Text),
                    FX12 = ParseToIntOrZero(disLaco12Text),
                    FX13 = ParseToIntOrZero(disLaco13Text),
                    FX14 = ParseToIntOrZero(disLaco14Text),
                    FX15 = ParseToIntOrZero(disLaco15Text),
                    FX16 = ParseToIntOrZero(disLaco16Text)
                },
                comp_laco = new CompLaco
                {
                    FX1 = ParseToIntOrZero(compLaco1Text),
                    FX2 = ParseToIntOrZero(compLaco2Text),
                    FX3 = ParseToIntOrZero(compLaco3Text),
                    FX4 = ParseToIntOrZero(compLaco4Text),
                    FX5 = ParseToIntOrZero(compLaco5Text),
                    FX6 = ParseToIntOrZero(compLaco6Text),
                    FX7 = ParseToIntOrZero(compLaco7Text),
                    FX8 = ParseToIntOrZero(compLaco8Text),
                    FX9 = ParseToIntOrZero(compLaco9Text),
                    FX10 = ParseToIntOrZero(compLaco10Text),
                    FX11 = ParseToIntOrZero(compLaco11Text),
                    FX12 = ParseToIntOrZero(compLaco12Text),
                    FX13 = ParseToIntOrZero(compLaco13Text),
                    FX14 = ParseToIntOrZero(compLaco14Text),
                    FX15 = ParseToIntOrZero(compLaco15Text),
                    FX16 = ParseToIntOrZero(compLaco16Text)
                },

                comp_maior_valor = string.IsNullOrWhiteSpace(compMaiorValText) ? 0 : int.Parse(compMaiorValText),
                comp_maior_gravar = string.IsNullOrWhiteSpace(compMaiorGraText) ? 0 : int.Parse(compMaiorGraText),
                comp_menor_valor = string.IsNullOrWhiteSpace(compMenorValText) ? 0 : int.Parse(compMenorValText),
                comp_menor_gravar = string.IsNullOrWhiteSpace(compMenorGraText) ? 0 : int.Parse(compMenorGraText),
                vel_maior_valor = string.IsNullOrWhiteSpace(veloMaiorValText) ? 0 : int.Parse(veloMaiorValText),
                vel_maior_gravar = string.IsNullOrWhiteSpace(veloMaiorGraText) ? 0 : int.Parse(veloMaiorGraText),
                vel_menor_valor = string.IsNullOrWhiteSpace(veloMenorValText) ? 0 : int.Parse(veloMenorValText),
                vel_menor_gravar = string.IsNullOrWhiteSpace(valoMenorGraText) ? 0 : int.Parse(valoMenorGraText),


                class_comp = new Dictionary<string, ClassComp>
                {
                    { "CC1", new ClassComp { tipo = ParseToIntOrZero(tipo1), nome = nome1 ?? "", inicio = ConverterStringParaInt(inicio1), fim = ConverterStringParaInt(fim1), eixo = ConverterStringParaInt(qtdEixo1) } },
                    { "CC2", new ClassComp { tipo = ParseToIntOrZero(tipo2), nome = nome2 ?? "", inicio = ConverterStringParaInt(inicio2), fim = ConverterStringParaInt(fim2), eixo = ConverterStringParaInt(qtdEixo2) } },
                    { "CC3", new ClassComp { tipo = ParseToIntOrZero(tipo3), nome = nome3 ?? "", inicio = ConverterStringParaInt(inicio3), fim = ConverterStringParaInt(fim3), eixo = ConverterStringParaInt(qtdEixo3) } },
                    { "CC4", new ClassComp { tipo = ParseToIntOrZero(tipo4), nome = nome4 ?? "", inicio = ConverterStringParaInt(inicio4), fim = ConverterStringParaInt(fim4), eixo = ConverterStringParaInt(qtdEixo4) } },
                    { "CC5", new ClassComp { tipo = ParseToIntOrZero(tipo5), nome = nome5 ?? "", inicio = ConverterStringParaInt(inicio5), fim = ConverterStringParaInt(fim5), eixo = ConverterStringParaInt(qtdEixo5) } },
                    { "CC6", new ClassComp { tipo = ParseToIntOrZero(tipo6), nome = nome6 ?? "", inicio = ConverterStringParaInt(inicio6), fim = ConverterStringParaInt(fim6), eixo = ConverterStringParaInt(qtdEixo6) } },
                    { "CC7", new ClassComp { tipo = ParseToIntOrZero(tipo7), nome = nome7 ?? "", inicio = ConverterStringParaInt(inicio7), fim = ConverterStringParaInt(fim7), eixo = ConverterStringParaInt(qtdEixo7) } },
                    { "CC8", new ClassComp { tipo = ParseToIntOrZero(tipo8), nome = nome8 ?? "", inicio = ConverterStringParaInt(inicio8), fim = ConverterStringParaInt(fim8), eixo = ConverterStringParaInt(qtdEixo8) } },
                    { "CC9", new ClassComp { tipo = ParseToIntOrZero(tipo9), nome = nome9 ?? "", inicio = ConverterStringParaInt(inicio9), fim = ConverterStringParaInt(fim9), eixo = ConverterStringParaInt(qtdEixo9) } },
                    { "CC10", new ClassComp { tipo = ParseToIntOrZero(tipo10), nome = nome10 ?? "", inicio = ConverterStringParaInt(inicio10), fim = ConverterStringParaInt(fim10), eixo = ConverterStringParaInt(qtdEixo10) } },
                    { "CC11", new ClassComp { tipo = ParseToIntOrZero(tipo11), nome = nome11 ?? "", inicio = ConverterStringParaInt(inicio11), fim = ConverterStringParaInt(fim11), eixo = ConverterStringParaInt(qtdEixo11) } },
                    { "CC12", new ClassComp { tipo = ParseToIntOrZero(tipo12), nome = nome12 ?? "", inicio = ConverterStringParaInt(inicio12), fim = ConverterStringParaInt(fim12), eixo = ConverterStringParaInt(qtdEixo12) } },
                    { "CC13", new ClassComp { tipo = ParseToIntOrZero(tipo13), nome = nome13 ?? "", inicio = ConverterStringParaInt(inicio13), fim = ConverterStringParaInt(fim13), eixo = ConverterStringParaInt(qtdEixo13) } },
                    { "CC14", new ClassComp { tipo = ParseToIntOrZero(tipo14), nome = nome14 ?? "", inicio = ConverterStringParaInt(inicio14), fim = ConverterStringParaInt(fim14), eixo = ConverterStringParaInt(qtdEixo14) } },
                    { "CC15", new ClassComp { tipo = ParseToIntOrZero(tipo15), nome = nome15 ?? "", inicio = ConverterStringParaInt(inicio15), fim = ConverterStringParaInt(fim15), eixo = ConverterStringParaInt(qtdEixo15) } }
                }
            };
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string jsonString = JsonSerializer.Serialize(objeto, options);
            return jsonString;
        }

        private string GerarDadosPLC()
        {
            //PLC
            string ip_estacao = conexaoPage.ip_estacao;
            string gateway_estacao = conexaoPage.gateway_estacao;
            string mask_estacao = conexaoPage.mask_estacao;
            //Modulo
            string ip_modulo = conexaoPage.ip_modulo;
            string mask_modulo = conexaoPage.mask_modulo;
            string gateway_modulo = conexaoPage.gateway_modulo;

            string tipo = bancoPage.tipo_banco;
            string host = bancoPage.host_banco;
            string usuario = bancoPage.usuario_banco;
            string porta = bancoPage.porta_banco;
            string exibicao = bancoPage.exibicao_banco;
            string servidor = bancoPage.servidor_banco;
            string senha = bancoPage.senha_banco;

            string local = dadosEstacaoPage.cbx_Local.SelectedItem?.ToString();
            string uf = dadosEstacaoPage.cbx_Uf.SelectedItem?.ToString();


            string nomeFaixa1Text = faixasPage.NomeFaixa1;
            string nomeFaixa2Text = faixasPage.NomeFaixa2;
            string nomeFaixa3Text = faixasPage.NomeFaixa3;
            string nomeFaixa4Text = faixasPage.NomeFaixa4;
            string nomeFaixa5Text = faixasPage.NomeFaixa5;
            string nomeFaixa6Text = faixasPage.NomeFaixa6;
            string nomeFaixa7Text = faixasPage.NomeFaixa7;
            string nomeFaixa8Text = faixasPage.NomeFaixa8;
            string nomeFaixa9Text = faixasPage.NomeFaixa9;
            string nomeFaixa10Text = faixasPage.NomeFaixa10;
            string nomeFaixa11Text = faixasPage.NomeFaixa11;
            string nomeFaixa12Text = faixasPage.NomeFaixa12;
            string nomeFaixa13Text = faixasPage.NomeFaixa13;
            string nomeFaixa14Text = faixasPage.NomeFaixa14;
            string nomeFaixa15Text = faixasPage.NomeFaixa15;
            string nomeFaixa16Text = faixasPage.NomeFaixa16;

            string compLaco1Text = App.MainViewModel.CompLaco1;
            string compLaco2Text = App.MainViewModel.CompLaco2;
            string compLaco3Text = App.MainViewModel.CompLaco3;
            string compLaco4Text = App.MainViewModel.CompLaco4;
            string compLaco5Text = App.MainViewModel.CompLaco5;
            string compLaco6Text = App.MainViewModel.CompLaco6;
            string compLaco7Text = App.MainViewModel.CompLaco7;
            string compLaco8Text = App.MainViewModel.CompLaco8;
            string compLaco9Text = App.MainViewModel.CompLaco9;
            string compLaco10Text = App.MainViewModel.CompLaco10;
            string compLaco11Text = App.MainViewModel.CompLaco11;
            string compLaco12Text = App.MainViewModel.CompLaco12;
            string compLaco13Text = App.MainViewModel.CompLaco13;
            string compLaco14Text = App.MainViewModel.CompLaco14;
            string compLaco15Text = App.MainViewModel.CompLaco15;
            string compLaco16Text = App.MainViewModel.CompLaco16;

            string disLaco1Text = App.MainViewModel.DistLaco1;
            string disLaco2Text = App.MainViewModel.DistLaco2;
            string disLaco3Text = App.MainViewModel.DistLaco3;
            string disLaco4Text = App.MainViewModel.DistLaco4;
            string disLaco5Text = App.MainViewModel.DistLaco5;
            string disLaco6Text = App.MainViewModel.DistLaco6;
            string disLaco7Text = App.MainViewModel.DistLaco7;
            string disLaco8Text = App.MainViewModel.DistLaco8;
            string disLaco9Text = App.MainViewModel.DistLaco9;
            string disLaco10Text = App.MainViewModel.DistLaco10;
            string disLaco11Text = App.MainViewModel.DistLaco11;
            string disLaco12Text = App.MainViewModel.DistLaco12;
            string disLaco13Text = App.MainViewModel.DistLaco13;
            string disLaco14Text = App.MainViewModel.DistLaco14;
            string disLaco15Text = App.MainViewModel.DistLaco15;
            string disLaco16Text = App.MainViewModel.DistLaco16;

            string compMaiorValText = App.MainViewModel.CompMaior;
            string compMaiorGraText = App.MainViewModel.CompMaiorGravar;
            string compMenorValText = App.MainViewModel.CompMenor;
            string compMenorGraText = App.MainViewModel.CompMenorGravar;
            string veloMaiorValText = App.MainViewModel.VelocMaior;
            string veloMaiorGraText = App.MainViewModel.VelocMaiorGravar;
            string veloMenorValText = App.MainViewModel.VelocMenor;
            string valoMenorGraText = App.MainViewModel.VelocMenorGravar;

            string tipo1 = App.MainViewModel.ClassTipo1;
            string tipo2 = App.MainViewModel.ClassTipo2;
            string tipo3 = App.MainViewModel.ClassTipo3;
            string tipo4 = App.MainViewModel.ClassTipo4;
            string tipo5 = App.MainViewModel.ClassTipo5;
            string tipo6 = App.MainViewModel.ClassTipo6;
            string tipo7 = App.MainViewModel.ClassTipo7;
            string tipo8 = App.MainViewModel.ClassTipo8;
            string tipo9 = App.MainViewModel.ClassTipo9;
            string tipo10 = App.MainViewModel.ClassTipo10;
            string tipo11 = App.MainViewModel.ClassTipo11;
            string tipo12 = App.MainViewModel.ClassTipo12;
            string tipo13 = App.MainViewModel.ClassTipo13;
            string tipo14 = App.MainViewModel.ClassTipo14;
            string tipo15 = App.MainViewModel.ClassTipo15;

            string nome1 = App.MainViewModel.ClassNome1;
            string nome2 = App.MainViewModel.ClassNome2;
            string nome3 = App.MainViewModel.ClassNome3;
            string nome4 = App.MainViewModel.ClassNome4;
            string nome5 = App.MainViewModel.ClassNome5;
            string nome6 = App.MainViewModel.ClassNome6;
            string nome7 = App.MainViewModel.ClassNome7;
            string nome8 = App.MainViewModel.ClassNome8;
            string nome9 = App.MainViewModel.ClassNome9;
            string nome10 = App.MainViewModel.ClassQtdEixo10;
            string nome11 = App.MainViewModel.ClassQtdEixo11;
            string nome12 = App.MainViewModel.ClassQtdEixo12;
            string nome13 = App.MainViewModel.ClassQtdEixo13;
            string nome14 = App.MainViewModel.ClassQtdEixo14;
            string nome15 = App.MainViewModel.ClassQtdEixo15;

            string inicio1 = App.MainViewModel.ClassInicio1;
            string inicio2 = App.MainViewModel.ClassInicio2;
            string inicio3 = App.MainViewModel.ClassInicio3;
            string inicio4 = App.MainViewModel.ClassInicio4;
            string inicio5 = App.MainViewModel.ClassInicio5;
            string inicio6 = App.MainViewModel.ClassInicio6;
            string inicio7 = App.MainViewModel.ClassInicio7;
            string inicio8 = App.MainViewModel.ClassInicio8;
            string inicio9 = App.MainViewModel.ClassInicio9;
            string inicio10 = App.MainViewModel.ClassInicio10;
            string inicio11 = App.MainViewModel.ClassInicio11;
            string inicio12 = App.MainViewModel.ClassInicio12;
            string inicio13 = App.MainViewModel.ClassInicio13;
            string inicio14 = App.MainViewModel.ClassInicio14;
            string inicio15 = App.MainViewModel.ClassInicio15;

            string fim1 = App.MainViewModel.ClassFim1;
            string fim2 = App.MainViewModel.ClassFim2;
            string fim3 = App.MainViewModel.ClassFim3;
            string fim4 = App.MainViewModel.ClassFim4;
            string fim5 = App.MainViewModel.ClassFim5;
            string fim6 = App.MainViewModel.ClassFim6;
            string fim7 = App.MainViewModel.ClassFim7;
            string fim8 = App.MainViewModel.ClassFim8;
            string fim9 = App.MainViewModel.ClassFim9;
            string fim10 = App.MainViewModel.ClassFim10;
            string fim11 = App.MainViewModel.ClassFim11;
            string fim12 = App.MainViewModel.ClassFim12;
            string fim13 = App.MainViewModel.ClassFim13;
            string fim14 = App.MainViewModel.ClassFim14;
            string fim15 = App.MainViewModel.ClassFim15;

            string qtdEixo1 = App.MainViewModel.ClassQtdEixo1;
            string qtdEixo2 = App.MainViewModel.ClassQtdEixo2;
            string qtdEixo3 = App.MainViewModel.ClassQtdEixo3;
            string qtdEixo4 = App.MainViewModel.ClassQtdEixo4;
            string qtdEixo5 = App.MainViewModel.ClassQtdEixo5;
            string qtdEixo6 = App.MainViewModel.ClassQtdEixo6;
            string qtdEixo7 = App.MainViewModel.ClassQtdEixo7;
            string qtdEixo8 = App.MainViewModel.ClassQtdEixo8;
            string qtdEixo9 = App.MainViewModel.ClassQtdEixo9;
            string qtdEixo10 = App.MainViewModel.ClassQtdEixo10;
            string qtdEixo11 = App.MainViewModel.ClassQtdEixo11;
            string qtdEixo12 = App.MainViewModel.ClassQtdEixo12;
            string qtdEixo13 = App.MainViewModel.ClassQtdEixo13;
            string qtdEixo14 = App.MainViewModel.ClassQtdEixo14;
            string qtdEixo15 = App.MainViewModel.ClassQtdEixo15;



            // Converte para JSON
            var objeto = new DadosJson
            {
                ip_servidor = txt_ipValor.Text,

                rede_estacao = new RedeEstacao
                {
                    ip = string.IsNullOrWhiteSpace(ip_estacao) ? "" : ip_estacao,
                    mask = string.IsNullOrWhiteSpace(mask_estacao) ? "" : mask_estacao,
                    gateway = string.IsNullOrWhiteSpace(gateway_estacao) ? "" : gateway_estacao,
                },
                rede_modulo = new RedeModulo
                {
                    ip = string.IsNullOrWhiteSpace(ip_modulo) ? "" : ip_modulo,
                    mask = string.IsNullOrWhiteSpace(mask_modulo) ? "" : mask_modulo,
                    gateway = string.IsNullOrWhiteSpace(gateway_modulo) ? "" : gateway_modulo,
                },
                banco_dados = new BancoDados
                {
                    tipo = string.IsNullOrWhiteSpace(tipo) ? "" : tipo,
                    host_banco = string.IsNullOrWhiteSpace(host) ? "" : host,
                    usuario = string.IsNullOrWhiteSpace(usuario) ? "" : usuario,
                    porta = string.IsNullOrWhiteSpace(porta) ? "" : porta,
                    exibicao = string.IsNullOrWhiteSpace(exibicao) ? "" : exibicao,
                    servidor = string.IsNullOrWhiteSpace(servidor) ? "" : servidor,
                    senha = string.IsNullOrWhiteSpace(senha) ? "" : Convert.ToBase64String(Encoding.UTF8.GetBytes(senha)),
                },
                num_estacao = string.IsNullOrWhiteSpace(txt_numeroValor.Text) ? "" : txt_numeroValor.Text,
                km_rodovia = string.IsNullOrWhiteSpace(txt_kmValor.Text) ? "" : txt_kmValor.Text,
                id_rodovia = string.IsNullOrWhiteSpace(txt_rodoviaValor.Text) ? "" : txt_rodoviaValor.Text,
                local_rodovia = string.IsNullOrWhiteSpace(local) ? "" : local,
                uf_rodovia = string.IsNullOrWhiteSpace(uf) ? "" : uf,

                id_faixa = new IdFaixa
                {
                    FX1 = string.IsNullOrWhiteSpace(nomeFaixa1Text) ? "" : nomeFaixa1Text,
                    FX2 = string.IsNullOrWhiteSpace(nomeFaixa2Text) ? "" : nomeFaixa2Text,
                    FX3 = string.IsNullOrWhiteSpace(nomeFaixa3Text) ? "" : nomeFaixa3Text,
                    FX4 = string.IsNullOrWhiteSpace(nomeFaixa4Text) ? "" : nomeFaixa4Text,
                    FX5 = string.IsNullOrWhiteSpace(nomeFaixa5Text) ? "" : nomeFaixa5Text,
                    FX6 = string.IsNullOrWhiteSpace(nomeFaixa6Text) ? "" : nomeFaixa6Text,
                    FX7 = string.IsNullOrWhiteSpace(nomeFaixa7Text) ? "" : nomeFaixa7Text,
                    FX8 = string.IsNullOrWhiteSpace(nomeFaixa8Text) ? "" : nomeFaixa8Text,
                    FX9 = string.IsNullOrWhiteSpace(nomeFaixa9Text) ? "" : nomeFaixa9Text,
                    FX10 = string.IsNullOrWhiteSpace(nomeFaixa10Text) ? "" : nomeFaixa10Text,
                    FX11 = string.IsNullOrWhiteSpace(nomeFaixa11Text) ? "" : nomeFaixa11Text,
                    FX12 = string.IsNullOrWhiteSpace(nomeFaixa12Text) ? "" : nomeFaixa12Text,
                    FX13 = string.IsNullOrWhiteSpace(nomeFaixa13Text) ? "" : nomeFaixa13Text,
                    FX14 = string.IsNullOrWhiteSpace(nomeFaixa14Text) ? "" : nomeFaixa14Text,
                    FX15 = string.IsNullOrWhiteSpace(nomeFaixa15Text) ? "" : nomeFaixa15Text,
                    FX16 = string.IsNullOrWhiteSpace(nomeFaixa16Text) ? "" : nomeFaixa16Text,
                },
                dist_laco = new DistLaco
                {
                    FX1 = string.IsNullOrWhiteSpace(disLaco1Text) ? 0 : int.Parse(disLaco1Text),
                    FX2 = string.IsNullOrWhiteSpace(disLaco2Text) ? 0 : int.Parse(disLaco2Text),
                    FX3 = string.IsNullOrWhiteSpace(disLaco3Text) ? 0 : int.Parse(disLaco3Text),
                    FX4 = string.IsNullOrWhiteSpace(disLaco4Text) ? 0 : int.Parse(disLaco4Text),
                    FX5 = string.IsNullOrWhiteSpace(disLaco5Text) ? 0 : int.Parse(disLaco5Text),
                    FX6 = string.IsNullOrWhiteSpace(disLaco6Text) ? 0 : int.Parse(disLaco6Text),
                    FX7 = string.IsNullOrWhiteSpace(disLaco7Text) ? 0 : int.Parse(disLaco7Text),
                    FX8 = string.IsNullOrWhiteSpace(disLaco8Text) ? 0 : int.Parse(disLaco8Text),
                    FX9 = string.IsNullOrWhiteSpace(disLaco9Text) ? 0 : int.Parse(disLaco9Text),
                    FX10 = string.IsNullOrWhiteSpace(disLaco10Text) ? 0 : int.Parse(disLaco10Text),
                    FX11 = string.IsNullOrWhiteSpace(disLaco11Text) ? 0 : int.Parse(disLaco11Text),
                    FX12 = string.IsNullOrWhiteSpace(disLaco12Text) ? 0 : int.Parse(disLaco12Text),
                    FX13 = string.IsNullOrWhiteSpace(disLaco13Text) ? 0 : int.Parse(disLaco13Text),
                    FX14 = string.IsNullOrWhiteSpace(disLaco14Text) ? 0 : int.Parse(disLaco14Text),
                    FX15 = string.IsNullOrWhiteSpace(disLaco15Text) ? 0 : int.Parse(disLaco15Text),
                    FX16 = string.IsNullOrWhiteSpace(disLaco16Text) ? 0 : int.Parse(disLaco16Text)
                },
                comp_laco =new CompLaco
                {
                    FX1 = string.IsNullOrWhiteSpace(compLaco1Text) ? 0 : int.Parse(compLaco1Text),
                    FX2 = string.IsNullOrWhiteSpace(compLaco2Text) ? 0 : int.Parse(compLaco2Text),
                    FX3 = string.IsNullOrWhiteSpace(compLaco3Text) ? 0 : int.Parse(compLaco3Text),
                    FX4 = string.IsNullOrWhiteSpace(compLaco4Text) ? 0 : int.Parse(compLaco4Text),
                    FX5 = string.IsNullOrWhiteSpace(compLaco5Text) ? 0 : int.Parse(compLaco5Text),
                    FX6 = string.IsNullOrWhiteSpace(compLaco6Text) ? 0 : int.Parse(compLaco6Text),
                    FX7 = string.IsNullOrWhiteSpace(compLaco7Text) ? 0 : int.Parse(compLaco7Text),
                    FX8 = string.IsNullOrWhiteSpace(compLaco8Text) ? 0 : int.Parse(compLaco8Text),
                    FX9 = string.IsNullOrWhiteSpace(compLaco9Text) ? 0 : int.Parse(compLaco9Text),
                    FX10 = string.IsNullOrWhiteSpace(compLaco10Text) ? 0 : int.Parse(compLaco10Text),
                    FX11 = string.IsNullOrWhiteSpace(compLaco11Text) ? 0 : int.Parse(compLaco11Text),
                    FX12 = string.IsNullOrWhiteSpace(compLaco12Text) ? 0 : int.Parse(compLaco12Text),
                    FX13 = string.IsNullOrWhiteSpace(compLaco13Text) ? 0 : int.Parse(compLaco13Text),
                    FX14 = string.IsNullOrWhiteSpace(compLaco14Text) ? 0 : int.Parse(compLaco14Text),
                    FX15 = string.IsNullOrWhiteSpace(compLaco15Text) ? 0 : int.Parse(compLaco15Text),
                    FX16 = string.IsNullOrWhiteSpace(compLaco16Text) ? 0 : int.Parse(compLaco1Text),

                },

                comp_maior_valor = string.IsNullOrWhiteSpace(compMaiorValText) ? 0 : int.Parse(compMaiorValText),
                comp_maior_gravar = string.IsNullOrWhiteSpace(compMaiorGraText) ? 0 : int.Parse(compMaiorGraText),
                comp_menor_valor = string.IsNullOrWhiteSpace(compMenorValText) ? 0 : int.Parse(compMenorValText),
                comp_menor_gravar = string.IsNullOrWhiteSpace(compMenorGraText) ? 0 : int.Parse(compMenorGraText),
                vel_maior_valor = string.IsNullOrWhiteSpace(veloMaiorValText) ? 0 : int.Parse(veloMaiorValText),
                vel_maior_gravar = string.IsNullOrWhiteSpace(veloMaiorGraText) ? 0 : int.Parse(veloMaiorGraText),
                vel_menor_valor = string.IsNullOrWhiteSpace(veloMenorValText) ? 0 : int.Parse(veloMenorValText),
                vel_menor_gravar = string.IsNullOrWhiteSpace(valoMenorGraText) ? 0 : int.Parse(valoMenorGraText),


                class_comp = new Dictionary<string, ClassComp>
                {
                    { "CC1", new ClassComp { tipo = ParseToIntOrZero(tipo1), nome = nome1 ?? "", inicio = ConverterStringParaInt(inicio1), fim = ConverterStringParaInt(fim1), eixo = ConverterStringParaInt(qtdEixo1) } },
                    { "CC2", new ClassComp { tipo = ParseToIntOrZero(tipo2), nome = nome2 ?? "", inicio = ConverterStringParaInt(inicio2), fim = ConverterStringParaInt(fim2), eixo = ConverterStringParaInt(qtdEixo2) } },
                    { "CC3", new ClassComp { tipo = ParseToIntOrZero(tipo3), nome = nome3 ?? "", inicio = ConverterStringParaInt(inicio3), fim = ConverterStringParaInt(fim3), eixo = ConverterStringParaInt(qtdEixo3) } },
                    { "CC4", new ClassComp { tipo = ParseToIntOrZero(tipo4), nome = nome4 ?? "", inicio = ConverterStringParaInt(inicio4), fim = ConverterStringParaInt(fim4), eixo = ConverterStringParaInt(qtdEixo4) } },
                    { "CC5", new ClassComp { tipo = ParseToIntOrZero(tipo5), nome = nome5 ?? "", inicio = ConverterStringParaInt(inicio5), fim = ConverterStringParaInt(fim5), eixo = ConverterStringParaInt(qtdEixo5) } },
                    { "CC6", new ClassComp { tipo = ParseToIntOrZero(tipo6), nome = nome6 ?? "", inicio = ConverterStringParaInt(inicio6), fim = ConverterStringParaInt(fim6), eixo = ConverterStringParaInt(qtdEixo6) } },
                    { "CC7", new ClassComp { tipo = ParseToIntOrZero(tipo7), nome = nome7 ?? "", inicio = ConverterStringParaInt(inicio7), fim = ConverterStringParaInt(fim7), eixo = ConverterStringParaInt(qtdEixo7) } },
                    { "CC8", new ClassComp { tipo = ParseToIntOrZero(tipo8), nome = nome8 ?? "", inicio = ConverterStringParaInt(inicio8), fim = ConverterStringParaInt(fim8), eixo = ConverterStringParaInt(qtdEixo8) } },
                    { "CC9", new ClassComp { tipo = ParseToIntOrZero(tipo9), nome = nome9 ?? "", inicio = ConverterStringParaInt(inicio9), fim = ConverterStringParaInt(fim9), eixo = ConverterStringParaInt(qtdEixo9) } },
                    { "CC10", new ClassComp { tipo = ParseToIntOrZero(tipo10), nome = nome10 ?? "", inicio = ConverterStringParaInt(inicio10), fim = ConverterStringParaInt(fim10), eixo = ConverterStringParaInt(qtdEixo10) } },
                    { "CC11", new ClassComp { tipo = ParseToIntOrZero(tipo11), nome = nome11 ?? "", inicio = ConverterStringParaInt(inicio11), fim = ConverterStringParaInt(fim11), eixo = ConverterStringParaInt(qtdEixo11) } },
                    { "CC12", new ClassComp { tipo = ParseToIntOrZero(tipo12), nome = nome12 ?? "", inicio = ConverterStringParaInt(inicio12), fim = ConverterStringParaInt(fim12), eixo = ConverterStringParaInt(qtdEixo12) } },
                    { "CC13", new ClassComp { tipo = ParseToIntOrZero(tipo13), nome = nome13 ?? "", inicio = ConverterStringParaInt(inicio13), fim = ConverterStringParaInt(fim13), eixo = ConverterStringParaInt(qtdEixo13) } },
                    { "CC14", new ClassComp { tipo = ParseToIntOrZero(tipo14), nome = nome14 ?? "", inicio = ConverterStringParaInt(inicio14), fim = ConverterStringParaInt(fim14), eixo = ConverterStringParaInt(qtdEixo14) } },
                    { "CC15", new ClassComp { tipo = ParseToIntOrZero(tipo15), nome = nome15 ?? "", inicio = ConverterStringParaInt(inicio15), fim = ConverterStringParaInt(fim15), eixo = ConverterStringParaInt(qtdEixo15) } }
                }
            };
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string jsonString = JsonSerializer.Serialize(objeto, options);
            return jsonString;
        }

        private int ParseToIntOrZero(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            input = input.Replace(',', '.');

            if (double.TryParse(input, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
            {
                return (int)Math.Round(result * 100);
            }

            return 0;
        }

        public int ConverterStringParaInt(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            return (int)Math.Round(float.Parse(valor.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture));
        }


        public bool checkEstacaoCarregada()
        {

            if (btn_Conexao.Content.ToString() == "Conectar")
            {
                ShowMessageDialog("Antes de executar essa função, carregue uma estação de:" + Environment.NewLine +
                                "  - Conexão");

                return false;
            }
            return true;
        }

        private async void btn_lerConfig_Click(object sender, RoutedEventArgs e)
        {

            var uiDispatcher = DispatcherQueue.GetForCurrentThread();

            Ping ping = new Ping();


            if (checkEstacaoCarregada())
            {
                bool confirmacao = await ShowDialog("Deseja receber os dados vindos do PLC?");

                if (confirmacao)
                {
                    manterConexao = true;

                    PingReply resp = ping.Send(txb_ip.Text);

                    if (resp.Status == IPStatus.Success)
                    {
                        dadosPLC= "LDCF";
                        flagSend = 1;
                        statusSend = 1;
                        nBytes = 4;

                        // Remove handler anterior, se houver
                        if (handlerLerConfig != null)
                        {
                            DadoRecebido -= handlerLerConfig;
                        }

                        // Define o novo handler
                        handlerLerConfig = dado =>
                        {
                            uiDispatcher.TryEnqueue(() =>
                            {
                                App.MainViewModel.LerConfigPLC(dado);
                            });
                        };

                        // Adiciona o handler
                        DadoRecebido += handlerLerConfig;

                        Debug.WriteLine(dadosPLC);
                        Conect();
                    }
                }
            }
        }

        private async void btn_resetar_Click(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();

            if (checkEstacaoCarregada())
            {
                bool confirmacao = await ShowDialog("Deseja realmente resetar o MDV8?");

                if (confirmacao)
                {
                    PingReply resp = ping.Send(txb_ip.Text);

                    if (resp.Status == IPStatus.Success)
                    {
                        dadosPLC = "RMDV";
                        flagSend = 1;
                        statusSend = 1;
                        nBytes = 4;

                        Conect();
                    }
                }
            }
        }

        private async void BtnDataAtual_Click(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();

            if (checkEstacaoCarregada())
            {
                bool confirmacao = await ShowDialog("Deseja enviar a DATA E HORA atual para o PLC?");

                if (confirmacao)
                {
                    PingReply resp = ping.Send(txb_ip.Text);
                    DateTime agora = DateTime.Now;

                    if (resp.Status == IPStatus.Success)
                    {
                        var dado = $"DATA{{\"ano\": {agora.Year}, \"mes\": {agora.Month}, \"dia\": {agora.Day}, \"hora\": {agora.Hour}, \"min\": {agora.Minute}, \"seg\": {agora.Second}}}";
                        var tamanho = dado.Length;

                        dadosPLC = $"Caracteres:{tamanho} // {dado}";
                        flagSend = 1;
                        statusSend = 1;
                        nBytes = 4;

                        Conect();
                    }
                }
            }
        }

        private async void btn_Equipamento_Click(object sender, RoutedEventArgs e)
        {
            await AtualizarEquipamentoAsync();
            controle_Equipamentos();
        }

        private async Task AtualizarEquipamentoAsync()
        {
            var resultado = await AbrirEquipamentosPage();

            if (resultado == ContentDialogResult.Primary && btn_Equipamento.Content.ToString() != "Ansitra QR")
            {
                btn_Equipamento.Content = "Ansitra QR";

                if(btn_Conexao.Content.ToString() == "Desconectar")
                {
                    FecharConexao();
                }
                mainFrame.Navigate(typeof(ConexaoPage));
                ResetarCampos();
            }
            else if (resultado == ContentDialogResult.Secondary && btn_Equipamento.Content.ToString() != "Ansitra WT")
            {
                btn_Equipamento.Content = "Ansitra WT";

                if (btn_Conexao.Content.ToString() == "Desconectar")
                {
                    FecharConexao();
                }
                mainFrame.Navigate(typeof(ConexaoPage));
                ResetarCampos();
            }
        }

        private void ResetarCampos()
        {
            txt_numeroValor.Text = string.Empty;
            txt_rodoviaValor.Text = string.Empty;
            txt_kmValor.Text = string.Empty;

            //Conexão
            conexaoPage.altera_estacao = string.Empty;
            conexaoPage.ip_estacao = string.Empty;
            conexaoPage.mask_estacao = string.Empty;
            conexaoPage.gateway_estacao = string.Empty;
            conexaoPage.ip_modulo = string.Empty;
            conexaoPage.mask_modulo = string.Empty;
            conexaoPage.gateway_modulo = string.Empty;

            conexaoPage.Ip_plc2 = string.Empty;
            conexaoPage.Mask_plc2 = string.Empty;
            conexaoPage.Gateway_plc2 = string.Empty;
            conexaoPage.Ip_sensor2 = string.Empty;
            conexaoPage.Mask_sensor2 = string.Empty;
            conexaoPage.Gateway_sensor2 = string.Empty;

            //Dados Estação
            dadosEstacaoPage.txb_Rodovia.Text = string.Empty;
            dadosEstacaoPage.txb_Estacao.Text = string.Empty;
            dadosEstacaoPage.txb_Numero.Text = string.Empty;
            dadosEstacaoPage.txb_Km.Text = string.Empty;
            dadosEstacaoPage.cbx_Local.SelectedIndex = -1;
            dadosEstacaoPage.cbx_Uf.SelectedIndex = -1;
            dadosEstacaoPage.cbx_IdRodovia.SelectedIndex = -1;

            //Faixas
            faixasPage.canal1.IsChecked = false;
            faixasPage.canal2.IsChecked = false;
            faixasPage.canal3.IsChecked = false;
            faixasPage.canal4.IsChecked = false;
            faixasPage.canal5.IsChecked = false;
            faixasPage.canal6.IsChecked = false;
            faixasPage.canal7.IsChecked = false;
            faixasPage.canal8.IsChecked = false;
            faixasPage.canal9.IsChecked = false;
            faixasPage.canal10.IsChecked = false;
            faixasPage.canal11.IsChecked = false;
            faixasPage.canal12.IsChecked = false;
            faixasPage.canal13.IsChecked = false;
            faixasPage.canal14.IsChecked = false;
            faixasPage.canal15.IsChecked = false;
            faixasPage.canal16.IsChecked = false;

            //Laços
            lacosPage.CompLaco1 = string.Empty;
            lacosPage.CompLaco2 = string.Empty;
            lacosPage.CompLaco3 = string.Empty;
            lacosPage.CompLaco4 = string.Empty;
            lacosPage.CompLaco5 = string.Empty;
            lacosPage.CompLaco6 = string.Empty;
            lacosPage.CompLaco7 = string.Empty;
            lacosPage.CompLaco8 = string.Empty;
            lacosPage.CompLaco9 = string.Empty;
            lacosPage.CompLaco10 = string.Empty;
            lacosPage.CompLaco11 = string.Empty;
            lacosPage.CompLaco12 = string.Empty;
            lacosPage.CompLaco13 = string.Empty;
            lacosPage.CompLaco14 = string.Empty;
            lacosPage.CompLaco15 = string.Empty;
            lacosPage.CompLaco16 = string.Empty;

            lacosPage.DistLaco1 = string.Empty;
            lacosPage.DistLaco2 = string.Empty;
            lacosPage.DistLaco3 = string.Empty;
            lacosPage.DistLaco4 = string.Empty;
            lacosPage.DistLaco5 = string.Empty;
            lacosPage.DistLaco6 = string.Empty;
            lacosPage.DistLaco7 = string.Empty;
            lacosPage.DistLaco8 = string.Empty;
            lacosPage.DistLaco9 = string.Empty;
            lacosPage.DistLaco10 = string.Empty;
            lacosPage.DistLaco11 = string.Empty;
            lacosPage.DistLaco12 = string.Empty;
            lacosPage.DistLaco13 = string.Empty;
            lacosPage.DistLaco14 = string.Empty;
            lacosPage.DistLaco15 = string.Empty;
            lacosPage.DistLaco16 = string.Empty;

            //Classe
            classePage.Tipo1.SelectedIndex = -1;
            classePage.Tipo2.SelectedIndex = -1;
            classePage.Tipo3.SelectedIndex = -1;
            classePage.Tipo4.SelectedIndex = -1;
            classePage.Tipo5.SelectedIndex = -1;
            classePage.Tipo6.SelectedIndex = -1;
            classePage.Tipo7.SelectedIndex = -1;
            classePage.Tipo8.SelectedIndex = -1;
            classePage.Tipo9.SelectedIndex = -1;
            classePage.Tipo10.SelectedIndex = -1;
            classePage.Tipo11.SelectedIndex = -1;
            classePage.Tipo12.SelectedIndex = -1;
            classePage.Tipo13.SelectedIndex = -1;
            classePage.Tipo14.SelectedIndex = -1;
            classePage.Tipo15.SelectedIndex = -1;

            classePage.Nome1.Text = string.Empty;
            classePage.Nome2.Text = string.Empty;
            classePage.Nome3.Text = string.Empty;
            classePage.Nome4.Text = string.Empty;
            classePage.Nome5.Text = string.Empty;
            classePage.Nome6.Text = string.Empty;
            classePage.Nome7.Text = string.Empty;
            classePage.Nome8.Text = string.Empty;
            classePage.Nome9.Text = string.Empty;
            classePage.Nome10.Text = string.Empty;
            classePage.Nome11.Text = string.Empty;
            classePage.Nome12.Text = string.Empty;
            classePage.Nome13.Text = string.Empty;
            classePage.Nome14.Text = string.Empty;
            classePage.Nome15.Text = string.Empty;

            classePage.Inicio1.Text = string.Empty;
            classePage.Inicio2.Text = string.Empty;
            classePage.Inicio3.Text = string.Empty;
            classePage.Inicio4.Text = string.Empty;
            classePage.Inicio5.Text = string.Empty;
            classePage.Inicio6.Text = string.Empty;
            classePage.Inicio7.Text = string.Empty;
            classePage.Inicio8.Text = string.Empty;
            classePage.Inicio9.Text = string.Empty;
            classePage.Inicio10.Text = string.Empty;
            classePage.Inicio11.Text = string.Empty;
            classePage.Inicio12.Text = string.Empty;
            classePage.Inicio13.Text = string.Empty;
            classePage.Inicio14.Text = string.Empty;
            classePage.Inicio15.Text = string.Empty;

            classePage.Fim1.Text = string.Empty;
            classePage.Fim2.Text = string.Empty;
            classePage.Fim3.Text = string.Empty;
            classePage.Fim4.Text = string.Empty;
            classePage.Fim5.Text = string.Empty;
            classePage.Fim6.Text = string.Empty;
            classePage.Fim7.Text = string.Empty;
            classePage.Fim8.Text = string.Empty;
            classePage.Fim9.Text = string.Empty;
            classePage.Fim10.Text = string.Empty;
            classePage.Fim11.Text = string.Empty;
            classePage.Fim12.Text = string.Empty;
            classePage.Fim13.Text = string.Empty;
            classePage.Fim14.Text = string.Empty;
            classePage.Fim15.Text = string.Empty;

            classePage.QtdEixo1.Text = string.Empty;
            classePage.QtdEixo2.Text = string.Empty;
            classePage.QtdEixo3.Text = string.Empty;
            classePage.QtdEixo4.Text = string.Empty;
            classePage.QtdEixo5.Text = string.Empty;
            classePage.QtdEixo6.Text = string.Empty;
            classePage.QtdEixo7.Text = string.Empty;
            classePage.QtdEixo8.Text = string.Empty;
            classePage.QtdEixo9.Text = string.Empty;
            classePage.QtdEixo10.Text = string.Empty;
            classePage.QtdEixo11.Text = string.Empty;
            classePage.QtdEixo12.Text = string.Empty;
            classePage.QtdEixo13.Text = string.Empty;
            classePage.QtdEixo14.Text = string.Empty;
            classePage.QtdEixo15.Text = string.Empty;

            //Filtros
            filtrosPage.CompMaiorValor = string.Empty;
            filtrosPage.CompMaiorGravar = string.Empty;
            filtrosPage.CompMenorValor = string.Empty;
            filtrosPage.CompMenorGravar = string.Empty;
            filtrosPage.VeloMaiorValor = string.Empty;
            filtrosPage.VeloMaiorGravar = string.Empty;
            filtrosPage.VeloMenorValor = string.Empty;
            filtrosPage.VeloMenorGravar = string.Empty;
        }

        private async void btn_importar_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add("*");

            var arquivo = await picker.PickSingleFileAsync();

            if (arquivo != null)
            {
                string nomeArquivo = arquivo.Name;

                // Adiciona ao ComboBox, se ainda não estiver lá
                if (!cbbx_arquivos.Items.Contains(nomeArquivo))
                {
                    cbbx_arquivos.Items.Add(nomeArquivo);
                }

                // Define como selecionado (isso dispara o SelectionChanged)
                cbbx_arquivos.SelectedItem = nomeArquivo;
            }
        }

        private void CarregarConfiguracao(DadosJson objeto)
        {
            if (objeto == null) return;

            for (int i = 1; i <= 16; i++)
            {
                var nomeProp = App.MainViewModel.GetType().GetProperty($"NomeFaixa{i}");
                var distProp = App.MainViewModel.GetType().GetProperty($"DistLaco{i}");
                var compProp = App.MainViewModel.GetType().GetProperty($"CompLaco{i}");

                var nome = objeto.id_faixa?.GetType().GetProperty($"FX{i}")?.GetValue(objeto.id_faixa)?.ToString() ?? "";
                var dist = objeto.dist_laco?.GetType().GetProperty($"FX{i}")?.GetValue(objeto.dist_laco)?.ToString() ?? "";
                var comp = objeto.comp_laco?.GetType().GetProperty($"FX{i}")?.GetValue(objeto.comp_laco)?.ToString() ?? "";

                nomeProp?.SetValue(App.MainViewModel, nome);
                distProp?.SetValue(App.MainViewModel, dist);
                compProp?.SetValue(App.MainViewModel, comp);

                // Checkbox
                var checkBoxProp = faixasPage.GetType().GetProperty($"canal{i}");
                if (checkBoxProp?.GetValue(faixasPage) is CheckBox checkBox)
                    checkBox.IsChecked = false;
            }

            txt_numeroValor.Text = objeto.num_estacao;
            txt_kmValor.Text = objeto.km_rodovia;

            if (objeto.rede_estacao != null)
            {
                conexaoPage.ip_estacao = objeto.rede_estacao.ip ?? "";
                conexaoPage.mask_estacao = objeto.rede_estacao.mask ?? "";
                conexaoPage.gateway_estacao = objeto.rede_estacao.gateway ?? "";
            }

            if (objeto.rede_modulo != null)
            {
                conexaoPage.ip_modulo = objeto.rede_modulo.ip ?? "";
                conexaoPage.mask_modulo = objeto.rede_modulo.mask ?? "";
                conexaoPage.gateway_modulo = objeto.rede_modulo.gateway ?? "";
            }

            if (objeto.rede_plc2 != null)
            {
                App.MainViewModel.IpPlc2 = objeto.rede_plc2.ip ?? "";
                App.MainViewModel.MaskPlc2 = objeto.rede_plc2.mask ?? "";
                App.MainViewModel.GatewayPlc2 = objeto.rede_plc2.gateway ?? "";
            }

            if (objeto.rede_sensor2 != null)
            {
                App.MainViewModel.IpSensor2 = objeto.rede_sensor2.ip ?? "";
                App.MainViewModel.MaskSensor2 = objeto.rede_sensor2.mask ?? "";
                App.MainViewModel.GatewaySensor2 = objeto.rede_sensor2.gateway ?? "";
            }

            App.MainViewModel.EstacaoSelecionada.tipo_banco = objeto.banco_dados.tipo ?? "";
            App.MainViewModel.EstacaoSelecionada.host_banco = objeto.banco_dados.host_banco ?? "";
            App.MainViewModel.EstacaoSelecionada.usuario_banco = objeto.banco_dados.usuario ?? "";
            App.MainViewModel.EstacaoSelecionada.porta_banco = objeto.banco_dados.porta ?? "";
            App.MainViewModel.EstacaoSelecionada.exibicao_banco = objeto.banco_dados.exibicao ?? "";
            App.MainViewModel.EstacaoSelecionada.servidor_banco = objeto.banco_dados.servidor ?? "";

            App.MainViewModel.EstacaoSelecionada.senha_banco = string.IsNullOrWhiteSpace(objeto.banco_dados.senha)
                ? ""
                : Encoding.UTF8.GetString(Convert.FromBase64String(objeto.banco_dados.senha));

            App.MainViewModel.EstacaoSelecionada.NumEstacao = objeto.num_estacao ?? "";
            App.MainViewModel.EstacaoSelecionada.Rodovia = objeto.id_rodovia ?? "";
            App.MainViewModel.EstacaoSelecionada.Km = objeto.km_rodovia ?? "";
            App.MainViewModel.EstacaoSelecionada.Local = objeto.local_rodovia ?? "";
            App.MainViewModel.EstacaoSelecionada.Estado = objeto.uf_rodovia ?? "";

            for (int i = 1; i <= 16; i++)
            {
                var fx = objeto.id_faixa?.GetType().GetProperty($"FX{i}")?.GetValue(objeto.id_faixa)?.ToString() ?? "";
                var dist = objeto.dist_laco?.GetType().GetProperty($"FX{i}")?.GetValue(objeto.dist_laco)?.ToString() ?? "";
                var comp = objeto.comp_laco?.GetType().GetProperty($"FX{i}")?.GetValue(objeto.comp_laco)?.ToString() ?? "";

                App.MainViewModel.GetType().GetProperty($"NomeFaixa{i}")?.SetValue(App.MainViewModel, fx);
                App.MainViewModel.GetType().GetProperty($"DistLaco{i}")?.SetValue(App.MainViewModel, dist);
                App.MainViewModel.GetType().GetProperty($"CompLaco{i}")?.SetValue(App.MainViewModel, comp);
            }

            App.MainViewModel.CompMaior = objeto.comp_maior_valor.ToString() ?? "";
            App.MainViewModel.CompMaiorGravar = objeto.comp_maior_gravar.ToString() ?? "";
            App.MainViewModel.CompMenor = objeto.comp_menor_valor.ToString() ?? "";
            App.MainViewModel.CompMenorGravar = objeto.comp_menor_gravar.ToString() ?? "";
            App.MainViewModel.VelocMaior = objeto.vel_maior_valor.ToString() ?? "";
            App.MainViewModel.VelocMaiorGravar = objeto.vel_maior_gravar.ToString() ?? "";
            App.MainViewModel.VelocMenor = objeto.vel_menor_valor.ToString() ?? "";
            App.MainViewModel.VelocMenorGravar = objeto.vel_menor_gravar.ToString() ?? "";

            int index = 1;
            foreach (var item in objeto.class_comp)
            {
                // Converte o tipo
                string tipoConvertido = App.MainViewModel.ListaTipos.ElementAtOrDefault(item.Value.tipo) ?? "";

                // Verifica se o nome é vazio, se sim, altera tipoConvertido para "-1"
                if (string.IsNullOrWhiteSpace(item.Value.nome))
                {
                    tipoConvertido = "-1";
                }

                // Atribui o valor de tipoConvertido à propriedade ClassTipo{index}
                App.MainViewModel.GetType().GetProperty($"ClassTipo{index}")?.SetValue(App.MainViewModel, tipoConvertido);
                App.MainViewModel.GetType().GetProperty($"ClassNome{index}")?.SetValue(App.MainViewModel, item.Value.nome ?? "");
                App.MainViewModel.GetType().GetProperty($"ClassInicio{index}")?.SetValue(App.MainViewModel, item.Value.inicio.ToString());
                App.MainViewModel.GetType().GetProperty($"ClassFim{index}")?.SetValue(App.MainViewModel, item.Value.fim.ToString());
                App.MainViewModel.GetType().GetProperty($"ClassQtdEixo{index}")?.SetValue(App.MainViewModel, item.Value.eixo.ToString());

                index++;
            }

            // Ativa os canais com base nos nomes
            if (objeto.id_faixa != null)
            {
                for (int i = 0; i < objeto.class_comp.Count; i++)
                {
                    string nomePropriedade = $"NomeFaixa{i + 1}";
                    var propriedade = App.MainViewModel.GetType().GetProperty(nomePropriedade);
                    var valor = propriedade?.GetValue(App.MainViewModel)?.ToString();

                    if (string.IsNullOrWhiteSpace(valor)) continue;

                    var canal = faixasPage.GetType().GetProperty($"canal{i + 1}")?.GetValue(faixasPage) as CheckBox;
                    canal.IsChecked = true;

                    string faixa = (i + 1).ToString();
                    string grupo = ((i / 4) + 1).ToString();
                    string posicao = ((i % 4) + 1).ToString();

                    faixasPage.PreencherComboBoxes(valor, faixa, grupo, posicao);
                    faixasPage.setaCamposFaixas(canal);
                }
            }
        }


        private async void cbbx_arquivos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = cbbx_arquivos.SelectedItem;
                var itemSelecionado = selectedItem as ComboBoxItem;

                if (itemSelecionado != null &&
                    itemSelecionado.Content.ToString().EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                    File.Exists(itemSelecionado.Tag.ToString()))
                {
                    string arquivoSelecionado = itemSelecionado.Content.ToString();
                    string caminhoArquivo = itemSelecionado.Tag.ToString();

                    string conteudoJson = await File.ReadAllTextAsync(caminhoArquivo);

                    // Desserializa diretamente para a classe DadosJson
                    DadosJson objeto = JsonSerializer.Deserialize<DadosJson>(conteudoJson);

                    CarregarConfiguracao(objeto);
                }
                else 
                {
                    if (selectedItem != null && cbbx_arquivos.Items.Contains(selectedItem))
                    {
                        cbbx_arquivos.Items.Remove(selectedItem);
                        cbbx_arquivos.SelectedIndex = -1;
                    }

                    var erroDialog = new ContentDialog
                    {
                        Title = "Importar arquivo",
                        Content = "O arquivo selecionado ja foi adicionado, ou não é um arquivo .json válido e foi removido da lista.",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };

                    await erroDialog.ShowAsync();
                }
            }
            catch (Exception)
            {

            }            
        }

        private void CarregarArquivosJson()
        {
            string pasta = @"C:\Users\Stine\OneDrive - STN\Aplicativos\AnsitraPLC - Mit. QR\AnsitraPLC_QR_Beta\AnsitraPLC QR MIT - 24-09-2024\arquivosJSON";

            if (Directory.Exists(pasta))
            {
                string[] arquivos = Directory.GetFiles(pasta, "*.json");

                cbbx_arquivos.Items.Clear();
                foreach (string caminhoCompleto in arquivos)
                {
                    // Adiciona apenas o nome do arquivo no ComboBox
                    string nomeArquivo = Path.GetFileName(caminhoCompleto);
                    cbbx_arquivos.Items.Add(new ComboBoxItem
                    {
                        Content = nomeArquivo,
                        Tag = caminhoCompleto // Armazena o caminho completo no Tag
                    });
                }
            }
        }



        private async void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Recupera o nome do arquivo a partir da EstacaoSelecionada
                string nomeEstacao = App.MainViewModel.EstacaoSelecionada?.IdEstacao?.ToString();

                // Verifica se o nome da estação é nulo ou vazio
                if (string.IsNullOrWhiteSpace(nomeEstacao))
                {
                    ContentDialog erroDialog = new ContentDialog
                    {
                        Title = "Importar arquivo:",
                        Content = "Ação nescessária - Realize a conexão com o banco de dados.",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot                        
                    };
                    await erroDialog.ShowAsync();
                    return;
                }

                var dadosJson = GerarDadosPLC();

                // Caminho do arquivo
                string caminho = @"C:\Users\Stine\OneDrive - STN\Aplicativos\AnsitraPLC - Mit. QR\AnsitraPLC_QR_Beta\AnsitraPLC QR MIT - 24-09-2024\arquivosJSON";

                // Concatena o caminho
                string caminhoCompleto = Path.Combine(caminho, $"{nomeEstacao}.json");

                // Garante que a pasta exista
                string pasta = Path.GetDirectoryName(caminhoCompleto);
                if (!Directory.Exists(pasta))
                {
                    Directory.CreateDirectory(pasta);
                }

                // Salva o arquivo no caminho especificado
                File.WriteAllText(caminhoCompleto, dadosJson);

                // Mostra confirmação
                ContentDialog sucessoDialog = new ContentDialog
                {
                    Title = "Sucesso",
                    Content = "Arquivo salvo com sucesso!",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await sucessoDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Em caso de erro, mostra mensagem
                ContentDialog erroDialog = new ContentDialog
                {
                    Title = "Erro ao salvar",
                    Content = $"Erro: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await erroDialog.ShowAsync();
            }
        }

        private bool plcParado = false; // Estado do PLC

        private async void btn_pararPLC_Click(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();

            if (checkEstacaoCarregada())
            {
                string acao = plcParado ? "reiniciar" : "parar";
                bool confirmacao = await ShowDialog($"Deseja {acao} o PLC?");

                if (confirmacao)
                {
                    PingReply resp = ping.Send(txb_ip.Text);

                    if (resp.Status == IPStatus.Success)
                    {
                        if (!plcParado)
                        {
                            dadosPLC = "RPLC";
                            btn_pararPLC.Content = "Run PLC";
                        }
                        else
                        {
                            dadosPLC = "SPLC";
                            btn_pararPLC.Content = "Stop PLC";
                        }

                        flagSend = 1;
                        statusSend = 1;
                        nBytes = 4;

                        Conect();

                        plcParado = !plcParado;
                    }
                }
            }
        }

        private async void btn_FTP_Click(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();

            if (checkEstacaoCarregada())
            {
                //UPDT
                var picker = new FolderPicker();
                var hwnd = WindowNative.GetWindowHandle(this);
                InitializeWithWindow.Initialize(picker, hwnd);

                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                // Filtro necessário mesmo que esteja escolhendo pastas
                picker.FileTypeFilter.Add("*");

                // Abre o seletor de pastas
                var pastaSelecionada = await picker.PickSingleFolderAsync();

                if (pastaSelecionada != null)
                {
                    string nomePasta = pastaSelecionada.Name;

                    PingReply resp = ping.Send(txb_ip.Text);

                    if (resp.Status == IPStatus.Success)
                    {
                        dadosPLC = "DOWN";
                        flagSend = 1;
                        statusSend = 1;
                        nBytes = 4;

                        Conect();
                    }
                    caminhoPastaSelecionada = pastaSelecionada.Path;
                }

            }
        }

        private void nav_item_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var invokedItem = args.InvokedItemContainer as NavigationViewItem;

            if (invokedItem != null && invokedItem.Tag != null)
            {
                var tag = invokedItem.Tag.ToString();

                if (tag == "item_conexao")
                {
                    Conexao.IsExpanded = !Conexao.IsExpanded;
                    if (!sender.IsPaneOpen)
                    {
                        sender.IsPaneOpen = true;
                    }
                }
                else if (tag == "Banco")
                {
                    Banco.IsExpanded = !Banco.IsExpanded;
                    if (!sender.IsPaneOpen)
                    {
                        sender.IsPaneOpen = true;
                    }
                }
                else if (tag == "Arquivos")
                {
                    Arquivos.IsExpanded = !Arquivos.IsExpanded;
                    if (!sender.IsPaneOpen)
                    {
                        sender.IsPaneOpen = true;
                    }
                }
            }
        }



        private void HandleItemReClicked(NavigationViewItem item)
        {
            var tag = item.Tag.ToString();
        }

        private void FooterItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (nav_control.IsPaneOpen == false)
            {
                nav_control.IsPaneOpen = true;
            }
        }

        private void control_nav_SelectionChange(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {

            if (args.SelectedItem is NavigationViewItem selecionaItem)
            {


                if (args.IsSettingsSelected)
                {
                    string configPage = "ConfiguracaoPage";
                    Type ConfigPage = Type.GetType($"AnsitraPLC_QR_MIT.{configPage}");
                    mainFrame.Navigate(ConfigPage);
                    return;

                }

                // Obter o tag do item selecionado
                string selecionaTag = selecionaItem.Tag.ToString();
                string nomePagina = string.Empty;

                // Definir o nome da página com base no tag
                switch (selecionaTag)
                {
                    case "item_conexao":
                        nomePagina = "ConexaoPage";
                        Type Conexaopage = Type.GetType($"AnsitraPLC_QR_MIT.{nomePagina}");
                        mainFrame.Navigate(Conexaopage);
                        break;
                    case "ConfiguracaoSistema":
                        nomePagina = "ConfiguracaoSistemaPage";
                        Type Configpage = Type.GetType($"AnsitraPLC_QR_MIT.{nomePagina}");
                        mainFrame.Navigate(Configpage);
                        break;
                    case "Monitoramento":
                        nomePagina = "MonitoramentoPage";
                        Type MonitPage = Type.GetType($"AnsitraPLC_QR_MIT.{nomePagina}");
                        mainFrame.Navigate(MonitPage);
                        break;
                    case "Banco":
                        nomePagina = "BancoPage";
                        Type BancoPage = Type.GetType($"AnsitraPLC_QR_MIT.{nomePagina}");
                        mainFrame.Navigate(typeof(BancoPage), null, new DrillInNavigationTransitionInfo());
                        break;
                    case "DadosEstacao":
                        mainFrame.Content = dadosEstacaoPage;
                        break;
                    case "Faixas":
                        mainFrame.Content = faixasPage;
                        break;
                    case "Lacos":
                        mainFrame.Content = lacosPage;
                        break;
                    case "Classe":
                        mainFrame.Content = classePage;
                        break;
                    case "Filtros":
                        mainFrame.Content = filtrosPage;
                        break;
                    case "Alarmes":
                        nomePagina = "AlarmesPage";
                        Type AlarmesPage = Type.GetType($"AnsitraPLC_QR_MIT.{nomePagina}");
                        mainFrame.Navigate(typeof(AlarmesPage), null, new DrillInNavigationTransitionInfo());
                        break;
                    case "Veiculo":
                        mainFrame.Content = veiculoPage;
                        break;
                    case "Contagem":
                        nomePagina = "ContagemPage";
                        Type ContagemPage = Type.GetType($"AnsitraPLC_QR_MIT.{nomePagina}");
                        mainFrame.Navigate(typeof(ContagemPage), null, new DrillInNavigationTransitionInfo());
                        break;
                    case "btn":
                        sender.IsPaneOpen = true;
                        break;

                }
            }
        }

        private void habilita_BtnConexao(object sender, TextChangedEventArgs e)
            {
            // Verifica se os dois campos possuem texto
            if (!string.IsNullOrEmpty(txb_ip.Text) && txb_ip.Text.Length > 6 && !string.IsNullOrEmpty(txb_porta.Text))
            {
                btn_Conexao.IsEnabled = true;
            }
            else
            {
                btn_Conexao.IsEnabled = false;
            }
        }
    }
}
