namespace Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        
        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }


        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);



        public abstract void Dispose();


        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

    }

    public interface IVector
    {
        double x { get; init; }

        double y { get; init; }
    }

     

    public interface IBall
    {
        event EventHandler<IVector> NewPositionNotification;

        IVector Velocity { get; set; }

        public void SetPosition(double newX, double newY);

        public void SetVelocity(double newX, double newY);
    }
}