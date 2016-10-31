using Orleans;
using System.Threading.Tasks;

namespace ServicesInterfaces
{
    /// <summary>
    /// Grain interface IEmployee
    /// </summary>
	public interface IEmployee : IGrainWithGuidKey
    {
        Task<int> GetLevel();
        Task Promote(int newLevel);

        Task<IManager> GetManager();
        Task SetManager(IManager manager);

        Task Greeting(GreetingData data);
    }
}
