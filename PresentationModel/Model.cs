using BusinessLogic;

namespace PresentationModel
{
    public class Model
    {
        public Model() { }

        public string GetString()
        {
            BorderDetection program = new BorderDetection();
            return program.GetString();
        }
    }
}
