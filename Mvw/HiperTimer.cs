using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;
namespace DVW
{
    class HiperTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);   //查询高精度计数器该时刻的实际值
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);        //查询高精度计数器每秒的计数次数

        private long _startTime, _stopTime;
        private readonly long _frequency;

        // 构造函数
        public HiperTimer()
        {
            _startTime = 0;
            _stopTime = 0;

            if (QueryPerformanceFrequency(out _frequency) == false)
            {// 不支持高性能计数器
                throw new Win32Exception();
            }
        }

        // 开始计时器
        public void Start()
        {
            // 来让等待线程工作
            //Thread.Sleep(0);
            QueryPerformanceCounter(out _startTime);
        }

        // 停止计时器
        public void Stop()
        {
            QueryPerformanceCounter(out _stopTime);
        }
        public double getUs()
        {

            QueryPerformanceCounter(out _stopTime);
            double estm=(double)(_stopTime - _startTime) * 1000000 / _frequency;
            return estm;
        }

        public void delay_us(int us)
        {
            long stm, etm;
            QueryPerformanceCounter(out stm);
            Start();
            while (true)
            {
                QueryPerformanceCounter(out etm);
                double estm = (etm - stm) * 1000000 / (double)_frequency;
                if (estm >= us) break;
            }
        }
        // 返回计时器经过时间(单位：秒)
        public double EliminatedSecond
        {
            get
            {
                return (_stopTime - _startTime) / (double)_frequency;
            }
        }

        public double EliminatedMilliSecond
        {
            get { return (double)(_stopTime - _startTime) * 1000 / _frequency; }
        }

        public double EliminatedMicroSecond
        {
            get { return (double)(_stopTime - _startTime) * 1000000 / _frequency; }
        }

    }
}
