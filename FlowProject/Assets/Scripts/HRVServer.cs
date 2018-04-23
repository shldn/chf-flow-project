using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

public class HRVServer {

    static int port = 80;
    static TcpListener hrvServer = null;

    public Action<HRVMessage> MessageReceived;

    private static HRVServer mInst = null;
    public static HRVServer Inst {
        get {
            if (mInst == null)
                mInst = new HRVServer();
            return mInst;
        }
    }

    private HRVServer() {

        Debug.Log("HRVServer Init");
        IPAddress localAddr = IPAddress.Parse(GetLocalIPAddress());

        try {
            hrvServer = new TcpListener(localAddr, port);
        } catch(Exception e) {
            Debug.LogError("Exception with TcpListener: " + e.ToString());
        }

        // Start listening for client requests.
        try { 
            hrvServer.Start();
        } catch(SocketException e) {
            Debug.LogError("Exception starting tcp listener: " + e.ErrorCode + " " + e.ToString());
        }

        ConnectToNextClient();
    }

    public static string GetLocalIPAddress() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        string returnIP = "";
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                if (string.IsNullOrEmpty(returnIP) || returnIP.StartsWith("192"))
                    returnIP = ip.ToString();
            }
        }
        return returnIP;
    }

    private void ConnectToNextClient() {
        try {
            hrvServer.BeginAcceptTcpClient(new AsyncCallback(AcceptConnectionFromViewer), hrvServer);
        }
        catch(Exception e) {
            Debug.LogError("Exception in Begin Accept TCP Client: " + e.ToString());
        }
    }

    private void AcceptConnectionFromViewer(IAsyncResult ar) {
        TcpListener listener = (TcpListener)ar.AsyncState;
        TcpClient client = hrvServer.EndAcceptTcpClient(ar);

        NetworkStream stream = client.GetStream();
        string msg = ReadMessage(stream);
        string json = GetMessageFromHTTPPostRequest(msg);

        HRVMessage hrvMsg = JsonUtility.FromJson<HRVMessage>(json);

        if (MessageReceived != null)
            MessageReceived(hrvMsg);

        // wait for next message
        ConnectToNextClient();
    }


    string ReadMessage(NetworkStream stream) {
        StringBuilder message = new StringBuilder();
        if (stream.CanRead) {
            byte[] myReadBuffer = new byte[1024];
            
            int numberOfBytesRead = 0;

            // Incoming message may be larger than the buffer size.
            do {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                message.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
            }
            while (stream.DataAvailable);
        }
        else {
            Debug.LogError("Sorry.  You cannot read from this NetworkStream.");
        }
        return message.ToString();
    }

    string GetMessageFromHTTPPostRequest(string msg) {

        // find the start of the data we are interested in. 
        int index = msg.IndexOf("Content-Length: ");
        if (index == -1)
            return "";
        index = msg.IndexOf("{", index);

        return index != -1 ? msg.Substring(index) : "";
    }
}
