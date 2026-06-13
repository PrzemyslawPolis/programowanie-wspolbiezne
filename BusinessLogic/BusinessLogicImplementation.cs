using Data;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private QuadTree activeTree = new QuadTree(new Boundary(0, 0, GetDimensions.TableWidth, GetDimensions.TableHeight));
        private bool isTaskRunning = false;
        private ConcurrentDictionary<Data.IBall, Ball> BallDict = new();
        private bool Disposed = false;
        private readonly Data.DataAbstractAPI layerBelow;

        private readonly object treeLock = new object();

        public BusinessLogicImplementation() : this(null) { }

        internal BusinessLogicImplementation(Data.DataAbstractAPI? underneathLayer)
        {
            layerBelow = underneathLayer == null ? Data.DataAbstractAPI.GetDataLayer() : underneathLayer;
        }

        public override void Dispose()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBelow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null) throw new ArgumentNullException(nameof(upperLayerHandler));

            BallDict.Clear();
            activeTree = new QuadTree(new Boundary(0, 0, GetDimensions.TableWidth, GetDimensions.TableHeight));

            layerBelow.Start(numberOfBalls, (startingPosition, dataBall) =>
            {
                Position newPosition = new Position(startingPosition.x, startingPosition.y);
                Ball logicBall = new Ball(newPosition, dataBall);

                BallDict.TryAdd(dataBall, logicBall);

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

                        foreach (Ball logicBall in BallDict.Values)
                        {
                            newTree.Insert(logicBall);
                        }

                        lock (treeLock)
                        {
                            activeTree = newTree;
                        }
                        await Task.Delay(15);
                    }
                });
            }
        }

        private void OnBallPositionNotification(object? sender, IVector newPos)
        {
            if (sender == null) return;
            Data.IBall ball = (Data.IBall)sender;

            if (BallDict.TryGetValue(ball, out var logicBall))
            {
                double radius = BusinessLogicAbstractAPI.GetDimensions.BallDimension / 2;
                double width = BusinessLogicAbstractAPI.GetDimensions.TableWidth;
                double height = BusinessLogicAbstractAPI.GetDimensions.TableHeight;

                lock (logicBall.BallLock)
                {
                    double currX = newPos.x;
                    double currY = newPos.y;
                    bool changedPos = false;

                    if ((currX <= radius && ball.Velocity.x < 0) || (currX >= width - radius && ball.Velocity.x > 0))
                    {
                        currX = Math.Clamp(currX, radius, width - radius);
                        ball.SetVelocity(-ball.Velocity.x, ball.Velocity.y);
                        changedPos = true;
                    }
                    if ((currY <= radius && ball.Velocity.y < 0) || (currY >= height - radius && ball.Velocity.y > 0))
                    {
                        currY = Math.Clamp(currY, radius, height - radius);
                        ball.SetVelocity(ball.Velocity.x, -ball.Velocity.y);
                        changedPos = true;
                    }

                    if (changedPos)
                    {
                        ball.SetPosition(currX, currY);
                    }

                    logicBall.position = new Position(currX, currY);
                }

                List<Ball> collisionBalls = new();
                QuadTree currentTreeSnapshot;

                lock (treeLock) { currentTreeSnapshot = activeTree; }

                currentTreeSnapshot.Query(new Boundary(logicBall.position.x - radius, logicBall.position.y - radius, radius * 4, radius * 4), collisionBalls);

                foreach (Ball col in collisionBalls)
                {
                    if (col.Id == logicBall.Id) continue;

                    object lock1 = logicBall.Id < col.Id ? logicBall.BallLock : col.BallLock;
                    object lock2 = logicBall.Id < col.Id ? col.BallLock : logicBall.BallLock;

                    lock (lock1)
                    {
                        lock (lock2)
                        {
                            double currX = logicBall.position.x;
                            double currY = logicBall.position.y;
                            double otherX = col.position.x;
                            double otherY = col.position.y;

                            double distX = otherX - currX;
                            double distY = otherY - currY;
                            double distSq = (distX * distX) + (distY * distY);

                            if (distSq == 0) continue; 

                            double dist = Math.Sqrt(distSq);
                            double overlap = radius * 2 - dist;

                            if (overlap > 0)
                            {
                                double moveX = (distX / dist) * (overlap / 2.0);
                                double moveY = (distY / dist) * (overlap / 2.0);

                                currX -= moveX;
                                currY -= moveY;

                                otherX += moveX;
                                otherY += moveY;

                                currX = Math.Clamp(currX, radius, width - radius);
                                currY = Math.Clamp(currY, radius, height - radius);

                                otherX = Math.Clamp(otherX, radius, width - radius);
                                otherY = Math.Clamp(otherY, radius, height - radius);

                                ball.SetPosition(currX, currY);
                                logicBall.position = new Position(currX, currY);

                                col.underneathBall.SetPosition(otherX, otherY);
                                col.position = new Position(otherX, otherY);

                                double dVx = ball.Velocity.x - col.underneathBall.Velocity.x;
                                double dVy = ball.Velocity.y - col.underneathBall.Velocity.y;

                                double dotProduct = (dVx * distX) + (dVy * distY);

                                if (dotProduct > 0)
                                {
                                    double changeX = distX * (dotProduct / distSq);
                                    double changeY = distY * (dotProduct / distSq);

                                    ball.SetVelocity(ball.Velocity.x - changeX, ball.Velocity.y - changeY);
                                    col.underneathBall.SetVelocity(col.underneathBall.Velocity.x + changeX, col.underneathBall.Velocity.y + changeY);
                                }
                            }
                        }
                    }
                }

                logicBall.UpdatePosition(logicBall.position.x, logicBall.position.y);
            }
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }
}