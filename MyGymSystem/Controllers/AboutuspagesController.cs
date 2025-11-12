using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGymSystem.Context;
using MyGymSystem.Models;

namespace MyGymSystem.Controllers
{
    public class AboutuspagesController : Controller
    {
        private readonly GymDbContext _context;

        public AboutuspagesController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Aboutuspages
        public async Task<IActionResult> Index()
        {
            var gymDbContext = _context.Aboutuspages.Include(a => a.Updatedbyadmin);
            return View(await gymDbContext.ToListAsync());
        }

        // GET: Aboutuspages/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aboutuspage = await _context.Aboutuspages
                .Include(a => a.Updatedbyadmin)
                .FirstOrDefaultAsync(m => m.Aboutuspageid == id);
            if (aboutuspage == null)
            {
                return NotFound();
            }

            return View(aboutuspage);
        }

     

        // GET: Aboutuspages/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aboutuspage = await _context.Aboutuspages.FindAsync(id);
            if (aboutuspage == null)
            {
                return NotFound();
            }
            ViewData["Updatedbyadminid"] = new SelectList(_context.Admins, "Adminid", "Adminid", aboutuspage.Updatedbyadminid);
            return View(aboutuspage);
        }

        // POST: Aboutuspages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Aboutuspageid,Missionstatement,Visionstatement,Teamdescription,Imagepath,Updatedbyadminid,Lastupdated")] Aboutuspage aboutuspage)
        {
            if (id != aboutuspage.Aboutuspageid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aboutuspage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutuspageExists(aboutuspage.Aboutuspageid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Updatedbyadminid"] = new SelectList(_context.Admins, "Adminid", "Adminid", aboutuspage.Updatedbyadminid);
            return View(aboutuspage);
        }

     

    

        private bool AboutuspageExists(long id)
        {
            return _context.Aboutuspages.Any(e => e.Aboutuspageid == id);
        }
    }
}
