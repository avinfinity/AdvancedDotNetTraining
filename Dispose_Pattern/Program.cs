using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispose_Pattern
{
    class Program
    {
        public enum ResourceState
        {
            Free = 0,
            Busy = 1
        }

        public static class Resource
        {
            public static AutoResetEvent ResourceHandle = new AutoResetEvent(false);
            public static ResourceState State = ResourceState.Free;
        }

        public class ResourceWrapper : IDisposable
        {
            public ResourceWrapper()
            {
                if(Resource.State == ResourceState.Free)
                {
                    Resource.State = ResourceState.Busy;
                    Console.WriteLine($"Resource owned by {Thread.CurrentThread.Name}");
                }
                else
                {
                    Console.WriteLine($"Awaiting for resource owned by {Thread.CurrentThread.Name}");
                    Resource.ResourceHandle.WaitOne();

                    Resource.State = ResourceState.Busy;
                    Console.WriteLine($"Resource owned by {Thread.CurrentThread.Name}");
                }
            }

            public void UseResource()
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine($"Resource used by {Thread.CurrentThread.Name}");
                    Task.Delay(1000).Wait();
                }
            }

            public void Dispose()
            {
                Resource.State = ResourceState.Free;
                Console.WriteLine($"Resource released by {Thread.CurrentThread.Name}");

                Resource.ResourceHandle.Set();
            }

            ~ResourceWrapper()
            {
                Resource.State = ResourceState.Free;
                Console.WriteLine($"Resource released by Finalizer thread with id {Thread.CurrentThread.ManagedThreadId}");

                Resource.ResourceHandle.Set();
            }
        }

        static void Main(string[] args)
        {
            Thread t1 = new Thread(new ThreadStart(Test)) { Name = "T1" };

            Thread t2 = new Thread(new ThreadStart(Test)) { Name = "T2" };

            t1.Start();

            t2.Start();

            Console.ReadKey();
        }

        static void Test()
        {
            ////1. Use Using keyword of C#
            //using (var resource = new ResourceWrapper())
            //{
            //    resource.UseResource();
            //}

            ////2. Explicit dispose:: Prone to exception and missing dispose call
            var resource = new ResourceWrapper();
            resource.UseResource();
            //resource.Dispose();

            ////3. Try Finally ...but resource2 can still be wrongly used in code even after dispose
            //var resource2 = new ResourceWrapper();
            //try
            //{
            //    resource2.UseResource();
            //}
            //finally
            //{
            //    resource2.Dispose();
            //}
            resource = null;
            GC.Collect();
        }
    }
}
