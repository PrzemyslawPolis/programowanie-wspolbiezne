using Data;
using System.Diagnostics;

namespace BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
       
        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(Data.DataAbstractAPI? underneathLayer)
        {
            layerBelow = underneathLayer == null ? Data.DataAbstractAPI.GetDataLayer() : underneathLayer;
        }


        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBelow.Dispose();
            Disposed = true;
        }

        private Dictionary<Data.IBall, Ball> BallDict = new();

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBelow.Start(numberOfBalls, (startingPosition, dataBall) =>
            {
                Ball logicBall = new Ball();

                BallDict.Add(dataBall, logicBall);

                dataBall.NewPositionNotification += OnBallPositionNotification;
                
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        private void OnBallPositionNotification(object? sender, IVector newPos)
        {
            if (sender != null)
            {
                Data.IBall ball = (Data.IBall)sender!;

                double radius = BusinessLogicAbstractAPI.GetDimensions.BallDimension / 2;
                double width = BusinessLogicAbstractAPI.GetDimensions.TableWidth;
                double height = BusinessLogicAbstractAPI.GetDimensions.TableHeight;

                double currX = newPos.x;
                double currY = newPos.y;

                
                if (currX <= radius || currX >= width-radius)
                {
                    if (currX < radius)
                    {
                        currX = radius;
                    }
                    else if (currX > width - radius)
                    {
                        currX = width - radius;
                    }
                    ball.SetPosition(currX, currY);
                    ball.SetVelocity(-ball.Velocity.x, ball.Velocity.y);
                }
                if (currY <= radius || currY >= height-radius)
                {
                    if (currY < radius)
                    {
                        currY = radius;
                    }
                    else if (currY > height - radius)
                    {
                        currY = height - radius;
                    }
                    ball.SetPosition(currX, currY);
                    ball.SetVelocity(ball.Velocity.x, -ball.Velocity.y);
                }

                if (BallDict.TryGetValue(ball, out var logicBall))
                {             
                    logicBall.UpdatePosition(currX, currY);
                }
            }
        }

        private bool Disposed = false;

        private readonly Data.DataAbstractAPI layerBelow;
    }
}
