using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Theading
{
    class Program
    {
        //[System.Runtime.Remoting.Contexts.Synchronization]
        public class Singleton //: ContextBoundObject
        {
            int counter=0;


            private object _IncrementLock = new object();
            private object _DecrementLock = new object();


            private Singleton() { }

            //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
            public void Increment()
            {
                Monitor.Enter(_IncrementLock);
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        counter++;

                        Console.WriteLine($"Counter value {counter} incremented by {Thread.CurrentThread.Name}");

                        Thread.Sleep(1000);
                    }
                }
                finally
                {
                    Monitor.Exit(_IncrementLock);
                }
            }

            //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
            public void Decrement()
            {
                Monitor.Enter(_DecrementLock);
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        counter--;

                        Console.WriteLine($"Counter value {counter} decremented by {Thread.CurrentThread.Name}");

                        Thread.Sleep(1000);
                    }
                }
                finally
                {
                    Monitor.Exit(_DecrementLock);
                }
            }

            public static readonly Singleton Instance = new Singleton();
        }

        static void Main(string[] args)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            Thread t1 = new Thread(new ThreadStart(Singleton.Instance.Increment)) { Name = "T1", IsBackground=true };

            Thread t2 = new Thread(new ThreadStart(Singleton.Instance.Increment)) { Name = "T2" , IsBackground = true };

            Thread t3 = new Thread(new ThreadStart(Singleton.Instance.Decrement)) { Name = "T3", IsBackground = true };

            Thread t4 = new Thread(new ThreadStart(Singleton.Instance.Decrement)) { Name = "T4" , IsBackground = true };

            t1.Start();

            t2.Start();

            t3.Start();

            t4.Start();


            ThreadPool.QueueUserWorkItem(new WaitCallback(SoSomeWork));

            Console.ReadKey();
        }

        static void SoSomeWork(object obj)
        {
            for (int i = 0; i < 100000000; i++)
            {
                Console.WriteLine($"Running on thread-pool thread with ID : {Thread.CurrentThread.ManagedThreadId}");
            }
        }
    }
}