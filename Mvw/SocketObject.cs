﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DVW
{
   /// <summary>  
    /// Socket基础类  
    /// </summary>  
    public abstract class SocketObject
    {
        /// <summary>  
        /// 初始化Socket方法  
        /// </summary>  
        /// <param name="ipAddress"></param>  
        /// <param name="port"></param>  
        public abstract void InitSocket(IPAddress ipAddress, int port);
        public abstract void InitSocket(string ipAddress, int port);

        /// <summary>  
        /// Socket启动方法  
        /// </summary>  
        public abstract void Start();

        /// <summary>  
        /// Sockdet停止方法  
        /// </summary>  
        public abstract void Stop();

    }
}
