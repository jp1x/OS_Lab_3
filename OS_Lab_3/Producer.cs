using System.Threading.Channels;

class Producer
{
    private ChannelWriter<int> Writer;
    public Producer(ChannelWriter<int> writer, CancellationToken tok)
    {
        Writer = writer;
        Task.WaitAll(Run(tok));
    }

    private async Task Run(CancellationToken tok)
    {
        var random = new Random();

        while (await Writer.WaitToWriteAsync())
        {
            if (tok.IsCancellationRequested)
            {
                Console.WriteLine("\tProducer Stopped");
                return;
            }
            if (Program.flag && Program.count <= 100)
            {
                var item = random.Next(1, 101);
                await Writer.WriteAsync(item);
                Program.count += 1;
                Console.WriteLine($"\tRecorded Data: {item}");
            }
        }
    }
}
