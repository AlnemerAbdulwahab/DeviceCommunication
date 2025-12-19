# Device Communication App

A real-time peer-to-peer messaging application built with C# Windows Forms that enables two devices to communicate from anywhere in the world using room codes. No IP addresses, no complex network configuration—just simple room codes!

## 🌟 Features

- **🌍 Global Communication**: Connect devices from anywhere in the world
- **🔑 Room Code System**: Simple 4-digit codes instead of IP addresses
- **⚡ Real-Time Messaging**: Instant message delivery using WebSocket technology
- **🎨 Modern UI**: Clean and intuitive Windows Forms interface
- **📋 Easy Sharing**: One-click room code copying to clipboard
- **🔄 Auto-Reconnect**: Seamlessly handles disconnections and reconnections
- **💯 100% Free**: No subscriptions, no limits, completely free to use

### Connection Screen
Share your room code or enter someone else's code to connect.

### Chat Screen
Real-time messaging interface with instant delivery.

## 🚀 How It Works

### Architecture Overview
```
Device 1 (Your PC)           Relay Server (Cloud)         Device 2 (Friend's PC)
     |                              |                              |
     |--- Connect via WebSocket -->|<--- Connect via WebSocket ---|
     |                              |                              |
     |--- Join Room: 1234-5678 --->|                              |
     |                              |<--- Join Room: 1234-5678 ---|
     |                              |                              |
     |<-------- Connected! -------->|<-------- Connected! ---------|
     |                              |                              |
     |--- Send: "Hello!" ---------->|---------> "Hello!" --------->|
```

### Room Code System
- Each app generates a unique 4-digit room code on startup
- Share your room code with anyone (WhatsApp, Email, SMS, Discord, etc.)
- Enter their room code to establish a connection
- Both devices connect through a relay server in the cloud
- Messages are instantly delivered in real-time

## 🛠️ Technology Stack

- **Language**: C# (.NET 6.0+)
- **UI Framework**: Windows Forms
- **Communication**: WebSocket (System.Net.WebSockets)
- **Relay Server**: Node.js (deployed separately)
- **Data Format**: JSON for message serialization

## 📋 Prerequisites

- Windows OS (7, 8, 10, 11)
- .NET 6.0 SDK or higher
- Internet connection
- Running relay server (see [DeviceCommunicationServer](https://github.com/AlnemerAbdulwahab/DeviceCommunicationServer))

## 🔧 Installation & Setup

### Run from Source

1. **Clone the repository**
```bash
git clone https://github.com/AlnemerAbdulwahab/DeviceCommunication.git
cd DeviceCommunication/DeviceCommunication
```

2. **Configure Server URL**

Open `Form1.cs` and update line 18 with your relay server URL:
```csharp
private const string SERVER_URL = "ws://your-server-url.onrender.com";
```

3. **Run the application**
```bash
dotnet run
```

## 📖 Usage Guide

### For the First User (Host)

1. **Launch the app**
   - Your room code appears: `1234-5678`

2. **Share your room code**
   - Click "Copy" button
   - Send via WhatsApp, SMS, Email, Discord, etc.

3. **Wait for connection**
   - Other person will connect to your room code
   - Chat window opens automatically

### For the Second User (Guest)

1. **Launch the app**
   - You'll see your own room code (ignore it)

2. **Enter host's room code**
   - Type the code they shared: `1234-5678`
   - Click "Connect"

3. **Start chatting**
   - Both devices switch to chat screen
   - Send messages in real-time!

## 🏗️ Project Structure
```
DeviceCommunication/
├── README.md                          # This file
└── DeviceCommunication/
    ├── Form1.cs                       # Main application logic
    ├── Form1.Designer.cs              # UI designer code
    ├── Program.cs                     # Entry point
    ├── DeviceCommunication.csproj     # Project configuration
    ├── bin/                           # Compiled executables
    └── obj/                           # Build artifacts
```

## 🔑 Key Components

### Form1.cs - Main Application

**WebSocket Connection**
```csharp
private ClientWebSocket webSocket;
private const string SERVER_URL = "ws://server-url.onrender.com";
```

**Room Code Generation**
```csharp
private void GenerateRoomCode()
{
    Random rnd = new Random();
    myRoomCode = $"{rnd.Next(1000, 9999)}-{rnd.Next(1000, 9999)}";
}
```

**Message Handling**
```csharp
private async Task SendMessage()
{
    var messageData = new { type = "message", content = message };
    string json = JsonSerializer.Serialize(messageData);
    byte[] buffer = Encoding.UTF8.GetBytes(json);
    await webSocket.SendAsync(new ArraySegment<byte>(buffer), 
                              WebSocketMessageType.Text, true, 
                              CancellationToken.None);
}
```

## 🔐 Security Considerations

- Messages are sent over WebSocket (upgrade to WSS for encryption)
- Room codes are randomly generated (65,536 combinations per segment)
- No message logging on the server (privacy-first design)
- Connections auto-close when browser/app closes

## 🐛 Troubleshooting

### "Server connection failed!"
- **Cause**: Relay server is down or URL is incorrect
- **Solution**: 
  1. Check if server is running: Visit `https://your-server-url.onrender.com`
  2. Update `SERVER_URL` in `Form1.cs`
  3. Ensure you have internet connection

### "Joined room! Waiting for other person..."
- **Cause**: Other person hasn't joined yet
- **Solution**: Wait for them to enter your room code and click Connect

### Messages not sending
- **Cause**: WebSocket connection lost
- **Solution**: Click "Disconnect" and reconnect

## 👨‍💻 Author

**Abdulwahab Alnemer**
- GitHub: [@AlnemerAbdulwahab](https://github.com/AlnemerAbdulwahab)



## About

This project was created as part of the Tuwaiq Academy Software Development Bootcamp as a hands-on exercise. The objective was to apply previous lessons.

The code was AI-generated as a learning tool. The primary objective was to study and understand the code structure, analyze how devices communicate and interact with each other, and gain the ability to modify and enhance the code independently through hands-on practice.
