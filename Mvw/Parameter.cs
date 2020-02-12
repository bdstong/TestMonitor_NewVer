using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DVW

{
    public partial class Parameter : Form
    {
        SendDtatDelegate SendDele;
        Form1 fm1;

        public Parameter(Form1 frm)
        {
            InitializeComponent();
            this.fm1 = frm;
        }

        public Parameter(Form1 frm, SendDtatDelegate delegete)
        {
            InitializeComponent();
            this.SendDele = delegete;
            this.fm1 = frm;
        }
        private UInt16[] Parameters = new UInt16[140];//参数140个
        private float max_voltage = 3.3f;
        private UInt16 max_range=4095;

//        private UInt16 StationAddr;
        const byte SAMPLE_DATA = 1;
        const byte PARAMETER_DATA = 2;
        const byte ACK_DATA = 3;

        const byte I2C_WRIIE_OK = 0;
        const byte I2C_WRITE_ERROR = 1;
        const byte I2C_NO_ANSWER = 2;
        const byte CAN_NO_ANSWER = 3;

        const byte SUBSTATION_PARAMETER = 3;
        const byte SUBSTATION_ALLINFO = 4;
        const byte SUBSTATION_PARAMETER_WRITE = 5;
        const byte SUBSTATION_KOUT_CMD = 6;
        const byte SUBSTATION_VOUT_CMD = 7;

        const byte SUBSTATION_DJ_CMD = 8;

        //public void UpdateEvent(ushort[] buffer,int StationID)
        //{
        //    //throw new NotImplementedException();
        //    StationAddr = buffer[0];
        //    switch (buffer[1])
        //    {

        //        case PARAMETER_DATA://接收到参数
        //            ack_label.ForeColor = Color.Black;
        //            ack_label.Text = "读取参数 OK";
        //            ParameterToTextBox(buffer, 2);
        //            labelStationNum.Text = "编号:" + StationAddr.ToString();
        //            break;
        //        case ACK_DATA:
        //            int a = buffer[2];
        //            if (a == I2C_WRIIE_OK)
        //            {
        //                ack_label.ForeColor = Color.Green;
        //                ack_label.Text = "参数修改 OK";
        //            }
        //            if (a == I2C_WRITE_ERROR)
        //            {
        //                ack_label.ForeColor = Color.Red;
        //                ack_label.Text = "参数写入错误！！！";
        //            }
        //            if (a == I2C_NO_ANSWER)
        //            {
        //                ack_label.ForeColor = Color.Red;
        //                ack_label.Text = "无反应";
        //            }
        //            break;

        //        //    case WRITE_SUCESSED://参数写入成功
        //        //        
        //        //        
        //        //        break;
        //        //    case WRITE_FAILED:
        //        //        ack_label.ForeColor = Color.Red;
        //        //        ack_label.Text = "参数写入错误！！！";
        //        //        break;
        //        //    case CAN_NO_ANSWER:
        //        //        ack_label.ForeColor = Color.Red;
        //        //        ack_label.Text = "远端无应答";
        //        //        break;

        //        //    case 4://恢复缺省参数
        //        //        ack_label.ForeColor = Color.Green;
        //        //        ack_label.Text = "恢复缺省参数 OK";
        //        //        break;

        //        //    case 5:
        //        //        ack_label.ForeColor = Color.Blue;
        //        //        ack_label.Text = "正在进行中........";
        //        //        break;
        //        //    case 7:
        //        //        ack_label.ForeColor = Color.Red;
        //        //        ack_label.Text = "数据校验和错误！！！";
        //        //        break;
        //    }
        //}
        private String Int16ToDyText(ushort ust)
        {
            float r = (float)ust * max_voltage / max_range;
            return r.ToString("f03");
        }
        private void ParameterToTextBox(UInt16[] DataBuf, int offset)//参数转换成文本框显示
        {
            int i = offset;
            Parameter0.Text = DataBuf[i++].ToString();
            Parameter1.Text = DataBuf[i++].ToString();
            Parameter2.Text = DataBuf[i++].ToString();
            Parameter3.Text = DataBuf[i++].ToString();
            Parameter4.Text = DataBuf[i++].ToString();
            Parameter5.Text = DataBuf[i++].ToString();
            Parameter6.Text = DataBuf[i++].ToString();
            Parameter7.Text = DataBuf[i++].ToString();
            Parameter8.Text = DataBuf[i++].ToString();
            Parameter9.Text = DataBuf[i++].ToString();
            Parameter10.Text = DataBuf[i++].ToString();
            Parameter11.Text = DataBuf[i++].ToString();
            Parameter12.Text = DataBuf[i++].ToString();
            Parameter13.Text = DataBuf[i++].ToString();
            Parameter14.Text = DataBuf[i++].ToString();
            Parameter15.Text = DataBuf[i++].ToString();

            Parameter16.Text = DataBuf[i++].ToString();
            Parameter17.Text = DataBuf[i++].ToString();
            Parameter18.Text = DataBuf[i++].ToString();
            Parameter19.Text = DataBuf[i++].ToString();
            Parameter20.Text = DataBuf[i++].ToString();
            Parameter21.Text = DataBuf[i++].ToString();
            Parameter22.Text = DataBuf[i++].ToString();
            Parameter23.Text = DataBuf[i++].ToString();
            Parameter24.Text = DataBuf[i++].ToString();
            Parameter25.Text = DataBuf[i++].ToString();
            Parameter26.Text = DataBuf[i++].ToString();
            Parameter27.Text = DataBuf[i++].ToString();
            Parameter28.Text = DataBuf[i++].ToString();
            Parameter29.Text = DataBuf[i++].ToString();
            Parameter30.Text = DataBuf[i++].ToString();
            Parameter31.Text = DataBuf[i++].ToString();

            Parameter32.Text = DataBuf[i++].ToString();
            Parameter33.Text = DataBuf[i++].ToString();
            Parameter34.Text = DataBuf[i++].ToString();
            Parameter35.Text = DataBuf[i++].ToString();
            Parameter36.Text = DataBuf[i++].ToString();
            Parameter37.Text = DataBuf[i++].ToString();
            Parameter38.Text = DataBuf[i++].ToString();
            Parameter39.Text = DataBuf[i++].ToString();
            Parameter40.Text = DataBuf[i++].ToString();
            Parameter41.Text = DataBuf[i++].ToString();
            Parameter42.Text = DataBuf[i++].ToString();
            Parameter43.Text = DataBuf[i++].ToString();
            Parameter44.Text = DataBuf[i++].ToString();
            Parameter45.Text = DataBuf[i++].ToString();
            Parameter46.Text = DataBuf[i++].ToString();
            Parameter47.Text = DataBuf[i++].ToString();

            //----------------------------------------------------------------
            Parameter48.Text = DataBuf[i++].ToString();
            Parameter49.Text = DataBuf[i++].ToString();
            Parameter50.Text = DataBuf[i++].ToString();
            Parameter51.Text = DataBuf[i++].ToString();
            Parameter52.Text = DataBuf[i++].ToString();
            Parameter53.Text = DataBuf[i++].ToString();
            Parameter54.Text = DataBuf[i++].ToString();
            Parameter55.Text = DataBuf[i++].ToString();

            Parameter56.Text = Int16ToDyText(DataBuf[i++]);
            Parameter57.Text = Int16ToDyText(DataBuf[i++]);
            Parameter58.Text = Int16ToDyText(DataBuf[i++]);
            Parameter59.Text = Int16ToDyText(DataBuf[i++]);
            Parameter60.Text = Int16ToDyText(DataBuf[i++]);
            Parameter61.Text = Int16ToDyText(DataBuf[i++]);
            Parameter62.Text = Int16ToDyText(DataBuf[i++]);
            Parameter63.Text = Int16ToDyText(DataBuf[i++]);

            Parameter64.Text = DataBuf[i++].ToString();
            Parameter65.Text = DataBuf[i++].ToString();
            Parameter66.Text = DataBuf[i++].ToString();
            Parameter67.Text = DataBuf[i++].ToString();
            Parameter68.Text = DataBuf[i++].ToString();
            Parameter69.Text = DataBuf[i++].ToString();
            //----------------------------------------------------------------

            Parameter70.Text = DataBuf[i++].ToString();
            Parameter71.Text = DataBuf[i++].ToString();
            Parameter72.Text = DataBuf[i++].ToString();
            Parameter73.Text = DataBuf[i++].ToString();
            Parameter74.Text = DataBuf[i++].ToString();
            Parameter75.Text = DataBuf[i++].ToString();
            Parameter76.Text = DataBuf[i++].ToString();
            Parameter77.Text = DataBuf[i++].ToString();
            Parameter78.Text = DataBuf[i++].ToString();
            Parameter79.Text = DataBuf[i++].ToString();
            Parameter80.Text = DataBuf[i++].ToString();
            Parameter81.Text = DataBuf[i++].ToString();
            Parameter82.Text = DataBuf[i++].ToString();
            Parameter83.Text = DataBuf[i++].ToString();
            Parameter84.Text = DataBuf[i++].ToString();
            Parameter85.Text = DataBuf[i++].ToString();
            Parameter86.Text = DataBuf[i++].ToString();
            Parameter87.Text = DataBuf[i++].ToString();
            Parameter88.Text = DataBuf[i++].ToString();
            Parameter89.Text = DataBuf[i++].ToString();
            Parameter90.Text = DataBuf[i++].ToString();
            Parameter91.Text = DataBuf[i++].ToString();
            Parameter92.Text = DataBuf[i++].ToString();
            Parameter93.Text = DataBuf[i++].ToString();
            Parameter94.Text = DataBuf[i++].ToString();
            Parameter95.Text = DataBuf[i++].ToString();
            Parameter96.Text = DataBuf[i++].ToString();
            Parameter97.Text = DataBuf[i++].ToString();
            Parameter98.Text = DataBuf[i++].ToString();
            Parameter99.Text = DataBuf[i++].ToString();
            Parameter100.Text = DataBuf[i++].ToString();
            Parameter101.Text = DataBuf[i++].ToString();
            Parameter102.Text = DataBuf[i++].ToString();
            Parameter103.Text = DataBuf[i++].ToString();
            Parameter104.Text = DataBuf[i++].ToString();
            Parameter105.Text = DataBuf[i++].ToString();
            Parameter106.Text = DataBuf[i++].ToString();
            Parameter107.Text = DataBuf[i++].ToString();
            Parameter108.Text = DataBuf[i++].ToString();
            Parameter109.Text = DataBuf[i++].ToString();
            Parameter110.Text = DataBuf[i++].ToString();
            Parameter111.Text = DataBuf[i++].ToString();
            Parameter112.Text = DataBuf[i++].ToString();
            Parameter113.Text = DataBuf[i++].ToString();

            Parameter114.Text = DataBuf[i++].ToString();
            Parameter115.Text = DataBuf[i++].ToString();

            Parameter116.Text = DataBuf[i++].ToString();
            Parameter117.Text = DataBuf[i++].ToString();
            Parameter118.Text = DataBuf[i++].ToString();
            Parameter119.Text = DataBuf[i++].ToString();

            Parameter120.Text = DataBuf[i++].ToString();

            Parameter121.Text = DataBuf[i++].ToString();
            Parameter122.Text = DataBuf[i++].ToString();
            Parameter123.Text = DataBuf[i++].ToString();
            Parameter124.Text = DataBuf[i++].ToString();

            Parameter125.Text = DataBuf[i++].ToString();
            Parameter126.Text = DataBuf[i++].ToString();
            Parameter127.Text = DataBuf[i++].ToString();
            Parameter128.Text = DataBuf[i++].ToString();

            Parameter129.Text = DataBuf[i++].ToString();
            Parameter130.Text = DataBuf[i++].ToString();
            Parameter131.Text = DataBuf[i++].ToString();
            Parameter132.Text = DataBuf[i++].ToString();

            Parameter133.Text = DataBuf[i++].ToString();
            Parameter134.Text = DataBuf[i++].ToString();
            Parameter135.Text = DataBuf[i++].ToString();
            Parameter136.Text = DataBuf[i++].ToString();

            Parameter137.Text = DataBuf[i++].ToString();
            Parameter138.Text = DataBuf[i++].ToString();
            Parameter139.Text = DataBuf[i++].ToString();
        }
        private UInt16 DyTextToInt16(String str)
        {
            UInt16 res = (UInt16)(double.Parse(str) / max_voltage * max_range);
            return res;
        }
        private int TextBoxToParameter(UInt16[] DataBuf)
        {
            int i = 0;
            try
            {
                DataBuf[i++] = UInt16.Parse(Parameter0.Text);
                DataBuf[i++] = UInt16.Parse(Parameter1.Text);
                DataBuf[i++] = UInt16.Parse(Parameter2.Text);
                DataBuf[i++] = UInt16.Parse(Parameter3.Text);
                DataBuf[i++] = UInt16.Parse(Parameter4.Text);
                DataBuf[i++] = UInt16.Parse(Parameter5.Text);
                DataBuf[i++] = UInt16.Parse(Parameter6.Text);
                DataBuf[i++] = UInt16.Parse(Parameter7.Text);
                DataBuf[i++] = UInt16.Parse(Parameter8.Text);
                DataBuf[i++] = UInt16.Parse(Parameter9.Text);
                DataBuf[i++] = UInt16.Parse(Parameter10.Text);
                DataBuf[i++] = UInt16.Parse(Parameter11.Text);
                DataBuf[i++] = UInt16.Parse(Parameter12.Text);
                DataBuf[i++] = UInt16.Parse(Parameter13.Text);
                DataBuf[i++] = UInt16.Parse(Parameter14.Text);
                DataBuf[i++] = UInt16.Parse(Parameter15.Text);

                DataBuf[i++] = UInt16.Parse(Parameter16.Text);
                DataBuf[i++] = UInt16.Parse(Parameter17.Text);
                DataBuf[i++] = UInt16.Parse(Parameter18.Text);
                DataBuf[i++] = UInt16.Parse(Parameter19.Text);
                DataBuf[i++] = UInt16.Parse(Parameter20.Text);
                DataBuf[i++] = UInt16.Parse(Parameter21.Text);
                DataBuf[i++] = UInt16.Parse(Parameter22.Text);
                DataBuf[i++] = UInt16.Parse(Parameter23.Text);
                DataBuf[i++] = UInt16.Parse(Parameter24.Text);
                DataBuf[i++] = UInt16.Parse(Parameter25.Text);
                DataBuf[i++] = UInt16.Parse(Parameter26.Text);
                DataBuf[i++] = UInt16.Parse(Parameter27.Text);
                DataBuf[i++] = UInt16.Parse(Parameter28.Text);
                DataBuf[i++] = UInt16.Parse(Parameter29.Text);
                DataBuf[i++] = UInt16.Parse(Parameter30.Text);
                DataBuf[i++] = UInt16.Parse(Parameter31.Text);

                DataBuf[i++] = UInt16.Parse(Parameter32.Text);
                DataBuf[i++] = UInt16.Parse(Parameter33.Text);
                DataBuf[i++] = UInt16.Parse(Parameter34.Text);
                DataBuf[i++] = UInt16.Parse(Parameter35.Text);
                DataBuf[i++] = UInt16.Parse(Parameter36.Text);
                DataBuf[i++] = UInt16.Parse(Parameter37.Text);
                DataBuf[i++] = UInt16.Parse(Parameter38.Text);
                DataBuf[i++] = UInt16.Parse(Parameter39.Text);
                DataBuf[i++] = UInt16.Parse(Parameter40.Text);
                DataBuf[i++] = UInt16.Parse(Parameter41.Text);
                DataBuf[i++] = UInt16.Parse(Parameter42.Text);
                DataBuf[i++] = UInt16.Parse(Parameter43.Text);
                DataBuf[i++] = UInt16.Parse(Parameter44.Text);
                DataBuf[i++] = UInt16.Parse(Parameter45.Text);
                DataBuf[i++] = UInt16.Parse(Parameter46.Text);
                DataBuf[i++] = UInt16.Parse(Parameter47.Text);
                //---------------------------------------------------------------                

                DataBuf[i++] = UInt16.Parse(Parameter48.Text);
                DataBuf[i++] = UInt16.Parse(Parameter49.Text);
                DataBuf[i++] = UInt16.Parse(Parameter50.Text);
                DataBuf[i++] = UInt16.Parse(Parameter51.Text);
                DataBuf[i++] = UInt16.Parse(Parameter52.Text);
                DataBuf[i++] = UInt16.Parse(Parameter53.Text);
                DataBuf[i++] = UInt16.Parse(Parameter54.Text);
                DataBuf[i++] = UInt16.Parse(Parameter55.Text);

                DataBuf[i++] = DyTextToInt16(Parameter56.Text);
                DataBuf[i++] = DyTextToInt16(Parameter57.Text);
                DataBuf[i++] = DyTextToInt16(Parameter58.Text);
                DataBuf[i++] = DyTextToInt16(Parameter59.Text);
                DataBuf[i++] = DyTextToInt16(Parameter60.Text);
                DataBuf[i++] = DyTextToInt16(Parameter61.Text);
                DataBuf[i++] = DyTextToInt16(Parameter62.Text);
                DataBuf[i++] = DyTextToInt16(Parameter63.Text);

                DataBuf[i++] = UInt16.Parse(Parameter64.Text);
                DataBuf[i++] = UInt16.Parse(Parameter65.Text);
                DataBuf[i++] = UInt16.Parse(Parameter66.Text);
                DataBuf[i++] = UInt16.Parse(Parameter67.Text);
                DataBuf[i++] = UInt16.Parse(Parameter68.Text);
                DataBuf[i++] = UInt16.Parse(Parameter69.Text);
 
                //----------------------------------------------------
                
                //////////////////////////////////////////////////////////
                
                
                DataBuf[i++] = UInt16.Parse(Parameter70.Text);
                DataBuf[i++] = UInt16.Parse(Parameter71.Text);
                DataBuf[i++] = UInt16.Parse(Parameter72.Text);
                DataBuf[i++] = UInt16.Parse(Parameter73.Text);
                DataBuf[i++] = UInt16.Parse(Parameter74.Text);
                DataBuf[i++] = UInt16.Parse(Parameter75.Text);
                DataBuf[i++] = UInt16.Parse(Parameter76.Text);
                DataBuf[i++] = UInt16.Parse(Parameter77.Text);
                DataBuf[i++] = UInt16.Parse(Parameter78.Text);
                DataBuf[i++] = UInt16.Parse(Parameter79.Text);
                DataBuf[i++] = UInt16.Parse(Parameter80.Text);
                DataBuf[i++] = UInt16.Parse(Parameter81.Text);
                DataBuf[i++] = UInt16.Parse(Parameter82.Text);
                DataBuf[i++] = UInt16.Parse(Parameter83.Text);
                DataBuf[i++] = UInt16.Parse(Parameter84.Text);
                DataBuf[i++] = UInt16.Parse(Parameter85.Text);
                DataBuf[i++] = UInt16.Parse(Parameter86.Text);
                DataBuf[i++] = UInt16.Parse(Parameter87.Text);
                DataBuf[i++] = UInt16.Parse(Parameter88.Text);
                DataBuf[i++] = UInt16.Parse(Parameter89.Text);
                DataBuf[i++] = UInt16.Parse(Parameter90.Text);
                DataBuf[i++] = UInt16.Parse(Parameter91.Text);
                DataBuf[i++] = UInt16.Parse(Parameter92.Text);
                DataBuf[i++] = UInt16.Parse(Parameter93.Text);
                DataBuf[i++] = UInt16.Parse(Parameter94.Text);
                DataBuf[i++] = UInt16.Parse(Parameter95.Text);
                DataBuf[i++] = UInt16.Parse(Parameter96.Text);
                DataBuf[i++] = UInt16.Parse(Parameter97.Text);
                DataBuf[i++] = UInt16.Parse(Parameter98.Text);
                DataBuf[i++] = UInt16.Parse(Parameter99.Text);

                DataBuf[i++] = UInt16.Parse(Parameter100.Text);

                DataBuf[i++] = UInt16.Parse(Parameter101.Text);
                DataBuf[i++] = UInt16.Parse(Parameter102.Text);
                DataBuf[i++] = UInt16.Parse(Parameter103.Text);
                DataBuf[i++] = UInt16.Parse(Parameter104.Text);
                DataBuf[i++] = UInt16.Parse(Parameter105.Text);
                DataBuf[i++] = UInt16.Parse(Parameter106.Text);
                DataBuf[i++] = UInt16.Parse(Parameter107.Text);
                DataBuf[i++] = UInt16.Parse(Parameter108.Text);

                DataBuf[i++] = UInt16.Parse(Parameter109.Text);
                DataBuf[i++] = UInt16.Parse(Parameter110.Text);

                DataBuf[i++] = UInt16.Parse(Parameter111.Text);
                DataBuf[i++] = UInt16.Parse(Parameter112.Text);
                DataBuf[i++] = UInt16.Parse(Parameter113.Text);

                DataBuf[i++] = UInt16.Parse(Parameter114.Text);
                DataBuf[i++] = UInt16.Parse(Parameter115.Text);

                DataBuf[i++] = UInt16.Parse(Parameter116.Text);
                DataBuf[i++] = UInt16.Parse(Parameter117.Text);
                DataBuf[i++] = UInt16.Parse(Parameter118.Text);
                DataBuf[i++] = UInt16.Parse(Parameter119.Text);

                DataBuf[i++] = UInt16.Parse(Parameter120.Text);

                DataBuf[i++] = UInt16.Parse(Parameter121.Text);
                DataBuf[i++] = UInt16.Parse(Parameter122.Text);
                DataBuf[i++] = UInt16.Parse(Parameter123.Text);
                DataBuf[i++] = UInt16.Parse(Parameter124.Text);

                DataBuf[i++] = UInt16.Parse(Parameter125.Text);
                DataBuf[i++] = UInt16.Parse(Parameter126.Text);
                DataBuf[i++] = UInt16.Parse(Parameter127.Text);
                DataBuf[i++] = UInt16.Parse(Parameter128.Text);

                DataBuf[i++] = UInt16.Parse(Parameter129.Text);
                DataBuf[i++] = UInt16.Parse(Parameter130.Text);
                DataBuf[i++] = UInt16.Parse(Parameter131.Text);
                DataBuf[i++] = UInt16.Parse(Parameter132.Text);

                DataBuf[i++] = UInt16.Parse(Parameter133.Text);
                DataBuf[i++] = UInt16.Parse(Parameter134.Text);
                DataBuf[i++] = UInt16.Parse(Parameter135.Text);
                DataBuf[i++] = UInt16.Parse(Parameter136.Text);

                DataBuf[i++] = UInt16.Parse(Parameter137.Text);
                DataBuf[i++] = UInt16.Parse(Parameter138.Text);
                DataBuf[i++] = UInt16.Parse(Parameter139.Text);
            }
            catch (FormatException ee)
            {

                MessageBox.Show(ee.Message);
                return 1;
            }
            return 0;
        }


        private void button20_Click(object sender, EventArgs e)
        {
            int i;
            OpenFileDialog op = new OpenFileDialog();
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf('\\'));
            op.InitialDirectory = path;

            //设置文件的类型
            op.Filter = "参数文件|*.csd|全部文件|*.*";
            //如果用户点击了打开按钮、选择了正确的路径则进行如下操作：
            if (op.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(op.FileName);
                //实例化一个文件流--->与写入文件相关联
                FileStream fs = new FileStream(op.FileName, FileMode.Open);
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                if (fs.Length != (Parameters.Length * 2 + 4))
                {
                    result = MessageBox.Show("文件长度错误,是否继续", "警告", buttons);
                    if (result == System.Windows.Forms.DialogResult.No) return;
                }
                int length = (int)(fs.Length);
                //实例化BinaryReader
                BinaryReader rd = new BinaryReader(fs);
                byte[] paras = new byte[length];
                paras = rd.ReadBytes(length);
                int data_length = (length - 4) / 2 + 2;
                UInt16[] data = new UInt16[data_length];


                for (i = 0; i < (data.Length); i++)
                {
                    data[i] = (UInt16)(paras[i * 2] + paras[i * 2 + 1] * 256);
                }

                UInt16 sum = 0;

                for (i = 1; i < data.Length - 1; i++)
                {
                    sum += data[i];
                }

                if (!(data[0] == 0xa55a && sum == data[data.Length - 1]))
                {
                    result = MessageBox.Show("文件校验错误,是否继续", "警告", buttons);
                    if (result == System.Windows.Forms.DialogResult.No) return;
                }
                if (Parameters.Length < (data.Length - 1))
                {
                    for (i = 0; i < Parameters.Length; i++)
                        Parameters[i] = data[i + 1];
                }
                else
                {
                    for (i = 0; i < (data.Length - 1); i++)
                        Parameters[i] = data[i + 1];
                }
                //MessageBox.Show("文件读取完毕");
                ParameterToTextBox(Parameters, 0);

                //关闭流
                rd.Close();
                fs.Close();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!fm1.portIsOpen()) return;
            ushort[] paraBuf = new ushort[142];
            int i = 0;
            int j = 0;
            try
            {
                ack_label.Text = "";
                byte slaveID = Byte.Parse(AddrID.Text.Trim());
                ushort startAddress = fm1.ParameterRegister;

                ushort numofPoints = 100;
                ushort[] register = fm1.master.ReadHoldingRegisters(slaveID, startAddress, numofPoints);
                //this.Invoke(UpdataEventDele, register, slaveID);
                for (i = 0; i < 100; i++) paraBuf[j++] = register[i];
                
                numofPoints = 40;
                register = fm1.master.ReadHoldingRegisters(slaveID, (ushort)(startAddress+100), numofPoints);
                for (i = 0; i < 40; i++) paraBuf[j++] = register[i];
                ParameterToTextBox(paraBuf, 0);

                ack_label.Text = "Read Completed";

            }
            catch (Exception exception)
            {
                ack_label.Text = exception.Message;
            }

        }
        private int SunstationParameterToBuf(byte[] buf)
        {
            byte SubStationNum = byte.Parse(AddrID.Text);
            int i, j;
            //byte[] buf = new byte[Parameters.Length * 2 + 2];

            j = 0;
            buf[j++] = SubStationNum;
            buf[j++] = SUBSTATION_PARAMETER_WRITE;
            for (i = 0; i < Parameters.Length; i++)
            {
                buf[j++] = (byte)Parameters[i];
                buf[j++] = (byte)(Parameters[i] >> 8);
            }
            return j;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string message = "是否恢复缺省参数？";
            string caption = "提示";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.No)
            {

                return;

            }

            //send_one_byte_cmd(0x06);
            ack_label.Text = "";

        }
        void ModBusWrite(ushort[] paraBuf)
        {
            if (!fm1.portIsOpen()) return;
            try
            {
                byte slaveID = Byte.Parse(AddrID.Text.Trim());
                ushort startAddress = fm1.ParameterRegister;

                int j = 0;
                ushort[] tmp1 = new ushort[100];
                for (int i = 0; i < tmp1.Length; i++) tmp1[i] = paraBuf[j++];
                fm1.master.WriteMultipleRegisters(slaveID, startAddress, tmp1);

                //ushort[] tmp3 = new ushort[paraBuf.Length-100];
                //for (int i = 0; i < tmp3.Length; i++) tmp3[i] = paraBuf[j++];
                //fm1.master.WriteMultipleRegisters(slaveID, (ushort)(startAddress + 100), tmp3);

                ack_label.Text = "Write Completed";

            }
            catch (Exception exception)
            {
                ack_label.Text = exception.Message;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            int i = 0;
            string message = "是否写入参数 ？，请确认参数正确";
            string caption = "提示";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.No) return;
            if (TextBoxToParameter(Parameters) == 0)//转换成功
            {
                ack_label.Text = "";

                for (i = 0; i < Parameters.Length; i++)
                {
                    if (Parameters[i] > 65535)
                    {
                        MessageBox.Show("输入的数值超出了65535");
                        return;
                    }
                }
                ModBusWrite(Parameters);
                ////byte[] buf = new byte[300];
                //ack_label.Text = "";
                //byte[] buf = new byte[Parameters.Length * 2 + 2];
                //int Length = SunstationParameterToBuf(buf);

                //SendDele(buf, Length);


            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            int i;
            SaveFileDialog sf = new SaveFileDialog();
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf('\\'));
            sf.InitialDirectory = path;

            if (TextBoxToParameter(Parameters) > 0)//文本框转换成成参数 
            {
                return;
            }

            //设置文件的类型
            sf.Filter = "参数文件|*.csd|全部文件|*.*";
            //如果用户点击了打开按钮、选择了正确的路径则进行如下操作：
            if (sf.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(sf.FileName);
                //实例化一个文件流--->与写入文件相关联
                FileStream fs = new FileStream(sf.FileName, FileMode.Create);
                //实例化BinaryWriter
                BinaryWriter bw = new BinaryWriter(fs);

                byte[] data = new byte[Parameters.Length * 2 + 4];
                UInt16 sum = 0;
                for (i = 0; i < Parameters.Length; i++)
                {
                    sum += Parameters[i];
                }
                data[0] = (byte)(0xa55a & 0xff);
                data[1] = (byte)(0xa55a >> 8);

                for (i = 0; i < Parameters.Length; i++)
                {
                    data[2 + i * 2] = (byte)(Parameters[i]);
                    data[2 + i * 2 + 1] = (byte)(Parameters[i] >> 8);
                }

                data[2 + i * 2] = (byte)sum;
                data[2 + i * 2 + 1] = (byte)(sum >> 8);

                //开始写入
                fs.Write(data, 0, data.Length);

                //清空缓冲区
                bw.Flush();
                //关闭流
                bw.Close();
                fs.Close();
            }

        }

        private void Parameter_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;//默认为FALSE,关闭窗体。true为不关闭。
            //this.WindowState = FormWindowState.Minimized;//窗体最小化。
            Hide();  
        }

    }
}
