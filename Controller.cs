using System.Reflection;

namespace APIcontroller
{
    class Controller
    {
        private string containing_path;
        private Dictionary<string, MethodInfo> paths = new Dictionary<string, MethodInfo>();

        private void initMethods()
        {
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

        public Controller(string containing_path)
        {
            this.containing_path = containing_path;

            initMethods();
        }
    }

    public class MethodNotStaticExeption : Exception
    {
        public MethodNotStaticExeption(string method_name) : base($"The method '{method_name}' was created as a resource but is not a static method.") { }
    }
    public class IncorrectReturnTypeExeption : Exception
    {
        public IncorrectReturnTypeExeption(string method_name) : base($"The method '{method_name}' was created as a resource but does not return 'byte[]', 'Span<byte>' or 'ReadOnlySpan<byte>'") { }
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
