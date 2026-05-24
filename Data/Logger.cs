using System.Collections.Concurrent;
using System.Text;

namespace Data
{
    internal class Logger : IDisposable
    {
        private readonly ConcurrentQueue<string> logQueue;
        private readonly Task loggingTask;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly string filePath;
        private bool disposed = false;

        public Logger()
        {
            logQueue = new ConcurrentQueue<string>();
            cancellationTokenSource = new CancellationTokenSource();

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dataProjectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\..\Data"));
            filePath = Path.Combine(dataProjectDirectory, "logs.json");

            // nadpisanie starego pliku po nowym uruchomieniu
            File.WriteAllText(filePath, string.Empty);

            loggingTask = Task.Run(LogRoutine);
        }

        // producent - zapisywanie stanu kulki do kolejki
        public void LogBallState(Ball ball)
        {
            if (disposed) return;

            // Serializacja do JSON
            string logEntry = $@"{{ ""Time"": ""{DateTime.Now:HH:mm:ss.fff}"", ""BallId"": ""{ball.GetHashCode()}"", ""X"": {ball.Position.x:F2}, ""Y"": {ball.Position.y:F2}, ""Vx"": {ball.Velocity.x:F2}, ""Vy"": {ball.Velocity.y:F2} }}";

            logQueue.Enqueue(logEntry);
        }

        // konsument - odczytywanie z kolejki i zapisywanie do pliku
        private async Task LogRoutine()
        {
            try
            {
                using StreamWriter writer = new StreamWriter(filePath, append: true, Encoding.ASCII);

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    // zapisywanie do pliku dopóki są logi w kolejce
                    while (logQueue.TryDequeue(out string log))
                    {
                        await writer.WriteLineAsync(log);
                    }

                    // wymuszenie zapisania odczytanych logów z bufora do pliku
                    await writer.FlushAsync();

                    // odczekiwanie 500ms przed kolejnym sprawdzeniem kolejki
                    await Task.Delay(500, cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // wyjątek jeśli zadanie zostało anulowane
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            cancellationTokenSource.Cancel();
            loggingTask.Wait();

            // zapisanie wszystkiego co zostało w kolejce
            if (!logQueue.IsEmpty)
            {
                using StreamWriter writer = new StreamWriter(filePath, append: true, Encoding.ASCII);
                while (logQueue.TryDequeue(out string log))
                {
                    writer.WriteLine(log);
                }
            }

            cancellationTokenSource.Dispose();
        }
    }
}