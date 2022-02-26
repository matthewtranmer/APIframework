using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIcontroller
{
    public class MethodNotStaticExeption : Exception
    {
        public MethodNotStaticExeption(string method_name) : base($"The method '{method_name}' was created as a resource but is not a static method.") { }
    }
    public class IncorrectReturnTypeExeption : Exception
    {
        public IncorrectReturnTypeExeption(string method_name) : base($"The method '{method_name}' was created as a resource but it's return type isnt of type 'Response'") { }
    }
}
