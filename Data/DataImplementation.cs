using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private bool Disposed = false;
        private List<Ball> BallsList = new();
        private Logger logger;

        private List<Timer> TimersList = new();

        public DataImplementation() { }

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();

            // czyszczenie starych timerów przy ponownym starcie
            foreach (var timer in TimersList)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
            }
            TimersList.Clear();
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

                // startowy czas dla kuli
                newBall.LastUpdateTime = DateTime.UtcNow;

                Timer ballTimer = new Timer(BallTimerCallback, newBall, 0, 15);
                TimersList.Add(ballTimer);
            }
        }

        private void BallTimerCallback(object state)
        {
            if (state is Ball ball)
            {
                // jeśli obliczenia trwają dłużej niż 15ms (zostanie wywołany kolejny callback, gdy poprzedni ciągle trwa)
                // ignorujemy nowy callback
                if (Interlocked.Exchange(ref ball.IsUpdating, 1) == 1) return;

                try
                {
                    if (Disposed) return;

                    // obliczenie upłyniętego czasu systemowego
                    DateTime now = DateTime.UtcNow;
                    double dt = (now - ball.LastUpdateTime).TotalSeconds;
                    ball.LastUpdateTime = now;

                    // ochrona przed dużymi skokami (> 100ms)
                    if (dt > 0.1) dt = 0.1;

                    ball.Accumulator += dt;

                    double fixedTimeStep = 0.015;

                    while (ball.Accumulator >= fixedTimeStep)
                    {                        
                        double moveX = ball.Velocity.x * fixedTimeStep * 50.0;
                        double moveY = ball.Velocity.y * fixedTimeStep * 50.0;

                        ball.Move(new Vector(moveX, moveY), false);
                        logger.LogBallState(ball);

                        ball.Accumulator -= fixedTimeStep;
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref ball.IsUpdating, 0);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // zatrzymywanie timerów
                    foreach (var timer in TimersList)
                    {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        timer.Dispose();
                    }
                    TimersList.Clear();
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
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

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