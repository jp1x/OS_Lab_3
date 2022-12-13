using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

class Program
{
    static public bool flag = true;
    static public int count = 0;

    static void Main(string[] args)
    {
        Channel<int> channel = Channel.CreateBounded<int>(200);

        var cts = new CancellationTokenSource();

        Task[] streams = new Task[5];
        for (int i = 0; i < 5; i++)
        {
            if (i < 3)
            {
                streams[i] = Task.Run(() => { new Producer(channel.Writer, cts.Token); }, cts.Token);
            }
            else
            {
                streams[i] = Task.Run(() => { new Consumer(channel.Reader, cts.Token); }, cts.Token);
            }
        }

        new Thread(() => {
            for (; ;)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    cts.Cancel();
                }
            }
        })

        { IsBackground = true }.Start();

        Task.WaitAll(streams);
    }
}
