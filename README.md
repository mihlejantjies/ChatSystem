# ChatSystem

# Console Chat System (C# / .NET 8)

A simple multi-user chat room that runs in the terminal. One **server** relays messages between any number of **clients** over TCP. Built as a beginner project to learn sockets, `async`/`await` and basic client-server design.

<!-- Add a screenshot of the server and two clients chatting here: -->
<!-- ![Chat demo](docs/demo.png) -->

## Features
- Multiple people chatting at once
- Guided startup questions: server address, port, then your name
- Name checks: no empty names, no duplicates, letters/numbers/`-`/`_` only, max 20 characters
- Join and leave announcements, and a list of who is online when you join
- Commands: `/help`, `/users`, `/quit`
- Message length limit (500 characters) and timestamps on messages
- Friendly error messages, with an option to retry if the connection fails

## Requirements
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 (optional) or any terminal

## Run it

### Visual Studio
1. Open `ChatSystem.sln`.
2. Right-click the solution, choose **Properties**, then **Configure Startup Projects**.
3. Choose **Multiple startup projects**, create a launch profile, and set **ChatServer** (listed first) and **ChatClient** to **Start**.
4. Press **F5**. For more chatters, right-click `ChatClient` and choose **Debug, Start New Instance**.

### Terminal
Open a terminal per window:

    cd ChatServer
    dotnet run

    cd ChatClient
    dotnet run

Start the server first. Press Enter at each prompt to use the defaults (this computer, port 5000).

### Chatting across computers on the same network
1. Start the server on one computer and find its IP address (`ipconfig` on Windows, look for IPv4).
2. On the other computers, run `ChatClient` and type that IP address when asked for the server address.
3. If Windows asks, allow the server through the firewall on private networks.

## Commands
| Command | What it does |
|---|---|
| `/help` | Shows the list of commands |
| `/users` | Shows who is online |
| `/quit` | Leaves the chat |

## How it works
| Piece | Role |
|---|---|
| `TcpListener` (server) | Waits for connections on a port |
| `TcpClient` (both) | A network connection to read from and write to |
| `StreamReader` / `StreamWriter` | Read and send lines of text |
| `async` / `await` | Wait for the network without freezing the program |
| `Task.Run` (client) | Receives messages in the background while you type |
| `lock` (server) | Stops two threads changing shared data at the same time |

### Protocol
All messages are single lines of text.
1. The client sends a name.
2. The server replies `OK <welcome text>` or `ERROR <reason>`. After an error the client can send another name.
3. After `OK`, each line the client sends is a chat message, or a command if it starts with `/`.

## Project structure

    ChatSystem/
    ├── ChatSystem.sln
    ├── ChatServer/
    │   ├── Program.cs      Accepts connections, handles clients, broadcasts messages
    │   └── ChatUser.cs     One connected person
    └── ChatClient/
        └── Program.cs      Asks questions, connects, sends and receives

## Limitations
- Messages are sent as plain text with no encryption or login, so don't use it for anything private.
- Messages are not saved; you only see what is sent while you are connected.
- It is designed for a local network. Using it over the internet would need port forwarding and added security.
- No automated tests yet.

## Ideas for next steps
- Private messages (`/msg name text`)
- Saving chat history to a file
- A WPF or web client
- Unit tests for name validation
- TLS encryption

## License
MIT. See [LICENSE](LICENSE).

