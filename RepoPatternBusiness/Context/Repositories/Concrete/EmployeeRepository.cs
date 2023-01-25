using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using No10.Context.Repositories.Abstract;
using No10.Models;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace No10.Context.Repositories.Concrete
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly BusinessDbContext _context;

        public EmployeeRepository(BusinessDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Employee employee)
        {
           await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public void Delete(Employee employee)
        {
            _context.Remove(employee);
            _context.SaveChanges();
        }

        public async Task<Employee> Get(Expression<Func<Employee,bool>> exp = null, params string[] includes)
        {
           IQueryable<Employee> query=_context.Employees;
            if (includes is not null)
            {
                foreach (var item in includes)
                {
                    query.Include(item);
                }
            }
            return exp == null ?  await query.FirstOrDefaultAsync() :await  query.Where(exp).FirstOrDefaultAsync();

        }

        public async Task<Employee> Get(params string[] includes)
        {
            IQueryable<Employee> query = _context.Employees;
            if (includes is not null)
            {
                foreach (var item in includes)
                {
                    query.Include(item);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Employee>> GetAllAsync(Expression<Func<Employee,bool>> exp = null, params string[] includes)
        {
            IQueryable<Employee> query=_context.Employees;
            if(includes is not null)
            {
                foreach (var item in includes)
                {
                    query.Include(item);
                }
            }
            return exp == null ?await query.ToListAsync(): await query.Where(exp).ToListAsync();
        }

        public async Task<List<Employee>> GetAllAsync(params string[] includes)
        {
            IQueryable<Employee> query = _context.Employees;
            if (includes is not null)
            {
                foreach (var item in includes)
                {
                    query.Include(item);
                }
            }
            return await query.ToListAsync();
        }

        public void Update(Employee employee)
        {
           _context.Update(employee);
            _context.SaveChanges();
        }
    }
}
