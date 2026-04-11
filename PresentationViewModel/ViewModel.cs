namespace PresentationViewModel
{
    public class ViewModel
    {
        public ViewModel() { }

        public string GetString()
        {
            PresentationModel.BallModel model = new PresentationModel.BallModel();
            return model.GetString();
        }
    }
}
