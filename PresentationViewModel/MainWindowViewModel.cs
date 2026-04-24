using PresentationModel;
using PresentationViewModel.MVVMLight;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace PresentationViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly PresentationModelAbstractAPI layerBelow;
        private IDisposable? Observer;
        private int ballsCount = 5;
        private bool Disposed = false;

        public int BallsCount
        {
            get => ballsCount;
            set { ballsCount = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<IBall> Balls { get; } = new ObservableCollection<IBall>();

        public ICommand StartCommand { get; }

        public MainWindowViewModel() : this(null) { }

        internal MainWindowViewModel(PresentationModelAbstractAPI? underneathLayer)
        {
            layerBelow = underneathLayer == null ? PresentationModel.PresentationModelAbstractAPI.CreateModel() : underneathLayer;

            Observer = layerBelow.Subscribe(ball =>
            {
                if (System.Windows.Application.Current?.Dispatcher != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => Balls.Add(ball));
                }
                else
                {
                    Balls.Add(ball);
                }
            });

            StartCommand = new RelayCommand(ExecuteStart);
        }

        private void ExecuteStart()
        {
            Balls.Clear();
            layerBelow.Start(BallsCount);
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(PresentationModel));
            Observer?.Dispose();
            layerBelow.Dispose();
            Disposed = true;
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }
}