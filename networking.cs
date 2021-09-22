using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MTLibrary {
    public struct Networking {
        public class Transmission {
            public Dictionary<String, String> content = new();

            /// <summary>
            /// Decodes <paramref name="candidateContent"/> bytes into the Transmission 
            /// <see cref="Transmission.content">Dictionary</see>
            /// </summary>
            private void DecodeContent(Byte[] candidateContent) {
                MemoryStream memStream = new(candidateContent);
                BinaryReader decoder = new(memStream);

                Int32 pairs = decoder.ReadInt32();

                for (Int32 i = 0; i < pairs; i++) {
                    String newKey = decoder.ReadString();
                    String newVal = decoder.ReadString();

                    if (!String.IsNullOrEmpty(this.content[newKey])) _= this.content.Remove(newKey);
                    this.content.Add(newKey, newVal);

                    decoder.Close();
                    memStream.Close();
                }

            }

            /// <summary>
            /// Encodes the <see cref="Transmission.content">Dictionary</see> bytes
            /// </summary>
            /// <returns>
            /// (<see cref="Dictionary{TKey, TValue}"/> as) <see cref="Byte"/>[]
            /// </returns>
            private Byte[] EncodeContent() {
                MemoryStream memStream = new();
                BinaryWriter encoder = new(memStream);

                Int32 length = this.content.Count;
                encoder.Write(length);

                Dictionary<String, String>.Enumerator contentEnumerator = this.content.GetEnumerator();
                for (Int32 i = 0; i < length; i++) {
                    if (contentEnumerator.MoveNext()) {
                        encoder.Write(contentEnumerator.Current.Key);
                        encoder.Write(contentEnumerator.Current.Value);
                    }
                }

                Byte[] got = memStream.ToArray();

                encoder.Close();
                memStream.Close();

                return got;
            }
            private void Decode(Byte[] data) {
                MemoryStream memStream = new(data);
                BinaryReader binReader = new(memStream);

                Int32 length = binReader.ReadInt32();
                this.DecodeContent(binReader.ReadBytes(length));

                binReader.Close();
                memStream.Close();
            }
            private Byte[] Encode() {
                MemoryStream memStream = new();
                BinaryWriter binWriter = new(memStream);

                Byte[] contentBytes = this.EncodeContent();
                binWriter.Write(contentBytes.Length);
                binWriter.Write(contentBytes);

                Byte[] got = memStream.ToArray();

                binWriter.Close();
                memStream.Close();

                return got;
            }

            /// <summary>
            /// Sends Encoded <see cref="content"/> over the specified
            /// <paramref name="socket"/>
            /// </summary>
            /// <returns>
            /// True, if the whole <see cref="Byte"/>[] array was sent; False, otherwise.
            /// </returns>
            public static Boolean Send(Transmission t, Socket socket) {
                _ = socket ??
                    throw new ArgumentNullException(nameof(socket));
                if (!socket.Connected)
                    throw new InvalidOperationException("Socket is null");

                Byte[] dat = t.Encode();
                return socket.Send(dat).Equals(dat.Length);
            }

            public Boolean Send(Socket socket) => Transmission.Send(this, socket);

            public Transmission(Socket sock) {
                Byte[] data = new Byte[sock.ReceiveBufferSize];
                _ = sock.Receive(data);
                this.Decode(data);
            }

            public Transmission(Byte[] data) => this.Decode(data);
        }
        public class Server {
            private IPHostEntry hostInfo;
            private IPAddress hostAddress;
            private IPEndPoint hostEndpoint;

            protected Action listener;
            protected Socket? sock;

            public Boolean acceptConnections = true;
            public delegate void Connector(Socket sock);
            public Connector Connect;

            /// <summary>
            /// Instantiates a <see cref="Socket"/> and
            /// accepts new connections into <paramref name="connector"/>
            /// <code>while (<see cref="Server.acceptConnections"/> == true)</code>
            /// using <see cref="AddressFamily.InterNetwork">IPv4</see>
            /// and <see cref="ProtocolType.Tcp">TCP</see>.
            /// </summary>
            /// <param name="connector">
            /// <see cref="Delegate"/> to Invoke with <see cref="Socket.Accept"/>
            /// </param>
            /// <param name="port">
            /// Port to use in <see cref="IPEndPoint"/>
            /// </param>
            public Server(Connector? connector, Int32 port = 11942) {
                this.hostInfo = Dns.GetHostEntry(Dns.GetHostName());
                this.hostAddress = this.hostInfo.AddressList[0];
                this.hostEndpoint = new IPEndPoint(this.hostAddress, port);

                this.Connect = connector ?? ((Socket s) => { });

                this.sock = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.sock.Bind(this.hostEndpoint);
                this.sock.Listen();
                this.listener = new(() => {
                    while (this.acceptConnections == true) {
                        this.Connect(this.sock.Accept());
                    }
                });
                this.listener.Invoke();
            }
            ~Server() {
                this.acceptConnections = false;
                if (this.sock != null)
                    this.sock.Close();
            }
        }
        public class Client {
            // private Guid uuid = Guid.NewGuid();

            protected Action listener = () => { };
            protected Socket? sock;

            public Boolean shouldListen = true;
            public delegate void Receiver(Socket sock, Byte[] data);
            public Receiver OnReceive = (Socket s, Byte[] d) => { };

            /// <summary>
            /// Sends a <see cref="Byte"/> array over the connected <see cref="Client.sock"/>.
            /// </summary>
            /// <param name="data">
            /// <see cref="Byte"/>s to send over the <see cref="Client.sock"/>.
            /// </param>
            /// <returns>
            /// The number of <see cref="Byte"/>s sent with <see cref="Client.sock"/>.
            /// </returns>
            public Int32 Send(Byte[] data) => this.sock != null ? this.sock.Send(data) : 0;

            /// <summary>
            /// Instantiates a listening <see cref="Socket"/>
            /// using <see cref="AddressFamily.InterNetwork">IPv4</see>
            /// and <see cref="ProtocolType.Tcp">TCP</see>.
            /// The Client listens and will call <see cref="OnReceive"/> with <see cref="Socket.Receive"/>
            /// <code>while (<see cref="Client.shouldListen"/> == true)</code>
            /// </summary>
            /// <param name="targetAddress">
            /// IP Address to connect with <see cref="IPEndPoint"/>.
            /// If null, then it will be set to <see cref="IPAddress.Any"/>
            /// </param>
            /// <param name="port">
            /// Port to connect with <see cref="IPEndPoint"/>.
            /// </param>
            public Client(IPAddress? targetAddress, Int32 port = 11942) {
                IPEndPoint targetEndpoint = new(targetAddress ?? IPAddress.Any, port);
                try {
                    this.sock = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.sock.Connect(targetEndpoint);

                    this.listener = new(() => {
                        while (this.shouldListen) {
                            Byte[] got = new Byte[this.sock.ReceiveBufferSize];
                            _ = this.sock.Receive(got);
                            this.OnReceive(this.sock, got);
                        }
                    });
                    this.listener.Invoke();
                } catch (Exception) { throw; }
            }
            ~Client() {
                this.shouldListen = false;
                if (this.sock != null) this.sock.Close();
            }
        }
    }
}
