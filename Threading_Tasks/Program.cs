using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Threading_Tasks
{
    class Program
    {
        static void Main_Old(string[] args)
        {
            //Simple task
            Task.Run(() =>
            {
                Console.WriteLine("Task in execution");

                //Task.Delay(5000).Wait();
            }).Wait();

            //Exception in task
            var task = Task.Run(() => 
            {
                //Task.Delay(5000).Wait();

                throw new InvalidOperationException("Exception occurred");
            });

            //Check for exceptions
            try
            {
                task.Wait();
            }
            catch (AggregateException excetion)
            {
                Console.WriteLine(excetion.InnerException.Message);
            }

            Task parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent task started");

                Task.Delay(2000).Wait();

                Task detachedtask = new Task(() => 
                {
                    Console.WriteLine("Child task started");

                    Task.Delay(2000).Wait();

                    Console.WriteLine("Child task completed");
                }, TaskCreationOptions.AttachedToParent);
                detachedtask.Start();

                Console.WriteLine("Parent task completed");
            }, TaskCreationOptions.DenyChildAttach); //If you deny, then even after calling AttachedToParent inside, this task would not consider any child tasks 

            parentTask.Wait();




            //Get exceptions from all tasks


            Task parentTask2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent task started");

                Task.Delay(2000).Wait();

                Task childtask = new Task(() =>
                {
                    Console.WriteLine("Child task started");

                    Task.Delay(2000).Wait();

                    Task grandChildtask = new Task(() =>
                    {
                        Console.WriteLine("Grand Child task started");

                        Task.Delay(2000).Wait();

                        Console.WriteLine("Grand Child task completed");

                        throw new Exception("Grand child exception");

                    }, TaskCreationOptions.AttachedToParent);
                    grandChildtask.Start();


                    Console.WriteLine("Child task completed");

                    throw new Exception("Child exception");


                }, TaskCreationOptions.AttachedToParent);
                childtask.Start();

                Console.WriteLine("Parent task completed");

                throw new Exception("Parent exception");
            });

            try
            {
                parentTask2.Wait();
            }
            catch (AggregateException exception)
            {
                Console.WriteLine(exception.Message);

                foreach(var innerException in exception.Flatten().InnerExceptions)
                {
                    Console.WriteLine(innerException.Message);
                }
            }

            Console.ReadKey();
        }

        static void Main_Old_old()
        {
            while(true)
            {
                Console.WriteLine("Press any key to start search");
                Console.ReadKey();

                Task<string> buttonClickTask = new Task<string>(() =>
                {
                    Random _random = new Random();

                    if (_random.Next(1, 10) % 2 == 0)
                    {
                        return "Success";
                    }
                    throw new Exception("Error in communication");
                });

                buttonClickTask
                    .ContinueWith(pt =>
                    {
                        Console.WriteLine($"Logging communication exception {pt.Exception.InnerException.Message}");

                    }, TaskContinuationOptions.OnlyOnFaulted);

                buttonClickTask
                    .ContinueWith<string>(pt =>
                    {
                        Console.WriteLine($"Task is successful. Result is : {pt.Result}");
                        return pt.Result.ToUpper();

                    }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent)
                    .ContinueWith(pt =>
                    {
                        Console.WriteLine($"Notified UI to update with result {pt.Result}");
                    }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent);

                buttonClickTask.Start();
            }
        }



        static void Main()
        {
            OnButtonClick();

            Console.ReadKey();
        }

        static async void OnButtonClick()
        {
            //Episode 1 : Run in sync
            Console.WriteLine($"Ahead of await statement {Thread.CurrentThread.ManagedThreadId}");

            var result = await Task.Run<string>(() => 
            {
                //Episode 2 : Run in aync

                for (int i = 0; i < 20; i++)
                {
                    Console.WriteLine($"Searching async on {Thread.CurrentThread.ManagedThreadId}");
                    Task.Delay(500).Wait();
                }

                return "Search is successful";
            });

            //Episode 3: sync
            Console.WriteLine($"Printing result on {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
