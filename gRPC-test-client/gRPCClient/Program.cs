using Grpc.Core;
using Shared;

namespace Client
{
    class Program
    {
        static void DoCalculator(Svc.SvcClient client, String[] args)
        {
            long x = 1;// long.Parse(args[2]);
            string op = "+";// args[3];
            long y = 2;// long.Parse(args[4]);
            var reply = client.Calculate(new CalculateRequest
            {
                X = x,
                Y = y,
                Op = op
            });
            Console.WriteLine($"The calculated result is: {reply.Result}");
        }

        static async Task DoTimeSeries(Svc.SvcClient client, String[] args)
        {
            Console.WriteLine("doing time series");
            using var duplex = client.Median();
            var responseTask = Task.Run(async () =>
            {
                while (await duplex.ResponseStream.MoveNext())
                {
                    var resp = duplex.ResponseStream.Current;
                    Console.WriteLine($"{resp.Timestamp}: {resp.Value}");
                }
            });
            int ts = 1;
            double temp = 10.0;
            var rnd = new Random();
            while (true)
            {
                await duplex.RequestStream.WriteAsync(new Temperature { Timestamp = ts, Value = temp });
                ts += 1;
                temp += rnd.NextDouble() - 0.5;
            }
        }

        static async Task Main(string[] args)
        {
            string host = "127.0.0.1";// args[0];
            int port = 9010;// int.Parse(args[1]);
            bool doTimeSeries = true;

            //var creds = new SslCredentials(
            //    File.ReadAllText("cert/ca.pem")
            //);
            //var channel = new Channel(
            //    host,
            //    port,
            //    creds
            //    );
            var channel = new Channel(host, port, ChannelCredentials.Insecure);
            var client = new Svc.SvcClient(channel);
            if (doTimeSeries/*args.Length == 2*/)
            {
                await DoTimeSeries(client, args);
            }
            else
            {
                DoCalculator(client, args);
            }
        }
    }
}
