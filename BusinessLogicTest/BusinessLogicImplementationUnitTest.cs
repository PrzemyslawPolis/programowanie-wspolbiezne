using Data;
using System.Numerics;

namespace BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
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
                    FakeBall fakeBall = new();
                    CreatedBalls.Add(fakeBall);


                    handler(new FakeVector(100, 100), fakeBall);
                }
            }
            
            public bool DisposeCalled { get; private set; } = false;

            public override void Dispose() 
            { 
                DisposeCalled = true;
            }
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
            FakeDataAPI fakeData = new();

            double radius = BusinessLogicAbstractAPI.GetDimensions.BallDimension / 2;
            double height = BusinessLogicAbstractAPI.GetDimensions.TableHeight;

            BusinessLogicImplementation logic = new(fakeData);

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

        [TestMethod]
        public void DisposeTestMethod()
        {
            FakeDataAPI fakeDataLayer = new();
            BusinessLogicImplementation newInstance = new(fakeDataLayer);

            bool newInstanceDisposed = false;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);

            Assert.IsFalse(newInstanceDisposed);
            Assert.IsFalse(fakeDataLayer.DisposeCalled);

            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.IsTrue(fakeDataLayer.DisposeCalled);

            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
        }
    }
}
