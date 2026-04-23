using Data;
using System.Numerics;

namespace BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationTest
    {
        internal class FakeBall : Data.IBall
        {
            public event EventHandler<IVector>? NewPositionNotification;

            public IVector Velocity { get; set; } = new FakeVector(0, 0);
            public double X { get; set; }
            public double Y { get; set; }

            public void SetPosition(double x, double y)
            {
                X = x; Y = y;
            }

            public void SetVelocity(double x, double y)
            {
                Velocity = new FakeVector(x, y);
            }

            public void SimulateMove(double newX, double newY)
            {
                X = newX;
                Y = newY;
                NewPositionNotification?.Invoke(this, new FakeVector(newX, newY));
            }
        }

        internal class FakeDataAPI : Data.DataAbstractAPI
        {
            public List<FakeBall> CreatedBalls { get; } = new List<FakeBall>();

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> handler)
            {
                for (int i = 0; i < numberOfBalls; i++)
                {
                    var fakeBall = new FakeBall();
                    CreatedBalls.Add(fakeBall);


                    handler(new FakeVector(100, 100), fakeBall);
                }
            }

            public override void Dispose() { }
        }

        internal class FakeVector : Data.IVector {
            public double x { get; init; }
            public double y { get; init; }


            public FakeVector(double XComponent, double YComponent)
            {
                x = XComponent;
                y = YComponent;
            }
        }

        [TestMethod]
        public void WallBounceTestMethod()
        {
            var fakeData = new FakeDataAPI();

            double radius = BusinessLogicAbstractAPI.GetDimensions.BallDimension / 2;
            double height = BusinessLogicAbstractAPI.GetDimensions.TableHeight;

            var logic = new BusinessLogicImplementation(fakeData);

            logic.Start(1, (pos, logicBall) => { });

            fakeData.CreatedBalls[0].Velocity = new FakeVector(-5.0, 10.0);

            fakeData.CreatedBalls[0].SimulateMove(-10.0, 100.0);

            Assert.AreEqual(5, fakeData.CreatedBalls[0].Velocity.x);
            Assert.AreEqual(10, fakeData.CreatedBalls[0].Velocity.y);
            Assert.AreEqual(0+radius, fakeData.CreatedBalls[0].X);
            Assert.AreEqual(100, fakeData.CreatedBalls[0].Y);

            fakeData.CreatedBalls[0].Velocity = new FakeVector(-5.0, 10.0);
            fakeData.CreatedBalls[0].SimulateMove(-10.0, height);

            Assert.AreEqual(5, fakeData.CreatedBalls[0].Velocity.x);
            Assert.AreEqual(-10, fakeData.CreatedBalls[0].Velocity.y);
            Assert.AreEqual(0 + radius, fakeData.CreatedBalls[0].X);
            Assert.AreEqual(height-radius, fakeData.CreatedBalls[0].Y);
        }
    }
}
