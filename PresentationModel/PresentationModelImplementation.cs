using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;

namespace PresentationModel
{
    internal class PresentationModelImplementation : PresentationModelAbstractAPI
    {
        internal PresentationModelImplementation() : this(null)
        { }

        internal PresentationModelImplementation(BusinessLogic.BusinessLogicAbstractAPI underneathLayer)
        {
            layerBelow = underneathLayer == null ? BusinessLogic.BusinessLogicAbstractAPI.GetBusinessLogicLayer() : underneathLayer;
            eventObservable = Observable.FromEventPattern<BallChangeEventArgs>(this, "BallChanged");
        }


        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(PresentationModel));
            layerBelow.Dispose();
            Disposed = true;
        }

        public override IDisposable Subscribe(IObserver<IBall> observer)
        {
            return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
        }

        public override void Start(int numberOfBalls)
        {
            layerBelow.Start(numberOfBalls, StartHandler);
        }


        public event EventHandler<BallChangeEventArgs> BallChanged;


        private bool Disposed = false;
        private readonly IObservable<EventPattern<BallChangeEventArgs>> eventObservable = null;
        private readonly BusinessLogic.BusinessLogicAbstractAPI layerBelow = null;

        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
        {
            double diameter = BusinessLogic.BusinessLogicAbstractAPI.GetDimensions.BallDimension;
            BallModel newBall = new BallModel(position.x, position.y, diameter, ball);
            BallChanged?.Invoke(this, new BallChangeEventArgs() { Ball = newBall });
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }

    public class BallChangeEventArgs : EventArgs
    {
        public IBall Ball { get; init; }
    }
}

