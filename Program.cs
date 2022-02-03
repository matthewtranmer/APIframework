using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIcontrollerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var paths = new Dictionary<string, MethodInfo>();

            Type group_type = typeof(ResourceGroupAttribute);
            Type[] assembly_types = Assembly.GetExecutingAssembly().GetTypes();
            List<Type> resource_groups = new List<Type>();

            foreach (Type type in assembly_types)
            {
                if (type.GetCustomAttribute(group_type) != null)
                {
                    resource_groups.Add(type);
                }
            }

            Type resource_type = typeof(ResourceAttribute);
            foreach (Type type in resource_groups)
            {
                MethodInfo[] methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    Attribute? resource_attribute = method.GetCustomAttribute(resource_type);
                    if (resource_attribute != null)
                    {
                        if (!method.IsStatic)
                        {
                            throw new MethodNotStaticExeption(method.Name);
                        }
                        //Create error for return type when implemented


                        Attribute? group_attribute = type.GetCustomAttribute(group_type);

                        object? group_path = group_attribute?.GetType().GetField("group_path")?.GetValue(group_attribute);
                        object? resource_path = resource_attribute?.GetType()?.GetField("resource_path")?.GetValue(resource_attribute);
                        string path = $"{group_path}{resource_path}";

                        paths.Add(path, method);
                    }
                }
            }

            paths["/accounts/delete"].Invoke(null, null);
            paths["/accounts/authenticate"].Invoke(null, null);
        }
    }

    [ResourceGroup("/accounts")]
    public class Login
    {
        [Resource("/authenticate")]
        public void authenticate()
        {
            Console.WriteLine("authenticate");
        }

        [Resource("/delete")]
        static public void delete()
        {
            Console.WriteLine("deleted");
        }
    }


    public class MethodNotStaticExeption : Exception
    {
        public MethodNotStaticExeption(string method_name) : base($"The method '{method_name}' was created as a resource but was not set as a static method.") { }
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ResourceGroupAttribute : Attribute
    {
        public string group_path;

        public ResourceGroupAttribute()
        {
            group_path = "/";
        }

        public ResourceGroupAttribute(string group_path)
        {
            this.group_path = group_path;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResourceAttribute : Attribute
    {
        public string resource_path;

        public ResourceAttribute()
        {
            resource_path = "";
        }
        public ResourceAttribute(string resource_path)
        {
            this.resource_path = resource_path;
        }
    }
}