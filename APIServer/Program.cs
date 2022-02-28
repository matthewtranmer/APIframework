using System.Net;
using System.Net.Sockets;

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
        class Accounts
        {
            [Resource("/login", Method.GET)]
            public static APIResponse login_get(APIRequest request)
            {
                return new APIResponse();
            }

            [Resource("/login", Method.POST)]
            public static APIResponse login_post(APIRequest request)
            {
                return new APIResponse();
            }
        }
    }
}