using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace ObjectPersistence_Surrogaters
{
    //Consider this as a third part code which cannot be marked as [Serializable]
    class A
    {
        public int X = 100;

        private int Y = 200;
    }


    public class A_Surrogator : ISerializationSurrogate
    {
        //Called during serialization
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            A source = obj as A;

            info.AddValue("A.X", source.X);

            //Access private members using reflection
            info.AddValue("A.Y", source.GetType().GetField("Y", System.Reflection.BindingFlags.Instance| System.Reflection.BindingFlags.NonPublic).GetValue(source));

        }

        //Called during de-serialization
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            A target = obj as A;

            target.X = info.GetInt32("A.X");

            target.GetType().GetField("Y", System.Reflection.BindingFlags.Instance |  System.Reflection.BindingFlags.NonPublic).SetValue(target, info.GetInt32("A.Y"));

            return target;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {

            string location = @"../../A_Objet_NoAttribute.bin";

            BinaryFormatter formatter = new BinaryFormatter();

            SurrogateSelector surrogateSelector = new SurrogateSelector();
            surrogateSelector.AddSurrogate(typeof(A), new StreamingContext(StreamingContextStates.All), new A_Surrogator());

            formatter.SurrogateSelector = surrogateSelector;

            var aObject = new A();


            //Serialize
            using (var stream = new StreamWriter(location,false))
            {
                formatter.Serialize(stream.BaseStream, aObject);
            }

            //De-serialize
            A aaObject = null;
            using (var stream = new StreamReader(location, false))
            {
                aaObject = formatter.Deserialize(stream.BaseStream) as A;
            }


            Console.ReadKey();
        }
    }
}
