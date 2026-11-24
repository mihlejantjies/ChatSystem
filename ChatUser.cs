using System.Net.Sockets;

// One person connected to the server.
public class ChatUser
{
    private readonly object _sendLock = new object();

    public string Name { get; }
    public TcpClient Connection { get; }   // the network link to this person
    public StreamWriter Writer { get; }    // used to send text TO this person

    public ChatUser(string name, TcpClient connection, StreamWriter writer)
    {
        Name = name;
        Connection = connection;
        Writer = writer;
    }

    // Send one line of text to this person.
    // The lock makes sure two messages can't be written at exactly the same moment,
    // which could mix their text together.
    public void Send(string message)
    {
        lock (_sendLock)
        {
            Writer.WriteLine(message);
        }
    }
}
