using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DVW;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MBusRead
{
    public partial class CurveForm : Form
    {
        private const string FILE_EXTENSION_NAME = "sda";

        private const int NULL_DATA = 0;
        private const int FILE_DATA = 1;
        private const int LIVE_DATA = 2;
        int DataSelect = NULL_DATA;

        List<DataFrame> RecievedDataList = new List<DataFrame>();
        int RecievedDataCount = 0;
        
        List<DataFrame> FileDataList = new List<DataFrame>();
        int FileDataCount = 0;

        int SavedDataCount = 0;
        int SaveFileTimerCount = 0;
        const int SavePeriod = 60;//秒


        String DataListFileName;
        int ImageMultiples = 1;//10
        private float Tension = 0.0f;//设置张力

        List<ComboBox> listComboBox = new List<ComboBox>();

        Form1 fm1;
        private byte boardID;

        private UInt16[] adcMin = new ushort[16];// { adc4mA, adc4mA, adc4mA, adc4mA, 0, 0, 0, 0 };//前8路模拟量，后8路是频率
        private UInt16[] adcMax = new ushort[16];// { adc20mA, adc20mA, adc20mA, adc20mA, adcFull, adcFull, adcFull, adcFull };//前4路 压力传感器
        private float[] valueMin = new float[16];// { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        private float[] valueMax = new float[16];// { 2.0, 70.0, 70.0, 70.0, 3.3, 3.3, 3.3, 3.3 };
        private string[] formatStr = new string[16];// { "f2", "f1", "f1", "f1", "f3", "f3", "f3", "f3", 
        private string[] adcChnnalName = new string[16];// { "泵 吸油", "系统压力", "泵 排量", "马达排量", "电压5", "电压6", "电压7", "电压8", "泵 扭矩", "泵 转速", "马达扭矩", "马达转速", "频率5", "频率6", "频率7", "频率8" };
        private UInt16[] adcMinOffset = new ushort[16];//  { adc4mA, adc4mA, adc4mA, adc4mA, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//前8路模拟量，后8路是频率

        //定义该委托的事件     
        //       public event UdateMsgDelegate MyUpdateMsg;
        SendDtatDelegate SendDele;

        ////而后在form2中定义一个MyDelegate变量：
        //public MyDelegate my_event;

        ////定义form2中的my_event对应函数。
        //private void frm2_event(string text)
        //{
        //    messageBox.show(text);
        //}
        //private float[] YSBegin = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //private float[] YSEnd = { 4095, 4095, 4095, 4095, 4095, 4095, 4095, 4095, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535 };
        //曲线的范围 Y轴
        private float[] YSBegin = { 0, 0, 0, 0, 0, 0, 0, 0, 5000, 0, 5000, 0, 0, 0, 0, 0 };
        private float[] YSEnd = { 4095, 4095, 4095, 4095, 4095, 4095, 4095, 4095, 15000, 2000, 15000, 2000, 20000, 20000, 20000, 20000 };

        HScrollBar hbar;
        VScrollBar vbar;

        private int Curve_Num; //曲线数目

        private Color L_CursorColor = Color.Red;//.Red;//.YellowGreen;//左光标线颜色
        private Color R_CursorColor = Color.Green;//.DarkSlateGray;//.DarkBlue;//.Red;//.YellowGreen;//左光标线颜色

        private int L_CursorPoint = 0;//鼠标左键光标起始位置
        private int R_CursorPoint = 0;//鼠标左键光标起始位置

        Random rdn = new Random();
        private UInt16 testcnt = 0;

        public string crvpath;

        public int Combox1_Select
        {
            set
            {
                comboBox1.SelectedIndex = value;
            }
            get
            {
                return comboBox1.SelectedIndex;
            }
        }
        public int Combox2_Select
        {
            set
            {
                comboBox2.SelectedIndex = value;
            }
            get
            {
                return comboBox2.SelectedIndex;
            }
        }
        public int Combox3_Select
        {
            set
            {
                comboBox3.SelectedIndex = value;
            }
            get
            {
                return comboBox3.SelectedIndex;
            }
        }
        public int Combox4_Select
        {
            set
            {
                comboBox4.SelectedIndex = value;
            }
            get
            {
                return comboBox4.SelectedIndex;
            }
        }
        public byte ID
        {
            set
            {
                boardID = value;
            }
            get
            {
                return boardID;
            }
        }

        public CurveForm(Form1 form, byte id, UInt16[] adcmin, UInt16[] adcmax, float[] valuemin, float[] valuemax, string[] formatstr, UInt16[] adcninoffset, string[] adcchnnalvame)
        {
            this.fm1 = form;
            boardID = id;
            for (int i = 0; i < 16; i++)
            {
                this.adcMin[i] = adcmin[i];
                this.adcMax[i] = adcmax[i];
                this.valueMin[i] = valuemin[i];
                this.formatStr[i] = formatstr[i];
                this.valueMax[i] = valuemax[i];
                this.adcMinOffset[i] = adcninoffset[i];
                this.adcChnnalName[i] = adcchnnalvame[i];
            }
            InitializeComponent();
            initForm();
        }

        public CurveForm(Form1 frm)
        {
            this.fm1 = frm;
            InitializeComponent();
            initForm();
        }

        public CurveForm(Form1 frm, SendDtatDelegate delegete)
        {
            this.fm1 = frm;
            this.SendDele = delegete;
            InitializeComponent();
            initForm();
        }

        public CurveForm()
        {
            InitializeComponent();
            initForm();
        }

        private void initForm()
        {
            L_CursorPoint = pictureBox1.Width - 1;
            R_CursorPoint = pictureBox1.Width - 1;

            Curve_Num = adcChnnalName.Length;

            for (int i = 0; i < Curve_Num; i++)
            {
                comboBox1.Items.Add(adcChnnalName[i]);
                comboBox2.Items.Add(adcChnnalName[i]);
                comboBox3.Items.Add(adcChnnalName[i]);
                comboBox4.Items.Add(adcChnnalName[i]);
            }
            comboBox1.Items.Add("空");
            comboBox2.Items.Add("空");
            comboBox3.Items.Add("空");
            comboBox4.Items.Add("空");

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;

            listComboBox.Add(comboBox1);
            listComboBox.Add(comboBox2);
            listComboBox.Add(comboBox3);
            listComboBox.Add(comboBox4);

            hbar = new HScrollBar();
            hbar.Parent = this;
            hbar.Location = new Point(pictureBox1.Left, pictureBox1.Bottom);
            hbar.Size = new Size(pictureBox1.Width, 20);
            hbar.Minimum = 0;
            hbar.Maximum = 100;
            hbar.SmallChange = 1;
            hbar.LargeChange = 1;//设为1，否则value不能到最大值，设成10，要小9
            hbar.Value = hbar.Maximum;// - hbar.Minimum) / 2;
            hbar.ValueChanged += new EventHandler(hbar_OnValueChanged);

            vbar = new VScrollBar();
            vbar.Parent = this;
            vbar.Location = new Point(pictureBox1.Right, pictureBox1.Top);
            vbar.Size = new Size(20, pictureBox1.Height);
            vbar.Minimum = 1;
            vbar.Maximum = 50;
            vbar.SmallChange = 1;
            vbar.LargeChange = 1;
            vbar.Value = ImageMultiples;// (vbar.Maximum - vbar.Minimum) / 2;
            vbar.ValueChanged += new EventHandler(vbar_OnValueChanged);

            timer1.Start();
        }

        private void RefrushCurveAndInfo()
        {
            pictureBox1.Refresh();
            pictureBox2.Refresh();
            pictureBox3.Refresh();
            pictureBox4.Refresh();
            switch (DataSelect)
            {
                case NULL_DATA:
                    pictureBox1.Image = null;
                    break;
                case LIVE_DATA:
                    UpdateTextMessage(pictureBox1, RecievedDataList, RecievedDataCount, ImageMultiples);
                    break;
                case FILE_DATA:
                    UpdateTextMessage(pictureBox1, FileDataList, FileDataCount, ImageMultiples);
                    break;
                default:
                    pictureBox1.Image = null;
                    break;
            }
        }

        private void hbar_OnValueChanged(object sender, EventArgs e)
        {
            float bl = (float)hbar.Value / 100;// hbar.Maximum;
            int Count;
            switch (DataSelect)
            {
                case NULL_DATA:
                    break;
                case LIVE_DATA:
                    Count = (int)(RecievedDataList.Count * bl);
                    if (Count < pictureBox1.Width / ImageMultiples)
                    {
                        int val = pictureBox1.Width / ImageMultiples * hbar.Maximum / RecievedDataList.Count;
                        if (val >= hbar.Minimum && val <= hbar.Maximum)
                            hbar.Value = val;
                        return;
                    }
                    RecievedDataCount = (int)(RecievedDataList.Count * bl);
                    RefrushCurveAndInfo();
                    break;
                case FILE_DATA:
                    Count = (int)(FileDataList.Count * bl);
                    if (Count < pictureBox1.Width / ImageMultiples)
                    {
                        int val = pictureBox1.Width / ImageMultiples * hbar.Maximum / FileDataList.Count;
                        if (val >= hbar.Minimum && val <= hbar.Maximum)
                            hbar.Value = val;
                        return;
                    }
                    FileDataCount = (int)(FileDataList.Count * bl);
                    RefrushCurveAndInfo();
                    break;
                default:
                    pictureBox1.Image = null;
                    break;
            }
        }

        private void vbar_OnValueChanged(object sender, EventArgs e)
        {
            ImageMultiples = vbar.Value;
            RefrushCurveAndInfo();
        }

        private void CurveForm_Load(object sender, EventArgs e)
        {
            fill_filename(crvpath);
            //if (listBox1.Items.Count > 0) listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                UInt16[] pval = new UInt16[36];
                for (int i = 0; i < pval.Length; i++) pval[i] = (UInt16)(rdn.Next(4095));
                pval[fm1.RunTime_Index] = testcnt++;
                UpdateEvent(pval, boardID);
            }

            if (++SaveFileTimerCount >= (5 * SavePeriod))
            {
                SaveFileTimerCount = 0;
                if (chkAutoSave.Checked == true)
                {
                    if (RecievedDataCount > SavedDataCount)
                    {
                        saveCurvData();
                        SavedDataCount = RecievedDataCount;
                    }
                }
            }
            
        }

        private void DisplaySampleValue(List<DataFrame> list, int index)
        {
            if (list.Count == 0) return;
 
            UInt16[] m_voltage = new UInt16[16];

            UInt16 m_SWStatus, m_kin, m_kout;
            UInt16 m_alarm, m_runtime, m_dacout, m_temperature, m_temperatureError;
            UInt16 mTest1, mTest2, mTest3, mTest4, mTest5;
            UInt16 mTest6, mTest7, mTest8, mTest9, mTest10, mTest11, mTest12;

            int i = 0;

            for (i = 0; i < 16; i++)
            {
                m_voltage[i] = list[index].data[i];
            }

            //曲线后第一个数据
            m_SWStatus = list[index].data[i++];

            m_kin = list[index].data[i++];
            m_kout = list[index].data[i++];

            m_alarm = list[index].data[i++];

            m_runtime = list[index].data[i++];
            m_dacout = list[index].data[i++];
            m_temperature = list[index].data[i++];
            m_temperatureError = list[index].data[i++];

            mTest1 = list[index].data[i++];
            mTest2 = list[index].data[i++];
            mTest3 = list[index].data[i++];
            mTest4 = list[index].data[i++];
            mTest5 = list[index].data[i++];
            mTest6 = list[index].data[i++];
            mTest7 = list[index].data[i++];
            mTest8 = list[index].data[i++];
            mTest9 = list[index].data[i++];
            mTest10 = list[index].data[i++];
            mTest11 = list[index].data[i++];
            mTest12 = list[index].data[i++];

            pidTarget.Text = mTest1.ToString();
            pidPeriod.Text = mTest2.ToString();
            pidKkk.Text = mTest3.ToString();

            labelkp.Text = (mTest4 / 1000.0f).ToString("f3");
            labelki.Text = (mTest5 / 1000.0f).ToString("f3");
            labelkd.Text = (mTest6 / 1000.0f).ToString("f3");

            BengTest.Text = mTest7.ToString();
            MadaTest.Text = mTest8.ToString();
            label1Inc.Text = mTest9.ToString();
            labelPidCnt.Text = mTest10.ToString();
        }
 
        private void UpdateTextMessage(PictureBox pic, List<DataFrame> datalist, int listupdateCount, int imageMultiples)
        {

            ////if ((L_CursorPoint % multiples) != 0) return;

            int pp = L_CursorPoint / imageMultiples;
            if (pp > datalist.Count - 1) pp = datalist.Count - 1;
            int index;

            if (listupdateCount <= pic.Width / ImageMultiples)
                index = pp;// return list[cursorToIndex].data[chn];
            else
                index = listupdateCount - pic.Width / ImageMultiples + pp;

            if (index > datalist.Count - 1) index = datalist.Count - 1;
            DisplaySampleValue(datalist, index);
        }

        private ushort getvalue(PictureBox pic, List<DataFrame> list, int cursorToIndex, int chn, int listupdateCount)
        {
            if (cursorToIndex >= listupdateCount) cursorToIndex = listupdateCount - 1;
            if (cursorToIndex < 0) cursorToIndex = 0;

            int len = pic.Width / ImageMultiples;
            if (listupdateCount < len)
            {
                if (cursorToIndex < listupdateCount)
                    return list[cursorToIndex].data[chn];
                else
                    return list[listupdateCount - 1].data[chn];

            }
            else
            {
                int pos = listupdateCount - len + cursorToIndex;
                if (pos >= list.Count) pos = list.Count - 1;
                return list[pos].data[chn];
            }
        }

        private float GetCursorValue(PictureBox pic, List<DataFrame> list, int cursor, int chn, int Multiples, int listupdateCount)
        {
            if (list.Count == 0) return 0;
            if (listupdateCount == 0) return 0;

            int cursorToIndex = cursor / Multiples;
            if ((cursor % Multiples) == 0)
            {
                if (cursorToIndex >= (listupdateCount - 1))
                {
                    ushort ret = getvalue(pic, list, listupdateCount - 1, chn, listupdateCount);
                    return ret;
                }
                else
                {
                    if (cursorToIndex > pic.Width / Multiples - 1) cursorToIndex = pic.Width / Multiples - 1;
                    ushort ret = getvalue(pic, list, cursorToIndex, chn, listupdateCount);
                    return ret;
                }
            }
            int left, right;
            if (cursorToIndex == 0)
            {
                left = 0;
                right = 1;
            }
            else if (cursorToIndex >= (listupdateCount - 1))
            {
                left = listupdateCount - 1;
                right = listupdateCount - 1;
            }
            else if (cursorToIndex >= (pic.Width / ImageMultiples - 1))
            {
                left = pic.Width / ImageMultiples - 1;
                right = pic.Width / ImageMultiples - 1;
            }
            else
            {
                left = cursorToIndex;
                right = cursorToIndex + 1;
            }
            float leftValue = getvalue(pic, list, left, chn, listupdateCount);
            float rightValue = getvalue(pic, list, right, chn, listupdateCount);
            return (float)(cursor % Multiples) / Multiples * (rightValue - leftValue) + leftValue;
        }

        private void DrawSampleValue(Graphics graphics, PictureBox pic, List<DataFrame> list, int multiples, float tension, int listupdateCount)
        {
            if (list.Count == 0) return;
            if (listupdateCount == 0) return;

            int pp = L_CursorPoint / multiples;
            if (pp > list.Count - 1) pp = list.Count - 1;
            int index;

            if (listupdateCount <= pictureBox1.Width / ImageMultiples)
                index = pp;// return list[cursorToIndex].data[chn];
            else
                index = listupdateCount - pictureBox1.Width / ImageMultiples + pp;

            if (index > list.Count - 1) index = list.Count - 1;

            UInt16[] m_voltage = new UInt16[16];

            UInt16 m_SWStatus, m_kin, m_kout;
            UInt16 m_alarm, m_runtime, m_dacout, m_temperature, m_temperatureError;
            UInt16 mTest1, mTest2, mTest3, mTest4, mTest5;
            UInt16 mTest6, mTest7, mTest8, mTest9, mTest10, mTest11, mTest12;

            int i = 0;

            for (i = 0; i < 16; i++)
            {
                m_voltage[i] = list[index].data[i];
            }

            //曲线后第一个数据
            m_SWStatus = list[index].data[i++];

            m_kin = list[index].data[i++];
            m_kout = list[index].data[i++];

            m_alarm = list[index].data[i++];

            m_runtime = list[index].data[i++];
            m_dacout = list[index].data[i++];
            m_temperature = list[index].data[i++];
            m_temperatureError = list[index].data[i++];

            mTest1 = list[index].data[i++];
            mTest2 = list[index].data[i++];
            mTest3 = list[index].data[i++];
            mTest4 = list[index].data[i++];
            mTest5 = list[index].data[i++];
            mTest6 = list[index].data[i++];
            mTest7 = list[index].data[i++];
            mTest8 = list[index].data[i++];
            mTest9 = list[index].data[i++];
            mTest10 = list[index].data[i++];
            mTest11 = list[index].data[i++];
            mTest12 = list[index].data[i++];

            int x = 5;
            int y = 5;

            String str;

            Font drawFont = new Font("宋体", 12);
            SolidBrush drawBrush = new SolidBrush(Color.Black);//System.Drawing.Brushes.Blue
            int linspan = (int)(drawFont.Size * 2); ;

            int increaseY = 70;

            //line 1
            str = ((float)m_runtime / 100).ToString("f2");
            graphics.DrawString("采样计时", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseY, y));

            str = list[index].dateTime.ToString("yyyy-MM-dd HH:mm:ss fff");// +" " + L_CursorPoint.ToString();//
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseY + 100, y));

            //line 2
            str = "|";
            for (int j = 0; j < 8; j++)
            {
                str += ((float)m_voltage[j] * fm1.max_voltage / 4095).ToString("f03");
                str += "|";
            }
            y += linspan;
            graphics.DrawString("电压 V", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseY, y));

            //line 3
            str = "|";
            for (int j = 0; j < 8; j++)
            {
                //swq += Title[j];
                str += ((float)m_voltage[j] * fm1.maxDianliu / 4095).ToString("00.00");
                str += "|";
            }
            y += linspan;
            graphics.DrawString("电流 mA", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseY, y));

            //line 4
            str = "|";
            for (int j = 0; j < 8; j++)
            {
                str += m_voltage[j + 8].ToString("d05");// ((float)m_DY[j] * fm1.max_voltage  / fm1.max_range).ToString("f03");
                str += "|";
            }
            y += linspan;
            graphics.DrawString("频率 Hz", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseY, y));

            //line 5
            str = get_FaStatus(m_kin, 6);   //6个
            y += linspan;
            graphics.DrawString("开入", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseY, y));

            //line 6
            str = get_FaStatus(m_kout, 10); //10个
            if (bit(m_kout, Form1.XLF_CMD_CHN) == 0) str += "XLF_ON ";
            if (bit(m_kout, Form1.JSF_CMD_CHN) == 0) str += "JSF_ON ";
            y += linspan;
            graphics.DrawString("开出", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseY, y));

            //line 7
            str = get_FaStatus(m_alarm, 8);
            y += linspan;
            graphics.DrawString("告警", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseY, y));

            str = "编号:" + list[index].boardID.ToString();
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseY + 200, y));
        }

        private void DrawDebugValue(Graphics graphics, PictureBox pic, List<DataFrame> list, int multiples, float tension, int listupdateCount)
        {
            if (list.Count == 0) return;
            if (listupdateCount == 0) return;

            int pp = L_CursorPoint / multiples;
            if (pp > list.Count - 1) pp = list.Count - 1;
            int index;

            if (listupdateCount <= pictureBox1.Width / ImageMultiples)
                index = pp;// return list[cursorToIndex].data[chn];
            else
                index = listupdateCount - pictureBox1.Width / ImageMultiples + pp;

            if (index > list.Count - 1) index = list.Count - 1;

            UInt16[] m_voltage = new UInt16[16];

            UInt16 m_SWStatus, m_kin, m_kout;
            UInt16 m_alarm, m_runtime, m_dacout, m_temperature, m_temperatureError;
            UInt16 mTest1, mTest2, mTest3, mTest4, mTest5;
            UInt16 mTest6, mTest7, mTest8, mTest9, mTest10, mTest11, mTest12;

            int i = 0;

            for (i = 0; i < 16; i++)
            {
                m_voltage[i] = list[index].data[i];
            }

            //曲线后第一个数据
            m_SWStatus = list[index].data[i++];

            m_kin = list[index].data[i++];
            m_kout = list[index].data[i++];

            m_alarm = list[index].data[i++];

            m_runtime = list[index].data[i++];
            m_dacout = list[index].data[i++];
            m_temperature = list[index].data[i++];
            m_temperatureError = list[index].data[i++];

            mTest1 = list[index].data[i++];
            mTest2 = list[index].data[i++];
            mTest3 = list[index].data[i++];
            mTest4 = list[index].data[i++];
            mTest5 = list[index].data[i++];
            mTest6 = list[index].data[i++];
            mTest7 = list[index].data[i++];
            mTest8 = list[index].data[i++];
            mTest9 = list[index].data[i++];
            mTest10 = list[index].data[i++];
            mTest11 = list[index].data[i++];
            mTest12 = list[index].data[i++];

            int x = 5;
            int y = 5;

            Font drawFont = new Font("宋体", 12);
            SolidBrush drawBrush = new SolidBrush(Color.Black);//System.Drawing.Brushes.Blue

            //4-20ma 0-200°
            double dutemp;
            UInt16 bengNiuJuPL, bengZhuanSu, madaNiuJuPL, madaZhuanSu;

            bengNiuJuPL = m_voltage[fm1.bengNiuJuPL_Chn];
            bengZhuanSu = m_voltage[fm1.bengZhuanSu_Chn];
            madaNiuJuPL = m_voltage[fm1.madaNiuJuPL_Chn];
            madaZhuanSu = m_voltage[fm1.madaZhuanSu_Chn];

            double bengNJ = fm1.getAdcDispValue(adcMin[fm1.bengNiuJuPL_Chn], adcMax[fm1.bengNiuJuPL_Chn], valueMin[fm1.bengNiuJuPL_Chn], valueMax[fm1.bengNiuJuPL_Chn], m_voltage[fm1.bengNiuJuPL_Chn], adcMinOffset[fm1.bengNiuJuPL_Chn]);
            double madaNJ = fm1.getAdcDispValue(adcMin[fm1.madaNiuJuPL_Chn], adcMax[fm1.madaNiuJuPL_Chn], valueMin[fm1.madaNiuJuPL_Chn], valueMax[fm1.madaNiuJuPL_Chn], m_voltage[fm1.madaNiuJuPL_Chn], adcMinOffset[fm1.madaNiuJuPL_Chn]);

            bengNJ = Math.Abs(bengNJ);
            madaNJ = Math.Abs(madaNJ);

            double bengGongLv = bengNJ * bengZhuanSu / 9550;
            double madaGongLv = madaNJ * madaZhuanSu / 9550;

            int linspan = (int)(drawFont.Size * 2); ;
            String str;
            int increaseX = 100;
            int x2 = 170;
            int increaseX2 = 110;
            //line 1
            dutemp = fm1.getAdcDispValue(adcMin[fm1.XiTongYaLi_Chn], adcMax[fm1.XiTongYaLi_Chn], valueMin[fm1.XiTongYaLi_Chn], valueMax[fm1.XiTongYaLi_Chn], m_voltage[fm1.XiTongYaLi_Chn], adcMinOffset[fm1.XiTongYaLi_Chn]);
            str = dutemp.ToString(formatStr[fm1.XiTongYaLi_Chn]);
            graphics.DrawString("系统压力", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseX, y));

            if (bengZhuanSu == 0 || bengNiuJuPL == 0)
                str = "-";
            else
                str = (madaGongLv / bengGongLv * 100).ToString("f1") + " %";
            graphics.DrawString("效率", drawFont, drawBrush, new Point(x2, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x2 + increaseX2, y));

            //line 2
            dutemp = fm1.getAdcDispValue(adcMin[fm1.XiYouYaLi_Chn], adcMax[fm1.XiYouYaLi_Chn], valueMin[fm1.XiYouYaLi_Chn], valueMax[fm1.XiYouYaLi_Chn], m_voltage[fm1.XiYouYaLi_Chn], adcMinOffset[fm1.XiYouYaLi_Chn]);
            str = dutemp.ToString(formatStr[fm1.XiYouYaLi_Chn]);
            y += linspan;
            graphics.DrawString("吸油压力", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseX, y));

            dutemp = fm1.getAdcDispValue(adcMin[fm1.WenDu_Chn], adcMax[fm1.WenDu_Chn], valueMin[fm1.WenDu_Chn], valueMax[fm1.WenDu_Chn], m_voltage[fm1.WenDu_Chn], adcMinOffset[fm1.WenDu_Chn]);
            str = dutemp.ToString("f0");
            graphics.DrawString("油温", drawFont, drawBrush, new Point(x2, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x2 + increaseX2, y));

            y += linspan;// / 2;
            //line 3
            dutemp = fm1.getAdcDispValue(adcMin[fm1.BengPLYaLi_Chn], adcMax[fm1.BengPLYaLi_Chn], valueMin[fm1.BengPLYaLi_Chn], valueMax[fm1.BengPLYaLi_Chn], m_voltage[fm1.BengPLYaLi_Chn], adcMinOffset[fm1.BengPLYaLi_Chn]);
            str = dutemp.ToString(formatStr[fm1.BengPLYaLi_Chn]);
            y += linspan;
            graphics.DrawString("泵 排量压力", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseX, y));

            dutemp = fm1.getAdcDispValue(adcMin[fm1.MadaPLYali_Chn], adcMax[fm1.MadaPLYali_Chn], valueMin[fm1.MadaPLYali_Chn], valueMax[fm1.MadaPLYali_Chn], m_voltage[fm1.MadaPLYali_Chn], adcMinOffset[fm1.MadaPLYali_Chn]);
            str = dutemp.ToString(formatStr[fm1.MadaPLYali_Chn]);
            graphics.DrawString("马达 排量压力", drawFont, drawBrush, new Point(x2, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x2 + increaseX2, y));

            //line 4
            y += linspan;
            str = bengZhuanSu.ToString(formatStr[fm1.bengZhuanSu_Chn]);
            graphics.DrawString("泵 转速", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseX, y));

            str = madaZhuanSu.ToString(formatStr[fm1.madaZhuanSu_Chn]);
            graphics.DrawString("马达 转速", drawFont, drawBrush, new Point(x2, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x2 + increaseX2, y));


            //line 5
            y += linspan;
            str = bengNJ.ToString(formatStr[fm1.bengNiuJuPL_Chn]);
            graphics.DrawString("泵 扭矩", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseX, y));

            str = madaNJ.ToString(formatStr[fm1.madaNiuJuPL_Chn]);
            graphics.DrawString("马达 扭矩", drawFont, drawBrush, new Point(x2, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x2 + increaseX2, y));

            //line 6
            y += linspan;
            str = bengGongLv.ToString("f1");
            graphics.DrawString("泵 功率", drawFont, drawBrush, new Point(x, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x + increaseX, y));

            str = madaGongLv.ToString("f1");
            graphics.DrawString("马达 功率", drawFont, drawBrush, new Point(x2, y));
            graphics.DrawString(str, drawFont, drawBrush, new Point(x2 + increaseX2, y));
        }

        private void DrawCursorValue(Graphics graphics, PictureBox pic, List<DataFrame> list, int multiples, float tension, int listupdateCount)
        {
            if (list.Count == 0) return;
            if (listupdateCount == 0) return;
            int x = 0;
            int y = 0;

            Font drawFont = new Font("宋体", 11);
            SolidBrush drawBrush = new SolidBrush(Color.Black);//System.Drawing.Brushes.Blue
            int linspan = (int)(drawFont.Size * 1.5); ;
            int linspan2 = (int)(drawFont.Size * 1.9); ;
            String str;

            float L_m_runtime = 0, R_m_runtime = 0;
            if (L_CursorPoint / ImageMultiples < pictureBox1.Width / ImageMultiples)
            {
                L_m_runtime = GetCursorValue(pictureBox1, list, L_CursorPoint, fm1.RunTime_Index, ImageMultiples, listupdateCount);//''fm1.RunTime_Index);// Curves_Value[fm1.RunTime_Index, L_CursorPoint];
            }
            if (L_CursorPoint / ImageMultiples < pictureBox1.Width / ImageMultiples)
            {
                R_m_runtime = GetCursorValue(pictureBox1, list, R_CursorPoint, fm1.RunTime_Index, ImageMultiples, listupdateCount);//fm1.RunTime_Index);// Curves_Value[fm1.RunTime_Index, R_CursorPoint];
            }

            str = L_CursorPoint.ToString() + "  " + R_CursorPoint.ToString() + " " + ((L_CursorPoint - R_CursorPoint) / ImageMultiples).ToString();
            graphics.DrawString(str, drawFont, drawBrush, new Point(x, y));

            y += linspan;
            //str = L_m_runtime.ToString("f1") + " " + R_m_runtime.ToString("f1") + " " + (L_m_runtime - R_m_runtime).ToString("f1");
            str = (L_m_runtime - R_m_runtime).ToString("f1");
            graphics.DrawString(str, drawFont, drawBrush, new Point(x, y));

            linspan2 = (int)(drawFont.Size * 2); ;

            string[] strs = new string[listComboBox.Count];
            for (int i = 0; i < listComboBox.Count; i++)
            {
                y += linspan2;
                if (listComboBox[i].SelectedIndex < Curve_Num)
                {
                    drawBrush.Color = listComboBox[i].ForeColor;
                    int chn = listComboBox[i].SelectedIndex;
                    double[] ret = getAllValue(pictureBox1, list, listupdateCount, chn, L_CursorPoint, R_CursorPoint);
                    str = ret[3].ToString(formatStr[chn]) + " " + ret[4].ToString(formatStr[chn]);
                    graphics.DrawString(str, drawFont, drawBrush, new Point(x, y));
                    y += linspan;
                    str = ret[0].ToString(formatStr[chn]) + "-" + ret[1].ToString(formatStr[chn]);
                    graphics.DrawString(str, drawFont, drawBrush, new Point(x, y));

                }
                else
                {
                    y += linspan;
                }
            }
        }

        private double[] getAllValue(PictureBox pic, List<DataFrame> list, int listupdateCount, int chn, int starCurort, int endCurort)
        {
            float LPointValue = GetCursorValue(pic, list, starCurort, chn, ImageMultiples, listupdateCount);
            float RPointValue = GetCursorValue(pic, list, endCurort, chn, ImageMultiples, listupdateCount);

            double sum = 0;
            float max = 0;
            float min = 65535;
            float avg = 0;
            int start = starCurort;
            int end = endCurort;
            if (starCurort == endCurort)
            {
                min = max = avg = GetCursorValue(pic, list, starCurort, chn, ImageMultiples, listupdateCount);
            }
            else
            {
                if (starCurort > endCurort)
                {
                    start = endCurort;
                    end = starCurort;
                }
                else
                {
                    start = starCurort;
                    end = endCurort;
                }

                int i;
                for (i = start; i <= end; i++)
                {
                    float temp = GetCursorValue(pic, list, i, chn, ImageMultiples, listupdateCount);
                    if (temp > max) max = temp;
                    if (temp < min) min = temp;
                    sum += temp;
                }
                avg = (float)(sum / i);
            }
            double minVal, maxVal, AvgVal, LVal, RVal;
            float Start = GetCursorValue(pic, list, start, chn, ImageMultiples, listupdateCount);
            float End = GetCursorValue(pic, list, end, chn, ImageMultiples, listupdateCount);

            LVal = fm1.getAdcDispValue(adcMin[chn], adcMax[chn], valueMin[chn], valueMax[chn], (ushort)Start, adcMinOffset[chn]);
            RVal = fm1.getAdcDispValue(adcMin[chn], adcMax[chn], valueMin[chn], valueMax[chn], (ushort)End, adcMinOffset[chn]);
            minVal = fm1.getAdcDispValue(adcMin[chn], adcMax[chn], valueMin[chn], valueMax[chn], (ushort)min, adcMinOffset[chn]);
            maxVal = fm1.getAdcDispValue(adcMin[chn], adcMax[chn], valueMin[chn], valueMax[chn], (ushort)max, adcMinOffset[chn]);
            AvgVal = fm1.getAdcDispValue(adcMin[chn], adcMax[chn], valueMin[chn], valueMax[chn], (ushort)avg, adcMinOffset[chn]);

            return new double[] { minVal, maxVal, AvgVal, LVal, RVal };
        }

        private void updatePara()
        {
            try
            {
                adcMinOffset[fm1.bengNiuJuPL_Chn] = UInt16.Parse(textBengNiuJuPL0.Text);
                adcMin[fm1.bengNiuJuPL_Chn] = 10000;
                adcMax[fm1.bengNiuJuPL_Chn] = 15000;
                valueMin[fm1.bengNiuJuPL_Chn] = 0;
                valueMax[fm1.bengNiuJuPL_Chn] = UInt16.Parse(textBengMaxNiuju.Text);

                adcMinOffset[fm1.madaNiuJuPL_Chn] = UInt16.Parse(textMadaNiuJuPL0.Text);
                adcMin[fm1.madaNiuJuPL_Chn] = 10000;
                adcMax[fm1.madaNiuJuPL_Chn] = 15000;
                valueMin[fm1.madaNiuJuPL_Chn] = 0;
                valueMax[fm1.madaNiuJuPL_Chn] = UInt16.Parse(textMadaMaxNiuju.Text);

            }
            catch (Exception e)
            {
                //throw;
                MessageBox.Show(e.ToString());
            }
        }

        private void drawline(PictureBox pic, int x1, int x2)
        {
            ControlPaint.DrawReversibleLine(this.PointToScreen(new Point(x1, pic.Height - 1)), this.PointToScreen(new Point(x1, 0)), SystemColors.Control);//this.BackColor);
            ControlPaint.DrawReversibleLine(this.PointToScreen(new Point(x2, pic.Height - 1)), this.PointToScreen(new Point(x2, 0)), SystemColors.Control);//this.BackColor);
        }

        private void drawValueAtCursor(int cursorPoint, PictureBox pic, Graphics graphics, List<DataFrame> list, int listupdateCount)//在光标尺位置draw
        {
            int fontSize = 13;
            for (int i = 0; i < listComboBox.Count; i++)
            {
                if (listComboBox[i].SelectedIndex < Curve_Num)
                {
                    int chn = listComboBox[i].SelectedIndex;
                    float value = GetCursorValue(pic, list, cursorPoint, chn, ImageMultiples, listupdateCount);//''fm1.RunTime_Index);// Curves_Value[fm1.RunTime_Index, L_CursorPoint];
                    double fVal = fm1.getAdcDispValue(adcMin[chn], adcMax[chn], valueMin[chn], valueMax[chn], (ushort)value, adcMinOffset[chn]);
                    float x = cursorPoint;
                    float y = pic.Height - 1 - (value - YSBegin[chn]) / (YSEnd[chn] - YSBegin[chn]) * (pic.Height - 1);
                    double[] ret = getAllValue(pic, list, listupdateCount, chn, L_CursorPoint, R_CursorPoint);
                    //return new double[] { minVal, maxVal, AvgVal, LVal, RVal };
                    string str = fVal.ToString(formatStr[chn]) + "(" + value.ToString("f0") + ")";

                    if (checkSursor.Checked == true)
                        graphics.DrawString(str, new Font("宋体", fontSize), new SolidBrush(listComboBox[i].ForeColor), x, y);

                    Pen R_LinePen = new Pen(listComboBox[i].ForeColor, 1);//new Pen(new SolidBrush(SliceColor))
                    if (R_CursorPoint % ImageMultiples == 0)
                        R_LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                    if (checkShuiping.Checked == true)
                        graphics.DrawLine(R_LinePen, new PointF(0, y), new PointF(pic.Width - 1, y));
                }
            }
        }

        private void DrawCursorLine(PictureBox pic, Graphics graphics, List<DataFrame> list, int listupdateCount)
        {
            Pen L_LinePen = new Pen(L_CursorColor, 1);//new Pen(new SolidBrush(SliceColor))
            if (L_CursorPoint % ImageMultiples == 0)
                L_LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            else
                L_LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;

            Pen R_LinePen = new Pen(R_CursorColor, 1);//new Pen(new SolidBrush(SliceColor))
            if (R_CursorPoint % ImageMultiples == 0)
                R_LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            else
                R_LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;

            //垂直线
            graphics.DrawLine(L_LinePen, new PointF(L_CursorPoint, pic.Height - 1), new PointF(L_CursorPoint, 0));
            graphics.DrawLine(R_LinePen, new PointF(R_CursorPoint, pic.Height - 1), new PointF(R_CursorPoint, 0));
        }

        private void DrawDatalistInPicturebox(Graphics graphics, PictureBox pic, List<DataFrame> datalist, int multiples, float tension, int listupdateCount)
        {
            if (datalist.Count == 0) return;
            updatePara();
            Pen CurvePen;
            int start = 0;
            if (listupdateCount > pic.Width / multiples) start = listupdateCount - pic.Width / multiples;

            PointF[] pf;

            foreach (ComboBox comboBox in listComboBox)
            {
                if (comboBox.SelectedIndex < Curve_Num)
                {
                    CurvePen = new Pen(comboBox.ForeColor, 1);
                    pf = GetCurPoinf(datalist, start, comboBox.SelectedIndex, pic, multiples, listupdateCount);
                    if (pf != null) graphics.DrawCurve(CurvePen, pf, tension);
                }
            }

            drawValueAtCursor(L_CursorPoint, pic, graphics, datalist, listupdateCount);//在曲线上显示值
            drawValueAtCursor(R_CursorPoint, pic, graphics, datalist, listupdateCount);//在曲线上显示值

            DrawCursorLine(pic, graphics, datalist, listupdateCount);//画左右标尺线 显示采样值
        }

        private PointF[] GetCurPoinf(List<DataFrame> datalist, int start, int chn, PictureBox img, int multiples, int listupdateCount)
        {
            if (start > listupdateCount) return null;

            int total_points = listupdateCount - start;
            if (total_points < 2) return null;

            int max_img_points = img.Width / multiples;

            int len;
            if (total_points > max_img_points) len = max_img_points;
            else len = total_points;

            PointF[] DrawPointF = new PointF[len];

            for (int i = 0; i < DrawPointF.Length; i++)
            {
                float curr_data = datalist[i + start].data[chn];
                float y = (img.Height - 1) - (curr_data - YSBegin[chn]) / (YSEnd[chn] - YSBegin[chn]) * (img.Height - 1);
                float x = multiples * i;
                DrawPointF[i] = new PointF(x, y);
            }
            return DrawPointF;
        }

        private string GetFileFullNameByTime(string path, byte id)
        {
            DateTime d1 = DateTime.Now;
            string s1 = d1.Year.ToString("d4") + d1.Month.ToString("d2") + d1.Day.ToString("d2");
            string s2 = d1.Hour.ToString("d2") + d1.Minute.ToString("d2") + d1.Second.ToString("d2");
            string s3 = d1.Millisecond.ToString("d3");
            string name = s1 + "-" + s2 + "-" + s3 + "." + FILE_EXTENSION_NAME;
            return path + "\\" + id.ToString("X2") + "-" + name; ;
        }

        public void UpdateEvent(ushort[] buffer, byte StationID)
        {
            if (boardID == StationID)
            {
                if (RecievedDataList.Count == 0) DataListFileName = GetFileFullNameByTime(crvpath, boardID);
                RecievedDataList.Add(new DataFrame(boardID, buffer));
                if (checkPause.Checked == false)
                {
                    RecievedDataCount = RecievedDataList.Count;
                    DataSelect = LIVE_DATA;
                    RefrushCurveAndInfo();
                    this.Text = "接收数据";
                }
            }
        }

        private int bit(UInt16 x, UInt16 n)
        {
            if ((x & (1 << n)) == 0)
                return 0;
            else return 1;
        }

        public string get_FaStatus(UInt16 FaZhuangTai, UInt16 num)
        {
            string fastr = "";
            for (UInt16 i = 0; i < num; i++)
            {
                if (bit(FaZhuangTai, i) == 1) fastr += "1 ";
                else fastr += "0 ";
            }

            return fastr;

            //if (bit(FaZhuangTai, 2) == 1) fastr += "1 ";
            //else fastr += "0 ";
            //if (bit(FaZhuangTai, 3) == 1) fastr += "1 ";
            //else fastr += "0 ";
            //if (bit(FaZhuangTai, 4) == 1) fastr += "1 ";
            //else fastr += "0 ";
            //if (bit(FaZhuangTai, 5) == 1) fastr += "1 ";
            //else fastr += "0 ";
            //if (bit(FaZhuangTai, 6) == 1) fastr += "1 ";
            //else fastr += "0 ";
            //if (bit(FaZhuangTai, 7) == 1) fastr += "1 ";
            //else fastr += "0 ";
            //return fastr;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefrushCurveAndInfo();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                L_CursorPoint = e.X;
                RefrushCurveAndInfo();
            }
            if (e.Button == MouseButtons.Right)
            {
                R_CursorPoint = e.X;
                RefrushCurveAndInfo();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //if (e.X % ImageMultiples == 0)
                if (e.X >= 0 && e.X < ((PictureBox)sender).Width)
                {
                    L_CursorPoint = e.X;
                    RefrushCurveAndInfo();
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                //if (e.X % ImageMultiples == 0)
                if (e.X >= 0 && e.X < ((PictureBox)sender).Width)
                {
                    R_CursorPoint = e.X;
                    RefrushCurveAndInfo();
                }
            }
        }

        public void saveCurvData()
        {
            if (chkAutoSave.Checked == false) return;
            if (RecievedDataList.Count < 2) return;
            SerializeItemList(DataListFileName, RecievedDataList);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveCurvData();
            if (RecievedDataList.Count < 2) return;
            SerializeItemList(DataListFileName, RecievedDataList);
            fill_filename(crvpath);

            //SaveFileDialog sf = new SaveFileDialog();
            //string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //path = path.Substring(0, path.LastIndexOf('\\'));
            //sf.InitialDirectory = path;

            ////设置文件的类型
            //sf.Filter = "曲线文件|*.crv|全部文件|*.*";
            ////如果用户点击了打开按钮、选择了正确的路径则进行如下操作：
            //if (sf.ShowDialog() == DialogResult.OK)
            //{
            //    //MessageBox.Show(sf.FileName);
            //    //实例化一个文件流--->与写入文件相关联
            //    //FileStream fs = new FileStream(sf.FileName, FileMode.Create);
            //    SaveCurve(sf.FileName);

            //}
        }

        private void fill_filename(string path)
        {
            UInt16 id = boardID;
            String sid = id.ToString("X2") + "-";

            DirectoryInfo TheFolder = new DirectoryInfo(path);
            //遍历文件夹
            // foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            //   this.listBox2.Items.Add(NextFolder.Name);
            //遍历文件
            listBox1.Items.Clear();
            foreach (FileInfo NextFile in TheFolder.GetFiles(sid + "*." + FILE_EXTENSION_NAME))
                this.listBox1.Items.Add(NextFile.Name);

            //if (listBox1.Items.Count > 0) listBox1.SelectedIndex = listBox1.Items.Count - 1;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //int i,j,k;

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            // path = new DirectoryInfo(crvpath);
            dialog.SelectedPath = crvpath;

            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                crvpath = foldPath;
                fill_filename(crvpath);
                //MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (listBox1.Items.Count > 0) listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }

            return;
            /*
                        string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        path = path.Substring(0, path.LastIndexOf('\\'));

                        OpenFileDialog op = new OpenFileDialog();
                        op.InitialDirectory = path;

                        DirectoryInfo TheFolder = new DirectoryInfo(path);
                        //遍历文件夹
                       // foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                         //   this.listBox2.Items.Add(NextFolder.Name);
                        //遍历文件
                        listBox1.Items.Clear();
                        foreach (FileInfo NextFile in TheFolder.GetFiles("*.crv"))
                            this.listBox1.Items.Add(NextFile.Name);

                        listBox1.SelectedIndex = file_selected_index;
            */
            // return;
            /*
                        //设置文件的类型
                        op.Filter = "曲线文件|*.crv|全部文件|*.*";
                        //如果用户点击了打开按钮、选择了正确的路径则进行如下操作：
                        if (op.ShowDialog() == DialogResult.OK)
                        {
                            open_crv(op.FileName);

                        }
            */

        }

        private void CurveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Visible == true)
            {
                e.Cancel = true;
                //this.WindowState = FormWindowState.Minimized;
                Hide();//隐藏，当然您也可以采用其他的自定义操作哦
            }

        }
        public void SerializeItemList(string fileName, List<DataFrame> list)
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream s = new FileStream(fileName, FileMode.Create);
            formatter.Serialize(s, list);
            s.Close();
        }


        public List<DataFrame> DeserializeItemList(string fileName)//,out List<DataFrame> list)
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream s = new FileStream(fileName, FileMode.Open);
            List<DataFrame> list = (List<DataFrame>)formatter.Deserialize(s);
            s.Close();
            return list;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //path = path.Substring(0, path.LastIndexOf('\\'));
            if (listBox1.SelectedIndex < 0) return;

            int file_selected_index = listBox1.SelectedIndex;
            String fileName = crvpath + '\\' + listBox1.SelectedItem.ToString();

            try
            {
                ((CurveForm)listBox1.Parent).Text = "正在读入数据文件......";
                FileDataList.Clear();
                FileDataCount = 0;
                //FileDataList.TrimExcess();
                GC.Collect();//回收内存

                FileDataList =DeserializeItemList(fileName);
                FileDataCount = FileDataList.Count;

                DataSelect = FILE_DATA;
                //FileDataList.TrimExcess();

                RefrushCurveAndInfo();
                ((CurveForm)listBox1.Parent).Text = fileName.ToString() + "        在下面显示曲线的区域右键双击，显示实时接收的数据";

            }
            catch (Exception)
            {
                if (DataSelect == FILE_DATA || DataSelect == NULL_DATA)
                {
                    DataSelect = NULL_DATA;
                    ((CurveForm)listBox1.Parent).Text = "无法打开文件" + "        在下面显示曲线的区域右键双击，显示实时接收的数据";
                }
                RefrushCurveAndInfo();
                //throw;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            switch (DataSelect)
            {
                case NULL_DATA:
                    break;
                case LIVE_DATA:
                    DrawDatalistInPicturebox(e.Graphics, (PictureBox)sender, RecievedDataList, ImageMultiples, Tension, RecievedDataCount);
                    break;
                case FILE_DATA:
                    DrawDatalistInPicturebox(e.Graphics, (PictureBox)sender, FileDataList, ImageMultiples, Tension, FileDataCount);
                    break;
                default:
                    break;
            }

        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            switch (DataSelect)
            {
                case NULL_DATA:
                    break;
                case LIVE_DATA:
                    DrawSampleValue(e.Graphics, (PictureBox)sender, RecievedDataList, ImageMultiples, Tension, RecievedDataCount);
                    break;
                case FILE_DATA:
                    DrawSampleValue(e.Graphics, (PictureBox)sender, FileDataList, ImageMultiples, Tension, FileDataCount);
                    break;
                default:
                    break;
            }

        }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            switch (DataSelect)
            {
                case NULL_DATA:
                    break;
                case LIVE_DATA:
                    DrawDebugValue(e.Graphics, (PictureBox)sender, RecievedDataList, ImageMultiples, Tension, RecievedDataCount);
                    break;
                case FILE_DATA:
                    DrawDebugValue(e.Graphics, (PictureBox)sender, FileDataList, ImageMultiples, Tension, FileDataCount);
                    break;
                default:
                    break;
            }

        }

        private void pictureBox4_Paint(object sender, PaintEventArgs e)
        {
            switch (DataSelect)
            {
                case NULL_DATA:
                    break;
                case LIVE_DATA:
                    DrawCursorValue(e.Graphics, (PictureBox)sender, RecievedDataList, ImageMultiples, Tension, RecievedDataCount);
                    break;
                case FILE_DATA:
                    DrawCursorValue(e.Graphics, (PictureBox)sender, FileDataList, ImageMultiples, Tension, FileDataCount);
                    break;
                default:
                    break;
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            //int t = 0;//
            //if (((MouseEventArgs)e).Button == MouseButtons.Left)
            //{
            //    t = 1;
            //}
            if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                if (RecievedDataCount > 1 && DataSelect != LIVE_DATA)
                {
                    DataSelect = LIVE_DATA;
                    RefrushCurveAndInfo();
                    this.Text = "接收数据";
                }
            }
        }
        /*
       private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
       {
       switch (e.CloseReason)
       {
       //应用程序要求关闭窗口
       case CloseReason.ApplicationExitCall:
       e.Cancel = false; //不拦截，响应操作
       break;
       //自身窗口上的关闭按钮
       case CloseReason.FormOwnerClosing:
       e.Cancel = true;//拦截，不响应操作
       break;
       //MDI窗体关闭事件
       case CloseReason.MdiFormClosing:
       e.Cancel = true;//拦截，不响应操作
       break;
       //不明原因的关闭
       case CloseReason.None:
       break;
       //任务管理器关闭进程
       case CloseReason.TaskManagerClosing:
       e.Cancel = false;//不拦截，响应操作
       break;
       //用户通过UI关闭窗口或者通过Alt+F4关闭窗口
       case CloseReason.UserClosing:
       e.Cancel = true;//拦截，不响应操作
       break;
       //操作系统准备关机
       case CloseReason.WindowsShutDown:
       e.Cancel = false;//不拦截，响应操作
       break;
       default:
       break;
       }

       //if(e.Cancel == false)
       // base.OnFormClosing(e); 
       }

       */
    }
}
