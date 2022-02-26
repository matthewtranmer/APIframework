using System.Net;
using System.Net.Sockets;
using System.Text;
using Cryptography;

namespace APIcontroller
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint end_point = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8063);
            socket.Bind(end_point);
            socket.Listen();

            Controller controller = new Controller("api", socket);
            controller.start();
            
        }

        [ResourceGroup("/accounts")]
        class containefr
        {
            static Random random = new Random();

            [Resource("/login", Method.GET)]
            public static APIResponse login_get(object body)
            {
                //Console.WriteLine("Called GET");

                Dictionary<string, string> response_body = new Dictionary<string, string>()
                {
                    { "Username", random.Next(0, 199999999).ToString() },
                    { "Password", "Matthew123" }
                };
                APIResponse response = new APIResponse(response_body);

                return response;
            }

            [Resource("/login", Method.POST)]
            public static APIResponse login_post(object body)
            {
                Console.WriteLine("Called POST");
                return new APIResponse();
            }
        }
    }
}