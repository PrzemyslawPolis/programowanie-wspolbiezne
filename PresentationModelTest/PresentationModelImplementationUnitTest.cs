
using static PresentationModel.Test.BallModelUnitTest;

namespace PresentationModel.Test
{
    [TestClass]
    public class PresentationModelImplementationUnitTest
    {
        internal class FakeBusinessLogicAPI : BusinessLogic.BusinessLogicAbstractAPI
        {
            public int StartedBallsCount { get; private set; } = 0;

            public override void Start(int numberOfBalls, Action<BusinessLogic.IPosition, BusinessLogic.IBall> upperLayerHandler)
            {
                StartedBallsCount = numberOfBalls;
                FakePosition fakePos = new() { x = 50, y = 50 };
                FakeLogicBall fakeBall = new();

                upperLayerHandler(fakePos, fakeBall);
            }

            public bool DisposeCalled { get; private set; } = false;
            public override void Dispose()
            {
                DisposeCalled = true;
            }
        }

        [TestMethod]
        public void StartMethodTestMethod()
        {
            FakeBusinessLogicAPI fakeBusinessLayer = new();
            PresentationModelImplementation newInstance = new(fakeBusinessLayer);

            newInstance.Start(5);

            Assert.AreEqual(5, fakeBusinessLayer.StartedBallsCount);
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            FakeBusinessLogicAPI fakeBusinessLayer = new();
            PresentationModelImplementation newInstance = new(fakeBusinessLayer);

            bool newInstanceDisposed = false;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);

            Assert.IsFalse(newInstanceDisposed);
            Assert.IsFalse(fakeBusinessLayer.DisposeCalled);

            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.IsTrue(fakeBusinessLayer.DisposeCalled);

            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
        }

        [TestMethod]
        public void ViewSubscriptionTestMethod()
        {
            FakeBusinessLogicAPI fakeLogic = new();
            PresentationModelImplementation presentationModel = new(fakeLogic);

            var emittedBalls = new List<PresentationModel.IBall>();

            using (IDisposable? subscription = presentationModel.Subscribe(ball => emittedBalls.Add(ball)))
            {
                presentationModel.Start(1);
            }

            Assert.AreEqual(1, emittedBalls.Count);
            IBall? receivedBall = emittedBalls[0];

            Assert.AreEqual(40, receivedBall.Left);
            Assert.AreEqual(40, receivedBall.Top);
        }

    }
}
