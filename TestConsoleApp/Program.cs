using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectValidation;
using System.Dynamic;
using System.Linq.Expressions;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            goto Day2;
            goto Day1;

            Day1:
            var testObject = new TestClass() { Name = "ThisNameIsVeryBIGGGGGGGGGGGGGGGGGGGGGGG" };
            var validationResults = Validator.Validate(testObject);

            foreach (var result in validationResults.Results)
            {
                Console.WriteLine("Property {0} has failed validation with message {1}", result.PropertyName, result.ErrorMessage);
            }

           

            dynamic a = new A();
            a.AnyMethod();

            dynamic elasticObject = new ElasticClass();
            elasticObject.SomeValue = 100;
            Console.WriteLine(elasticObject.SomeValue);



            var devices = new Devices();

            var selectedDevices = devices.Where((dynamic device) => device.Model == "Q4001");

            foreach(var device in selectedDevices)
            {
                Console.WriteLine(device.DeviceID);
            }


            Day2:







            Console.ReadKey();
        }
    }

    class Foo
    {
        public Foo()
        {

        }

        void Fun()
        {

        }
    }

    class A : DynamicObject
    {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            if (binder.Name == "AnyMethod")
                result = 10;

            return true;
        }
    }

    class ElasticClass : DynamicObject
    {
        Dictionary<string, object> _LocalStore = new Dictionary<string, object>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _LocalStore[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            if (!_LocalStore.ContainsKey(binder.Name))
                return false;

            result =  _LocalStore[binder.Name];
            return true;
        }
    }

    class TestClass
    {
        [LengthCheckValidation(10, ErrorMessage = "Maximum 10 characters are allowed")]
        public string Name { get; set; }

        [IsNullValidation(ErrorMessage = "SomeData should not be null")]
        public object SomeData { get; }
    }
}
