using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.WebSockets;
using System.Text.Json;

namespace DeviceCommunication
{
    public partial class Form1 : Form
    {
        private string myRoomCode;
        private ClientWebSocket webSocket;
        private bool isRunning = false;
        private CancellationTokenSource cancellationToken;

        // REPLACE THIS WITH YOUR RENDER SERVER URL!
        private const string SERVER_URL = "wss://devicecommunicationserver.onrender.com";

        // UI Controls
        private Panel connectionPanel;
        private Panel chatPanel;
        private TextBox txtRoomCode;
        private Label lblStatus;
        private TextBox txtChat;
        private TextBox txtMessage;
        private Button btnSend;
        private Button btnDisconnect;
        private Label lblTitle;
        private Label lblMyRoomCode;
        private Button btnConnect;
        private Label lblInstruction;
        private Button btnCopyCode;

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            GenerateRoomCode();
        }

        private void GenerateRoomCode()
        {
            Random rnd = new Random();
            myRoomCode = $"{rnd.Next(1000, 9999)}-{rnd.Next(1000, 9999)}";
            lblMyRoomCode.Text = $"Your Room Code: {myRoomCode}";
            lblStatus.Text = "✓ Ready!\n\nShare your room code with anyone, anywhere in the world,\nor enter their room code to connect.";
        }

        private void SetupUI()
        {
            this.Text = "Device Communication";
            this.Size = new Size(600, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);

            lblTitle = new Label();
            lblTitle.Text = "Device Communication";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(550, 45);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.ForeColor = Color.FromArgb(51, 51, 51);
            this.Controls.Add(lblTitle);

            connectionPanel = new Panel();
            connectionPanel.Location = new Point(20, 80);
            connectionPanel.Size = new Size(540, 430);
            connectionPanel.BackColor = Color.White;
            connectionPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(connectionPanel);

            Label lblYourInfoTitle = new Label();
            lblYourInfoTitle.Text = "🌍 Works Anywhere in the World! 🌍";
            lblYourInfoTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblYourInfoTitle.Location = new Point(20, 20);
            lblYourInfoTitle.Size = new Size(500, 30);
            lblYourInfoTitle.ForeColor = Color.FromArgb(76, 175, 80);
            lblYourInfoTitle.TextAlign = ContentAlignment.MiddleCenter;
            connectionPanel.Controls.Add(lblYourInfoTitle);

            lblMyRoomCode = new Label();
            lblMyRoomCode.Text = "Your Room Code: ####-####";
            lblMyRoomCode.Location = new Point(20, 70);
            lblMyRoomCode.Size = new Size(400, 40);
            lblMyRoomCode.Font = new Font("Consolas", 16, FontStyle.Bold);
            lblMyRoomCode.ForeColor = Color.FromArgb(33, 150, 243);
            connectionPanel.Controls.Add(lblMyRoomCode);

            btnCopyCode = new Button();
            btnCopyCode.Text = "Copy";
            btnCopyCode.Location = new Point(430, 75);
            btnCopyCode.Size = new Size(80, 30);
            btnCopyCode.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnCopyCode.BackColor = Color.FromArgb(156, 39, 176);
            btnCopyCode.ForeColor = Color.White;
            btnCopyCode.FlatStyle = FlatStyle.Flat;
            btnCopyCode.Cursor = Cursors.Hand;
            btnCopyCode.FlatAppearance.BorderSize = 0;
            btnCopyCode.Click += (s, e) =>
            {
                Clipboard.SetText(myRoomCode);
                MessageBox.Show("Room code copied to clipboard!", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            connectionPanel.Controls.Add(btnCopyCode);

            Label lblInfo = new Label();
            lblInfo.Text = "Share this code via WhatsApp, SMS, Email, Discord, etc.";
            lblInfo.Location = new Point(20, 115);
            lblInfo.Size = new Size(500, 25);
            lblInfo.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblInfo.ForeColor = Color.Gray;
            connectionPanel.Controls.Add(lblInfo);

            Label separator = new Label();
            separator.BorderStyle = BorderStyle.Fixed3D;
            separator.Location = new Point(20, 155);
            separator.Size = new Size(500, 2);
            connectionPanel.Controls.Add(separator);

            lblInstruction = new Label();
            lblInstruction.Text = "Enter the other person's Room Code to connect:";
            lblInstruction.Location = new Point(20, 170);
            lblInstruction.Size = new Size(500, 30);
            lblInstruction.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblInstruction.ForeColor = Color.FromArgb(66, 66, 66);
            connectionPanel.Controls.Add(lblInstruction);

            Label lblRoomCode = new Label();
            lblRoomCode.Text = "Room Code:";
            lblRoomCode.Location = new Point(20, 210);
            lblRoomCode.Size = new Size(100, 25);
            lblRoomCode.Font = new Font("Segoe UI", 10);
            connectionPanel.Controls.Add(lblRoomCode);

            txtRoomCode = new TextBox();
            txtRoomCode.Location = new Point(20, 240);
            txtRoomCode.Size = new Size(300, 35);
            txtRoomCode.Font = new Font("Consolas", 14);
            txtRoomCode.PlaceholderText = "1234-5678";
            connectionPanel.Controls.Add(txtRoomCode);

            btnConnect = new Button();
            btnConnect.Text = "Connect";
            btnConnect.Location = new Point(330, 238);
            btnConnect.Size = new Size(120, 38);
            btnConnect.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnConnect.BackColor = Color.FromArgb(33, 150, 243);
            btnConnect.ForeColor = Color.White;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Cursor = Cursors.Hand;
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Click += async (s, e) => await ConnectToRoom();
            connectionPanel.Controls.Add(btnConnect);

            lblStatus = new Label();
            lblStatus.Text = "Initializing...";
            lblStatus.Location = new Point(20, 300);
            lblStatus.Size = new Size(500, 100);
            lblStatus.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblStatus.ForeColor = Color.FromArgb(255, 152, 0);
            lblStatus.TextAlign = ContentAlignment.TopCenter;
            connectionPanel.Controls.Add(lblStatus);

            chatPanel = new Panel();
            chatPanel.Location = new Point(20, 80);
            chatPanel.Size = new Size(540, 430);
            chatPanel.BackColor = Color.White;
            chatPanel.BorderStyle = BorderStyle.FixedSingle;
            chatPanel.Visible = false;
            this.Controls.Add(chatPanel);

            Label lblChatTitle = new Label();
            lblChatTitle.Text = "Connected - Start Chatting! 🎉";
            lblChatTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblChatTitle.Location = new Point(10, 10);
            lblChatTitle.Size = new Size(520, 30);
            lblChatTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblChatTitle.ForeColor = Color.FromArgb(76, 175, 80);
            chatPanel.Controls.Add(lblChatTitle);

            txtChat = new TextBox();
            txtChat.Location = new Point(10, 50);
            txtChat.Size = new Size(520, 300);
            txtChat.Multiline = true;
            txtChat.ReadOnly = true;
            txtChat.ScrollBars = ScrollBars.Vertical;
            txtChat.Font = new Font("Consolas", 10);
            txtChat.BackColor = Color.FromArgb(250, 250, 250);
            chatPanel.Controls.Add(txtChat);

            txtMessage = new TextBox();
            txtMessage.Location = new Point(10, 360);
            txtMessage.Size = new Size(420, 35);
            txtMessage.Font = new Font("Segoe UI", 11);
            txtMessage.KeyPress += TxtMessage_KeyPress;
            chatPanel.Controls.Add(txtMessage);

            btnSend = new Button();
            btnSend.Text = "Send";
            btnSend.Location = new Point(440, 358);
            btnSend.Size = new Size(90, 39);
            btnSend.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSend.BackColor = Color.FromArgb(76, 175, 80);
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Cursor = Cursors.Hand;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += async (s, e) => await SendMessage();
            chatPanel.Controls.Add(btnSend);

            btnDisconnect = new Button();
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.Location = new Point(210, 395);
            btnDisconnect.Size = new Size(120, 30);
            btnDisconnect.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDisconnect.BackColor = Color.FromArgb(244, 67, 54);
            btnDisconnect.ForeColor = Color.White;
            btnDisconnect.FlatStyle = FlatStyle.Flat;
            btnDisconnect.Cursor = Cursors.Hand;
            btnDisconnect.FlatAppearance.BorderSize = 0;
            btnDisconnect.Click += BtnDisconnect_Click;
            chatPanel.Controls.Add(btnDisconnect);
        }

        private async Task ConnectToRoom()
        {
            string roomCode = txtRoomCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(roomCode))
            {
                MessageBox.Show("Please enter a room code", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (roomCode == myRoomCode)
            {
                MessageBox.Show("You cannot connect to your own room code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConnect.Enabled = false;
            lblStatus.Text = $"Connecting to room {roomCode}...";
            lblStatus.ForeColor = Color.Orange;

            try
            {
                webSocket = new ClientWebSocket();
                cancellationToken = new CancellationTokenSource();

                await webSocket.ConnectAsync(new Uri(SERVER_URL), cancellationToken.Token);

                var joinMessage = new
                {
                    type = "join",
                    roomCode = roomCode
                };

                string json = JsonSerializer.Serialize(joinMessage);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken.Token);

                isRunning = true;
                Task.Run(() => ReceiveMessages(cancellationToken.Token));

                lblStatus.Text = "Joined room! Waiting for other person...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}\n\nMake sure the server is running!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Connection failed. Check server URL in code.";
                lblStatus.ForeColor = Color.Red;
                btnConnect.Enabled = true;
            }
        }

        private async Task ReceiveMessages(CancellationToken token)
        {
            byte[] buffer = new byte[4096];

            while (isRunning && !token.IsCancellationRequested && webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var message = JsonSerializer.Deserialize<JsonElement>(json);

                        string messageType = message.GetProperty("type").GetString();

                        this.Invoke((MethodInvoker)delegate
                        {
                            if (messageType == "connected")
                            {
                                connectionPanel.Visible = false;
                                chatPanel.Visible = true;
                                AppendMessage("✓ Connected! Both devices are now in the same room.");
                                AppendMessage("You can now send messages!\n");
                            }
                            else if (messageType == "message")
                            {
                                string content = message.GetProperty("content").GetString();
                                AppendMessage($"[Other Device]: {content}");
                            }
                            else if (messageType == "disconnected")
                            {
                                AppendMessage("\n[Other device disconnected]");
                                MessageBox.Show("The other device disconnected", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ResetToConnection();
                            }
                        });
                    }
                }
                catch (Exception)
                {
                    if (isRunning)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            AppendMessage("\n[Connection lost]");
                            ResetToConnection();
                        });
                    }
                    break;
                }
            }
        }

        private void TxtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                _ = SendMessage();
                e.Handled = true;
            }
        }

        private async Task SendMessage()
        {
            string message = txtMessage.Text.Trim();

            if (string.IsNullOrWhiteSpace(message))
                return;

            try
            {
                var messageData = new
                {
                    type = "message",
                    content = message
                };

                string json = JsonSerializer.Serialize(messageData);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                AppendMessage($"[You]: {message}");
                txtMessage.Clear();
                txtMessage.Focus();
            }
            catch (Exception ex)
            {
                AppendMessage($"[Error: {ex.Message}]");
            }
        }

        private void AppendMessage(string message)
        {
            txtChat.AppendText($"{message}\r\n");
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
            ResetToConnection();
        }

        private void Disconnect()
        {
            isRunning = false;
            cancellationToken?.Cancel();
            webSocket?.Dispose();
        }

        private void ResetToConnection()
        {
            chatPanel.Visible = false;
            connectionPanel.Visible = true;
            txtChat.Clear();
            txtMessage.Clear();
            btnConnect.Enabled = true;
            lblStatus.Text = "Disconnected. Enter a room code to connect again.";
            lblStatus.ForeColor = Color.Gray;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Disconnect();
            base.OnFormClosing(e);
        }
    }
}
