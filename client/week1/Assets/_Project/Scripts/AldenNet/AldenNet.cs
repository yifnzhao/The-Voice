using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using UnityEngine;
#endif

namespace AldenNet
{
    public class PredefinedMsg
    {
        public const uint HEARBEAT_ToClient = 9998;
        public const uint HEARBEAT_ToServer = 9999;
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public class AldenNet : MonoBehaviour
#else
    public class AldenNet
#endif
    {
        public int port = 6000;
        bool isServer = false;

        AldenNetClient client = null;
        AldenNetServer server = null;

        public AldenNetServer GetSerer() { return server; }
        public AldenNetClient GetClient() { return client; }

        public void Init(bool _isServer)
        {
            if (_isServer)
            {
                isServer = true;
                server = new AldenNetServer(port);
            }
            else
            {
                isServer = false;
                client = new AldenNetClient(port);
            }
        }


#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        private void Start()
        {
            // test as a client
            Init(false);

            // test as a server
            //Init(true);
            //server.Listen();    // stuck here!
        }

        private void Update()
        {
            if (server != null)
                server.Update();
            if (client != null)
                client.Update();
        }

        private void OnGUI()
        {
            //if (GUILayout.Button("Send"))
            //{
            //    if (isServer)
            //    {
            //        //server.SendToPeer()
            //    }
            //    else
            //    {
            //        for (int i = 0; i < 3; i++)
            //        {
            //            string str = UnityEngine.Random.Range(0, 100).ToString();
            //            Debug.Log(str);
            //            byte[] test = Encoding.UTF8.GetBytes(str);
            //            client.Send(test);
            //        }
            //    }
            //}
        }

        void OnApplicationQuit()
        {
            if (isServer)
            {
                if (server != null)
                    server.Close();
                server = null;
            }
            else
            {
                if (client != null)
                    client.Close();
                client = null;
            }
        }
#endif
    }

    public class AldenNetServer
    {
        public class AldenNetPeer
        {
            public TcpClient tcpClient;
            public uint clientID;
            public bool dataRecvFlag = false;

            public delegate void OutputDelegate(byte[] _data);
            public OutputDelegate Output;
            public byte[] dataRecv;
            public AldenNetPeer(TcpClient _tcpClient)
            {
                tcpClient = _tcpClient;
            }

            public void Update()
            {
                if (dataRecvFlag)
                {
                    try
                    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                        Debug.Log("ClientID:" + clientID + " Received: " + dataRecv.Length + " bytes");
#else
                        Console.WriteLine("ClientID:" + clientID + " Received: " + dataRecv.Length + " bytes");
#endif
                        if (Output != null)
                        {
                            Output(dataRecv);
                            dataRecv = null;
                        }
                    }
                    catch (Exception e)
                    {
                        dataRecvFlag = false;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                        Debug.LogError(e.ToString());
#else
                        Console.WriteLine(e.ToString());
#endif
                    }
                    dataRecvFlag = false;
                }
            }
        }


        const uint heartbeatInterval = 1000;    // ms
        TcpListener clientRecv = null;
        object lockObj = new object();
        bool done = false;

        IPEndPoint ipep = null;

        List<AldenNetPeer> peerList = new List<AldenNetPeer>();
        List<Thread> threads = new List<Thread>();

        uint clientID = 0;

        Timer timer = null;

        public delegate void OnPeerConnectedDelegate(AldenNetPeer _p);
        public OnPeerConnectedDelegate OnPeerConnected;
        public delegate void OnPeerDisonnectedDelegate(AldenNetPeer _p);
        public OnPeerDisonnectedDelegate OnPeerDisonnected;


        public AldenNetServer(int _port)
        {
            ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);

            if (clientRecv == null)
                clientRecv = new TcpListener(ipep);
            clientRecv.Start();

            //timer = new Timer(new TimerCallback(SendHeartbeat), null, 0, heartbeatInterval);

        }

        public List<AldenNetPeer> GetAllPeer() { return peerList; }

        public AldenNetPeer GetPeer(uint _clientID)
        {
            try
            {
                if (peerList.Count == 0)
                    throw new Exception("Peer list is empty");
                if (!peerList.Exists(p => p.clientID == _clientID))
                    throw new Exception("Peer by clientID:" + _clientID + " not found");
                return peerList.Find(p => p.clientID == _clientID);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.Log(e.ToString());
#else
                Console.WriteLine(e.ToString());
#endif
                return null;
            }
        }

        public void SendToPeer(AldenNetPeer _peer, byte[] _data)
        {
            try
            {
                if (_peer == null || !peerList.Exists(x => x == _peer))
                    throw new Exception("Peer not exist");
                if (_peer.tcpClient == null || !_peer.tcpClient.Connected)
                    return;
                NetworkStream clientStream = _peer.tcpClient.GetStream();
                if (clientStream == null)
                    return;

                if (clientStream.CanWrite)
                {
                    int len = _data.Length;
                    clientStream.Write(BitConverter.GetBytes(len), 0, sizeof(Int32));
                    clientStream.Write(_data, 0, _data.Length);
                    clientStream.Flush();
                }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.Log(_data.Length + " bytes sent to clientID:" + _peer.clientID);
#else
                Console.WriteLine(_data.Length + " bytes sent to clientID:" + _peer.clientID);
#endif
            }
            catch (Exception e)
            {
            }
        }

        public void Listen()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Debug.Log("Server Listening @" + ipep.ToString());
#else
            Console.WriteLine("Server Listening @" + ipep.ToString());
#endif
            while (!done)
            {

                TcpClient newClient = clientRecv.AcceptTcpClient();
                // find in peers
                AldenNetPeer peer = null;
                if (peerList.Exists(a => a.tcpClient == newClient))
                {
                    peer = peerList.Find(x => x.tcpClient == newClient);
                }
                else
                {
                    peer = new AldenNetPeer(newClient);
                    peer.clientID = clientID;
                    clientID++;
                    peerList.Add(peer);
                }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.Log("Client " + peer.clientID + " Connected");
#else
                Console.WriteLine("Client " + peer.clientID + " Connected");
#endif
                // event
                if (OnPeerConnected != null)
                    OnPeerConnected(peer);

                // start new thread for every client
                Thread clientThread = new Thread(() => RecvThread(newClient));
                clientThread.Start();
                clientThread.Name = peer.clientID.ToString();
                threads.Add(clientThread);
            }
        }

        AldenNetPeer GetPeer(TcpClient _tcpClient)
        {
            return peerList.Find(x => x.tcpClient == _tcpClient);
        }

        void RemovePeer(AldenNetPeer _peer)
        {
            if (OnPeerDisonnected != null)
                OnPeerDisonnected(_peer);

            lock (lockObj)
            {
                peerList.Remove(_peer);
                foreach (var p in threads)
                {
                    if (p.Name == _peer.clientID.ToString())
                    {
                        threads.Remove(p);
                        break;
                    }
                }
            }
        }

        void SendHeartbeat(object timmer)
        {
            try
            {
                lock (lockObj)
                {
                    Console.WriteLine("peerList size:" + peerList.Count);
                    foreach (var p in peerList)
                    {
                        if (p != null)
                        {
                            byte[] b = BitConverter.GetBytes(PredefinedMsg.HEARBEAT_ToClient);
                            SendToPeer(p, b);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                            //Debug.Log("Heartbeat sent to Client" + p.clientID);
#else
                            //Console.WriteLine("Heartbeat sent to Client" + p.clientID);
#endif
                        }
                    }
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.LogError(e.ToString());
#else
                Console.WriteLine(e.ToString());
#endif
            }

        }

        void RecvThread(TcpClient _client)
        {
            try
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.Log("wait for receive");
#else
                Console.WriteLine("wait for receive");
#endif
                while (true)
                {
                    lock (lockObj)
                    {
                        AldenNetPeer p = GetPeer(_client);
                        if (!_client.Connected)
                        {
                            // disconnected
                            RemovePeer(p);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                            Debug.Log("Client" + p.clientID + " disconnected");
#else
                            Console.WriteLine("Client" + p.clientID + " disconnected");
#endif
                            break;
                        }

                        NetworkStream serverStream = _client.GetStream();
                        if (!serverStream.DataAvailable)
                            continue;
                        byte[] dataRecv = null;
                        byte[] sizebyte = new byte[sizeof(Int32)];
                        serverStream.Read(sizebyte, 0, sizeof(Int32));
                        int sizeInt = BitConverter.ToInt32(sizebyte, 0);
                        dataRecv = new byte[sizeInt];
                        serverStream.Read(dataRecv, 0, sizeInt);

                        // handle hearbeat
                        if (sizeInt == sizeof(uint) && BitConverter.ToUInt32(dataRecv, 0) == PredefinedMsg.HEARBEAT_ToServer)
                        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                            //Debug.Log("Heartbeat back from Client" + p.clientID);
#else
                            //Console.WriteLine("Heartbeat back from Client" + p.clientID);
#endif
                            continue;       // application layer don't need handle this msg
                        }
                        if (dataRecv == null || dataRecv.Length == 0)
                            continue;

                        p.dataRecvFlag = true;
                        p.dataRecv = dataRecv;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                        //Debug.Log("Received: " + dataRecv.Length + " bytes");
                        //dataRecvFlag = true;
#else
                        p.Update();
#endif
                    }
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.LogError(e.ToString());
#else
                Console.WriteLine(e.ToString());
#endif
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public void Update()
        {
            foreach (var p in peerList)
            {
                if (p != null)
                    p.Update();
            }
        }
#endif

        public void Close()
        {
            done = true;
            if (clientRecv != null)
                clientRecv.Stop();
            foreach (var t in threads)
            {
                if (t != null)
                    t.Abort();
            }
        }
    }
    public class AldenNetClient
    {
        TcpClient clientSend = null;
        Thread thread;
        object lockObj = new object();
        NetworkStream clientStream;
        bool dataRecvFlag = false;
        byte[] dataRecv;

        public delegate void OutputDelegate(byte[] _data);
        public OutputDelegate Output;
        bool done = false;
        public AldenNetClient(int _port)
        {
            if (clientSend == null)
            {
                clientSend = new TcpClient();
                clientSend.Connect("localhost", _port);
                clientStream = clientSend.GetStream();

                thread = new Thread(new ThreadStart(RecvThread));
                thread.Start();
            }
        }

        public void Send(byte[] _data)
        {
            try
            {
                if (clientSend == null)
                {
                    return;
                }
                clientStream = clientSend.GetStream();

                if (clientStream.CanWrite)
                {
                    int len = _data.Length;
                    clientStream.Write(BitConverter.GetBytes(len), 0, sizeof(Int32));
                    clientStream.Write(_data, 0, _data.Length);
                    //clientStream.Flush();
                }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.Log(_data.Length + " bytes sent");
#else
                Console.WriteLine(_data.Length + " bytes sent");
#endif
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.LogError(e.ToString());
#else
                Console.WriteLine(e.ToString());
#endif
            }
        }

        void RecvThread()
        {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Debug.Log("wait for receive");
#else
            Console.WriteLine("wait for receive");
#endif
            while (!done)
            {
                try
                {
                    if (clientStream == null)
                        continue;
                    if (!clientStream.DataAvailable)
                        continue;
                    lock (lockObj)
                    {
                        //dataRecv = Encoding.ASCII.GetBytes(str);
                        byte[] sizebyte = new byte[sizeof(Int32)];
                        clientStream.Read(sizebyte, 0, sizeof(Int32));
                        int sizeInt = BitConverter.ToInt32(sizebyte, 0);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                        Debug.Log("Total Net Package Size:" + sizeInt);
#endif
                        if (sizeInt <= 0)
                            continue;
                        dataRecv = new byte[sizeInt];
                        int read = 0;
                        int block = 0;
                        while ((block = clientStream.Read(dataRecv, read, dataRecv.Length - read)) > 0)
                        {
                            read += block;

                            if (read >= sizeInt)
                                break;
                        }
                        //clientStream.Read(dataRecv, 0, sizeInt);

                        // handle hearbeat
                        if (sizeInt == sizeof(uint) && BitConverter.ToUInt32(dataRecv, 0) == PredefinedMsg.HEARBEAT_ToClient)
                        {
                            Send(BitConverter.GetBytes(PredefinedMsg.HEARBEAT_ToServer));
                        }
                    }
                    if (dataRecv == null || dataRecv.Length == 0)
                        continue;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    dataRecvFlag = true;
#else
                    Console.WriteLine("Received: " + dataRecv.Length + " bytes");
                    if (Output != null)
                        Output(dataRecv);
#endif
                }
                catch (Exception e)
                {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    Debug.LogError(e.ToString());
#else
                    Console.WriteLine(e.ToString());
#endif
                }
            }

        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public void Update()
        {
            if (dataRecvFlag)
            {
                try
                {
                    Debug.Log("Received: " + dataRecv.Length + " bytes");
                    if (Output != null)
                    {
                        Output(dataRecv);
                        dataRecv = null;
                    }
                }
                catch (Exception e)
                {
                    dataRecvFlag = false;
                    Debug.LogError(e.ToString());
                }
                dataRecvFlag = false;
            }
        }
#endif
        public void Close()
        {
            done = true;
            if (clientSend != null)
            {
                clientSend.Close();
                clientSend.Dispose();
                clientSend = null;
            }
            if (clientStream != null)
                clientStream.Close();
            if (thread != null)
                thread.Abort();
        }
    }


}
