using Model.Models.Work;

namespace Model.Models.Statistics.Work
{
    public interface IWorkResultManager
    {
        void Save(int WorkID, int UserID, string Result);
        
        string GetResult(int WorkID, int UserID);
    }
}
