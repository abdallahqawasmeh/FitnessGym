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
    public class MembershipplansController : Controller
    {
        private readonly GymDbContext _context;

        public MembershipplansController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Membershipplans
        public async Task<IActionResult> Index()
        {
            var gymDbContext = _context.Membershipplans.Include(m => m.Createdbyadmin);
            return View(await gymDbContext.ToListAsync());
        }

        // GET: Membershipplans/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipplan = await _context.Membershipplans
                .Include(m => m.Createdbyadmin)
                .FirstOrDefaultAsync(m => m.Planid == id);
            if (membershipplan == null)
            {
                return NotFound();
            }

            return View(membershipplan);
        }

        // GET: Membershipplans/Create
        public IActionResult Create()
        {
            ViewData["Createdbyadminid"] = new SelectList(_context.Admins, "Adminid", "Adminid");
            return View();
        }

        // POST: Membershipplans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Planid,Planname,Description,Description1,Description2,Description3,Description4,Durationmonths,Price,Createdbyadminid,Plantype,Status")] Membershipplan membershipplan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershipplan);
                await _context.SaveChangesAsync();

              
                return RedirectToAction(nameof(Index));
            }
            ViewData["Createdbyadminid"] = new SelectList(_context.Admins, "Adminid", "Adminid", membershipplan.Createdbyadminid);
            return View(membershipplan);
        }

        // GET: Membershipplans/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipplan = await _context.Membershipplans.FindAsync(id);
            if (membershipplan == null)
            {
                return NotFound();
            }
            ViewData["Createdbyadminid"] = new SelectList(_context.Admins, "Adminid", "Adminid", membershipplan.Createdbyadminid);
            return View(membershipplan);
        }

        // POST: Membershipplans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Planid,Planname,Description,Description1,Description2,Description3,Description4,Durationmonths,Price,Createdbyadminid,Plantype,Status")] Membershipplan membershipplan)
        {
            if (id != membershipplan.Planid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membershipplan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipplanExists(membershipplan.Planid))
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
            ViewData["Createdbyadminid"] = new SelectList(_context.Admins, "Adminid", "Adminid", membershipplan.Createdbyadminid);
            return View(membershipplan);
        }

        // GET: Membershipplans/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipplan = await _context.Membershipplans
                .Include(m => m.Createdbyadmin)
                .FirstOrDefaultAsync(m => m.Planid == id);
            if (membershipplan == null)
            {
                return NotFound();
            }

            return View(membershipplan);
        }

        // POST: Membershipplans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var membershipplan = await _context.Membershipplans.FindAsync(id);
            if (membershipplan != null)
            {
                _context.Membershipplans.Remove(membershipplan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipplanExists(long id)
        {
            return _context.Membershipplans.Any(e => e.Planid == id);
        }
    }
}
