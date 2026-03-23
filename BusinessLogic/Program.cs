using Data;

namespace BusinessLogic
{
    public class Program
    {
        public Program() { }
        Class1 data = new Class1();
        public string GetString()
        {
            return data.GetData();
        }
    }
}