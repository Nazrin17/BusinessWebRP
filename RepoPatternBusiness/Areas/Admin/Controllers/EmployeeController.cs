using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using No10.Context;
using No10.Context.Repositories.Abstract;
using No10.Dtos.Employee;
using No10.Helpers;
using No10.Models;
using System.Data;

namespace No10.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "admin")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public EmployeeController( IMapper mapper, IWebHostEnvironment env, IEmployeeRepository repository)
        {
            _mapper = mapper;
            _env = env;
            _repository = repository;
        }

        public async Task< IActionResult >Index()
        {
            List<Employee> emps =await _repository.GetAllAsync("Icons");
            if (emps == null)
            {
                return View();
            }
            List<EmployeeGetDto> getdtos = _mapper.Map < List < EmployeeGetDto >> (emps);
            return View(getdtos);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async  Task<IActionResult>Create(EmployeePostDto postDto)
        {
            Employee emp = _mapper.Map<Employee>(postDto);
            if (!ModelState.IsValid)
            {
                foreach (var item in postDto.Icons)
                {
                    if (item.IconName == null || item.IconUrl == null)
                    {
                        ModelState.AddModelError("Icons", "Field is required");
                    }
                }
                //var errors=  ModelState.Values.SelectMany(e => e.Errors).ToList();
                //foreach (var item in errors)
                //{
                //    ModelState.AddModelError("", item.ErrorMessage);

                //}

                return View();
            }
            string imagename = Guid.NewGuid() + postDto.formFile.FileName;
            string path=Path.Combine(_env.WebRootPath,"assets/img/team",imagename);
            using (FileStream fileStream = new FileStream(path,FileMode.Create))
            {
                postDto.formFile.CopyTo(fileStream);
            }
            emp.Image = imagename;
            foreach (var item in postDto.Icons)
            {
                emp.Icons.Add(item);
            }
           await  _repository.CreateAsync(emp);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            Employee emp = await _repository.Get(e => e.Id == id, "Icons");
            if (emp == null)
            {
                return NotFound();
            }
            Helper.DeleteFile(_env.WebRootPath, emp.Image, "assets/img/team");
            _repository.Delete(emp);
           return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            Employee emp = await _repository.Get(e => e.Id == id, "Icon");
            if (emp == null) 
            {
                return NotFound();
            }
            EmployeeGetDto getDto = _mapper.Map<EmployeeGetDto>(emp);
            EmployeeUpdateDto updateDto = new EmployeeUpdateDto { getDto = getDto };
            return View(updateDto) ;
        }
        [HttpPost]
        public async Task<IActionResult> Update(EmployeeUpdateDto updateDto)
        {
            Employee emp = await _repository.Get(e=>e.Id==updateDto.getDto.Id,"Icons");
            if (emp == null)
            {
                return NotFound();
            }
            emp.Position = updateDto.PostDto.Position;
            emp.Name = updateDto.PostDto.Name;
            if (updateDto.PostDto.formFile != null)
            {
                string imagename = Guid.NewGuid() + updateDto.PostDto.formFile.FileName;
                string path = Path.Combine(_env.WebRootPath, "assets/img/team", imagename);
                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    updateDto.PostDto.formFile.CopyTo(fileStream);
                }
                Helper.DeleteFile(_env.WebRootPath, emp.Image, "assets/img/team");
                emp.Image = imagename;
            }
            if(updateDto.PostDto.Icons != null)
            {
             for (int i = 0; i < updateDto.PostDto.Icons.Count; i++)
             {
                emp.Icons[i] = updateDto.PostDto.Icons[i];
             }
            }
            else { emp.Icons = null; }
            _repository.Update(emp);
            return RedirectToAction(nameof(Index));
        }
    }
}
