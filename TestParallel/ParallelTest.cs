using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestParallel
{
    public class ParallelTest
    {

        static void Main_OLD(string[] args)
        {
            string[] paragrahContent = new string[] { "a", "b", "c", "d", "e", "f", "g","h"};

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var encryptedCipher = paragrahContent.Select(item => Encrypt(item)).ToList();

            Parallel.ForEach(paragrahContent, (item) => Encrypt(item));

            encryptedCipher = paragrahContent.AsParallel().Select(item => Encrypt(item)).ToList();

            paragrahContent
                .AsParallel()//This uses range partitioning of data (Fixed size chunk)
                .Select(item => Encrypt(item))
                .ForAll(encyptedItem => Console.WriteLine($" {encyptedItem} by {Thread.CurrentThread.ManagedThreadId}"));

            Partitioner.Create( paragrahContent,true) //This is chunk partitioner with load balancing
                .AsParallel()
                .WithDegreeOfParallelism(2)
                .Select(item => Encrypt(item))
                .ForAll(encyptedItem => Console.WriteLine($" {encyptedItem} by {Thread.CurrentThread.ManagedThreadId}"));

            stopWatch.Stop();

            Console.WriteLine(stopWatch.ElapsedMilliseconds / 1000);

            Console.ReadKey();
        }


        static void Main()
        {
            string[] paragrahContent = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" };

            var queryResults = Query(paragrahContent, item => item == "b");

            foreach(var item in queryResults)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }

        static string Encrypt(string inputData)
        {
            if(inputData == "b")
                Thread.Sleep(10000);

            for (int i = 0; i < inputData.Length; i++)
            {
                Thread.Sleep(2000);
            }
            return inputData.ToUpper();
        }

        static IEnumerable<T> Query<T>(IEnumerable<T> source, Func<T,bool> predicate)
        {
            ConcurrentBag<T> _result = new ConcurrentBag<T>();

            Parallel.ForEach(source, (item) => 
            {
                if (predicate(item))
                    _result.Add(item);
            });

            return _result;
        }
    }
}