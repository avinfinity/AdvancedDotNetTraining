using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{

    class A : MarshalByRefObject
    {

        public A()
        {
            Console.WriteLine("Instance OF A Instantiated in " + AppDomain.CurrentDomain.FriendlyName);

        }
        public void Test()
        {
            Console.WriteLine("A.Test Excecuted in " + AppDomain.CurrentDomain.FriendlyName);

        }
    }
    class Program
    {
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        static void Main(string[] args)
        {

            Console.WriteLine("Main Instantiated in " + AppDomain.CurrentDomain.FriendlyName);
            AppDomain _remoteAppDomainProxy = AppDomain.CreateDomain("ServerDomain");
            _remoteAppDomainProxy.UnhandledException += new UnhandledExceptionEventHandler(_remoteAppDomainProxy_UnhandledException);
            var handle = _remoteAppDomainProxy.CreateInstance("ConsoleApplication1", "ConsoleApplication1.A");
            A objRef = handle.Unwrap() as A;
            objRef.Test();


        }

        static void _remoteAppDomainProxy_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            throw new NotImplementedException();
        }
    }
}
