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
    public class SubscriptionsController : Controller
    {
        private readonly GymDbContext _context;

        public SubscriptionsController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Subscriptions
        public async Task<IActionResult> Index()
        {
            var gymDbContext = _context.Subscriptions.Include(s => s.Member).Include(s => s.Payment).Include(s => s.Plan);
            return View(await gymDbContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var memberId = HttpContext.Session.GetInt32("MNId");
            if (memberId is null) return RedirectToAction("Login", "Authentication");

            var mySubs = await _context.Subscriptions
                .AsNoTracking()
                .Include(s => s.Plan)
                .Include(s => s.Payment)
                .Where(s => s.Memberid == memberId.Value)
                .OrderByDescending(s => s.Startdate)
                .ToListAsync();

            return View( mySubs);
        }

        

        [HttpGet]
        public async Task<IActionResult> Subscribe(long planId)
        {
            var plan = await _context.Membershipplans.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Planid == planId && p.Status == true);
            if (plan is null) return NotFound();


            if (string.Equals(plan.Plantype, "Personal", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Trainers = await _context.Trainers.AsNoTracking().ToListAsync();
            }
            else
            {
                ViewBag.Trainers = null;
            }

            return View("Subscribe", plan); 
        }




        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.Payment)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(m => m.Subscriptionid == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

    
        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewData["Memberid"] = new SelectList(_context.Members, "Memberid", "Memberid", subscription.Memberid);
            ViewData["Paymentid"] = new SelectList(_context.Payments, "Paymentid", "Paymentid", subscription.Paymentid);
            ViewData["Planid"] = new SelectList(_context.Membershipplans, "Planid", "Planid", subscription.Planid);
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Subscriptionid,Memberid,Planid,Startdate,Enddate,Paymentid,Status,Createdat")] Subscription subscription)
        {
            if (id != subscription.Subscriptionid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.Subscriptionid))
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
            ViewData["Memberid"] = new SelectList(_context.Members, "Memberid", "Memberid", subscription.Memberid);
            ViewData["Paymentid"] = new SelectList(_context.Payments, "Paymentid", "Paymentid", subscription.Paymentid);
            ViewData["Planid"] = new SelectList(_context.Membershipplans, "Planid", "Planid", subscription.Planid);
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.Payment)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(m => m.Subscriptionid == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(long id)
        {
            return _context.Subscriptions.Any(e => e.Subscriptionid == id);
        }
    }
}
