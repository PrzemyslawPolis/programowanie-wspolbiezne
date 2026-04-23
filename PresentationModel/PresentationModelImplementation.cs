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
            layerBellow = underneathLayer == null ? BusinessLogic.BusinessLogicAbstractAPI.GetBusinessLogicLayer() : underneathLayer;
            eventObservable = Observable.FromEventPattern<BallChangeEventArgs>(this, "BallChanged");
        }


        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(PresentationModel));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override IDisposable Subscribe(IObserver<IBall> observer)
        {
            return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
        }

        public override void Start(int numberOfBalls)
        {
            layerBellow.Start(numberOfBalls, StartHandler);
        }


        public event EventHandler<BallChangeEventArgs> BallChanged;


        private bool Disposed = false;
        private readonly IObservable<EventPattern<BallChangeEventArgs>> eventObservable = null;
        private readonly BusinessLogic.BusinessLogicAbstractAPI layerBellow = null;

        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
        {
            double diameter = BusinessLogic.BusinessLogicAbstractAPI.GetDimensions.BallDimension;
            BallModel newBall = new BallModel(position.x, position.y, ball) { Diameter = diameter };
            BallChanged.Invoke(this, new BallChangeEventArgs() { Ball = newBall });
        }


        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        [Conditional("DEBUG")]
        internal void CheckUnderneathLayerAPI(Action<BusinessLogic.BusinessLogicAbstractAPI> returnNumberOfBalls)
        {
            returnNumberOfBalls(layerBellow);
        }

        [Conditional("DEBUG")]
        internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull)
        {
            returnBallChangedIsNull(BallChanged == null);
        }
    }

    public class BallChangeEventArgs : EventArgs
    {
        public IBall Ball { get; init; }
    }
}

