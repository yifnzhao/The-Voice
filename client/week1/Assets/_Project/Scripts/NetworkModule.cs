using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using UnityEngine;
#endif

namespace ShuoScripts
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public class NetworkModule : MonoBehaviour
#else
    public class NetworkModule
#endif
    {
        public int sendPort = 6000;
        public int recvPort = 6001;

        TcpClient clientSend = null;
        TcpListener clientRecv = null;

        Thread thread;
        object lockObj = new object();

        public delegate void OutputDelegate(byte []_data);
        public OutputDelegate Output;
        IPEndPoint ipep = null;
        bool done = false;
        bool dataRecvFlag = false;
        byte[] dataRecv;
        bool isServer = false;
        bool isClient = false;

        public void Init(bool _isServer)
        {
            if (_isServer)
            {
                isServer = true;
                isClient = false;
            }
            else
            {
                isClient = true;
                isServer = false;

                if (clientSend == null)
                {
                    clientSend = new TcpClient();
                    clientSend.Connect("localhost", sendPort);
                }
            }
        }

        public void Recv()
        {
            try
            {

                if (isClient)
                {
                    thread = new Thread(new ThreadStart(ClientRecvThread));
                    thread.Start();
                }
                else if (isServer)
                {
                    if (clientRecv == null)
                        clientRecv = new TcpListener(recvPort);

                    clientRecv.Start();
                    thread = new Thread(new ThreadStart(RecvThread));
                    thread.Start();
                }


            }
            catch(Exception e)
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.Log(e.ToString());
#else
                Console.WriteLine(e.ToString());
#endif
            }

        }

        void ClientRecvThread()
        {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Debug.Log("wait for receive");
#else
            Console.WriteLine("wait for receive");
#endif
            while (true)
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
                        Debug.Log("+++ sizeInt" + sizeInt);
#endif
                        if (sizeInt == 0)
                            continue;
                        dataRecv = new byte[sizeInt];
                        clientStream.Read(dataRecv, 0, sizeInt);
                    }
                    if (dataRecv.Length == 0)
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
        NetworkStream serverStream;

        void RecvThread()
        {
            try
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.Log("wait for receive");
#else
                Console.WriteLine("wait for receive");
#endif
                TcpClient newClient = null;
                newClient = clientRecv.AcceptTcpClient();
                while (!done)
                {
                    serverStream = newClient.GetStream();
                    if (!serverStream.DataAvailable)
                        continue;
                    lock (lockObj)
                    {
                        byte[] sizebyte = new byte[sizeof(Int32)];
                        serverStream.Read(sizebyte, 0, sizeof(Int32));
                        int sizeInt = BitConverter.ToInt32(sizebyte, 0);
                        dataRecv = new byte[sizeInt];
                        serverStream.Read(dataRecv, 0, sizeInt);
                    }
                    if (dataRecv.Length == 0)
                        continue;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    Debug.Log("Received: " + dataRecv.Length + " bytes");
                    dataRecvFlag = true;
#else
                    Console.WriteLine("Received: " + dataRecv.Length + " bytes");
                    if (Output != null)
                        Output(dataRecv);
#endif
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
        NetworkStream clientStream;
        public void Send(byte[] _data)
        {
            try
            {
                if (isClient)
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
                    }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Debug.Log(_data.Length + " bytes sent");
#else
                    Console.WriteLine(_data.Length + " bytes sent");
#endif
                }
                else if (isServer)
                {
                    if (serverStream != null && serverStream.CanWrite)
                    {
                        int len = _data.Length;
                        serverStream.Write(BitConverter.GetBytes(len), 0, sizeof(Int32));
                        serverStream.Write(_data, 0, _data.Length);
                        
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
        private void Start()
        {
            Init(false);
            Recv();


            //if (udpClientSend == null)
             //   udpClientSend = new UdpClient(sendPort);
            //udpClientSend.Connect("127.0.0.1", sendPort);
        }

        private void Update()
        {
            lock (lockObj)
            {
                if (dataRecvFlag)
                {
                    try
                    {
                        Debug.Log("Received: " + dataRecv.Length + " bytes");
                        if (Output != null)
                            Output(dataRecv);
                    }
                    catch (Exception e)
                    {
                        dataRecvFlag = false;
                        Debug.LogError(e.ToString());
                    }
                    dataRecvFlag = false;
                }
            }

            // test
            if (Input.GetKeyUp(KeyCode.Space))
            {
                string test = "testDatatestDatatestDatatestData";
                Send(Encoding.ASCII.GetBytes(test));
            }
        }
        void OnApplicationQuit()
        {
            done = true;
            if (clientSend != null)
                clientSend.Close();

            if (clientRecv != null)
                clientRecv.Stop();

            if (thread != null)
                thread.Abort();
        }
#endif
    }
}
