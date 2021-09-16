using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MTLibrary
{
    public struct networking
    {
        public class Transmission
        {
            Dictionary<String, String> content = new();
            byte[] buffer;

            private void DecodeContent(byte[] candidateContent)
            {
                // Instantiate streams
                MemoryStream memStream = new(candidateContent);
                BinaryReader decoder = new(memStream);

                // Read number of dictionary pairs
                int pairs = decoder.ReadInt32();

                // Decode key, value pairs
                for (int i = 0; i < pairs; i++)
                {
                    // Read key & value from candidate stream
                    String newKey = decoder.ReadString();
                    String newVal = decoder.ReadString();

                    // Check if the candidate key is already in content
                    String got = String.Empty;
                    this.content.TryGetValue(newKey, out got);
                    if ((got == String.Empty || String.IsNullOrEmpty(got)) == false)
                    {
                        // If so, remove the key and value
                        this.content.Remove(newKey);
                    }
                    // Then, add the key & value to the content dictionary
                    this.content.Add(newKey, newVal);

                    // Finally, close streams
                    decoder.Close();
                    memStream.Close();
                }

            }
            private byte[] EncodeContent()
            {
                // Instantiate streams
                MemoryStream memStream = new();
                BinaryWriter encoder = new(memStream);

                // Encode number of dictionary pairs
                int length = this.content.Count;
                encoder.Write((Int32) length);

                // Encode key, value pairs
                Dictionary<String, String>.Enumerator contentEnumerator = this.content.GetEnumerator();
                for (int i = 0; i < length; i++)
                {
                    contentEnumerator.MoveNext();
                    encoder.Write((String) contentEnumerator.Current.Key);
                    encoder.Write((String) contentEnumerator.Current.Value);
                }

                // Fetch byte array
                byte[] got = memStream.ToArray();

                // Close streams
                encoder.Close();
                memStream.Close();

                // Return the bytes
                return got;
            }
            private void Decypher()
            {
                // Instantiate streams
                MemoryStream memStream = new(this.buffer);
                BinaryReader binReader = new(memStream);

                // Read content length, then decode it
                int length = binReader.ReadInt32();
                this.DecodeContent(binReader.ReadBytes(length));

                // Close streams
                binReader.Close();
                memStream.Close();
            }
            private byte[] Cypher()
            {
                // Instantiate streams
                MemoryStream memStream = new();
                BinaryWriter binWriter = new(memStream);

                // Write content length
                binWriter.Write((Int32) this.content.Count);

                // Encode & Write content bytes
                binWriter.Write((byte[]) this.EncodeContent());

                // Retrieve the Cyphered bytes
                byte[] got = memStream.ToArray();

                // Close streams
                binWriter.Close();
                memStream.Close();

                // Return the Cyphered bytes
                return got;
            }

            /// <summary>
            /// Sends the Cyphered Dictionary over the specified Socket
            /// </summary>
            /// <param name="sock">Socket to send data over</param>
            public void Send(Socket sock)
            {
                if (sock.Connected)
                {
                    sock.Send(this.Cypher());
                }
            }

            /// <summary>
            /// Recieves bytes over a Socket connection, and parses them into a Dictionary
            /// </summary>
            /// <param name="sock">Socket to receive on</param>
            public Transmission(Socket sock)
            {
                this.buffer = new byte[sock.ReceiveBufferSize];
                sock.Receive(this.buffer);
                this.Decypher();
            }

            /// <summary>
            /// Parses an array of bytes as a <String, String> Dictionary
            /// </summary>
            /// <param name="data"></param>
            public Transmission(byte[] data)
            {
                this.buffer = data;
                this.Decypher();
            }
        }
        public class Server
        {
            private IPHostEntry hostInfo;
            private IPAddress hostAddress;
            private IPEndPoint hostEndpoint;
            
            protected Socket sock;
            protected Action connector;

            public Action<Socket> OnConnect;
            public Boolean acceptConnections = true;

            public Server(int port=11942)
            {
                this.hostInfo = Dns.GetHostEntry(Dns.GetHostName());
                this.hostAddress = this.hostInfo.AddressList[0];
                this.hostEndpoint = new IPEndPoint(this.hostAddress, port);
                
                this.sock = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.sock.Bind(this.hostEndpoint);
                this.sock.Listen();
                this.connector = new(()=>
                {
                    while (this.acceptConnections == true)
                    {
                        OnConnect.Invoke(this.sock.Accept());
                    }
                });
                this.connector.Invoke();
            }
            ~Server()
            {
                this.acceptConnections = false;
                this.sock.Close();
            }
        }
        public class Client
        {
            private Guid uuid = Guid.NewGuid();

            protected Action listener;
            protected Socket sock;

            public Boolean shouldListen = true;
            public Action<Socket, byte[]> OnReceived;

            public void Send(byte[] data)
            {
                // Send data if Socket is Connected
                if (this.sock.Connected) this.sock.Send(data);
            }

            public void Connect(IPAddress targetAddress=null, int port=11942)
            {
                // Determine IP information
                if (targetAddress.Equals(null)) targetAddress = IPAddress.Any;
                IPEndPoint targetEndpoint = new IPEndPoint(targetAddress, port);

                // Try to Connect the Socket to the target IP
                try
                {
                    // Instantiate the Socket
                    this.sock = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.sock.Connect(targetEndpoint);

                    // Declare listening method
                    this.listener = new(() =>
                    {
                        while (this.shouldListen)
                        {
                            byte[] got = new byte[this.sock.ReceiveBufferSize];
                            this.sock.Receive(got);
                            this.OnReceived.Invoke(this.sock, got);
                        }
                    });
                    // Start listening to the Socket
                    this.listener.Invoke();
                } catch (Exception)
                {
                    // Could not Connect!
                    throw;
                }
            }
            ~Client() {
                this.shouldListen = false;
                this.sock.Close();
            }
        }
    }
}
