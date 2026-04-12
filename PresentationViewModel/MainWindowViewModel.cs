using System.Collections.ObjectModel;
using PresentationModel;
using PresentationViewModel.MVVMLight;

namespace PresentationViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(PresentationModelAbstractAPI modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? PresentationModelAbstractAPI.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<PresentationModel.IBall>(x => Balls.Add(x));
        }

        #endregion ctor

        #region public API

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            Observer.Dispose();
        }

        public ObservableCollection<PresentationModel.IBall> Balls { get; } = new ObservableCollection<PresentationModel.IBall>();

        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private PresentationModelAbstractAPI ModelLayer;
        private bool Disposed = false;

        #endregion private
    }
}