using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicesInterfaces
{
    public interface IManager : IGrainWithGuidKey
    {
        Task<IEmployee> AsEmployee();
        Task<List<IEmployee>> GetDirectReports();
        Task AddDirectReport(IEmployee employee);
    }
}