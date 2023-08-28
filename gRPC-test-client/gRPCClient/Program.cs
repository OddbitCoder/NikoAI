using Grpc.Core;
using Shared;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "127.0.0.1";// args[0];
            int port = 9010;// int.Parse(args[1]);
            long x = 1;// long.Parse(args[2]);
            string op = "+";// args[3];
            long y = 2;// long.Parse(args[4]);
            var channel = new Channel(
                host,
                port,
                ChannelCredentials.Insecure
                );
            var client = new Svc.SvcClient(channel);
            var reply = client.Calculate(new CalculateRequest
            {
                X = x,
                Y = y,
                Op = op
            });
            Console.WriteLine($"The calculated result is: {reply.Result}");
        }
    }
}
