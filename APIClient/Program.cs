using System;
using Cryptography;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Buffers;
using System.Text.Json;
using System.Text;
using System.Net;

namespace APIClient
{
    public enum Method
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    class APIClient
    {
        private SecureSocket socket_wrapper;

        public APIClient(Socket socket)
        {
            socket_wrapper = new SecureSocket(socket);
        }

        public (Dictionary<string, string>? response, string? general_error) request(Method method, string uri, Dictionary<string, string>? body)
        {
            string json_body = JsonSerializer.Serialize(body);

            Dictionary<string, string>? headers = new Dictionary<string, string>();
            headers["method"] = method.ToString();
            headers["URI"] = uri;
            headers["body"] = json_body;

            string json_headers = JsonSerializer.Serialize(headers);

            socket_wrapper.secureSend(Encoding.UTF8.GetBytes(json_headers));
            Span<byte> response_json = socket_wrapper.secureRecv();

            Dictionary<string, string>? response = JsonSerializer.Deserialize<Dictionary<string, string>>(response_json);

            string? server_error = response?["srv_error_message"];
            if (server_error != null)
            {
                return (null, server_error);
            }
            
            string? response_body_json = response?["response_body"];
            Dictionary<string, string>? response_body = default;
            if (response_body_json != null)
            {
                response_body = JsonSerializer.Deserialize<Dictionary<string, string>>(response_body_json);
            }

            return (response_body, null);
        }
    }

    class Program
    {
        static void Main()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8063));

            APIClient client = new APIClient(socket);

            var request_body = new Dictionary<string, string>()
            {
                { "name", "matthew" }
            };

            (var response, var error) = client.request(Method.POST, "api/accounts/login", request_body);
            if (error != null)
            {
                Console.WriteLine(error);
                return;
            }

            Console.WriteLine(response);
        }
    }
}