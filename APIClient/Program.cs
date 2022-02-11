using System;
using Cryptography;
using System.Net.Sockets;
using System.Collections.Generic;
using ProtoBuf;
using System.Buffers;

namespace APIClient
{
    public enum Method
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    [ProtoContract]
    class RequestMessage
    {
        [ProtoMember(1)]
        public Method method;

        [ProtoMember(2)]
        public string uri;
    }

    class APIClient
    {
        private SecureSocket socket_wrapper;
        private MemoryPool<byte> memory_pool;

        public APIClient(Socket socket)
        {
            socket_wrapper = new SecureSocket(socket);
            memory_pool = MemoryPool<byte>.Shared;
        }

        

        public void request(Method method, string uri)
        {
            request<byte>(method, uri, null);
        }

        public void request<T>(Method method, string uri, T? body) where T : class
        {
            RequestMessage request = new RequestMessage()
            {
                method = method,
                uri = uri,
            };

            

            MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream, request);

            IMemoryOwner<byte> rented_memory = memory_pool.Rent(Convert.ToInt32(stream.Length));
            stream.Read(rented_memory.Memory.Span);
        }
    }

    class Program
    {
        static void Main()
        {

        }
    }
}