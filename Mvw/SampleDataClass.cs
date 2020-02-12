using System;
using System.Text;
using System.IO;
// Add references to Soap and Binary formatters.
using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization;
using System.Collections.Generic;


namespace MBusRead
{
    [Serializable]
    public class DataFrame : ISerializable
    {
        private ushort[] data_value;
        private DateTime dateTime_value;
        private byte id_value;
        
        public DataFrame()
        {
            // Empty constructor required to compile.
        }
        public DataFrame(byte id,ushort[] buffer)
        {
            this.id_value = id;
            this.dateTime_value = DateTime.Now;
            this.data_value = buffer;// (ushort[])buffer.Clone();
        }

        public DateTime dateTime
        {
            get { return dateTime_value; }
            set { dateTime_value = value; }
        }

        public byte boardID
        {
            get { return id_value; }
            set { id_value = value; }
        }

        public ushort[] data
        {
            get { return data_value; }
            set { data_value = value; }
        }

        //private string myProperty_value;
        //public string MyProperty
        //{
        //    get { return myProperty_value; }
        //    set { myProperty_value = value; }
        //}

        // Implement this method to serialize data. The method is called  on serialization.
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            info.AddValue("id", id_value, typeof(byte));
            info.AddValue("time", dateTime_value, typeof(DateTime));
            info.AddValue("data", data_value, typeof(ushort[]));
        }

        // The special constructor is used to deserialize values.
        public DataFrame(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            id_value = (byte)info.GetValue("id", typeof(byte));
            dateTime_value = (DateTime)info.GetValue("time", typeof(DateTime));
            data_value = (ushort[])info.GetValue("data", typeof(ushort[]));
        }
    }

    [Serializable]
    public class MyItemType : ISerializable
    {
        public MyItemType()
        {
            // Empty constructor required to compile.
        }

        // The value to serialize.
        private string myProperty_value;

        public string MyProperty
        {
            get { return myProperty_value; }
            set { myProperty_value = value; }
        }

        // Implement this method to serialize data. The method is called  on serialization.
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            info.AddValue("props", myProperty_value, typeof(string));

        }

        // The special constructor is used to deserialize values.
        public MyItemType(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            myProperty_value = (string)info.GetValue("props", typeof(string));
        }
    }
    [Serializable]
    public class NewItemType : MyItemType
    {
        public ushort[] data;

        private string myProperty_Newvalue;
        public string MyPropertyNew
        {
            get { return myProperty_Newvalue; }
            set { myProperty_Newvalue = value; }
        }

        public NewItemType(ushort[] nm)
            : base()
        {
            data = (ushort[])nm.Clone();
        }

        public NewItemType()
            : base()
        {
        }
        // Implement this method to serialize data. The method is called  on serialization.
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Use the AddValue method to specify serialized values.
            info.AddValue("props_new", myProperty_Newvalue, typeof(string));
            info.AddValue("data", data, typeof(ushort[]));

        }

        // The special constructor is used to deserialize values.
        public NewItemType(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Reset the property value using the GetValue method.
            myProperty_Newvalue = (string)info.GetValue("props_new", typeof(string));
            data = (ushort[])info.GetValue("data", typeof(ushort[]));
        }
        //public override void GetObjectData(SerializationInfo si, StreamingContext context)
        //{
        //    base.GetObjectData(si, context);
        //    si.AddValue("num", num);
        //}
        //protected NewItemType(SerializationInfo si, StreamingContext context) :
        //    base(si, context)
        //{
        //    num = si.Getushort32("num");
        //}
    }
    // This is a console application. 
    public class Test
    {

        static void Main1()
        {
            List<NewItemType> list = new List<NewItemType>();
            // This is the name of the file holding the data. You can use any file extension you like.
            ushort[] x1 = { 1, 2, 3 };
            ushort[] x2 = { 4, 5, 6 };

            NewItemType t1 = new NewItemType(x1);
            NewItemType t2 = new NewItemType(x2);
            t1.MyProperty = "Hello";
            t1.MyPropertyNew = "123";

            t2.MyProperty = "World";
            t2.MyPropertyNew = "456";

            list.Add(t1);
            list.Add(t2);

            string fileName = "d:\\dataStuff.myData";
            // Use a BinaryFormatter or SoapFormatter.
            IFormatter formatter = new BinaryFormatter();
            //IFormatter formatter = new SoapFormatter();
            Test.SerializeItem(fileName, formatter); // Serialize an instance of the class.
            Console.WriteLine("Deserialize");
            Test.DeserializeItem(fileName, formatter); // Deserialize the instance.
            Console.WriteLine("Done");

            string fileName1 = "d:\\datalist.myData";
            Test.SerializeItemList(fileName1, list); // Serialize an instance of the class.
            list.Clear();

            list = Test.DeserializeItemList(fileName1); // Deserialize the instance.

            Console.ReadLine();

        }

        public static void SerializeItem(string fileName, IFormatter formatter)
        {
            // Create an instance of the type and serialize it.
            MyItemType t = new MyItemType();
            t.MyProperty = "Hello World";

            FileStream s = new FileStream(fileName, FileMode.Create);
            formatter.Serialize(s, t);
            s.Close();
        }


        public static void DeserializeItem(string fileName, IFormatter formatter)
        {
            FileStream s = new FileStream(fileName, FileMode.Open);
            MyItemType t = (MyItemType)formatter.Deserialize(s);
            Console.WriteLine(t.MyProperty);
        }

        public static void SerializeItemList(string fileName, List<NewItemType> list)
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream s = new FileStream(fileName, FileMode.Create);
            formatter.Serialize(s, list);
            s.Close();
        }


        public static List<NewItemType> DeserializeItemList(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream s = new FileStream(fileName, FileMode.Open);
            List<NewItemType> list1 = (List<NewItemType>)formatter.Deserialize(s);
            return list1;
        }
    }
}
