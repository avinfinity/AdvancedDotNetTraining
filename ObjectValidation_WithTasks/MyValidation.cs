using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectValidation_WithTasks
{
    public class IsNullValidationAttribute : ValidationAttribute
    {
        public override bool Validate(object tobeValited)
        {
            Task.Delay(5000).Wait();
            return tobeValited == null;
        }
    }

    public class LengthCheckValidationAttribute : ValidationAttribute
    {
        private readonly int _MaxLength;
        public LengthCheckValidationAttribute(int maxLength)
        {
            _MaxLength = maxLength;
        }

        public override bool Validate(object tobeValited)
        {
            Task.Delay(5000).Wait();

            if (tobeValited == null)
                return false;

            var str = tobeValited.ToString();

            return str.Length < _MaxLength;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class ValidationAttribute : Attribute
    {
        public abstract bool Validate(object tobeValited);

        public string ErrorMessage { get; set; }
    }
}