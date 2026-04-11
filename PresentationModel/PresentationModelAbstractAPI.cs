using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PresentationModel
{
    public interface IBall : INotifyPropertyChanged
    {
        double Top { get; }
        double Left { get; }
        double Diameter { get; }
    }

    public abstract class PresentationModelAbstractAPI : IObservable<IBall>, IDisposable
    {
        public static PresentationModelAbstractAPI CreateModel()
        {
            return modelInstance.Value;
        }

        public abstract void Start(int numberOfBalls);

        public abstract IDisposable Subscribe(IObserver<IBall> observer);

        public abstract void Dispose();

        private static Lazy<PresentationModelAbstractAPI> modelInstance = new Lazy<PresentationModelAbstractAPI>(() => new PresentationModelImplementation());
    }
}
