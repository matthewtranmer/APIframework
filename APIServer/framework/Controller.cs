using System.Reflection;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using Cryptography;
using DataStructures;
using log4net;

namespace APIcontroller
{
    class Controller
    {
        private Socket socket;
        private Dictionary<string, Dictionary<string, MethodInfo>> paths = new Dictionary<string, Dictionary<string, MethodInfo>>();
        private SynchronizationQueue<Socket> connections = new SynchronizationQueue<Socket>(1024);

        private void init_methods(string? containing_path)
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

                    if (method.ReturnType != typeof(APIResponse))
                    {
                        throw new IncorrectReturnTypeExeption(method.Name);
                    }

                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        throw new NoParametersException(method.Name);
                    }
                    if (parameters[0].ParameterType != typeof(APIRequest))
                    {
                        throw new FirstParameterTypeException(method.Name);
                    }

                    Attribute? group_attribute = type.GetCustomAttribute(group_type);

                    object? group_path = group_attribute?.GetType().GetField("group_path")?.GetValue(group_attribute);
                    object? resource_path = resource_attribute?.GetType()?.GetField("resource_path")?.GetValue(resource_attribute);
                    Method? request_method = (Method?)resource_attribute?.GetType()?.GetField("request_method")?.GetValue(resource_attribute);
                    string path = $"{containing_path}{group_path}{resource_path}";

                    if (!paths.ContainsKey(path))
                    {
                        paths.Add(path, new Dictionary<string, MethodInfo>());
                    }

                    var stored_dictonary = paths[path];
                    stored_dictonary.Add(request_method.ToString(), method);
                }
            }
        }

        void sendError(SecureSocket socket, string error_message)
        {
            string error_json = "{\"srv_error_message\":\"" + error_message + "\"}";
            socket.secureSend(Encoding.UTF8.GetBytes(error_json));
        }

        void handle_connection(Socket raw_conn)
        {
            SecureSocket secure_connection = new SecureSocket(raw_conn);
            Span<byte> request = secure_connection.secureRecv();

            Dictionary<string, string>? headers = JsonSerializer.Deserialize<Dictionary<string, string>>(request);

            if (headers == null)
            {
                sendError(secure_connection, "Error receiving headers");
            }

            if (!headers.ContainsKey("URI"))
            {
                sendError(secure_connection, "URI was not provided in the request");
            }

            if (!headers.ContainsKey("method"))
            {
                sendError(secure_connection, "Request method was not provided in the request");
            }

            string URI = headers["URI"];
            string method = headers["method"];

            if (!paths.ContainsKey(URI))
            {
                sendError(secure_connection, "URI could not be found");
            }

            var resource = paths[URI];
            if (!resource.ContainsKey(method))
            {
                sendError(secure_connection, "The given request method is not defined");
            }

            Dictionary<string, string>? body = JsonSerializer.Deserialize<Dictionary<string, string>>(headers["body"]);
            APIRequest api_request = new APIRequest() { request_body = body };

            object? response_object = resource[method].Invoke(null, new object?[1] { api_request });
            APIResponse? response_struct = response_object as APIResponse?;

            string? body_json = JsonSerializer.Serialize(response_struct?.response_body);

            Dictionary<string, string?>? response_dict = new Dictionary<string, string?>()
            {
                { "response_code", response_struct?.response_code },
                { "custom_error_message", response_struct?.error_message },
                { "response_body", body_json },
                { "srv_error_message", null }
            };

            string response = JsonSerializer.Serialize(response_dict);
            secure_connection.secureSend(Encoding.UTF8.GetBytes(response));
        }

        private async Task worker()
        {
            while (true)
            {
                Socket raw_conn = await connections.deQueue();

                handle_connection(raw_conn);
                raw_conn.Close();
            }
        }       

        private Task[] start_workers()
        {
            Task[] tasks = new Task[Settings.workers];
            for (int i = 0; i<Settings.workers; i++)
            {
                tasks[i] = worker();
                Console.WriteLine($"Worker #{i} Started");
            }

            return tasks;
        }

        private void start_server()
        {
            Console.WriteLine("Server Started");
            while (true)
            {
                var sock = socket.Accept();
                Console.WriteLine("Accepted Request");

                if (connections.isFull())
                {
                    Console.WriteLine("Full");
                    sock.Close();
                    continue;
                }

                sock.ReceiveTimeout = 100;
                sock.SendTimeout = 200; 
                connections.enQueue(sock);
            }
        }

        public void start()
        {
            start_workers();
            start_server();
        }

        public Controller(string? containing_path, Socket socket)
        {
            this.socket = socket;
            socket.ReceiveTimeout = 100;
            socket.SendTimeout = 200;
            
            init_methods(containing_path);
        }
    }
}
