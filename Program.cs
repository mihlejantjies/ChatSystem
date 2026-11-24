// THE CLIENT
// This is what each person runs to join the chat.
// Start the server first.

using System.Net.Sockets;

const int DefaultPort = 5000;

Console.WriteLine("=== Chat Client ===");
Console.WriteLine();

// ----- Question 1: where is the server? -----
Console.WriteLine("Where is the server running?");
Console.WriteLine("  - On this computer: just press Enter");
Console.WriteLine("  - On another computer: type its IP address, for example 192.168.0.12");
Console.Write("Server address: ");
string host = (Console.ReadLine() ?? "").Trim();
if (host == "")
{
    host = "127.0.0.1"; // 127.0.0.1 always means "this computer"
}

// ----- Question 2: which port? -----
int port = AskForPort();

// ----- Connect (and offer to try again if it fails) -----
TcpClient? tcp = null;
while (tcp == null)
{
    Console.WriteLine();
    Console.WriteLine($"Connecting to {host} on port {port}...");

    var attempt = new TcpClient();
    try
    {
        await attempt.ConnectAsync(host, port);
        tcp = attempt;
    }
    catch (SocketException ex)
    {
        attempt.Close();
        Console.WriteLine($"Could not connect: {ex.Message}");
        Console.WriteLine("Check that the server is running and that the address and port are correct.");

        if (!AskYesNo("Try again?"))
        {
            return;
        }
    }
}

NetworkStream stream = tcp.GetStream();
var reader = new StreamReader(stream);                      // reads text FROM the server
var writer = new StreamWriter(stream) { AutoFlush = true }; // sends text TO the server

// ----- Question 3: what is your name? (the server checks it) -----
Console.WriteLine();
Console.WriteLine("Connected! Now choose a name.");
Console.WriteLine("Use letters, numbers, - and _ only (no spaces), up to 20 characters.");

string? welcome = null;
while (welcome == null)
{
    Console.Write("Your name: ");
    string name = (Console.ReadLine() ?? "").Trim();

    if (name == "")
    {
        Console.WriteLine("Your name cannot be empty.");
        continue;
    }

    await writer.WriteLineAsync(name);

    // The server answers "OK ..." or "ERROR ...".
    string? reply = await reader.ReadLineAsync();

    if (reply == null)
    {
        Console.WriteLine("The server closed the connection.");
        return;
    }

    if (reply.StartsWith("OK "))
    {
        welcome = reply.Substring(3);
    }
    else if (reply.StartsWith("ERROR "))
    {
        Console.WriteLine(reply.Substring(6));
    }
    else
    {
        Console.WriteLine("Unexpected reply from the server: " + reply);
        return;
    }
}

Console.WriteLine();
Console.WriteLine(welcome);
Console.WriteLine("Type a message and press Enter. Type /quit to leave.");
Console.WriteLine();

// Listen for messages from the server in the BACKGROUND,
// so we can still type at the same time.
bool serverClosed = false;
bool leaving = false; // becomes true when WE choose to leave

_ = Task.Run(async () =>
{
    try
    {
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            Console.WriteLine(line);
        }
    }
    catch (IOException)
    {
        // connection closed
    }
    catch (ObjectDisposedException)
    {
        // we closed it ourselves
    }

    if (!leaving)
    {
        serverClosed = true;
        Console.WriteLine("Disconnected from the server. Press Enter to exit.");
    }
});

// Meanwhile, this loop reads what YOU type and sends it.
while (true)
{
    string? input = Console.ReadLine();

    if (serverClosed || input == null)
    {
        break;
    }

    input = input.Trim();

    if (input == "/quit")
    {
        break;
    }

    if (input == "")
    {
        continue;
    }

    try
    {
        await writer.WriteLineAsync(input);
    }
    catch (IOException)
    {
        Console.WriteLine("Could not send. The connection to the server was lost.");
        break;
    }
}

leaving = true;
tcp.Close();
Console.WriteLine("Goodbye!");

// ---------- Methods ----------

// Asks which port to use. Pressing Enter picks the default.
int AskForPort()
{
    while (true)
    {
        Console.Write($"Port (press Enter for {DefaultPort}): ");
        string text = (Console.ReadLine() ?? "").Trim();

        if (text == "")
        {
            return DefaultPort;
        }

        if (int.TryParse(text, out int chosen) && chosen >= 1024 && chosen <= 65535)
        {
            return chosen;
        }

        Console.WriteLine("Please enter a number between 1024 and 65535.");
    }
}

// Asks a yes/no question and returns true for yes.
bool AskYesNo(string question)
{
    while (true)
    {
        Console.Write($"{question} (y/n): ");
        string answer = (Console.ReadLine() ?? "").Trim().ToLower();

        if (answer == "y" || answer == "yes")
        {
            return true;
        }

        if (answer == "n" || answer == "no")
        {
            return false;
        }

        Console.WriteLine("Please type y or n.");
    }
}
