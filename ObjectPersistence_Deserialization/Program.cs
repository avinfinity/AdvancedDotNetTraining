using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace ObjectPersistence_Deserialization
{
    class Program
    {
        [Serializable]
        public class A
        {
            private int X = 100;
            protected int y = 190;
            public int z = 20;
        }

        [Serializable]
        public class B
        {
            private int X = 100;
            protected int y = 190;
            //public int z = 20;
        }


        class SerializationBinderForTypeA : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return typeof(A);
            }
        }

        class SerializationBinderForTypeB : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return typeof(B);
            }
        }


        static void Main(string[] args)
        {
            //De-serializing any type to any new type using Binders

            string location = @"../../../ObjectPersitence/A_OBJ.bin";

            BinaryFormatter formatter = new BinaryFormatter() { Binder = new SerializationBinderForTypeA() };

            using (var stream = new StreamReader(location))
            {
                var aObj = formatter.Deserialize(stream.BaseStream) as A;

                stream.BaseStream.Position = 0;
                formatter.Binder = new SerializationBinderForTypeB();

                var aObj2 = formatter.Deserialize(stream.BaseStream) as B;

                Console.WriteLine("Completed de-serializing");
            }
        }
    }
}
