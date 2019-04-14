using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DBLibrary;
using DBLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppBallet.Models;

namespace WebAppBallet.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext _context;
        public HomeController(ApplicationContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Directory()
        {
            List<UserModel> list = new List<UserModel>();
            List<UserModel> users = new List<UserModel>();
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _context.Users.FirstOrDefaultAsync(s => string.Compare(s.Login, User.Identity.Name) == 0);
                List<Friend> userFriends = await _context.Friends.Where(f => f.AddedById == currentUser.Id).ToListAsync();
                foreach (var friend in userFriends)
                {
                    users.Add(await _context.Users.Include(u => u.Department).Where(u => u.Id == friend.AddedToId).Select(u => new UserModel
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
                    }).FirstOrDefaultAsync());
                }
                ViewData["Chosens"] = users.OrderBy(u => u.Department.SequenceNumber).ThenBy(u => u.SequenceNumber).GroupBy(u => u.Department.DepartmentName).ToList();
                var friends = await _context.Friends.Where(f => f.AddedById == currentUser.Id).Select(f => f.AddedToId).ToListAsync();
                ViewData["Friends"] = friends;
            }
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

        public IActionResult AllDirectory()
        {
            return PartialView("_AllDirectory");
        }

        public IActionResult ChosenDirectory()
        {
            return PartialView("_ChosenDirectory");
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddFriend(int AddedToId)
        {
            User currentUser = _context.Users.FirstOrDefault(s => string.Compare(s.Login, User.Identity.Name) == 0);
            var status = false;
            using (_context)
            {
                var friend = _context.Friends.FirstOrDefault(f => f.AddedToId == AddedToId && f.AddedById == currentUser.Id);
                if (_context.Friends.Contains(friend))
                    _context.Friends.Remove(_context.Friends.First(f => f.AddedToId == AddedToId && f.AddedById == currentUser.Id));
                else
                    _context.Friends.Add(new Friend
                    {
                        AddedBy = currentUser,
                        AddedById = currentUser.Id,
                        AddedToId = AddedToId,
                        AddedTo = _context.Users.First(u => u.Id == AddedToId)
                    });
                _context.SaveChanges();
                status = true;
            }
            return Json(status);
            //new JsonResult { Data = new { status = status } };
        }
         
        public string GetCulture(string code = "")
        {
            if (!String.IsNullOrEmpty(code))
            {
                CultureInfo.CurrentCulture = new CultureInfo(code);
                CultureInfo.CurrentUICulture = new CultureInfo(code);
            }
            return $"CurrentCulture:{CultureInfo.CurrentCulture.Name}, CurrentUICulture:{CultureInfo.CurrentUICulture.Name}";
        }

        public IActionResult Schedule()
        {
            return View();
        }

        public async Task<JsonResult> GetEvents()
        {
            using (_context)
            {
                var events = await _context.Events.ToListAsync();
                return new JsonResult(events);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,producer")]
        public async Task<JsonResult> SaveEvent(Event e)
        {
            var status = false;
            using (_context)
            {
                if (e.Id > 0)
                {
                    //Update the event
                    var v = await _context.Events.Where(a => a.Id == e.Id).FirstOrDefaultAsync();
                    if (v != null)
                    {
                        v.Title = e.Title;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.Color = e.Color;
                    }
                }
                else
                {
                    if (e.IsFullDay)
                    {
                        var str = e.Start.GetDateTimeFormats();
                        _context.Events.Add(e);
                    }
                    else
                    {
                        DateTime i = e.Start;
                        do
                        {
                            Event addEvent = new Event();
                            addEvent.Title = e.Title;
                            addEvent.Description = e.Description;
                            addEvent.Start = i;
                            if (e.Start.DayOfYear == e.End.Value.DayOfYear)
                                addEvent.End = e.End;
                            else addEvent.End = i.AddHours(e.End.Value.Hour - i.Hour);
                            addEvent.IsFullDay = e.IsFullDay;
                            addEvent.Color = e.Color;
                            _context.Events.Add(addEvent);
                            i = i.AddDays(1);

                        } while (i <= e.End);
                    }
                    /*for (DateTime i = e.Start; i <= e.End; i = i.AddDays(1))
                    {
                        Event addEvent = new Event();
                        addEvent.Subject = e.Subject;
                        addEvent.Description = e.Description;
                        addEvent.Start = i;
                        addEvent.End = i;
                        addEvent.IsFullDay = e.IsFullDay;
                        addEvent.ThemeColor = e.ThemeColor;
                        dc.Events.Add(addEvent);
                    }*/
                    //dc.Events.Add(e);
                }

                _context.SaveChanges();
                status = true;

            }
            return new JsonResult(status);
            //return Json(status);
        }

        [HttpPost]
        [Authorize(Roles = "admin,producer")]
        public async Task<JsonResult> DeleteEvent(int Id)
        {
            var status = false;
            using (_context)
            {
                var v = await _context.Events.Where(a => a.Id == Id).FirstOrDefaultAsync();
                if (v != null)
                {
                    _context.Events.Remove(v);
                    _context.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult(status);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
