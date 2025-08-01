🚧 Projeto de Monitoramento Rodoviário com WinUI 3
Este repositório apresenta uma aplicação desenvolvida durante meu estágio na empresa Stine, com foco em monitoramento inteligente de rodovias. O projeto foi realizado individualmente ao longo de 10 meses, utilizando tecnologias como C#, XAML e WinUI 3, integrando comunicação com PLCs, banco de dados SQL e interface gráfica responsiva.

🛠️ Tecnologias Utilizadas
C# com XAML (WinUI 3)

Python (executado nos PLCs)

Banco de Dados SQL Server

Protocolo TCP/IP

PLC com comunicação direta via socket

🔧 Funcionalidades Principais
Monitoramento de tráfego em tempo real:
Identificação da quantidade e tipo de veículos por faixa, com base nos dados coletados pelos laços indutivos instalados nas pistas.

Conexão com PLCs via TCP/IP:
Comunicação estável e eficiente com os dispositivos distribuídos ao longo das rodovias.

Integração com banco de dados:
Execução de queries para leitura e atualização de informações de configuração e status dos equipamentos.

Visualização em tempo real:
Exibição do estado atual dos dispositivos conectados, incluindo nome, classificação por faixa, distância e comprimento dos laços.

Módulo de alarmes:
Notificações em caso de temperatura elevada, porta do equipamento aberta ou falha na comunicação com os PLCs.

Sincronização de data e hora:
Ajuste automático dos relógios dos dispositivos de campo, com suporte a múltiplos fusos horários.

Processamento local em Python:
Os PLCs executam scripts Python para interpretar comandos e processar dados recebidos da aplicação.

📌 Sobre o Projeto
O sistema foi idealizado e implementado por mim, abrangendo:

Estruturação completa da aplicação;

Comunicação com hardware em campo (PLCs);

Integração e manipulação de dados em banco SQL;

Desenvolvimento da interface gráfica com WinUI 3;

Sincronização e tratamento de dados em tempo real.

📷 Exemplos da Interface

<img width="1760" height="904" alt="Captura de tela 2025-07-08 140659" src="https://github.com/user-attachments/assets/8d222628-3769-489a-9dc2-09386b76905f" />
<img width="1764" height="905" alt="Captura de tela 2025-07-08 141000" src="https://github.com/user-attachments/assets/0fc220a4-749e-48ec-ac13-fc7a81c0e29f" />
<img width="1765" height="906" alt="Captura de tela 2025-07-08 140923" src="https://github.com/user-attachments/assets/ae3f11d5-41d4-4b7f-a549-d82160bc913d" />
<img width="1764" height="910" alt="Captura de tela 2025-07-08 141111" src="https://github.com/user-attachments/assets/03ba8c2b-6cfd-4149-b9e8-15b745403ba3" />
<img width="1765" height="910" alt="Captura de tela 2025-07-08 141035" src="https://github.com/user-attachments/assets/ab6df523-8ad5-4213-9fd9-2e8671e6e294" />

💡 Aprendizados
Durante o desenvolvimento deste projeto, adquiri experiência sólida em:

Desenvolvimento de aplicações desktop modernas com WinUI 3;

Comunicação com dispositivos físicos via socket TCP/IP;

Integração de sistemas heterogêneos;

Gerenciamento de dados em tempo real.
