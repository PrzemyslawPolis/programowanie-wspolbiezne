using BusinessLogic;

namespace PresentationModel
{
    public class Model
    {
        public Model() { }

        public string GetString()
        {
            Program program = new Program();
            return program.GetString();
        }
    }
}
