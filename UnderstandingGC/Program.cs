using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace ConsoleApplication2
{
    class Base
    {
        int y = 34;
    }
    class A:Base
    {
        int x = 10;
        B obj = new B();
    }
    class B
    {
        int z = 30;

    }
    class C { }
    class Program
    {
        static A globalRef;
        static void Main(string[] args)
        {
            Console.WriteLine("Take Snapshot");
            Console.ReadKey();

            Allocate();
         
            C obj = new C();

            Console.WriteLine("Take Snapshot");
            Console.ReadKey();


            //GC.Collect();
            obj = null;

            Console.WriteLine("Take Snapshot");
            Console.ReadKey();


            //GC.Collect(0);
            globalRef = null;

            Console.WriteLine("Take Snapshot");
            Console.ReadKey();

            //GC.Collect(1);
            //GC.Collect();
        }
        static void Allocate()
        {
            A obj = new A();
            ArrayList _collection = new ArrayList();
            _collection.Add(obj);
            globalRef = obj;

            Console.WriteLine("Take Snapshot");
            Console.ReadKey();

            Console.WriteLine(GC.GetGeneration(obj));
            //GC.Collect();//Mark 
            Console.WriteLine(GC.GetGeneration(obj));

            Console.WriteLine("Take Snapshot");
            Console.ReadKey();

            //GC.Collect();
            Console.WriteLine(GC.GetGeneration(obj));
            //GC.Collect();
            Console.WriteLine(GC.GetGeneration(obj));

            Console.WriteLine("Take Snapshot");
            Console.ReadKey();
        }
    }
}
