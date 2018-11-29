using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetThreading = System.Threading;

namespace Threading.Mutex
{
    class Program
    {
        static class StaticHandles
        {
            public static NetThreading.Mutex _HandleOne = new NetThreading.Mutex();

            public static NetThreading.Mutex _HandleTwo = new NetThreading.Mutex();

            public static NetThreading.ManualResetEvent _ManualResetEvent = new NetThreading.ManualResetEvent(false);

            public static NetThreading.AutoResetEvent _AutoResetEvent1 = new NetThreading.AutoResetEvent(false);

            public static NetThreading.AutoResetEvent _AutoResetEvent2 = new NetThreading.AutoResetEvent(false);

            public static NetThreading.AutoResetEvent _AutoResetEvent3 = new NetThreading.AutoResetEvent(false);
        }

        class A
        {
            public void Task1()
            {
                StaticHandles._HandleOne.WaitOne(); //Check the state of handle

                StaticHandles._AutoResetEvent1.Set();

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Mutex handle one owned by {NetThreading.Thread.CurrentThread.Name}");
                    NetThreading.Thread.Sleep(1000);
                }

                StaticHandles._HandleOne.ReleaseMutex(); //Set mutex free
            }
        }

        class B
        {
            public void Task2()
            {
                StaticHandles._HandleTwo.WaitOne(); //Check the state of handle

                StaticHandles._AutoResetEvent2.Set();

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Mutex handle one owned by {NetThreading.Thread.CurrentThread.Name}");
                    NetThreading.Thread.Sleep(1000);
                }

                StaticHandles._HandleTwo.ReleaseMutex(); //Set mutex free
            }
        }

        class C
        {
            public void Task3()
            {

                NetThreading.WaitHandle.WaitAll(new NetThreading.WaitHandle[] { StaticHandles._AutoResetEvent1, StaticHandles._AutoResetEvent2 });

                Console.WriteLine($"Awaiting for HandleOne and HandleTwo by {NetThreading.Thread.CurrentThread.Name}");

                NetThreading.WaitHandle.WaitAll(new NetThreading.WaitHandle[] { StaticHandles._HandleOne, StaticHandles._HandleTwo });

                Console.WriteLine($"Got handles for HandleOne and HandleTwo by {NetThreading.Thread.CurrentThread.Name}");
            }
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(Environment.ProcessorCount);
            //Console.Read();

            //GlobalMutex 
            NetThreading.Mutex globalMutex;
            if(NetThreading.Mutex.TryOpenExisting("MyOwnUniqueName", out globalMutex))
            {
                Console.WriteLine("Another instance of application in execution. Kindly wait for completion");
                NetThreading.Thread.Sleep(5000);
                Environment.Exit(0);
            }
            else
            {
                globalMutex = new NetThreading.Mutex(true, "MyOwnUniqueName");
            }

            new NetThreading.Thread(new A().Task1) { Name = "T1" }.Start();

            new NetThreading.Thread(new B().Task2) { Name = "T2" }.Start();

            new NetThreading.Thread(new C().Task3) { Name = "T3" }.Start();

            globalMutex.ReleaseMutex();

            Console.ReadKey();


            
        }
    }
}