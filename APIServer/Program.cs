using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIcontroller
{
    class Program
    {
        static void Main(string[] args)
        {
            Controller a = new Controller("");
        }

        [ResourceGroup("/accounts")]
        class containefr
        {
            [Resource("/login")]
            public static byte login()
            {
                Console.WriteLine("Called");
                return 4;
            }
        }
    }
}