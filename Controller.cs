using System.Reflection;
using System.Net.Sockets;
using Cryptography;

namespace APIcontroller
{
    class Controller
    {
        private string containing_path;
        private SecureSocket socket_wrapper;
        private Dictionary<string, MethodInfo> paths = new Dictionary<string, MethodInfo>();

        private void initMethods()
        {
            Type group_type = typeof(ResourceGroupAttribute);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> resource_groups = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                Type[] assembly_types = assembly.GetTypes();
                foreach (Type type in assembly_types)
                {
                    if (type.GetCustomAttribute(group_type) != null)
                    {
                        resource_groups.Add(type);
                    }
                }
            }
            

            Type resource_type = typeof(ResourceAttribute);
            foreach (Type type in resource_groups)
            {
                MethodInfo[] methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    Attribute? resource_attribute = method.GetCustomAttribute(resource_type);
                    if (resource_attribute == null)
                    {
                        continue;
                    }

                    if (!method.IsStatic)
                    {
                        throw new MethodNotStaticExeption(method.Name);
                    }

                    if (
                      method.ReturnType != typeof(byte[])
                      && method.ReturnType != typeof(Span<byte>)
                      && method.ReturnType != typeof(ReadOnlySpan<byte>))
                    {
                        throw new IncorrectReturnTypeExeption(method.Name);
                    }

                    Attribute? group_attribute = type.GetCustomAttribute(group_type);

                    object? group_path = group_attribute?.GetType().GetField("group_path")?.GetValue(group_attribute);
                    object? resource_path = resource_attribute?.GetType()?.GetField("resource_path")?.GetValue(resource_attribute);
                    string path = $"{group_path}{resource_path}";

                    paths.Add(path, method);
                }
            }
        }

        //make async
        private void network_handle()
        {
            while (true)
            {
                Span<byte> api_request = socket_wrapper.secureRecv();
            }
        }

        public Controller(string containing_path)//, Socket socket)
        {
            this.containing_path = containing_path;
            //socket_wrapper = new SecureSocket(socket);
            initMethods();

            

            paths["/accounts/login"].Invoke(null, null);
        }
    }
}
