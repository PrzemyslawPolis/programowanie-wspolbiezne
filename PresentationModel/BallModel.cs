using BusinessLogic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PresentationModel
{
    public class BallModel : IBall
    {
        public BallModel(double left, double top, double diameter, BusinessLogic.IBall underneathBall)
        {
            this.Diameter = diameter;
            double radius = Diameter / 2;
            TopBackingField = top - radius;
            LeftBackingField = left - radius;
            underneathBall.NewPositionNotification += OnNewPosition;
        }


        public double Top
        {
            get { return TopBackingField; }
            private set
            {
                if (TopBackingField == value) return;
                TopBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Left
        {
            get { return LeftBackingField; }
            private set
            {
                if (LeftBackingField == value) return;
                LeftBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter { get; init; } = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        private double TopBackingField;
        private double LeftBackingField;

        private void OnNewPosition(object sender, IPosition pos)
        {
            //Koordynaty w oknie to Top/Left (odległość od krawędzi ekranu), i liczą się względem lewego górnego rogu, a nie środka
            double radius = Diameter / 2;
            Top = pos.y - radius;
            Left = pos.x - radius;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
