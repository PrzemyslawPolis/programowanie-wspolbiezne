namespace PresentationViewModel
{
    public class ViewModel
    {
        public ViewModel() { }

        public string GetString()
        {
            PresentationModel.Model model = new PresentationModel.Model();
            return model.GetString();
        }
    }
}
