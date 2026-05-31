using System;
using System.Diagnostics;

namespace Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private bool Disposed = false;

        private List<Ball> BallsList = [];

        private CancellationTokenSource CancellationTokenSource = new();

        private Logger logger;

        public DataImplementation() {}


        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            Random random = new Random();

            if (!CancellationTokenSource.IsCancellationRequested)
            {
                CancellationTokenSource.Cancel();
            }
            CancellationTokenSource.Dispose();
            CancellationTokenSource = new CancellationTokenSource();

            BallsList.Clear();
            logger?.Dispose();
            logger = new Logger();

            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.Next(100, 500), random.Next(100, 500));
                double initVx = (double)random.Next(-50, 50) / 10.0;
                double initVy = (double)random.Next(-50, 50) / 10.0;
                Vector initialVelocity = new(initVx, initVy);
                Ball newBall = new(startingPosition, initialVelocity);
                BallsList.Add(newBall);
                upperLayerHandler(startingPosition, newBall);

                StartBallMovement(newBall, CancellationTokenSource.Token);
            }
        }

        private void StartBallMovement(Ball ball, CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
        
                double accumulator = 0.0; 
                // obliczenia co 15ms
                double fixedTimeStep = 0.015;

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        // ile minęło czasu od ostatniego ruchu
                        double timeElapsed = stopwatch.Elapsed.TotalSeconds;
                        stopwatch.Restart();

                        // maksmylanie nadrabiane jest 100ms
                        // ochrona przed bardzo dużym skokiem jeśli aplikacja była zawieszona
                        if (timeElapsed > 0.1) timeElapsed = 0.1;

                        // sumowanie upłyniętego czasu
                        accumulator += timeElapsed;

                        // wykonywanie kilku kroków naraz, aby nadrobić upłynięty czas powyżej 15ms
                        while (accumulator >= fixedTimeStep)
                        {
                            double moveX = ball.Velocity.x * fixedTimeStep * 50.0;
                            double moveY = ball.Velocity.y * fixedTimeStep * 50.0;

                            ball.Move(new Vector(moveX, moveY), false);

                            logger.LogBallState(ball);

                            accumulator -= fixedTimeStep;
                        }

                        // odczekiwanie minimum 15ms
                        await Task.Delay(15, cancellationToken);
                    }
                }
                catch (TaskCanceledException)
                {
                    // Wyjątek jeśli zadanie zostało anulowane
                }
            }, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    CancellationTokenSource.Cancel();
                    CancellationTokenSource.Dispose();
                    BallsList.Clear();
                    logger?.Dispose();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        
        //metody do testów
        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

    }
}