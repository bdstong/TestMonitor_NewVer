using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using Modbus.Device;    //for modbus master
using System.IO.Ports;  //for serial port
using MBusRead;

namespace DVW
{
    //在命名空间中定义一个delegate委托：
    public delegate void SendDtatDelegate(byte[] buffer, int len);

    public delegate void UpateMsgDelegate(ushort[] buffer, byte id);//更新信息的委托

    public delegate void DispMessageDelegate(String str);

    public partial class Form1 : Form
    {
        private const string DATA_SUB_DIR = "data";

        List<PictureBox> listDO = new List<PictureBox>();

        public UInt16 ResetRegister = 100;
        public UInt16 ParameterRegister = 1000;
        public UInt16 MotorCmdRegister = 2000;
        public UInt16 PIDCmdRegister = 3000;
        public UInt16 KZRunCmdRegister = 4000;

        public UInt16 StopMotorRegister = 1000;

        public UInt16 BengMotor = 1;
        public UInt16 MadaMotor = 2;

        public UInt16 NormalFlag = 1;
        public UInt16 ReverseFlag = 0;

        public float max_voltage;

        //电压 通道
        public byte XiYouYaLi_Chn = 0;
        public byte XiTongYaLi_Chn = 1;
        public byte BengPLYaLi_Chn = 2;
        public byte MadaPLYali_Chn = 3;

        public byte MadaGaoYaKou_Chn = 4;
        public byte MadaDiYaKou_Chn = 5;

        public byte WenDu_Chn = 7;
        public byte RunTime_Index = 20;

        //频率通道
        public byte bengNiuJuPL_Chn = 8;
        public byte bengZhuanSu_Chn = 9;
        public byte madaNiuJuPL_Chn = 10;
        public byte madaZhuanSu_Chn = 11;

        public float maxDianliu = 20.0f;//满量程的电流值
        public double maxYali = 70;//满量程的压力
        public UInt16 adcValueOf4mA = 819;//4mA程数字量
        public double maxYali0 = 2.0;//满量程的压力 吸油压力

        public const UInt16 adc4mA = 819;//4mA程数字量
        public const UInt16 adc20mA = 4095;//20mA程数字量
        public const UInt16 adcFull = 4095;//ADC满量程数字量

        //1#
        private UInt16[] adcMin1 = { adc4mA, adc4mA, adc4mA, adc4mA, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//前8路模拟量，后8路是频率
        private UInt16[] adcMax1 = { adc20mA, adc20mA, adc20mA, adc20mA, adcFull, adcFull, adcFull, adcFull, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535 };//前4路 压力传感器
        private float[] valueMin1 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private float[] valueMax1 = { 2.0f, 70.0f, 70.0f, 70.0f, 3.3f, 3.3f, 3.3f, 3.3f, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535 };
        private string[] formatStr1 = new string[] { "f2", "f1", "f1", "f1", "f3", "f3", "f3", "f3", "f0", "f0", "f0", "f0", "f0", "f0", "f0", "f0" };
        private string[] adcChnnalName1 = { "泵 吸油", "系统压力", "泵 排量", "马达排量", "电压5", "电压6", "电压7", "电压8", "泵 扭矩", "泵 转速", "马达扭矩", "马达转速", "频率5", "频率6", "频率7", "频率8" };
        private UInt16[] adcMinOffset1 = { adc4mA, adc4mA, adc4mA, adc4mA, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//前8路模拟量，后8路是频率

        //2#
        private UInt16[] adcMin2 = { adc4mA, adc4mA, adc4mA, adc4mA, adc4mA, adc4mA, 0, adc4mA, 0, 0, 0, 0, 0, 0, 0, 0 };//前8路模拟量，后8路是频率
        private UInt16[] adcMax2 = { adc20mA, adc20mA, adc20mA, adc20mA, adc20mA, adc20mA, adcFull, adc20mA, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535 };//前4路 压力传感器
        private float[] valueMin2 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private float[] valueMax2 = { 2.0f, 70.0f, 70.0f, 70.0f, 70.0f, 2.0f, 3.3f, 200.0f, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535 };
        private string[] formatStr2 = new string[] { "f2", "f1", "f1", "f1", "f1", "f2", "f3", "f1", "f0", "f0", "f0", "f0", "f0", "f0", "f0", "f0" };
        private string[] adcChnnalName2 = { "泵 吸油", "系统压力", "泵 排量", "马达排量", "高压口", "低压口", "电压7", "油   温", "泵 扭矩", "泵 转速", "马达扭矩", "马达转速", "频率5", "频率6", "频率7", "频率8" };
        private UInt16[] adcMinOffset2 = { adc4mA, adc4mA, adc4mA, adc4mA, adc4mA, adc4mA, 0, adc4mA, 0, 0, 0, 0, 0, 0, 0, 0 };//前8路模拟量，后8路是频率

        //private double PLMaxVoltage = 2.6;//1.530;排量高
        //private double PLMinVoltage = 1.83;//1.078;排量低

        //const byte SAMPLE_DATA = 1;
        //const byte PARAMETER_DATA = 2;
        //const byte ACK_DATA = 3;

        //const byte I2C_WRIIE_OK = 0;
        //const byte I2C_WRITE_ERROR = 1;
        //const byte I2C_NO_ANSWER = 2;
        //const byte CAN_NO_ANSWER = 3;

        //const byte SUBSTATION_PARAMETER = 3;
        //const byte SUBSTATION_SAMPLE_DATA = 4;
        //const byte SUBSTATION_PARAMETER_WRITE = 5;
        //const byte SUBSTATION_KOUT_CMD = 6;
        //const byte SUBSTATION_VOUT_CMD = 7;

        //const byte SUBSTATION_DJ_CMD = 8;

        //const byte SUBSTATION_BENG_RUN_NOR = 11;
        //const byte SUBSTATION_BENG_RUN_REV = 12;
        //const byte SUBSTATION_BENG_PID_NOR = 13;
        //const byte SUBSTATION_BENG_PID_REV = 14;
        //const byte SUBSTATION_BENG_STOP = 15;

        //const byte SUBSTATION_MADA_RUN_NOR = 21;
        //const byte SUBSTATION_MADA_RUN_REV = 22;
        //const byte SUBSTATION_MADA_PID_NOR = 23;
        //const byte SUBSTATION_MADA_PID_REV = 24;
        //const byte SUBSTATION_MADA_STOP = 25;

        //const byte ALL_SUBSTATION_SAMPLE = 0xfe;
        //const byte ALL_SUBSTATION_RESET = 0xff;


        const byte M1_NORMAL_CMD_CHN = 0;   //#define M1_NORMAL_CMD_CHN		0	//KOUT1
        const byte M1_RESERVE_CMD_CHN = 1;  //#define M1_RESERVE_CMD_CHN	1	
        const byte M2_NORMAL_CMD_CHN = 2;   //#define M2_NORMAL_CMD_CHN		2	//KOUT3
        const byte M2_RESERVE_CMD_CHN = 3;  //#define M2_RESERVE_CMD_CHN	3	
        const byte M1_RESET_CMD_CHN = 4;    //#define M1_RESET_CMD_CHN		4	//KOUT5
        const byte M2_RESET_CMD_CHN = 5;    //#define M2_RESET_CMD_CHN		5	

        public const byte XLF_CMD_CHN = 8;  //#define XLF_CMD_CHN			8	//KOUT9
        public const byte JSF_CMD_CHN = 9;  //#define JSF_CMD_CHN			9	//KOUT10

        public const byte YOUMEN_DA_CHN = 6;  //			//KOUT7
        public const byte YOUMEN_XIAO_CHN = 7;  //			//KOUT8


        DataFIFO dataFIFO;
        MyTcpClient tcpClient;
        LinkModeType LinkMode;

        private SerialPort serialPort = new SerialPort();
        public ModbusSerialMaster master;

        //      serialPort.NewLine = "\n";// \n 换行 

        string ip = string.Empty;
        string port = string.Empty;

        private bool Auto_Send_Flag;

        private bool Send_Cmd_Flag = false;

        //private bool Send_Parameters_Flag = false;

        //Thread thread_run;
        Thread NMbus_thread;

        //private static AutoResetEvent event_1 = new AutoResetEvent(false);//true);

        private static EventWaitHandle event_1 = new EventWaitHandle(false, EventResetMode.ManualReset);

        //private System.Windows.Forms.Timer timer1;
        private string INIfilepath;//INI配置文件完整路径
        private UInt16[] RecivedDataBuf = new UInt16[512];
        byte[] cmd_bytes = new byte[512];
        int cmd_Send_Length = 0;

        private UInt16[] Parameters = new UInt16[140];//参数140个
        private CurveForm CuvForm;
        private CurveForm CuvForm1;
        private Parameter ParaForm;

        private UInt16[] m_voltage = new UInt16[16];

        private UInt16 m_SWStatus, m_kin, m_kout;
        private UInt16 m_alarm, m_runtime;
        private UInt16 m_dacout, m_temperature, m_temperatureError;

        private UInt16 mTest1, mTest2, mTest3, mTest4, mTest5;
        private UInt16 mTest6, mTest7, mTest8, mTest9, mTest10, mTest11, mTest12;
        private UInt16 bengTest, madaTest;

        //        private UInt16 StationAddr;

        private void AccessAppSettings()
        {
            //获取Configuration对象
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //根据Key读取<add>元素的Value
            string name = config.AppSettings.Settings["name"].Value;
            //写入<add>元素的Value
            config.AppSettings.Settings["name"].Value = "fx163";
            //增加<add>元素
            config.AppSettings.Settings.Add("url", "http://www.fx163.net");
            //删除<add>元素
            config.AppSettings.Settings.Remove("name");
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }
        public static string GetAppConfig(string strKey)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }
        public static void UpdateAppConfig(string newKey, string newValue)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            bool exist = false;
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == newKey)
                {
                    exist = true;
                }
            }
            if (exist)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            config.AppSettings.Settings.Add(newKey, newValue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        //声明委托
        public event UpateMsgDelegate UpdataEventDele;

        public SendDtatDelegate SendDel;
        public void SendDatadelegate(byte[] buffer, int len)
        {
            while (Send_Cmd_Flag) Thread.Sleep(1);
            cmd_Send_Length = CopyDataToBuf(cmd_bytes, buffer, len);
            Send_Cmd_Flag = true;
        }

        public DispMessageDelegate DispDele;
        public void DispMessInFrame(String mess)
        {
            labelMbusError.Text = mess;
        }

        #region 声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #endregion

        #region INI文件读写函数 IniWriteValue & IniReadValue
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.INIfilepath);
        }
        public string IniReadValue(string Section, string Key, string Def)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, Def, temp, 255, this.INIfilepath);
            return temp.ToString();
        }
        #endregion

        private enum LinkModeType
        {
            NO_LINK,
            NET_LINK,
            SER_LINK,
        };

        public bool portIsOpen()
        {
            return serialPort.IsOpen;
        }

        private byte GetSubStationNum()
        {
            byte rchk = 0;
            if (radioButton0.Checked == true) rchk = 0;
            if (radioButton1.Checked == true) rchk = 1;
            if (radioButton2.Checked == true) rchk = 2;
            if (radioButton3.Checked == true) rchk = 3;
            if (radioButton4.Checked == true) rchk = 4;
            if (radioButton5.Checked == true) rchk = 5;
            if (radioButton6.Checked == true) rchk = 6;
            if (radioButton7.Checked == true) rchk = 7;
            if (radioButton8.Checked == true) rchk = 8;
            if (radioButton9.Checked == true) rchk = 9;
            if (radioButton10.Checked == true) rchk = 10;
            if (radioButton11.Checked == true) rchk = 11;
            if (radioButton12.Checked == true) rchk = 12;
            if (radioButton13.Checked == true) rchk = 13;
            if (radioButton14.Checked == true) rchk = 14;
            if (radioButton15.Checked == true) rchk = 15;
            rchk++;
            return rchk;
        }
        private void fill_commmlist()
        {
            int x;
            char[] pName = new char[10];
            string ss;

            COMMPortsList.Items.Clear();

            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            if (ports.Length > 0)
            {
                foreach (string str in System.IO.Ports.SerialPort.GetPortNames())
                {
                    str.CopyTo(0, pName, 0, str.Length);
                    ss = str;
                    for (x = str.Length - 1; x > 3; x--)
                    {
                        if (str[x] > 0x80) ss = str.Remove(x);
                    }
                    COMMPortsList.Items.Add(ss);
                }
                COMMPortsList.Sorted = true;
                COMMPortsList.SelectedIndex = 0;
            }
            else
            {
                //MessageBox.Show("没有发现串口！");
                ack_label.Text = "没有发现串口！";
            }
        }

        public Form1()
        {
            InitializeComponent();
            LinkMode = LinkModeType.NO_LINK;

        }
        /*
         在多线程程序中，新创建的线程不能访问UI线程创建的窗口控件，
         如果需要访问窗口中的控件，可以在窗口构造函数中将CheckForIllegalCrossThreadCalls设置为 false
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
 
        也可以针对某一控件进行设置，例如：
            TextBox.CheckForIllegalCrossThreadCalls = false;

 
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            SendDel = new SendDtatDelegate(SendDatadelegate);//实例化委托

            CuvForm = new CurveForm(this, 1, adcMin1, adcMax1, valueMin1, valueMax1, formatStr1, adcMinOffset1, adcChnnalName1);
            CuvForm1 = new CurveForm(this, 2, adcMin2, adcMax2, valueMin2, valueMax2, formatStr2, adcMinOffset2, adcChnnalName2);
            ParaForm = new Parameter(this);

            UpdataEventDele += this.UpdateEvent;//先添加自己的
            UpdataEventDele += CuvForm.UpdateEvent;// 添加子窗口的
            UpdataEventDele += CuvForm1.UpdateEvent;

            DispDele = new DispMessageDelegate(DispMessInFrame);

            dataFIFO = new DataFIFO(1024);

            read_setting();

            byte id1 = Byte.Parse(DJAddr.Text);
            CuvForm.ID = id1;
            byte id2 = Byte.Parse(DJAddr1.Text);
            CuvForm1.ID = id2;



            radioButton0.Checked = true;
            btnConnServer.Enabled = true;
            btnDisConn.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = false;

            //timer1.Start();
            //timer2.Start();

            //thread_run = new Thread(new ThreadStart(ReciveDataTask));
            //thread_run.Start();

            NMbus_thread = new Thread(new ThreadStart(Thread_Mbus_Read));
            NMbus_thread.Start();

            MyTcpClient.pushSockets = new PushSockets(Rec);

            tcpClient = new MyTcpClient();
            fill_commmlist();

            listDO.Add(DO0);
            listDO.Add(DO1);
            listDO.Add(DO2);
            listDO.Add(DO3);
            listDO.Add(DO4);
            listDO.Add(DO5);
            listDO.Add(DO6);
            listDO.Add(DO7);
            listDO.Add(DO8);
            listDO.Add(DO9);

        }

        /// <summary>  
        /// 处理推送过来的消息  
        /// </summary>  
        /// <param name="rec"></param>  
        private void Rec(Sockets sks)
        {
            this.Invoke(new ThreadStart(delegate
            {
                if (sks.ex != null)
                {
                    //if (sks.ClientDispose == true)
                    //{
                    //    //由于未知原因引发异常.导致客户端下线.   比如网络故障.或服务器断开连接.  
                    //    //listBoxStates.Items.Add(string.Format("客户端下线.!"));
                    //    ad_cnt.ForeColor = Color.Red;
                    //    ad_cnt.Text = "客户端下线!";
                    //}
                    //listBoxStates.Items.Add(string.Format("异常消息：{0}", sks.ex));
                    ack_label.ForeColor = Color.Red;
                    //ack_label.Text = string.Format("异常消息：{0}", sks.ex);
                    ack_label.Text = string.Format("网络异常");
                    Auto_Send_Flag = false;
                }
                else if (sks.Offset == 0)
                {
                    //客户端主动下线  
                    //listBoxStates.Items.Add(string.Format("客户端下线.!"));
                    ack_label.ForeColor = Color.Red;
                    ack_label.Text = string.Format("服务器主动下线");
                    Auto_Send_Flag = false;

                    btnConnServer.Enabled = true;
                    btnDisConn.Enabled = false;
                    btnStart.Enabled = false;
                    btnStop.Enabled = false;
                    LinkMode = LinkModeType.NO_LINK;
                }
                else
                {
                    //Array.Copy(sks.RecBuffer, RecBuffer, sks.Offset);
                    byte[] buffer = new byte[sks.Offset];
                    Array.Copy(sks.RecBuffer, buffer, sks.Offset);
                    dataFIFO.Write(buffer, 0, sks.Offset);
                    //string str = Encoding.UTF8.GetString(buffer);
                    //listBoxText.Items.Add(string.Format("服务端{0}发来消息：{1}", sks.Ip, str));
                    //wdg.Text = str;
                    //tcpClient.SendData(RecBuffer, 0, sks.Offset);//
                    //this.Invoke(UpDateUI, rbuf[0] + rbuf[1] * 256);//BeginInvoke
                    //for (int i = 0; i < sks.Offset / 2; i++)
                    //{
                    //    RecivedDataBuf[i] = (UInt16)(buffer[4+2 * i] + buffer[4+2 * i + 1] * 256);

                    //}
                    ////UpdateEvent(RecivedDataBuf);
                }
            }));
        }
        private void ReciveDataTask()
        {
            byte[] rbuf = new byte[500];
            int num, i;

            for (; ; )
            {
                dataFIFO.Read(rbuf, 0, 1);
                if (rbuf[0] != 0xa5) continue;

                dataFIFO.Read(rbuf, 0, 1);
                if (rbuf[0] != 0x5a) continue;

                dataFIFO.Read(rbuf, 0, 2);
                num = rbuf[0] + rbuf[1] * 256;//后面的是 数据长度n+n个数据+数据和

                dataFIFO.Read(rbuf, 0, num + 2);

                UInt16 sum = 0;
                for (i = 0; i < num; i++)
                {
                    sum += rbuf[i];
                }

                if (sum != (UInt16)(rbuf[num] + rbuf[num + 1] * 256))
                {
                    //this.Invoke(SetText, (int)-5);
                    continue;
                }

                //一帧数据接收完成校验正确
                for (i = 0; i < num / 2; i++)
                {
                    RecivedDataBuf[i] = (UInt16)(rbuf[2 * i] + rbuf[2 * i + 1] * 256);
                }

                //this.Invoke(UpDateUI, RecivedDataBuf);

                this.Invoke(UpdataEventDele, RecivedDataBuf);

            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CuvForm.saveCurvData();
            CuvForm1.saveCurvData();
            //event_1.Reset();
            //Thread.Sleep(100);
            //if (thread_run != null) thread_run.Abort();
            if (NMbus_thread != null) NMbus_thread.Abort();
            save_setting();
        }

        private void UpdateEvent(ushort[] buffer, byte StationID)
        {
            try
            {
                byte testID1 = byte.Parse(DJAddr.Text);
                byte testID2 = byte.Parse(DJAddr1.Text);
                byte ZhuansuID = byte.Parse(YoumenAddr.Text);
                byte monitorID = (byte)GetSubStationNum();

                UpdateData(buffer, 0);

                if (StationID == monitorID)
                {
                    UpdateMonitor();
                }
                if (StationID == testID1)
                {
                    UpdateTest1();
                    //label45.Text = ((float)m_voltage[BengPaiLiang_Chn] * max_voltage / adcFull).ToString("f03");
                    ////get_y(Int16 y_start, Int16 y_end, Int16 x_start, Int16 x_end, Int16 x) //y=a*x+b 2011-5-27
                    //Int16 max = (Int16)(PLMaxVoltage / max_voltage * adcFull);
                    //Int16 min = (Int16)(PLMinVoltage / max_voltage * adcFull);
                    //int hper = get_y(0, 100, min, max, (Int16)m_voltage[BengPaiLiang_Chn]);

                    //String ss = " " + hper.ToString() + "%";
                    //label45.Text += ss.ToString();
                }
                if (StationID == testID2)
                {
                    UpdateTest2();
                }
                if (StationID == ZhuansuID)
                {
                    FDJZhuansu.Text = m_voltage[bengZhuanSu_Chn].ToString();
                }

            }
            catch (Exception)
            {
                
                //throw;
            }
        }

        private void read_setting()
        {
            //string s;
            //s=System.Windows.Forms.Application.ExecutablePath;
            //s = System.Environment.CurrentDirectory;
            //s = System.IO.Directory.GetCurrentDirectory();
            //s = System.Windows.Forms.Application.StartupPath;
            //s = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //MessageBox.Show(s);
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf('\\'));
            string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            INIfilepath = path + "\\" + name + ".INI";
            //INIfilepath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Setting.ini";
            //            checkedListBox1.SelectionMode = SelectionMode.MultiExtended;
            //string s = IniReadValue("Setting", "RcItemNums", "1");
            //int n = Int32.Parse(s);
            /*
                        foreach (CheckBox ckb in panel1.Controls)
                        {
                            string nm = IniReadValue("RcItemCfg", ckb.Name+"_Name", "未配置的单元");
                            ckb.Text = nm;
                            ckb.ContextMenuStrip = contextMenu_Edit_OP;
                            ckb.MouseDown += new MouseEventHandler(ckb_MouseDown);
                        }
                        */
            txtServerIP.Text = IniReadValue("Setting", "ServerIP", "192.168.4.1");
            txtServerPort.Text = IniReadValue("Setting", "ServerPort", "8899");

            string PortName = IniReadValue("Setting", "ComPort", "COM1");
            if (PortName.Length == 0) PortName = "COM1";
            serialPort.PortName = PortName;

            serialPort.BaudRate = Int32.Parse(IniReadValue("Setting", "Baud", "115200"));
            serialPort.Parity = System.IO.Ports.Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = System.IO.Ports.StopBits.One;

            max_voltage = float.Parse(IniReadValue("Setting", "MAX_DY", "3.3"));//最大量程电压 毫伏
            //fullAdcValue = UInt16.Parse(IniReadValue("Setting", "MAX_FW", "4095"));//最大量程数字量


            DJAddr.Text = IniReadValue("DJAdj", "DJAddr", "1");
            M1ZFtime.Text = IniReadValue("DJAdj", "M1ZFtime", "100");
            M2ZFtime.Text = IniReadValue("DJAdj", "M2ZFtime", "100");
            M1FWtime.Text = IniReadValue("DJAdj", "M1FWtime", "100");
            M2FWtime.Text = IniReadValue("DJAdj", "M2FWtime", "100");

            DJAddr1.Text = IniReadValue("DJAdj1", "DJAddr", "2");
            M1ZFtime1.Text = IniReadValue("DJAdj1", "M1ZFtime", "100");
            M2ZFtime1.Text = IniReadValue("DJAdj1", "M2ZFtime", "100");
            M1FWtime1.Text = IniReadValue("DJAdj1", "M1FWtime", "100");
            M2FWtime1.Text = IniReadValue("DJAdj1", "M2FWtime", "100");

            YoumenAddr.Text = IniReadValue("Setting", "YoumenAddr", "1");
            YoumenTime.Text = IniReadValue("Setting", "YoumenTime", "20");


            textBoxTsensormin.Text = IniReadValue("Setting", "textBoxTsensormin", "3.85");
            textBoxTsensormax.Text = IniReadValue("Setting", "textBoxTsensormax", "19.85");


            read_frm("CvForm", CuvForm, path);
            read_frm("CvForm1", CuvForm1, path);
            readKzSet0("CtrolSet0");
            //readKzSet("CtrolSet");
            readKzSet1("CtrolSet1");
            readKzSet2("CtrolSet2");
            read_niujuset("NiujuSet");
        }
        private void save_setting()
        {
            IniWriteValue("Setting", "ServerIP", txtServerIP.Text);
            IniWriteValue("Setting", "ServerPort", txtServerPort.Text);

            IniWriteValue("Setting", "ComPort", serialPort.PortName);
            IniWriteValue("Setting", "Open", serialPort.IsOpen.ToString());


            IniWriteValue("Setting", "YoumenAddr", YoumenAddr.Text);
            IniWriteValue("Setting", "YoumenTime", YoumenTime.Text);

            IniWriteValue("DJAdj", "DJAddr", DJAddr.Text);
            IniWriteValue("DJAdj", "M1ZFtime", M1ZFtime.Text);
            IniWriteValue("DJAdj", "M2ZFtime", M2ZFtime.Text);
            IniWriteValue("DJAdj", "M1FWtime", M1FWtime.Text);
            IniWriteValue("DJAdj", "M2FWtime", M2FWtime.Text);

            IniWriteValue("DJAdj1", "DJAddr", DJAddr1.Text);
            IniWriteValue("DJAdj1", "M1ZFtime", M1ZFtime1.Text);
            IniWriteValue("DJAdj1", "M2ZFtime", M2ZFtime1.Text);
            IniWriteValue("DJAdj1", "M1FWtime", M1FWtime1.Text);
            IniWriteValue("DJAdj1", "M2FWtime", M2FWtime1.Text);

            IniWriteValue("Setting", "textBoxTsensormin", textBoxTsensormin.Text);
            IniWriteValue("Setting", "textBoxTsensormax", textBoxTsensormax.Text);

            save_cfm("CvForm", CuvForm);
            save_cfm("CvForm1", CuvForm1);
            saveKzSet0("CtrolSet0");
            //saveKzSet("CtrolSet");
            saveKzSet1("CtrolSet1");
            saveKzSet2("CtrolSet2");
            save_niujuset("NiujuSet");

        }

        private void read_frm(String sec, CurveForm CForm, String path)
        {

            CForm.Combox1_Select = UInt16.Parse(IniReadValue(sec, "COMBOX1_SEL", "0"));
            CForm.Combox2_Select = UInt16.Parse(IniReadValue(sec, "COMBOX2_SEL", "1"));
            CForm.Combox3_Select = UInt16.Parse(IniReadValue(sec, "COMBOX3_SEL", "2"));
            CForm.Combox4_Select = UInt16.Parse(IniReadValue(sec, "COMBOX4_SEL", "3"));
            CForm.crvpath = IniReadValue(sec, "Path", path + "\\" + DATA_SUB_DIR);

            String s = IniReadValue(sec, "AutoSave", "True");
            if (s == "True")
            {
                CForm.chkAutoSave.Checked = true;
            }
            else
                CForm.chkAutoSave.Checked = false;

            DirectoryInfo TheFolder = new DirectoryInfo(CForm.crvpath);
            if (TheFolder.Exists == false)
            {
                Directory.CreateDirectory(CForm.crvpath);
             }

            CForm.textBengNiuJuPL0.Text = IniReadValue(sec, "BengNiuJuPL0", "10000");
            CForm.textMadaNiuJuPL0.Text = IniReadValue(sec, "MadaNiuJuPL0", "10000");
            CForm.textBengMaxNiuju.Text = IniReadValue(sec, "BengMaxNiuju", "3000");
            CForm.textMadaMaxNiuju.Text = IniReadValue(sec, "MadaMaxNiuju", "3000");
        }
        private void save_cfm(String sec, CurveForm CForm)
        {
            IniWriteValue(sec, "COMBOX1_SEL", CForm.Combox1_Select.ToString());
            IniWriteValue(sec, "COMBOX2_SEL", CForm.Combox2_Select.ToString());
            IniWriteValue(sec, "COMBOX3_SEL", CForm.Combox3_Select.ToString());
            IniWriteValue(sec, "COMBOX4_SEL", CForm.Combox4_Select.ToString());

            IniWriteValue(sec, "Path", CForm.crvpath);

            IniWriteValue(sec, "AutoSave", CForm.chkAutoSave.Checked.ToString());

            IniWriteValue(sec, "BengNiuJuPL0", CForm.textBengNiuJuPL0.Text);
            IniWriteValue(sec, "MadaNiuJuPL0", CForm.textMadaNiuJuPL0.Text);
            IniWriteValue(sec, "BengMaxNiuju", CForm.textBengMaxNiuju.Text);
            IniWriteValue(sec, "MadaMaxNiuju", CForm.textMadaMaxNiuju.Text);

        }

        private void readKzSet0(String sec)//1#test
        {
            textPid_P0.Text = IniReadValue(sec, "Pid_P", "2.45");
            textPid_I0.Text = IniReadValue(sec, "Pid_I", "3.50");
            textPid_D0.Text = IniReadValue(sec, "Pid_D", "1.25");
            TextPeriod0.Text = IniReadValue(sec, "Period", "10");
            textPid_K0.Text = IniReadValue(sec, "PidKp", "15");
            textTarget0.Value =Decimal.Parse( IniReadValue(sec, "Target", "10"));
        }
        private void saveKzSet0(String sec)//1#test
        {
            IniWriteValue(sec, "Pid_P", textPid_P0.Text);
            IniWriteValue(sec, "Pid_I", textPid_I0.Text);
            IniWriteValue(sec, "Pid_D", textPid_D0.Text);
            IniWriteValue(sec, "Period", TextPeriod0.Text);
            IniWriteValue(sec, "PidKp", textPid_K0.Text);
            IniWriteValue(sec, "Target", textTarget0.Value.ToString());
        }

        /*      
              private void readKzSet(String sec)//1#泵
              {
                  textPid_P.Text = IniReadValue(sec, "Pid_P", "2.45");
                  textPid_I.Text = IniReadValue(sec, "Pid_I", "3.50");
                  textPid_D.Text = IniReadValue(sec, "Pid_D", "1.25");
                  TextPeriod.Text=IniReadValue(sec, "Period", "10");
                  textPid_K.Text=IniReadValue(sec, "PidKp", "15");
                  textTarget.Text = IniReadValue(sec, "Target", "10");

               }
              private void saveKzSet(String sec)//1#泵
              {
                  IniWriteValue(sec, "Pid_P", textPid_P.Text);
                  IniWriteValue(sec, "Pid_I", textPid_I.Text);
                  IniWriteValue(sec, "Pid_D", textPid_D.Text);
                  IniWriteValue(sec, "Period", TextPeriod.Text);
                  IniWriteValue(sec, "PidKp", textPid_K.Text);
                  IniWriteValue(sec, "Target", textTarget.Text);
 
               }
      */
        private void readKzSet1(String sec)//2#泵
        {
            textPid_P1.Text = IniReadValue(sec, "Pid_P", "2.45");
            textPid_I1.Text = IniReadValue(sec, "Pid_I", "3.50");
            textPid_D1.Text = IniReadValue(sec, "Pid_D", "1.25");
            TextPeriod1.Text = IniReadValue(sec, "Period", "30");
            textPid_K1.Text = IniReadValue(sec, "PidKp", "20");
            //textTarget1.Text = IniReadValue(sec, "Target", "5.0");
            textTarget1.Value = Decimal.Parse(IniReadValue(sec, "Target", "10"));

        }
        private void saveKzSet1(String sec)//2#泵
        {
            IniWriteValue(sec, "Pid_P", textPid_P1.Text);
            IniWriteValue(sec, "Pid_I", textPid_I1.Text);
            IniWriteValue(sec, "Pid_D", textPid_D1.Text);
            IniWriteValue(sec, "Period", TextPeriod1.Text);
            IniWriteValue(sec, "PidKp", textPid_K1.Text);
            //IniWriteValue(sec, "Target", textTarget1.Text);
            IniWriteValue(sec, "Target", textTarget1.Value.ToString());

        }

        private void readKzSet2(String sec)//2#马达
        {
            textTarget2.Text = IniReadValue(sec, "Target", "5.0");
            TestPeriod2.Text = IniReadValue(sec, "Period", "10");
            textNormalDelay2.Text = IniReadValue(sec, "NormalDelay", "10");
            textReverseDelay2.Text = IniReadValue(sec, "ReverseDelay", "10");
            textRange2.Text = IniReadValue(sec, "Range", "2");
        }
        private void saveKzSet2(String sec)//2#马达
        {
            IniWriteValue(sec, "Target", textTarget2.Text);
            IniWriteValue(sec, "Period", TestPeriod2.Text);
            IniWriteValue(sec, "NormalDelay", textNormalDelay2.Text);
            IniWriteValue(sec, "ReverseDelay", textReverseDelay2.Text);
            IniWriteValue(sec, "Range", textRange2.Text);
        }


        private void read_niujuset(String sec)
        {
            textBengNiuJuPL0.Text = IniReadValue(sec, "Beng1NiuJuPL0", "10000");
            textMadaNiuJuPL0.Text = IniReadValue(sec, "Mada1NiuJuPL0", "10000");

            textBengMaxNiuju.Text = IniReadValue(sec, "Beng1MaxNiuju", "3000");
            textMadaMaxNiuju.Text = IniReadValue(sec, "Mada1MaxNiuju", "3000");

            textBeng1NiuJuPL0.Text = IniReadValue(sec, "Beng2NiuJuPL0", "10000");
            textBeng1MaxNiuju.Text = IniReadValue(sec, "Beng2MaxNiuju", "3000");

            textMada1NiuJuPL0.Text = IniReadValue(sec, "Mada2NiuJuPL0", "10000");
            textMada1MaxNiuju.Text = IniReadValue(sec, "Mada2MaxNiuju", "3000");
        }
        private void save_niujuset(String sec)
        {
            IniWriteValue(sec, "Beng1NiuJuPL0", textBengNiuJuPL0.Text);
            IniWriteValue(sec, "Mada1NiuJuPL0", textMadaNiuJuPL0.Text);

            IniWriteValue(sec, "Beng1MaxNiuju", textBengMaxNiuju.Text);
            IniWriteValue(sec, "Mada1MaxNiuju", textMadaMaxNiuju.Text);

            IniWriteValue(sec, "Beng2NiuJuPL0", textBeng1NiuJuPL0.Text);
            IniWriteValue(sec, "Beng2MaxNiuju", textBeng1MaxNiuju.Text);

            IniWriteValue(sec, "Mada2NiuJuPL0", textMada1NiuJuPL0.Text);
            IniWriteValue(sec, "Mada2MaxNiuju", textMada1MaxNiuju.Text);
        }

        private void Thread_Mbus_Read()
        {
            int period = 200;//ms
            var pt = new HiperTimer();
            pt.Start();
            for (; ; )
            {
                Thread.Sleep(0);
                if (pt.getUs() < 1000 * period) continue;
                pt.Start();

                event_1.WaitOne();
                try
                {
                    byte slaveID = Byte.Parse(DJAddr.Text.Trim());
                    ushort startAddress = 1000;
                    ushort numofPoints = 36;
                    ushort[] register;
                    if (checkBox1.Checked)
                    {
                        //read AI(3xxxx)                        
                        register = master.ReadInputRegisters(slaveID, startAddress, numofPoints);
                        this.Invoke(UpdataEventDele, register, slaveID);
                    }

                    if (checkBox2.Checked)
                    {
                        //read AI(3xxxx)  
                        slaveID = Byte.Parse(DJAddr1.Text.Trim());
                        register = master.ReadInputRegisters(slaveID, startAddress, numofPoints);
                        this.Invoke(UpdataEventDele, register, slaveID);
                    }

                }
                catch (Exception exception)
                {
                    //labelMbusError.Text = exception.Message;
                    //this.Invoke(DispDele, exception.Message);
                }

            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen) return;
            try
            {
                byte slaveID = Byte.Parse(DJAddr.Text.Trim());
                ushort startAddress = 1000;
                ushort numofPoints = 36;
                ushort[] register;
                if (checkBox1.Checked)
                {
                    //read AI(3xxxx)                        
                    register = master.ReadInputRegisters(slaveID, startAddress, numofPoints);
                    this.Invoke(UpdataEventDele, register, slaveID);
                }

                if (checkBox2.Checked)
                {
                    //read AI(3xxxx)  
                    slaveID = Byte.Parse(DJAddr1.Text.Trim());
                    register = master.ReadInputRegisters(slaveID, startAddress, numofPoints);
                    this.Invoke(UpdataEventDele, register, slaveID);
                }

            }
            catch (Exception exception)
            {
                labelMbusError.Text = exception.Message;
            }
        }

        //void MySendDataBuf(byte[] buffer, int offset, int size)
        //{
        //    if(LinkMode==LinkModeType.NET_LINK) tcpClient.SendData(buffer, offset, size);
        //    if (LinkMode == LinkModeType.SER_LINK)
        //    {
        //        try
        //        {
        //            serialPort.Write(buffer, offset, size);
        //        }
        //        catch (System.IO.IOException)
        //        //catch (UnauthorizedAccessException)
        //        {

        //            //OpenCOM.Enabled = true;
        //            //CloseCOM.Enabled = false;
        //            //btnStop.Enabled = false;
        //            //btnStart.Enabled = false;
        //            ////ad_cnt.Text = "串口错误";
        //            //COMMPortsList.Enabled = true;
        //            //fill_commmlist();
        //            //for (i = 0; i < COMMPortsList.Items.Count; i++)
        //            //{
        //            //    if ((string)COMMPortsList.Items[i] == serialPort.PortName)
        //            //        COMMPortsList.SelectedIndex = i;
        //            //}
        //        }

        //    }
        //}
        //private void timer1_Tick1(object sender, EventArgs e)
        //{
        //    //this.Invoke(UpdataEventDele, RecivedDataBuf);
        //    /*
        //    byte[] tmbytes = new byte[32];
        //    //int Send_Length = RemoteOneCmdToBuf(tmbytes, SUBSTATION_SAMPLE_DATA);
        //    int Send_Length = AskAllSample(tmbytes);

        //    //if (Send_Parameters_Flag)
        //    //{
        //    //    int fgp = 16;
        //    //    int i;
        //    //    for (i = 0; i < cmd_Send_Length / fgp; i++)
        //    //    {

        //    //        MySendDataBuf(cmd_bytes, i * fgp, fgp);
        //    //        Thread.Sleep(fgp * 2);

        //    //    }
        //    //    Thread.Sleep(fgp * 2);
        //    //    MySendDataBuf(cmd_bytes, i * fgp, cmd_Send_Length % fgp);
        //    //    Send_Parameters_Flag=false;
        //    //    Thread.Sleep(fgp * 2);
        //    //}
        //    if (Send_Cmd_Flag==true)
        //    {
        //        MySendDataBuf(cmd_bytes, 0, cmd_Send_Length);
        //        Send_Cmd_Flag = false;
        //        Thread.Sleep(cmd_Send_Length*2);
        //        return;
        //    }

        //    if (!Auto_Send_Flag) return;
        //    MySendDataBuf(tmbytes, 0, Send_Length);


        //    //String str = "1234567890test";
        //    //byte[] buffer = Encoding.UTF8.GetBytes(str);
        //    //tcpClient.SendData(buffer, 0, str.Length);//
        //    if (noanswercnt < 5) noanswercnt++;
        //    else
        //    {
        //        ack_label.Text = "无应答 ";
        //        Clear_Info();
        //    }
        //    */
        //}

        private void OpenCOM_Click(object sender, EventArgs e)//打开网络
        {

            try
            {
                this.ip = txtServerIP.Text.Trim();
                this.port = txtServerPort.Text.Trim();

                tcpClient.InitSocket(ip, int.Parse(port));
                tcpClient.Start();
                //listBoxStates.Items.Add("连接成功!");
                ack_label.ForeColor = Color.Black;
                ack_label.Text = "连接成功!";
                btnConnServer.Enabled = false;
                btnDisConn.Enabled = true;
                btnStart.Enabled = true;
                txtServerIP.Enabled = false;
                LinkMode = LinkModeType.NET_LINK;
                OpenCOM.Enabled = false;
                CloseCOM.Enabled = false;
                COMMPortsList.Enabled = false;
            }
            catch (Exception ex)
            {
                //listBoxStates.Items.Add(string.Format("连接失败!原因：{0}", ex.Message));
                //ack_label.Text = string.Format("连接失败!原因：{0}", ex.Message);
                ack_label.ForeColor = Color.Red;
                ack_label.Text = "连接失败!";// ex.Message;
                btnConnServer.Enabled = true;
                LinkMode = LinkModeType.NO_LINK;
            }
        }

        private void CloseCOM_Click(object sender, EventArgs e)//关闭网络
        {
            tcpClient.Stop();
            btnConnServer.Enabled = true;

            ack_label.ForeColor = Color.Black;
            ack_label.Text = "断开连接";

            Auto_Send_Flag = false;
            btnDisConn.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            txtServerIP.Enabled = true;
            LinkMode = LinkModeType.NO_LINK;
            OpenCOM.Enabled = true;
            COMMPortsList.Enabled = true;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            Auto_Send_Flag = true;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            //timer1.Enabled = true;
            event_1.Set();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            Auto_Send_Flag = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            ack_label.ForeColor = Color.Black;
            ack_label.Text = "停 止";
            //timer1.Enabled = false;
            event_1.Reset();
        }

        private int bit(int x, int n)
        {
            if ((x & (1 << n)) == 0)
                return 0;
            else return 1;
        }
        private void Clear_Info()
        {
            labelRunTime.Text = "-";

            Labelvoltage1.Text = "-----";
            Labelvoltage2.Text = "-----";
            Labelvoltage3.Text = "-----";
            Labelvoltage4.Text = "-----";
            Labelvoltage5.Text = "-----";
            Labelvoltage6.Text = "-----";
            Labelvoltage7.Text = "-----";
            Labelvoltage8.Text = "-----";

            labelCurrent1.Text = "-----";
            labelCurrent2.Text = "-----";
            labelCurrent3.Text = "-----";
            labelCurrent4.Text = "-----";
            labelCurrent5.Text = "-----";
            labelCurrent6.Text = "-----";
            labelCurrent7.Text = "-----";
            labelCurrent8.Text = "-----";

            labelfreq1.Text = "-----";
            labelfreq2.Text = "-----";
            labelfreq3.Text = "-----";
            labelfreq4.Text = "-----";
            labelfreq5.Text = "-----";
            labelfreq6.Text = "-----";
            labelfreq7.Text = "-----";
            labelfreq8.Text = "-----";

            labelkin1.Text = "-";
            labelkin2.Text = "-";
            labelkin3.Text = "-";
            labelkin4.Text = "-";
            labelkin5.Text = "-";
            labelkin6.Text = "-";
            labelkin7.Text = "-";
            labelkin8.Text = "-";

            labelkout1.Text = "---";
            labelkout2.Text = "---";
            labelkout3.Text = "---";
            labelkout4.Text = "---";
            labelkout5.Text = "---";
            labelkout6.Text = "---";
            labelkout7.Text = "---";
            labelkout8.Text = "---";

            labelkout9.Text = "---";
            labelkout10.Text = "---";
        }

        private void UpdateData(ushort[] DataBuffer, int offset)
        {
            int i = offset;

            for (int k = 0; k < 16; k++)
            {
                m_voltage[k] = DataBuffer[i++];
            }

            m_SWStatus = (ushort)(DataBuffer[i++] - 1);
            m_kin = DataBuffer[i++];
            m_kout = DataBuffer[i++];
            m_alarm = DataBuffer[i++];
            m_runtime = DataBuffer[i++];
            m_dacout = DataBuffer[i++];
            m_temperature = DataBuffer[i++];
            m_temperatureError = DataBuffer[i++];

            mTest1 = DataBuffer[i++];
            mTest2 = DataBuffer[i++];
            mTest3 = DataBuffer[i++];
            mTest4 = DataBuffer[i++];
            mTest5 = DataBuffer[i++];
            mTest6 = DataBuffer[i++];
            mTest7 = DataBuffer[i++];
            mTest8 = DataBuffer[i++];
            mTest9 = DataBuffer[i++];
            mTest10 = DataBuffer[i++];
            mTest11 = DataBuffer[i++];
            mTest12 = DataBuffer[i++];

            bengTest = mTest7;
            madaTest = mTest8;
        }

        private void updatePara1()
        {
            try
            {
                adcMinOffset1[bengNiuJuPL_Chn] = UInt16.Parse(textBengNiuJuPL0.Text);
                adcMin1[bengNiuJuPL_Chn] = 10000;
                adcMax1[bengNiuJuPL_Chn] = 15000;
                valueMin1[bengNiuJuPL_Chn] = 0;
                valueMax1[bengNiuJuPL_Chn] = UInt16.Parse(textBengMaxNiuju.Text);

                adcMinOffset1[madaNiuJuPL_Chn] = UInt16.Parse(textMadaNiuJuPL0.Text);
                adcMin1[madaNiuJuPL_Chn] = 10000;
                adcMax1[madaNiuJuPL_Chn] = 15000;
                valueMin1[madaNiuJuPL_Chn] = 0;
                valueMax1[madaNiuJuPL_Chn] = UInt16.Parse(textMadaMaxNiuju.Text);

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updatePara2()
        {
            try
            {
                adcMinOffset2[bengNiuJuPL_Chn] = UInt16.Parse(textBeng1NiuJuPL0.Text);
                adcMin2[bengNiuJuPL_Chn] = 10000;
                adcMax2[bengNiuJuPL_Chn] = 15000;
                valueMin2[bengNiuJuPL_Chn] = 0;
                valueMax2[bengNiuJuPL_Chn] = UInt16.Parse(textBeng1MaxNiuju.Text);

                adcMinOffset2[madaNiuJuPL_Chn] = UInt16.Parse(textMada1NiuJuPL0.Text);
                adcMin2[madaNiuJuPL_Chn] = 10000;
                adcMax2[madaNiuJuPL_Chn] = 15000;
                valueMin2[madaNiuJuPL_Chn] = 0;
                valueMax2[madaNiuJuPL_Chn] = UInt16.Parse(textMada1MaxNiuju.Text);

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateTest1()
        {
            updatePara1();

            labelRtime.Text = m_runtime.ToString();
            labelBengtest.Text = bengTest.ToString();
            labelMadatest.Text = madaTest.ToString();

            double xiYouYaLi = getAdcDispValue(adcMin1[XiYouYaLi_Chn], adcMax1[XiYouYaLi_Chn], valueMin1[XiYouYaLi_Chn], valueMax1[XiYouYaLi_Chn], m_voltage[XiYouYaLi_Chn], adcMinOffset1[XiYouYaLi_Chn]);
            double xiTongYaLi = getAdcDispValue(adcMin1[XiTongYaLi_Chn], adcMax1[XiTongYaLi_Chn], valueMin1[XiTongYaLi_Chn], valueMax1[XiTongYaLi_Chn], m_voltage[XiTongYaLi_Chn], adcMinOffset1[XiTongYaLi_Chn]);
            double bengPLYaLi = getAdcDispValue(adcMin1[BengPLYaLi_Chn], adcMax1[BengPLYaLi_Chn], valueMin1[BengPLYaLi_Chn], valueMax1[BengPLYaLi_Chn], m_voltage[BengPLYaLi_Chn], adcMinOffset1[BengPLYaLi_Chn]);
            double madaPLYali = getAdcDispValue(adcMin1[MadaPLYali_Chn], adcMax1[MadaPLYali_Chn], valueMin1[MadaPLYali_Chn], valueMax1[MadaPLYali_Chn], m_voltage[MadaPLYali_Chn], adcMinOffset1[MadaPLYali_Chn]);
            XiYouYaLi.Text = xiYouYaLi.ToString(formatStr1[XiYouYaLi_Chn]);
            XiTongYaLi.Text = xiTongYaLi.ToString(formatStr1[XiTongYaLi_Chn]);
            BengPLYaLi.Text = bengPLYaLi.ToString(formatStr1[BengPLYaLi_Chn]);
            MadaPLYali.Text = madaPLYali.ToString(formatStr1[MadaPLYali_Chn]);

            //XiYouYaLi.Text = getYali(0, maxYali0, m_voltage[XiYouYaLi_Chn]).ToString("f2");
            //XiTongYaLi.Text = getYali(0, maxYali, m_voltage[XiTongYaLi_Chn]).ToString("f1");
            //BengPLYaLi.Text = getYali(0, maxYali, m_voltage[BengPLYaLi_Chn]).ToString("f1");
            //MadaPLYali.Text = getYali(0, maxYali, m_voltage[MadaPLYali_Chn]).ToString("f1");

            int bengNiuJuPL = m_voltage[bengNiuJuPL_Chn];
            int bengzhuanSu = m_voltage[bengZhuanSu_Chn];
            int madaNiuJuPL = m_voltage[madaNiuJuPL_Chn];
            int madaZhuanSu = m_voltage[madaZhuanSu_Chn];


            //int bengNiuju_0 = UInt16.Parse(textBengNiuJuPL0.Text);
            //int madaNiuju_0 = UInt16.Parse(textMadaNiuJuPL0.Text);


            //int bengMaxNiuju = UInt16.Parse(textBengMaxNiuju.Text);
            //int madaMaxNiuju = UInt16.Parse(textMadaMaxNiuju.Text);

            //double bengNJ = Math.Abs((bengNiuJuPL - bengNiuju_0) * bengMaxNiuju / 5000);
            //double madaNJ = Math.Abs((madaNiuJuPL - madaNiuju_0) * madaMaxNiuju / 5000);

            double bengNJ = getAdcDispValue(adcMin1[bengNiuJuPL_Chn], adcMax1[bengNiuJuPL_Chn], valueMin1[bengNiuJuPL_Chn], valueMax1[bengNiuJuPL_Chn], m_voltage[bengNiuJuPL_Chn], adcMinOffset1[bengNiuJuPL_Chn]);
            double madaNJ = getAdcDispValue(adcMin1[madaNiuJuPL_Chn], adcMax1[madaNiuJuPL_Chn], valueMin1[madaNiuJuPL_Chn], valueMax1[madaNiuJuPL_Chn], m_voltage[madaNiuJuPL_Chn], adcMinOffset1[madaNiuJuPL_Chn]);

            bengNJ = Math.Abs(bengNJ);
            madaNJ = Math.Abs(madaNJ);

            BengNiuJu.Text = bengNJ.ToString(formatStr1[bengNiuJuPL_Chn]);
            MadaNiuJu.Text = madaNJ.ToString(formatStr1[madaNiuJuPL_Chn]);

            labeBengnnj0.Text = bengNiuJuPL.ToString(formatStr1[bengZhuanSu_Chn]);
            labeMadanj0.Text = madaNiuJuPL.ToString(formatStr1[madaZhuanSu_Chn]);

            BengZhuanSu.Text = bengzhuanSu.ToString(formatStr1[bengZhuanSu_Chn]);
            MadaZhuanSu.Text = madaZhuanSu.ToString(formatStr1[madaZhuanSu_Chn]);

            double bengGongLv = (double)bengNJ * bengzhuanSu / 9550;
            double madaGongLv = (double)madaNJ * madaZhuanSu / 9550;

            labelBengGL.Text = bengGongLv.ToString("f1");
            labelMadaGL.Text = madaGongLv.ToString("f1");

            if (bengzhuanSu == 0 || bengNiuJuPL == 0)
            {
                labelxl.Text = "-";
            }
            else
            {
                labelxl.Text = (madaGongLv / bengGongLv * 100).ToString("f1") + " %";
            }

            string KoutStrOn = "ON";
            string KoutStrOff = "OFF";
            if (bit(m_kout, XLF_CMD_CHN) != 0)
            {
                DO_XLF.BackColor = Color.DarkGray;
                labelXLF.Text = "XLF:" + KoutStrOff;
            }
            else
            {
                DO_XLF.BackColor = Color.Red;
                labelXLF.Text = "XLF:" + KoutStrOn;
            }
            if (bit(m_kout, JSF_CMD_CHN) != 0)
            {
                DO_JSF.BackColor = Color.DarkGray;
                labelJSF.Text = "JSF:" + KoutStrOff;
            }
            else
            {
                DO_JSF.BackColor = Color.Red;
                labelJSF.Text = "JSF:" + KoutStrOn;
            }
        }

        private void UpdateTest2()
        {
            updatePara2();

            labelRtime1.Text = m_runtime.ToString();
            labelBengtest1.Text = bengTest.ToString();
            labelMadatest1.Text = madaTest.ToString();

            double xiYouYaLi = getAdcDispValue(adcMin2[XiYouYaLi_Chn], adcMax2[XiYouYaLi_Chn], valueMin2[XiYouYaLi_Chn], valueMax2[XiYouYaLi_Chn], m_voltage[XiYouYaLi_Chn], adcMinOffset2[XiYouYaLi_Chn]);
            double xiTongYaLi = getAdcDispValue(adcMin2[XiTongYaLi_Chn], adcMax2[XiTongYaLi_Chn], valueMin2[XiTongYaLi_Chn], valueMax2[XiTongYaLi_Chn], m_voltage[XiTongYaLi_Chn], adcMinOffset2[XiTongYaLi_Chn]);
            double bengPLYaLi = getAdcDispValue(adcMin2[BengPLYaLi_Chn], adcMax2[BengPLYaLi_Chn], valueMin2[BengPLYaLi_Chn], valueMax2[BengPLYaLi_Chn], m_voltage[BengPLYaLi_Chn], adcMinOffset2[BengPLYaLi_Chn]);
            double madaPLYali = getAdcDispValue(adcMin2[MadaPLYali_Chn], adcMax2[MadaPLYali_Chn], valueMin2[MadaPLYali_Chn], valueMax2[MadaPLYali_Chn], m_voltage[MadaPLYali_Chn], adcMinOffset2[MadaPLYali_Chn]);
            XiYouYaLi1.Text = xiYouYaLi.ToString(formatStr2[XiYouYaLi_Chn]);
            XiTongYaLi1.Text = xiTongYaLi.ToString(formatStr2[XiTongYaLi_Chn]);
            BengPLYaLi1.Text = bengPLYaLi.ToString(formatStr2[BengPLYaLi_Chn]);
            MadaPLYali1.Text = madaPLYali.ToString(formatStr2[MadaPLYali_Chn]);


            double RuKouYali = getAdcDispValue(adcMin2[MadaGaoYaKou_Chn], adcMax2[MadaGaoYaKou_Chn], valueMin2[MadaGaoYaKou_Chn], valueMax2[MadaGaoYaKou_Chn], m_voltage[MadaGaoYaKou_Chn], adcMinOffset2[MadaGaoYaKou_Chn]);
            double ChuKouYaLi = getAdcDispValue(adcMin2[MadaDiYaKou_Chn], adcMax2[MadaDiYaKou_Chn], valueMin2[MadaDiYaKou_Chn], valueMax2[MadaDiYaKou_Chn], m_voltage[MadaDiYaKou_Chn], adcMinOffset2[MadaDiYaKou_Chn]);

            label_gaoyakou.Text = RuKouYali.ToString(formatStr2[MadaGaoYaKou_Chn]);
            label_diyakou.Text = ChuKouYaLi.ToString(formatStr2[MadaDiYaKou_Chn]);


            //XiYouYaLi1.Text = getYali(0, maxYali0, m_voltage[XiYouYaLi_Chn]).ToString("f2");
            //XiTongYaLi1.Text = getYali(0, maxYali, m_voltage[XiTongYaLi_Chn]).ToString("f1");
            //BengPLYaLi1.Text = getYali(0, maxYali, m_voltage[BengPLYaLi_Chn]).ToString("f1");
            //MadaPLYali1.Text = getYali(0, maxYali, m_voltage[MadaPLYali_Chn]).ToString("f1");

            int bengNiuJuPL = m_voltage[bengNiuJuPL_Chn];
            int bengzhuanSu = m_voltage[bengZhuanSu_Chn];
            int madaNiuJuPL = m_voltage[madaNiuJuPL_Chn];
            int madaZhuanSu = m_voltage[madaZhuanSu_Chn];


            //int bengNiuju_0 = UInt16.Parse(textBeng1NiuJuPL0.Text);
            //int madaNiuju_0 = UInt16.Parse(textMada1NiuJuPL0.Text);

            //int bengMaxNiuju = UInt16.Parse(textBeng1MaxNiuju.Text);
            //int madaMaxNiuju = UInt16.Parse(textMada1MaxNiuju.Text);

            //double bengNJ = Math.Abs((bengNiuJuPL - bengNiuju_0) * bengMaxNiuju / 5000);
            //double madaNJ = Math.Abs((madaNiuJuPL - madaNiuju_0) * madaMaxNiuju / 5000);
            double bengNJ = getAdcDispValue(adcMin2[bengNiuJuPL_Chn], adcMax2[bengNiuJuPL_Chn], valueMin2[bengNiuJuPL_Chn], valueMax2[bengNiuJuPL_Chn], m_voltage[bengNiuJuPL_Chn], adcMinOffset2[bengNiuJuPL_Chn]);
            double madaNJ = getAdcDispValue(adcMin2[madaNiuJuPL_Chn], adcMax2[madaNiuJuPL_Chn], valueMin2[madaNiuJuPL_Chn], valueMax2[madaNiuJuPL_Chn], m_voltage[madaNiuJuPL_Chn], adcMinOffset2[madaNiuJuPL_Chn]);

            bengNJ = Math.Abs(bengNJ);
            madaNJ = Math.Abs(madaNJ);

            BengNiuJu1.Text = bengNJ.ToString(formatStr1[bengNiuJuPL_Chn]);
            MadaNiuJu1.Text = madaNJ.ToString(formatStr1[madaNiuJuPL_Chn]);

            labeBengn1nj0.Text = bengNiuJuPL.ToString(formatStr1[bengZhuanSu_Chn]);
            labeMada1nj0.Text = madaNiuJuPL.ToString(formatStr1[madaZhuanSu_Chn]);

            BengZhuanSu1.Text = bengzhuanSu.ToString(formatStr1[bengZhuanSu_Chn]);
            MadaZhuanSu1.Text = madaZhuanSu.ToString(formatStr1[madaZhuanSu_Chn]);

            double bengGongLv = (double)bengNJ * bengzhuanSu / 9550;
            double madaGongLv = (double)madaNJ * madaZhuanSu / 9550;

            labelBengGL1.Text = bengGongLv.ToString("f1");
            labelMadaGL1.Text = madaGongLv.ToString("f1");

            if (bengzhuanSu == 0 || bengNiuJuPL == 0)
            {
                labelxl1.Text = "-";
            }
            else
            {
                labelxl1.Text = (madaGongLv / bengGongLv * 100).ToString("f1") + " %";
            }

            string KoutStrOn = "ON";
            string KoutStrOff = "OFF";
            if (bit(m_kout, XLF_CMD_CHN) != 0)
            {
                DO_XLF1.BackColor = Color.DarkGray;
                labelXLF1.Text = "XLF:" + KoutStrOff;
            }
            else
            {
                DO_XLF1.BackColor = Color.Red;
                labelXLF1.Text = "XLF:" + KoutStrOn;
            }
            if (bit(m_kout, JSF_CMD_CHN) != 0)
            {
                DO_JSF1.BackColor = Color.DarkGray;
                labelJSF1.Text = "JSF:" + KoutStrOff;
            }
            else
            {
                DO_JSF1.BackColor = Color.Red;
                labelJSF1.Text = "JSF:" + KoutStrOn;
            }

        }

        private void UpdateMonitor()
        {
            if (checkMonitor.Checked == false) return;
            labelRunTime.Text = m_runtime.ToString();

            Labelvoltage1.Text = ((float)m_voltage[0] * max_voltage / adcFull).ToString("f03");
            Labelvoltage2.Text = ((float)m_voltage[1] * max_voltage / adcFull).ToString("f03");
            Labelvoltage3.Text = ((float)m_voltage[2] * max_voltage / adcFull).ToString("f03");
            Labelvoltage4.Text = ((float)m_voltage[3] * max_voltage / adcFull).ToString("f03");
            Labelvoltage5.Text = ((float)m_voltage[4] * max_voltage / adcFull).ToString("f03");
            Labelvoltage6.Text = ((float)m_voltage[5] * max_voltage / adcFull).ToString("f03");
            Labelvoltage7.Text = ((float)m_voltage[6] * max_voltage / adcFull).ToString("f03");
            Labelvoltage8.Text = ((float)m_voltage[7] * max_voltage / adcFull).ToString("f03");

            labelCurrent1.Text = ((float)m_voltage[0] * maxDianliu / adcFull).ToString("00.000");
            labelCurrent2.Text = ((float)m_voltage[1] * maxDianliu / adcFull).ToString("00.000");
            labelCurrent3.Text = ((float)m_voltage[2] * maxDianliu / adcFull).ToString("00.000");
            labelCurrent4.Text = ((float)m_voltage[3] * maxDianliu / adcFull).ToString("00.000");
            labelCurrent5.Text = ((float)m_voltage[4] * maxDianliu / adcFull).ToString("00.000");
            labelCurrent6.Text = ((float)m_voltage[5] * maxDianliu / adcFull).ToString("00.000");
            labelCurrent7.Text = ((float)m_voltage[6] * maxDianliu / adcFull).ToString("00.000");
            labelCurrent8.Text = ((float)m_voltage[7] * maxDianliu / adcFull).ToString("00.000");

            labelfreq1.Text = m_voltage[8].ToString();
            labelfreq2.Text = m_voltage[9].ToString();
            labelfreq3.Text = m_voltage[10].ToString();
            labelfreq4.Text = m_voltage[11].ToString();
            labelfreq5.Text = m_voltage[12].ToString();
            labelfreq6.Text = m_voltage[13].ToString();
            labelfreq7.Text = m_voltage[14].ToString();
            labelfreq8.Text = m_voltage[15].ToString();

            string KinStrOn = "0";
            string KinStrOff = "1";
            if (bit(m_kin, 0) != 0) labelkin1.Text = KinStrOff;
            else labelkin1.Text = KinStrOn;
            if (bit(m_kin, 1) != 0) labelkin2.Text = KinStrOff;
            else labelkin2.Text = KinStrOn;
            if (bit(m_kin, 2) != 0) labelkin3.Text = KinStrOff;
            else labelkin3.Text = KinStrOn;
            if (bit(m_kin, 3) != 0) labelkin4.Text = KinStrOff;
            else labelkin4.Text = KinStrOn;
            if (bit(m_kin, 4) != 0) labelkin5.Text = KinStrOff;
            else labelkin5.Text = KinStrOn;
            if (bit(m_kin, 5) != 0) labelkin6.Text = KinStrOff;
            else labelkin6.Text = KinStrOn;
            if (bit(m_kin, 6) != 0) labelkin7.Text = KinStrOff;
            else labelkin7.Text = KinStrOn;
            if (bit(m_kin, 7) != 0) labelkin8.Text = KinStrOff;
            else labelkin8.Text = KinStrOn;

            //labelkin8.Text=CuvForm.get_FaStatus(m_kin);
            if (bit(m_SWStatus, 3) != 0) labelsw1.Text = KinStrOff;
            else labelsw1.Text = KinStrOn;
            if (bit(m_SWStatus, 2) != 0) labelsw2.Text = KinStrOff;
            else labelsw2.Text = KinStrOn;
            if (bit(m_SWStatus, 1) != 0) labelsw3.Text = KinStrOff;
            else labelsw3.Text = KinStrOn;
            if (bit(m_SWStatus, 0) != 0) labelsw4.Text = KinStrOff;
            else labelsw4.Text = KinStrOn;

            //wdg.Text = m_GongLv_Target.ToString();
            string KoutStrOn = "ON";
            string KoutStrOff = "OFF";
            if (bit(m_kout, 0) != 0) labelkout1.Text = KoutStrOff;
            else labelkout1.Text = KoutStrOn;
            if (bit(m_kout, 1) != 0) labelkout2.Text = KoutStrOff;
            else labelkout2.Text = KoutStrOn;
            if (bit(m_kout, 2) != 0) labelkout3.Text = KoutStrOff;
            else labelkout3.Text = KoutStrOn;
            if (bit(m_kout, 3) != 0) labelkout4.Text = KoutStrOff;
            else labelkout4.Text = KoutStrOn;
            if (bit(m_kout, 4) != 0) labelkout5.Text = KoutStrOff;
            else labelkout5.Text = KoutStrOn;
            if (bit(m_kout, 5) != 0) labelkout6.Text = KoutStrOff;
            else labelkout6.Text = KoutStrOn;
            if (bit(m_kout, 6) != 0) labelkout7.Text = KoutStrOff;
            else labelkout7.Text = KoutStrOn;
            if (bit(m_kout, 7) != 0) labelkout8.Text = KoutStrOff;
            else labelkout8.Text = KoutStrOn;

            if (bit(m_kout, 8) != 0) labelkout9.Text = KoutStrOff;
            else labelkout9.Text = KoutStrOn;
            if (bit(m_kout, 9) != 0) labelkout10.Text = KoutStrOff;
            else labelkout10.Text = KoutStrOn;

            for (int i = 0; i < 10; i++)
            {
                if (bit(m_kout, i) == 0)
                    listDO[i].BackColor = Color.Red;
                else
                    listDO[i].BackColor = Color.DarkGray;
            }
        }

        //public Int16 get_y(Int16 y_start, Int16 y_end, Int16 x_start, Int16 x_end, Int16 x) //y=a*x+b 2011-5-27
        //{
        //    Int16 y;
        //    Int32 a, b;

        //    if (x_start == x_end) return y_start;
        //    if (y_start == y_end) return y_start;
        //    if (x_start < x_end)
        //    {
        //        if (x <= x_start) return y_start;
        //        if (x >= x_end) return y_end;
        //    }
        //    else
        //    {
        //        if (x >= x_start) return y_start;
        //        if (x <= x_end) return y_end;
        //    }
        //    a = (Int32)(y_start - y_end) * 10000 / (x_start - x_end);
        //    b = y_start - (Int32)x_start * a / 10000;
        //    y = (Int16)(x * a / 10000 + b);

        //    return y;
        //}

        private void button14_Click(object sender, EventArgs e)
        {

            ParaForm.Show();
            ParaForm.WindowState = FormWindowState.Normal;
            ParaForm.Focus();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            CuvForm.WindowState = FormWindowState.Normal;
            CuvForm.Show();
            CuvForm.Focus();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            CuvForm1.WindowState = FormWindowState.Normal;
            CuvForm1.Show();
            CuvForm1.Focus();
        }

        private void open_comm(SerialPort port)
        {
            try
            {
                port.ReadTimeout = 10;
                port.Open();
                master = ModbusSerialMaster.CreateRtu(serialPort);
                master.Transport.Retries = 0;   //don't have to do retries
                master.Transport.ReadTimeout = 100; //milliseconds
                //Console.WriteLine(DateTime.Now.ToString() + " =>Open " + serialPort.PortName + " sucessfully!");
                //openbtn.Enabled = false;
                //closebtn.Enabled = true;
            }

            catch (UnauthorizedAccessException)
            {
                string errs = "无法访问" + serialPort.PortName;
                ack_label.Text = errs;
            }
            catch (ApplicationException)
            {
                string errs = "无法打开串口" + serialPort.PortName;
                ack_label.Text = errs;
            }
            catch (Exception)
            {
                string errs = serialPort.PortName + ":操作失败";
                ack_label.Text = errs;
            }
            finally
            {
                if (port.IsOpen)
                {
                    COMMPortsList.Enabled = false;
                    OpenCOM.Enabled = false;
                    CloseCOM.Enabled = true;
                    btnStop.Enabled = false;
                    btnStart.Enabled = true;
                    ack_label.Text = port.PortName + "打开";
                    LinkMode = LinkModeType.SER_LINK;
                    txtServerIP.Enabled = false;
                    btnConnServer.Enabled = false;
                    btnDisConn.Enabled = false;
                }
            }
            fill_commmlist();
            for (int i = 0; i < COMMPortsList.Items.Count; i++)
            {
                if ((string)COMMPortsList.Items[i] == port.PortName)
                    COMMPortsList.SelectedIndex = i;
            }
        }
        private void OpenCOM_Click_1(object sender, EventArgs e)
        {
            if (COMMPortsList.Items.Count > 0)
            {
                serialPort.PortName = COMMPortsList.Text;
                open_comm(serialPort);
            }
            else
            {
                MessageBox.Show("没有发现串口");
                fill_commmlist();
            }

        }

        private void CloseCOM_Click_1(object sender, EventArgs e)
        {
            try
            {
                serialPort.Close();
                ack_label.Text = COMMPortsList.Text + "关闭";
                COMMPortsList.Enabled = true;
                OpenCOM.Enabled = true;
                CloseCOM.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                LinkMode = LinkModeType.NO_LINK;
                txtServerIP.Enabled = true;
                btnConnServer.Enabled = true;
                Auto_Send_Flag = false;

                timer1.Enabled = false;
            }
            catch (IOException)//InvalidOperationException)
            {
                //MessageBox.Show(COMMPortsList.Text + "已经处于关闭状态", "错误");
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            byte[] rbuf = new byte[500];
            int i;
            if (LinkMode != LinkModeType.SER_LINK) return;
            try
            {
                if (!serialPort.IsOpen) return;
                if ((i = serialPort.BytesToRead) > 0)
                {
                    i = serialPort.Read(rbuf, 0, i);
                    dataFIFO.Write(rbuf, 0, i);
                }
            }
            catch (System.IO.IOException)
            //catch (UnauthorizedAccessException)
            {

                //OpenCOM.Enabled = true;
                //CloseCOM.Enabled = false;
                //btnStop.Enabled = false;
                //btnStart.Enabled = false;
                ////ad_cnt.Text = "串口错误";
                //COMMPortsList.Enabled = true;
                //fill_commmlist();
                //for (i = 0; i < COMMPortsList.Items.Count; i++)
                //{
                //    if ((string)COMMPortsList.Items[i] == serialPort.PortName)
                //        COMMPortsList.SelectedIndex = i;
                //}
            }

        }

        //----------------------------------------------------------------------------------------------------

        private int CopyDataToBuf(byte[] DstBuf, byte[] SrcBuf, int len)
        {
            int i, j;
            UInt16 sum;

            j = 0;
            DstBuf[j++] = 0xa5;
            DstBuf[j++] = 0x5a;
            j += 2;

            for (i = 0; i < len; i++)
            {
                DstBuf[j++] = SrcBuf[i];
            }

            //
            DstBuf[2] = (byte)(j - 4);//data lengtgh
            DstBuf[3] = (byte)((j - 4) >> 8);//data lengtgh

            sum = 0;
            for (i = 0; i < j - 4; i++)
            {
                sum += DstBuf[i + 4];
            }

            DstBuf[j++] = (byte)sum;
            DstBuf[j++] = (byte)(sum >> 8);


            return j;
        }
        private int OneCmdToBuf(byte[] pbuf, byte cmd)
        {
            int i, j;
            UInt16 sum;

            j = 0;
            pbuf[j++] = 0xa5;
            pbuf[j++] = 0x5a;
            j += 2;


            pbuf[j++] = cmd;

            //
            pbuf[2] = (byte)(j - 4);//data lengtgh
            pbuf[3] = (byte)((j - 4) >> 8);//data lengtgh

            sum = 0;
            for (i = 0; i < j - 4; i++)
            {
                sum += pbuf[i + 4];
            }

            pbuf[j++] = (byte)sum;
            pbuf[j++] = (byte)(sum >> 8);


            return j;
        }


        private int RemoteOneCmdToBuf(byte[] pbuf, byte CmdId)
        {
            byte SubStationNum = (byte)GetSubStationNum();
            byte[] buf = new byte[2];
            buf[0] = SubStationNum;
            buf[1] = CmdId;
            return CopyDataToBuf(pbuf, buf, 2);
        }
        private int RemoteThreeCmdToBuf(byte[] pbuf, byte cmd1, byte cmd2, byte cmd3)
        {
            byte SubStationNum = (byte)GetSubStationNum();
            byte[] buf = new byte[4];
            buf[0] = SubStationNum;
            buf[1] = cmd1;
            buf[2] = cmd2;
            buf[3] = cmd3;
            return CopyDataToBuf(pbuf, buf, 4);
        }

        //private int SunstationParameterToBuf(byte[] pbuf)
        //{
        //    byte SubStationNum = (byte)GetSubStationNum();
        //    int i, j;
        //    byte[] buf = new byte[Parameters.Length * 2 + 2];

        //    j = 0;
        //    buf[j++] = SubStationNum;
        //    buf[j++] = SUBSTATION_PARAMETER_WRITE;
        //    for (i = 0; i < Parameters.Length; i++)
        //    {
        //        buf[j++] = (byte)Parameters[i];
        //        buf[j++] = (byte)(Parameters[i] >> 8);
        //    }
        //    return CopyDataToBuf(pbuf, buf, j);
        //}
        //private int AskAllSample(byte[] buffer)
        //{
        //    UInt16 id = 0;
        //    byte djID = byte.Parse(DJAddr.Text);
        //    byte djID1 = byte.Parse(DJAddr1.Text);
        //    byte monitorID = (byte)GetSubStationNum();
        //    byte id1 = CuvForm.ID;
        //    byte id2 = CuvForm.ID;

        //    id |= (UInt16)(1 << djID);
        //    id |= (UInt16)(1 << djID1);
        //    id |= (UInt16)(1 << id1);
        //    id |= (UInt16)(1 << id2);

        //    if (checkMonitor.Checked) id |= (UInt16)(1 << monitorID);

        //    byte[] tempbuf = new byte[16];

        //    tempbuf[0] = 0;// SubStationNum;
        //    tempbuf[1] = ALL_SUBSTATION_SAMPLE;// ALL_SUBSTATION_RESET;
        //    tempbuf[2] = (byte)id;
        //    tempbuf[3] = (byte)(id >> 8);

        //    int len = CopyDataToBuf(buffer, tempbuf, 4);
        //    return len;
        //}
        //----------------------------------------------------------------------------------------------------

        public void ModBusWriteSingleCoil(byte StationID, ushort index, bool opCode)
        {
            if (!portIsOpen()) return;
            labelMbusError.Text = "";
            try
            {
                master.WriteSingleCoil(StationID, index, opCode);

                labelMbusError.Text = "Write Completed " + StationID.ToString();
            }
            catch (Exception exception)
            {
                labelMbusError.Text = exception.Message + " " + StationID.ToString();
            }
        }

        public void ModBusWriteSingleRegister(byte StationID, ushort RegisterAddr, ushort data)
        {
            if (!portIsOpen()) return;
            labelMbusError.Text = "";
            try
            {
                master.WriteSingleRegister(StationID, RegisterAddr, data);

                labelMbusError.Text = "Write Completed " + StationID.ToString();
            }
            catch (Exception exception)
            {
                labelMbusError.Text = exception.Message + " " + StationID.ToString();
            }
        }
        public void ModBusWriteMultipleRegisters(byte StationID, ushort RegisterAddr, ushort[] DataBuf)
        {
            if (!portIsOpen()) return;
            labelMbusError.Text = "";
            try
            {
                master.WriteMultipleRegisters(StationID, RegisterAddr, DataBuf);

                labelMbusError.Text = "Write Completed " + StationID.ToString();
            }
            catch (Exception exception)
            {
                labelMbusError.Text = exception.Message + " " + StationID.ToString();
            }
        }

        private void ModBusSendKOutCmd(byte SubStationNum, UInt16 offCmd, UInt16 onCmd, UInt16 delay)
        {
            ushort[] dbuf = new ushort[3];
            dbuf[0] = offCmd;
            dbuf[1] = onCmd;
            dbuf[2] = delay;
            ModBusWriteMultipleRegisters(SubStationNum, MotorCmdRegister, dbuf);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                UInt16 offCmd = 1 << M1_RESET_CMD_CHN | 1 << M1_RESERVE_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M1_NORMAL_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M1ZFtime.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                UInt16 offCmd = 1 << M1_RESET_CMD_CHN | 1 << M1_NORMAL_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M1_RESERVE_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M1ZFtime.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                UInt16 offCmd = 1 << M1_RESERVE_CMD_CHN | 1 << M1_NORMAL_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M1_RESET_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M1FWtime.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                UInt16 offCmd = 1 << M2_RESET_CMD_CHN | 1 << M2_RESERVE_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M2_NORMAL_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M2ZFtime.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                UInt16 offCmd = 1 << M2_RESET_CMD_CHN | 1 << M2_NORMAL_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M2_RESERVE_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M2ZFtime.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                UInt16 offCmd = 1 << M2_RESERVE_CMD_CHN | 1 << M2_NORMAL_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M2_RESET_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M2FWtime.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DO_ON(byte SubStationNum, byte DoIndex)
        {
            ModBusWriteSingleCoil(SubStationNum, DoIndex, true);
        }

        private void DO_OFF(byte SubStationNum, byte DoIndex)
        {
            ModBusWriteSingleCoil(SubStationNum, DoIndex, false);
        }
        //private void Kout_ON(byte which)
        //{
        //    cmd_Send_Length = RemoteThreeCmdToBuf(cmd_bytes, SUBSTATION_KOUT_CMD, which, 1);
        //    Send_Cmd_Flag = true;
        //}

        //private void Kout_OFF(byte which)
        //{
        //    cmd_Send_Length = RemoteThreeCmdToBuf(cmd_bytes, SUBSTATION_KOUT_CMD, which, 0);
        //    Send_Cmd_Flag = true;
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            ModBusWriteSingleRegister(0, ResetRegister, 0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(YoumenAddr.Text);
                UInt16 offCmd = 1 << YOUMEN_DA_CHN;//turn off
                UInt16 onCmd = 1 << YOUMEN_XIAO_CHN;//turn on
                UInt16 delay = UInt16.Parse(YoumenTime.Text);
                if (delay == 0) delay = 10;

                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(YoumenAddr.Text);
                UInt16 offCmd = 1 << YOUMEN_XIAO_CHN;//turn off
                UInt16 onCmd = 1 << YOUMEN_DA_CHN;//turn on
                UInt16 delay = UInt16.Parse(YoumenTime.Text);
                if (delay == 0) delay = 10;

                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
               throw;
            }
        }


        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                UInt16 offCmd = 1 << M1_RESET_CMD_CHN | 1 << M1_RESERVE_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M1_NORMAL_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M1ZFtime1.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
               throw;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                UInt16 offCmd = 1 << M1_RESERVE_CMD_CHN | 1 << M1_NORMAL_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M1_RESET_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M1FWtime1.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                UInt16 offCmd = 1 << M1_RESET_CMD_CHN | 1 << M1_NORMAL_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M1_RESERVE_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M1ZFtime1.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                UInt16 offCmd = 1 << M2_RESET_CMD_CHN | 1 << M2_RESERVE_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M2_NORMAL_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M2ZFtime1.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                UInt16 offCmd = 1 << M2_RESERVE_CMD_CHN | 1 << M2_NORMAL_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M2_RESET_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M2FWtime1.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                UInt16 offCmd = 1 << M2_RESET_CMD_CHN | 1 << M2_NORMAL_CMD_CHN;//turn off
                UInt16 onCmd = 1 << M2_RESERVE_CMD_CHN;//turn on
                UInt16 delay = UInt16.Parse(M2ZFtime1.Text);
                ModBusSendKOutCmd(SubStationNum, offCmd, onCmd, delay);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private double getAdcRange(double ylmin, double ylmax, string syali)
        {
            double dyperyl = (adcFull - adcValueOf4mA) / (ylmax - ylmin);
            double yali = Double.Parse(syali);
            return yali * dyperyl;
        }

        public double getAdcDispValue(UInt16 adclmin, UInt16 adcmax, double ylmin, double ylmax, UInt16 adcval, UInt16 adcminoffset)
        {
            double dyperyl = (ylmax - ylmin) / (adcmax - adclmin);
            return ((adcval - adcminoffset) * dyperyl);
        }

        private double getAdc(double ylmin, double ylmax, string syali)
        {
            return (getAdcRange(ylmin, ylmax, syali) + adcValueOf4mA);
        }
        public double getYali(double ylmin, double ylmax, UInt16 adcval)
        {
            double dyperyl = (ylmax - ylmin) / (adcFull - adcValueOf4mA);
            return (adcval - adcValueOf4mA) * dyperyl;
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            textYaLiOut.Text = ((int)getAdc(0, maxYali, textYaLiInput.Text)).ToString();
            lalrange.Text = getYali(0, maxYali, (ushort)double.Parse(textYaLiInput.Text)).ToString("f02") + "MPa";
        }

        //public double get_y(double y_start, double y_end, double x_start, double x_end, double x) //y=a*x+b 2011-5-27
        //{
        //    double y;
        //    double a, b;

        //    if (x_start == x_end) return y_start;
        //    if (y_start == y_end) return y_start;
        //    if (x_start < x_end)
        //    {
        //        if (x <= x_start) return y_start;
        //        if (x >= x_end) return y_end;
        //    }
        //    else
        //    {
        //        if (x >= x_start) return y_start;
        //        if (x <= x_end) return y_end;
        //    }
        //    a = (y_start - y_end) / (x_start - x_end);
        //    b = y_start - x_start * a;
        //    y = (x * a + b);

        //    return y;
        //}


        //private void sendYaliValue(UInt16 value)
        //{
        //    try
        //    {
        //        byte SubStationNum = byte.Parse(DJAddr1.Text);
        //        UInt16 yaliTarget = value;// (UInt16)getAdc(0, maxYali, textTarget1.Text);// UInt16.Parse(textTarget.Text);
        //        UInt16 yaliError = (UInt16)getAdcRange(0, maxYali, textRange1.Text);//UInt16.Parse(textRange.Text);
        //        byte normalDelay = Byte.Parse(textNormalDelay1.Text);
        //        byte reverseDelay = Byte.Parse(textReverseDelay1.Text);
        //        byte kzPeriod = Byte.Parse(TextPeriod1.Text);

        //        int len = 0;
        //        byte[] buf = new byte[16];
        //        buf[len++] = SubStationNum;
        //        buf[len++] = SUBSTATION_BENG_RUN_NOR;

        //        buf[len++] = (byte)yaliTarget;
        //        buf[len++] = (byte)(yaliTarget >> 8);

        //        buf[len++] = (byte)(yaliError);
        //        buf[len++] = (byte)(yaliError >> 8);

        //        buf[len++] = normalDelay;
        //        buf[len++] = reverseDelay;

        //        buf[len++] = kzPeriod;
        //        buf[len++] = 1;//which chnnal
        //        cmd_Send_Length = CopyDataToBuf(cmd_bytes, buf, len);
        //        Send_Cmd_Flag = true;

        //    }
        //    catch (FormatException ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //        //            throw;
        //    }
        //}

        // private void stop()
        //{
        //    byte SubStationNum = byte.Parse(DJAddr1.Text);

        //    int len = 0;
        //    byte[] buf = new byte[16];
        //    buf[len++] = SubStationNum;
        //    buf[len++] = SUBSTATION_BENG_STOP;//

        //    buf[len++] = 0;
        //    buf[len++] = 0;
        //    cmd_Send_Length = CopyDataToBuf(cmd_bytes, buf, len);
        //    Send_Cmd_Flag = true;
        // }

        //private void sendRunBufNor(byte SubStationNum, UInt16 Target, UInt16 Period, byte normalDelay, byte reverseDelay, byte chn, UInt16 Range)
        //{
        //    int len = 0;
        //    byte[] buf = new byte[16];
        //    buf[len++] = SubStationNum;
        //    buf[len++] = SUBSTATION_MADA_RUN_NOR;

        //    buf[len++] = (byte)Target;
        //    buf[len++] = (byte)(Target >> 8);

        //    buf[len++] = (byte)Period;
        //    buf[len++] = (byte)(Period >> 8);

        //    buf[len++] = normalDelay;
        //    buf[len++] = reverseDelay;

        //    buf[len++] = chn;//which chnnal
        //    buf[len++] = (byte)(Range);

        //    cmd_Send_Length = CopyDataToBuf(cmd_bytes, buf, len);
        //    Send_Cmd_Flag = true;
        //}

        //private void sendRunBufRev(byte SubStationNum, UInt16 Target, UInt16 Period, byte normalDelay, byte reverseDelay, byte chn, UInt16 Range)
        //{
        //    int len = 0;
        //    byte[] buf = new byte[16];
        //    buf[len++] = SubStationNum;
        //    buf[len++] = SUBSTATION_MADA_RUN_REV;

        //    buf[len++] = (byte)Target;
        //    buf[len++] = (byte)(Target >> 8);

        //    buf[len++] = (byte)Period;
        //    buf[len++] = (byte)(Period >> 8);

        //    buf[len++] = normalDelay;
        //    buf[len++] = reverseDelay;

        //    buf[len++] = chn;//which chnnal
        //    buf[len++] = (byte)(Range);

        //    cmd_Send_Length = CopyDataToBuf(cmd_bytes, buf, len);
        //    Send_Cmd_Flag = true;
        //}


        private void button23_Click_1(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                ModBusWriteSingleRegister(SubStationNum, StopMotorRegister, MadaMotor);

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button13_Click_1(object sender, EventArgs e)//#2马达调排量
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                UInt16 Target = (UInt16)getAdc(0, maxYali, textTarget2.Text);// UInt16.Parse(textTarget.Text);
                UInt16 Range = (UInt16)getAdcRange(0, maxYali, textRange2.Text);//UInt16.Parse(textRange.Text);
                UInt16 normalDelay = UInt16.Parse(textNormalDelay2.Text);
                UInt16 reverseDelay = UInt16.Parse(textReverseDelay2.Text);
                UInt16 Period = Byte.Parse(TestPeriod2.Text);

                UInt16[] pData = new ushort[8];

                pData[0] = Target;
                pData[1] = Period;
                pData[2] = normalDelay;
                pData[3] = reverseDelay;
                pData[4] = MadaMotor;    //泵
                pData[5] = XiTongYaLi_Chn;// 1;            //控制模拟量1-系统压力
                pData[6] = 0;            //控制方向  reverse
                pData[7] = Range;

                ModBusWriteMultipleRegisters(SubStationNum, KZRunCmdRegister, pData);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private UInt16 GetpidfromStr(String str)
        {
            double f = double.Parse(str);
            UInt16 temp = (UInt16)(f * 1000);
            return temp;
        }


        private void DO0_Click(object sender, EventArgs e)
        {
            byte slaveID = GetSubStationNum();
            if (serialPort.IsOpen == true)
            {
                ushort index = ushort.Parse(((PictureBox)sender).Tag.ToString());
                if (((PictureBox)sender).BackColor == Color.Red)
                    ModBusWriteSingleCoil(slaveID, index, false);
                else
                    ModBusWriteSingleCoil(slaveID, index, true);
            }

        }

        private void button24_Click_1(object sender, EventArgs e)
        {
            textPid_P0.Text = "3.350";
            textPid_I0.Text = "1.000";
            textPid_D0.Text = "1.300";

            TextPeriod0.Text = "1000";
            textPid_K0.Text = "30";
        }

        private void button25_Click_1(object sender, EventArgs e)
        {
            textPid_P1.Text = "2.223";
            textPid_I1.Text = "3.502";
            textPid_D1.Text = "1.350";

            TextPeriod1.Text = "30";
            textPid_K1.Text = "15";
        }

        private void DO_XLF_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                if (DO_XLF.BackColor == Color.DarkGray)
                {
                    DO_ON(SubStationNum, XLF_CMD_CHN);
                }
                else
                {
                    DO_OFF(SubStationNum, XLF_CMD_CHN);
                }

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DO_JSF_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                if (DO_JSF.BackColor == Color.DarkGray)
                {
                    DO_ON(SubStationNum, JSF_CMD_CHN);
                }
                else
                {
                    DO_OFF(SubStationNum, JSF_CMD_CHN);
                }

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DO_XLF1_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                if (DO_XLF1.BackColor == Color.DarkGray)
                {
                    DO_ON(SubStationNum, XLF_CMD_CHN);
                }
                else
                {
                    DO_OFF(SubStationNum, XLF_CMD_CHN);
                }

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DO_JSF1_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                if (DO_JSF1.BackColor == Color.DarkGray)
                {
                    DO_ON(SubStationNum, JSF_CMD_CHN);
                }
                else
                {
                    DO_OFF(SubStationNum, JSF_CMD_CHN);
                }

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStopKeep1_Click_1(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                ModBusWriteSingleRegister(SubStationNum, StopMotorRegister, BengMotor);

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStopKeep2_Click(object sender, EventArgs e)
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                ModBusWriteSingleRegister(SubStationNum, StopMotorRegister, MadaMotor);

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnKeep2Yali_Click(object sender, EventArgs e)//#2泵保持系统压力
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                UInt16[] pData = new ushort[10];

                pData[0] = (UInt16)getAdc(0, maxYali, textTarget1.Value.ToString());
                pData[1] = UInt16.Parse(TextPeriod1.Text);
                pData[2] = UInt16.Parse(textPid_K1.Text);
                pData[3] = GetpidfromStr(textPid_P1.Text);
                pData[4] = GetpidfromStr(textPid_I1.Text);
                pData[5] = GetpidfromStr(textPid_D1.Text);
                pData[6] = BengMotor;    //泵
                pData[7] = XiTongYaLi_Chn;//1            //控制模拟量1-系统压力
                pData[8] = NormalFlag;            //控制方向  normal
                pData[9] = 0;// (UInt16)getAdcRange(0, maxYali, textRange1.Text);//UInt16.Parse(textRange.Text);

                ModBusWriteMultipleRegisters(SubStationNum, PIDCmdRegister, pData);

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnKeep1Zhuansu_Click(object sender, EventArgs e)//#1泵保持马达转速
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr.Text);
                UInt16[] pData = new ushort[10];

                pData[0] = (ushort)textTarget0.Value;// UInt16.Parse(textTarget0.Text);
                pData[1] = UInt16.Parse(TextPeriod0.Text);
                pData[2] = UInt16.Parse(textPid_K0.Text);
                pData[3] = GetpidfromStr(textPid_P0.Text);
                pData[4] = GetpidfromStr(textPid_I0.Text);
                pData[5] = GetpidfromStr(textPid_D0.Text);
                pData[6] = BengMotor;    //泵
                pData[7] = madaZhuanSu_Chn;// 11;            //控制模拟量11-马达转速
                pData[8] = NormalFlag;            //控制方向  normal
                pData[9] = 0;// UInt16.Parse(textRange0.Text);

                ModBusWriteMultipleRegisters(SubStationNum, PIDCmdRegister, pData);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMadaKeepYali_Click(object sender, EventArgs e)//马达2保持压力
        {
            try
            {
                byte SubStationNum = byte.Parse(DJAddr1.Text);
                UInt16[] pData = new ushort[10];

                pData[0] = (UInt16)getAdc(0, maxYali, textTarget1.Value.ToString());
                pData[1] = UInt16.Parse(TextPeriod1.Text);
                pData[2] = UInt16.Parse(textPid_K1.Text);
                pData[3] = GetpidfromStr(textPid_P1.Text);
                pData[4] = GetpidfromStr(textPid_I1.Text);
                pData[5] = GetpidfromStr(textPid_D1.Text);
                pData[6] = MadaMotor;    //泵
                pData[7] = XiTongYaLi_Chn;//1            //控制模拟量1-系统压力
                pData[8] = ReverseFlag;            //控制方向  reverse
                pData[9] = 0;// (UInt16)getAdcRange(0, maxYali, textRange1.Text);//UInt16.Parse(textRange.Text);

                ModBusWriteMultipleRegisters(SubStationNum, PIDCmdRegister, pData);

            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //private void btnKeep1Pailiang_Click(object sender, EventArgs e)//#1泵保持系统压力
        //{
        //    try
        //    {
        //        //double inf1 = double.Parse(textTarget.Text);
        //        //double tar = get_y(PLMinVoltage, PLMaxVoltage, 0, 100, inf1);

        //        //double inf2 = double.Parse(textRange.Text);
        //        //double err = get_y(PLMinVoltage, PLMaxVoltage, 0, 100, inf2) - PLMinVoltage;

        //        //UInt16 Target = (UInt16)(tar / max_voltage * adcFull);
        //        //UInt16 Range = (UInt16)(err / max_voltage * adcFull);

        //        //byte SubStationNum = byte.Parse(DJAddr.Text);
        //        //UInt16[] pData = new ushort[10];

        //        //pData[0] = Target;// (UInt16)getAdc(0, maxYali, textTarget1.Text);
        //        //pData[1] = UInt16.Parse(TextPeriod.Text);
        //        //pData[2] = UInt16.Parse(textPid_K.Text);
        //        //pData[3] = GetpidfromStr(textPid_P.Text);
        //        //pData[4] = GetpidfromStr(textPid_I.Text);
        //        //pData[5] = GetpidfromStr(textPid_D.Text);
        //        //pData[6] = BengMotor;    //泵
        //        //pData[7] = 7;            //控制模拟量7-泵的排量电压
        //        //pData[8] = 1;            //控制方向  normal
        //        //pData[9] = Range;

        //        //ModBusWriteMultipleRegisters(SubStationNum, PIDCmdRegister, pData);

        //        byte SubStationNum = byte.Parse(DJAddr.Text);
        //        UInt16[] pData = new ushort[10];

        //        pData[0] = (UInt16)getAdc(0, maxYali, textTarget.Text);
        //        pData[1] = UInt16.Parse(TextPeriod.Text);
        //        pData[2] = UInt16.Parse(textPid_K.Text);
        //        pData[3] = GetpidfromStr(textPid_P.Text);
        //        pData[4] = GetpidfromStr(textPid_I.Text);
        //        pData[5] = GetpidfromStr(textPid_D.Text);
        //        pData[6] = BengMotor;    //泵
        //        pData[7] = 1;            //控制模拟量1-系统压力
        //        pData[8] = 1;            //控制方向  normal
        //        pData[9] = 0;// (UInt16)getAdcRange(0, maxYali, textRange1.Text);
        //        ModBusWriteMultipleRegisters(SubStationNum, PIDCmdRegister, pData);


        //    }
        //    catch (FormatException ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //        //            throw;
        //    }
        //}

    }
}
