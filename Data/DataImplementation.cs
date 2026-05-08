using System;
using System.Diagnostics;

namespace Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private bool Disposed = false;

        private readonly Timer MoveTimer;
        private List<Ball> BallsList = [];

        private CancellationTokenSource CancellationTokenSource = new();

        public DataImplementation()
        {
            MoveTimer = new Timer(Move, null, Timeout.InfiniteTimeSpan, TimeSpan.FromMilliseconds(15)); //75 FPS
        }


        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            MoveTimer.Change(Timeout.InfiniteTimeSpan, TimeSpan.FromMilliseconds(15));
            Random random = new Random();
            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.Next(100, 500), random.Next(100, 500));
                double initVx = (double)random.Next(-100, 100) / 10.0;
                double initVy = (double)random.Next(-100, 100) / 10.0;
                Vector initialVelocity = new(initVx, initVy);
                Ball newBall = new(startingPosition, initialVelocity);
                BallsList.Add(newBall);
                upperLayerHandler(startingPosition, newBall);

                StartBallMovement(newBall, CancellationTokenSource.Token);
            }
            MoveTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(15));
        }

        private void StartBallMovement(Ball ball, CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    ball.Move(new Vector(ball.Velocity.x, ball.Velocity.y), true);
                    await Task.Delay(15, cancellationToken);
                }
            }, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    MoveTimer.Dispose();
                    BallsList.Clear();
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


        private void Move(object? x)
        {
            foreach (Ball item in BallsList)
                item.Move(new Vector(item.Velocity.x, item.Velocity.y), false);
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