namespace Model.Models.Testing.Load
{
    public interface ITestMaker
    {
        CTest Make(string setting, int id);
    }
}
