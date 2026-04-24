using PresentationModel;
using System.ComponentModel;

namespace PresentationViewModel.Test
{
    [TestClass]
    public sealed class MainWindowViewModelUnitTest
    {
        internal class FakePresentationModelAPI : PresentationModelAbstractAPI
        {
            public int LastStartedCount { get; private set; }
            public override void Start(int numberOfBalls) => LastStartedCount = numberOfBalls;
            public IObserver<IBall> Observer { get; private set; }
            public override IDisposable Subscribe(IObserver<IBall> observer)
            {
                Observer = observer;
                return new DummyDisposable();
            }
            public bool DisposeCalled { get; private set; } = false;
            public override void Dispose()
            {
                DisposeCalled = true;
            }
            private class DummyDisposable : IDisposable
            {
                public void Dispose() { }
            }
        }

        internal class FakeBall : IBall
        {
            public double Top => throw new NotImplementedException();

            public double Left => throw new NotImplementedException();

            public double Diameter => throw new NotImplementedException();

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        [TestMethod]
        public void StartTestMethod()
        {
            FakePresentationModelAPI mockModel = new();
            MainWindowViewModel viewModel = new MainWindowViewModel(mockModel);
            viewModel.BallsCount = 5; 
            viewModel.StartCommand.Execute(null);

            Assert.AreEqual(5, mockModel.LastStartedCount);

            viewModel.BallsCount = 7;
            viewModel.StartCommand.Execute(null);
            Assert.AreEqual(7, mockModel.LastStartedCount);
        }

        [TestMethod]
        public void SubscriptionTestMethod()
        {           
            FakePresentationModelAPI fakeModel = new();
            MainWindowViewModel viewModel = new MainWindowViewModel(fakeModel);
            FakeBall testBall = new();

            fakeModel.Observer.OnNext(testBall);

            Assert.AreEqual(1, viewModel.Balls.Count);
            Assert.AreSame(testBall, viewModel.Balls[0]);
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            FakePresentationModelAPI fakePresentationModelLayer = new();
            MainWindowViewModel newInstance = new(fakePresentationModelLayer);

            bool newInstanceDisposed = false;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);

            Assert.IsFalse(newInstanceDisposed);
            Assert.IsFalse(fakePresentationModelLayer.DisposeCalled);

            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.IsTrue(fakePresentationModelLayer.DisposeCalled);

            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
        }
    }
}