using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DBLibrary;
using DBLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using WebAppBallet.Models;

namespace WebAppBallet.Controllers
{
    [Authorize(Roles = "admin,kip")]
    public class UsersController : Controller
    {
        private readonly ApplicationContext _context;

        public UsersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.Users.Include(u => u.Department).Include(u => u.Role);
            return View(await applicationContext.ToListAsync());
        }
        // GET: Users
        public async Task<IActionResult> List()
        {
            List<UserModel> list = new List<UserModel>();
            list = await _context.Users.Include(u => u.Department).Select(u => new UserModel
            {
                Id = u.Id,
                FIO = u.FIO,
                Position = u.Position,
                LocalNumber = u.LocalNumber,
                MobileNumber = u.MobileNumber,
                CityNumber = u.CityNumber,
                Email = u.Email,
                CabinetNumber = u.CabinetNumber,
                SequenceNumber = u.SequenceNumber,
                DepartmentId = u.DepartmentId,
                Department = u.Department
            }).ToListAsync();
            var departments = list.OrderBy(u => u.Department.SequenceNumber).ThenBy(u => u.SequenceNumber).GroupBy(u => u.Department.DepartmentName).ToList();
            return View(departments);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments.OrderBy(d => d.SequenceNumber), "Id", "DepartmentName");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleDescription");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Login,Password,FirstName,Name,LastName,Position,CityNumber,LocalNumber,MobileNumber,CabinetNumber,SequenceNumber,RoleId,DepartmentId")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments.OrderBy(d => d.SequenceNumber), "Id", "DepartmentName", user.DepartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleDescription", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Password"] = user.Password;
            ViewData["DepartmentId"] = new SelectList(_context.Departments.OrderBy(d => d.SequenceNumber), "Id", "DepartmentName", user.DepartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleDescription", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Login,Password,FirstName,Name,LastName,Position,CityNumber,LocalNumber,MobileNumber,CabinetNumber,SequenceNumber,RoleId,DepartmentId")] User user, string Pwd)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(user.Password == null)
                        user.Password = Pwd;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments.OrderBy(d => d.SequenceNumber), "Id", "DepartmentName", user.DepartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleDescription", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
