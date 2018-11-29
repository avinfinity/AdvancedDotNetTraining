using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPersistence
{
    public class Program
    {

        #region Deep copy using attributes
        [Serializable]
        public class A
        {
            private int X = 100;
            protected int y = 190;
            public int z = 20;
        }

        [Serializable]
        public class B : A
        {
            private int Xx = 100;
            protected int yy = 190;
            public int zz = 20;
        }

        [Serializable]
        public class C
        {
            B obj_Ref = new B();

            private int Xxx = 100;
            protected int yyy = 190;
            public int zzz = 20;
        }

        #endregion


        #region Shallow copy without any attribute

        public class A_NoAttribute
        {
            private int X = 100;
            protected int y = 190;
            public int z = 20;
        }

        public class B_NoAttribute : A_NoAttribute
        {
            private int Xx = 100;
            protected int yy = 190;
            public int zz = 20;
        }

        public class C_NoAttribute
        {
            B_NoAttribute obj_Ref = new B_NoAttribute();

            private int Xxx = 100;
            protected int yyy = 190;
            public int zzz = 20;
        }

        #endregion


        [Serializable]
        public class D : System.Runtime.Serialization.ISerializable
        {
            B obj_Ref = new B();

            private int Xxx = 100;
            protected int yyy = 190;
            public int zzz = 20;

            public D()
            {

            }

            public D(SerializationInfo info, StreamingContext context)
            {
                this.Xxx =  info.GetInt32("A_X");
                obj_Ref.z = info.GetInt32("B_Z");
                zzz = info.GetInt32("C_X");
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("A_X", this.Xxx);
                info.AddValue("B_Z", this.obj_Ref.zz);
                info.AddValue("C_X", this.zzz);
                info.AddValue("TimeStamp", DateTime.Now);
            }
        }

        static void Main(string[] args)
        {
            var cObject = new C();

            //Deep Copy Serialization

            //1. Binary formatting
            Serialize(cObject, "@../../MY_OBJ.bin", new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter());

            //2. SOAP formatting
            Serialize(cObject, "@../../MY_OBJ_Soap.xml", new System.Runtime.Serialization.Formatters.Soap.SoapFormatter());

            //3. Custom serialization using explicit implementation of ISerializable
            var targetD = new D();
            Serialize(targetD, @"../../MY_OBJ_Custom.bin", new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter());
            Serialize(targetD, @"../../MY_OBJ_Custom.xml", new System.Runtime.Serialization.Formatters.Soap.SoapFormatter());

            //De-serialization using constructor overload of D type
            targetD = Deserialize(@"../../MY_OBJ_Custom.xml", new System.Runtime.Serialization.Formatters.Soap.SoapFormatter()) as D;


            //Serialize A class
            A aObject = new A();
            Serialize(aObject, @"../../A_OBJ.bin", new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter());
            Serialize(aObject, @"../../A_OBJ.xml", new System.Runtime.Serialization.Formatters.Soap.SoapFormatter());

            //Shallow copy serialization

            //1. Using XML serialization
            var cTarget = new C_NoAttribute();
            var xmlSerializer = System.Xml.Serialization.XmlSerializer.FromTypes(new Type[] { typeof(C_NoAttribute) }).FirstOrDefault();
            using (var fileStream = new StreamWriter(@"../../MY_OBJ_Shallow.xml", false))
            {
                xmlSerializer.Serialize(fileStream.BaseStream, cTarget);
            }

            //2. Using Json Serialize
            var cJsonTarget = new C_NoAttribute();
            Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();

            using (var streamWriter = new StreamWriter(@"../../MY_OBJ_Shallow.json", false))
            {
                using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                {
                    jsonSerializer.Serialize(writer, cJsonTarget);
                }
            }

            Console.ReadKey();
        }

        static void Serialize(object targetObject, string location, System.Runtime.Serialization.IFormatter formatter)
        {
            using (StreamWriter writer = new StreamWriter(location, false))
            {
                formatter.Serialize(writer.BaseStream, targetObject);
                writer.Flush();
            }
        }

        static object Deserialize(string location, System.Runtime.Serialization.IFormatter formatter)
        {
            using (StreamReader writer = new StreamReader(location))
            {
                return formatter.Deserialize(writer.BaseStream);
            }
        }

    }
}
