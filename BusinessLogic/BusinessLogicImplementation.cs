using Data;
using System.Diagnostics;

namespace BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
       
        private readonly object _lock = new object();

        private QuadTree activeTree = new QuadTree(new Boundary(0, 0, GetDimensions.TableWidth, GetDimensions.TableHeight)); //początkowe puste drzewo, żeby przeszło przez inicjalizację
        private bool isTaskRunning = false;

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

            lock (_lock)
            {
                BallDict.Clear();
                activeTree = new QuadTree(new Boundary(0, 0, GetDimensions.TableWidth, GetDimensions.TableHeight));
            }

            layerBelow.Start(numberOfBalls, (startingPosition, dataBall) =>
            {
                Position newPosition = new Position(startingPosition.x, startingPosition.y);

                Ball logicBall = new Ball(newPosition, dataBall);

                lock (_lock)
                {
                    BallDict.Add(dataBall, logicBall);
                }

                dataBall.NewPositionNotification += OnBallPositionNotification;
                
                upperLayerHandler(newPosition, logicBall);
            });

            if (!isTaskRunning)
            {
                isTaskRunning = true;
                Task.Run(async () => {
                    while (!Disposed)
                    {
                        QuadTree newTree = new QuadTree(new Boundary(0, 0, GetDimensions.TableWidth, GetDimensions.TableHeight));
                        lock (_lock)
                        {
                            foreach (Ball logicBall in BallDict.Values)
                            {
                                newTree.Insert(logicBall);
                            }
                        }
                        activeTree = newTree;
                        await Task.Delay(15);
                    }
                });
            }
        }

        private void OnBallPositionNotification(object? sender, IVector newPos)
        {
            if (sender != null)
            {
                Data.IBall ball = (Data.IBall)sender!;

                if(BallDict.TryGetValue(ball, out var logicBall))
                {
                    double radius = BusinessLogicAbstractAPI.GetDimensions.BallDimension / 2;
                    double width = BusinessLogicAbstractAPI.GetDimensions.TableWidth;
                    double height = BusinessLogicAbstractAPI.GetDimensions.TableHeight;

                    double currX = newPos.x;
                    double currY = newPos.y;


                    if ((currX <= radius && ball.Velocity.x < 0) || (currX >= width - radius && ball.Velocity.x > 0))
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
                        logicBall.position = new Position(currX, currY);
                        ball.SetVelocity(-ball.Velocity.x, ball.Velocity.y);
                    }
                    if ((currY <= radius && ball.Velocity.y < 0) || (currY >= height - radius && ball.Velocity.y > 0))
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
                        logicBall.position = new Position(currX, currY);
                        ball.SetVelocity(ball.Velocity.x, -ball.Velocity.y);
                    }

                    List<Ball> colissionBalls = new();


                    activeTree.Query(new Boundary(currX - radius, currY - radius, radius * 4, radius * 4), colissionBalls);

                    foreach (Ball col in colissionBalls)
                    {
                        if (col == logicBall) continue;


                        lock (_lock) //sekcja krytyczna
                        {
                            double distX = col.position.x - currX;
                            double distY = col.position.y - currY;
                            double distSq = (distX * distX) + (distY * distY);

                            if (distSq == 0)
                            {
                                distX = 0.01;
                                distY = 0.01;
                                distSq = distX * distX + distY * distY;
                            }

                            double dist = Math.Sqrt(distSq);
                            double overlap = radius * 2 - dist;

                            if (overlap > 0)
                            {
                                double moveX = (distX / dist) * (overlap / 2.0);
                                double moveY = (distY / dist) * (overlap / 2.0);

                                currX -= moveX;
                                currY -= moveY;

                                double otherX = col.position.x + moveX;
                                double otherY = col.position.y + moveY;

                                currX = Math.Clamp(currX, radius, width - radius);
                                currY = Math.Clamp(currY, radius, height - radius);

                                otherX = Math.Clamp(otherX, radius, width - radius);
                                otherY = Math.Clamp(otherY, radius, height - radius);

                                ball.SetPosition(currX, currY);

                                col.position = new Position(otherX, otherY);
                                col.underneathBall.SetPosition(otherX, otherY);

                                distX = currX - otherX;
                                distY = currY - otherY;

                                distSq = (distX * distX) + (distY * distY);
                                if (distSq == 0) continue;

                                double dVx = ball.Velocity.x - col.underneathBall.Velocity.x;
                                double dVy = ball.Velocity.y - col.underneathBall.Velocity.y;

                                double dotProduct = (dVx * distX) + (dVy * distY);

                                if (dotProduct > 0) continue;

                                double changeX = distX * (dotProduct / distSq);
                                double changeY = distY * (dotProduct / distSq);

                                ball.SetVelocity(ball.Velocity.x - changeX, ball.Velocity.y - changeY);
                                col.underneathBall.SetVelocity(col.underneathBall.Velocity.x + changeX, col.underneathBall.Velocity.y + changeY);
                            }
                        }
                    }

                    logicBall.UpdatePosition(currX, currY);
                }
            }
        }

        private bool Disposed = false;

        private readonly Data.DataAbstractAPI layerBelow;

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }
}
