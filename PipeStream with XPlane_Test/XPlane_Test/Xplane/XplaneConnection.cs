using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace XPlane
{
	public delegate void NewPakcet(byte[] data);

	public class XplaneConnection
	{
		//X-Plane binds to port 49000 for receiving data, 49001 for sending
		//This application recevie data at 49003 port
		public static readonly int XPLANE_RECEIVER_PORT = 49000;
		public static readonly int XPLANE_SENDER_PORT = 49001;
		public static readonly int CLIENT_RECEIVER_PORT = 49003;
		public static readonly int CLIENT_SENDER_PORT = 49004;

		//locker
		private object _lockRecv = new object();
		private object _lockSend = new object();

		public Socket clientSocket;
		public Socket serverSocket;
		private string ipAddress = String.Empty;
		private bool isConnectionOpened = false;

		public XplaneConnection(string ipAddress = "127.0.0.1")
		{
			this.ipAddress = ipAddress;
		}

		public void OpenConnections()
		{
			if (!isConnectionOpened)
			{
				// Addressing
				IPAddress ipAddress_server = IPAddress.Parse(this.ipAddress);
				IPAddress ipAddress_client = IPAddress.Parse(this.ipAddress);

				this.clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp);
				this.clientSocket.ReceiveBufferSize = 5120;
				this.clientSocket.EnableBroadcast = true;
				this.clientSocket.Bind(new IPEndPoint(ipAddress_client, CLIENT_RECEIVER_PORT));
				this.clientSocket.Connect(new IPEndPoint(ipAddress_server, XPLANE_SENDER_PORT));

				this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				this.serverSocket.Bind(new IPEndPoint(ipAddress_client, CLIENT_SENDER_PORT));
				this.serverSocket.Connect(new IPEndPoint(ipAddress_server, XPLANE_RECEIVER_PORT));

				this.isConnectionOpened = true;
			}
		}

		public void CloseConnections()
		{
			clientSocket.Close();
			serverSocket.Close();
			this.isConnectionOpened = false;
		}

		public void SendPacket(byte[] data)
		{
			lock (this._lockSend)
			{
				if (this.isConnectionOpened == true)
					this.serverSocket.Send(data);
			}
		}

		public int ReceivePacket(ref byte[] data)
		{
			int i;

			lock (this._lockRecv)
			{
				i = this.clientSocket.Receive(data);                   
			}

			return i;
		}
	}
}