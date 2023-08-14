using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDo_List.Models;
using ToDo_List.ViewModels;

namespace ToDo_List.Controllers
{
    public class ListController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserModel> _userManager;

        public ListController(AppDbContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var tasks = _context.TaskTabl.Where(t => t.User == user).ToList();
            return View(tasks);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTaskViewModel taskViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User); 
                var task = new TaskModel
                {
                    Title = taskViewModel.Title,
                    Description = taskViewModel.Description,
                    User = user,
                    IsCompleted = false
                };

                _context.TaskTabl.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.TaskTabl.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var taskViewModel = new EditTaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description
            };

            return View(taskViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditTaskViewModel taskViewModel)
        {
            if (id != taskViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var task = await _context.TaskTabl.FindAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                task.Title = taskViewModel.Title;
                task.Description = taskViewModel.Description;

                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(taskViewModel);
        }
        public ActionResult Delete(int id)
        {
            var task = _context.TaskTabl.Find(id);
            _context.TaskTabl.Remove(task);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, bool isCompleted)
        {
            var task = await _context.TaskTabl.FindAsync(taskId);
            if (task == null)
            {
                return NotFound();
            }

            task.IsCompleted = isCompleted;
            _context.Update(task);
            await _context.SaveChangesAsync();

            return Ok(); 
        }
    }
}
