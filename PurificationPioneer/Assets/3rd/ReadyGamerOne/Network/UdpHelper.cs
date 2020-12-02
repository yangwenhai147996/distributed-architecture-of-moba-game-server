﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Network
{
    public class UdpHelper
    {
        public bool IsValid => clientSocket != null;
        public int LocalUdpPort { get; private set; }
        public string LocalIp { get; private set; }
        public string TargetIp { get; private set; }
        public int TargetPort { get; private set; }
        
        #region private

        private Socket clientSocket;
        private Thread recvThread;
        private byte[] udpRecvBuff;
        
        
        private Action<byte[], int, int> onRecvCmd;
        private Action<Exception> onException;
        private Func<bool> ifEnableSocketLog = () => true;
                

        #endregion

        
        public UdpHelper(string targetIp,int targetPort, int localPort, Action<Exception> onException, Action<byte[],int,int> onRecvCmd,
            int maxUdpPackageSize, Func<bool> enableSocketLog=null, Action onFinishedSetup=null)
        {
            Assert.IsNotNull(onException);
            Assert.IsNotNull(onRecvCmd);

            if (null != enableSocketLog)
                this.ifEnableSocketLog = enableSocketLog;
                
            this.onException = onException;
            this.onRecvCmd = onRecvCmd;
            this.udpRecvBuff=new byte[maxUdpPackageSize];
            this.LocalUdpPort = localPort;
            this.TargetIp = targetIp;
            this.TargetPort = targetPort;
            NetUtil.GetSuitableIp(
                targetIp,
                targetPort,
                suitableIp =>
                {
                    this.LocalIp = suitableIp;
                    
                    //创建udpSocket
                    try
                    {
                        this.clientSocket=new Socket(
                            AddressFamily.InterNetwork,
                            SocketType.Dgram,
                            ProtocolType.Udp);
                        //绑定本地端口
                        var localPoint = new IPEndPoint(IPAddress.Parse(this.LocalIp), LocalUdpPort);
                        this.clientSocket.Bind(localPoint);
                        
                        this.recvThread=new Thread(RecvThread);
                        this.recvThread.Start();
                        
        #if DebugMode
                        if(ifEnableSocketLog())
                            Debug.Log($"Udp[local-{this.LocalIp}:{LocalUdpPort}] 开始接收");
        #endif
                        onFinishedSetup?.Invoke();
                    }
                    catch (Exception e)
                    {
                        this.onException(e);
                    }                    
                });
            

        }

        /// <summary>
        /// 关闭接收器
        /// </summary>
        public void CloseReceiver()
        {
            udpRecvBuff = null;
            
            if (null != recvThread)
            {
                recvThread.Interrupt();
                recvThread.Abort();
            }
            recvThread = null;
            
            if (this.clientSocket != null && this.clientSocket.Connected)
            {
                this.clientSocket.Close();
            }
            this.clientSocket = null;
            
#if DebugMode
            if(ifEnableSocketLog())
                Debug.Log($"Udp[local-{this.LocalIp}:{LocalUdpPort}] 关闭连接");
#endif
        }


        /// <summary>
        /// 发送内容
        /// </summary>
        /// <param name="content"></param>
        public void Send(byte[] content)
        {
            try
            {
                this.clientSocket.BeginSendTo(
                    content,
                    0,
                    content.Length,
                    SocketFlags.None,
                    new IPEndPoint(IPAddress.Parse(TargetIp), TargetPort), 
                    OnUdpSend,
                    this.clientSocket);
                
#if DebugMode
                if(ifEnableSocketLog())
                    Debug.Log($"Udp[Local:{this.LocalIp}:{LocalUdpPort}->{TargetIp}:{TargetPort}] 发送数据[{content.Length}]");
#endif
            }
            catch (Exception e)
            {
                onException(e);
            }
        }

        #region Private

        
        /// <summary>
        /// UDP收数据线程
        /// </summary>
        private void RecvThread()
        {
            while (true)
            {
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    var receivedLength = this.clientSocket.ReceiveFrom(this.udpRecvBuff,ref remote);
                    
#if DebugMode
                    if(ifEnableSocketLog())
                        Debug.Log($"Udp[local-{this.LocalIp}:{LocalUdpPort}] 收到数据[{receivedLength}]");
#endif
                    
                    this.onRecvCmd(this.udpRecvBuff, 0, receivedLength);
                }
                catch (Exception e)
                {  
                    if(this.onException!=null) 
                        this.onException(e);
                    else
                        throw e;
                    break;
                }
            }
        }   
        
        private void OnUdpSend(IAsyncResult ar)
        {
            try
            {
                var client = ar.AsyncState as Socket;
                Assert.IsNotNull(client);
                client.EndSendTo(ar);
            }
            catch (Exception e)
            {
                onException(e);
            }
        }        

        #endregion
    }
}