using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIcontroller
{
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
        public Method request_method;

        public ResourceAttribute()
        {
            resource_path = "";
        }
        public ResourceAttribute(string resource_path, Method request_method)
        {
            this.resource_path = resource_path;
            this.request_method = request_method;
        }
    }
}
