using System.Threading.Channels;

class Consumer
{
    private ChannelReader<int> Reader;

    public Consumer(ChannelReader<int> reader, CancellationToken tok)
    {
        Reader = reader;
        Task.WaitAll(Run(tok));
    }

    private async Task Run(CancellationToken tok)
    {

        while (await Reader.WaitToReadAsync())
        {
            if (Reader.Count != 0)
            {
                var item = await Reader.ReadAsync();
                Program.count -= 1;
                Console.WriteLine($"\tReceived Data: {item}");
            }
            if (Reader.Count >= 100)
            {
                Program.flag = false;
            }
            else if (Reader.Count <= 80)
            {
                Program.flag = true;
            }

            if (tok.IsCancellationRequested)
            {
                if (Reader.Count == 0)
                {
                    Console.WriteLine("\tConsumer Stopped ");
                    return;
                }
            }
        }
    }
}