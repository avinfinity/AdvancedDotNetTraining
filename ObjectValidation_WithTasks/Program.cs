using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectValidation_WithTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            var testObject = new TestClass() { Name = "ThisNameIsVeryBIGGGGGGGGGGGGGGGGGGGGGGG", SomeData = null };

            Stopwatch watch = new Stopwatch();
            //Sync validate
            watch.Start();
            var validationResults = Validator.Validate(testObject);
            watch.Stop();

            Console.WriteLine($"Time spent in sync validation {watch.ElapsedMilliseconds / 1000} seconds");
            watch.Reset();

            //async validate
            watch.Start();
            validationResults = Validator.ValidateAsync(testObject).Result;
            watch.Stop();

            Console.WriteLine($"Time spent in async validation {watch.ElapsedMilliseconds / 1000} seconds");

            //Print validation results
            foreach (var result in validationResults.Results)
            {
                Console.WriteLine("Property {0} has failed validation with message {1}", result.PropertyName, result.ErrorMessage);
            }

            Console.ReadKey();
        }


        class TestClass
        {
            [LengthCheckValidation(10, ErrorMessage = "Maximum 10 characters are allowed")]
            public string Name { get; set; }

            [IsNullValidation(ErrorMessage = "SomeData should not be null")]
            public object SomeData { get; set; }
        }
    }
}
