using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DVW
{
    public class DataFIFO
    {
        private int BUFFER_LEN;
        private byte[] buffer;
        private int pReader, pWriter;

        //public int BytesToRead;

        public DataFIFO(int buf_size)
        {
            buffer = new byte[buf_size];
            BUFFER_LEN = buf_size;

            pReader = pWriter = 0;
        }
        public int Write(byte[] data, int offset, int length)
        {
            int i, pNextWriter;
            pNextWriter = (pWriter + 1) % BUFFER_LEN;
            if (pNextWriter == pReader) return 0;
            for (i = 0; i < length; i++)
            {
                buffer[pWriter] = data[i];
                pNextWriter = (pWriter + 1) % BUFFER_LEN;
                if (pNextWriter == pReader)
                {
                    i++;
                    break;
                }
                pWriter = pNextWriter;
            }
            return i;
        }
        private byte ReadOneByte()
        {
            byte by;
            while (pWriter == pReader)
            {
                Thread.Sleep(1);
            }
            by=buffer[pReader];
            pReader = (pReader + 1) % BUFFER_LEN;
            return by;
        }
        public void Read(byte[] data, int offset,int length)
        {
//            if(length+offset) >data.Length) return;
            for (int i = 0; i < length; i++)
            {
                data[i + offset] = ReadOneByte();
            }
        }
    }
}
