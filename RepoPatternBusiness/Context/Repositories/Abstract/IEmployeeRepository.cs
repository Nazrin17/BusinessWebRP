using No10.Models;
using System.Linq.Expressions;

namespace No10.Context.Repositories.Abstract
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync(Expression<Func<Employee,bool>> exp = null, params string[] includes);
        Task<List<Employee>> GetAllAsync( params string[] includes);

        Task<Employee> Get(Expression<Func<Employee,bool>> exp=null,params string[] includes);
        Task<Employee> Get( params string[] includes);


        Task CreateAsync(Employee employee);
        void Update(Employee employee);
        void Delete(Employee employee);
    } 
}
