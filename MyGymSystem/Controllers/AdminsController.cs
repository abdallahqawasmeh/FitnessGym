using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGymSystem.Context;
using MyGymSystem.Join;
using MyGymSystem.Models;
using NuGet.ProjectModel;

namespace MyGymSystem.Controllers
{
    public class AdminsController : Controller
    {
        private readonly GymDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminsController(GymDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }




        [HttpGet]
        public IActionResult Report()
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            var subscriptions = _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.Plan)
                .ToList();

            decimal totalPrice = subscriptions.Sum(s => s.Plan?.Price ?? 0);
            decimal totalProfit = totalPrice * 0.07m;

            ViewBag.TotalPrice = totalPrice;

            // ====== NEW: data for charts ======

            // Revenue by date
            var revenueByDate = subscriptions
                .GroupBy(s => s.Startdate.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    total = g.Sum(x => x.Plan?.Price ?? 0)
                })
                .OrderBy(x => x.date)
                .ToList();

            // Subscriptions count per plan
            var subsByPlan = subscriptions
                .GroupBy(s => s.Plan.Planname)
                .Select(g => new
                {
                    plan = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .ToList();

            ViewBag.RevenueByDate = JsonSerializer.Serialize(revenueByDate);
            ViewBag.SubsByPlan = JsonSerializer.Serialize(subsByPlan);

            return View(subscriptions);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Report(DateTime? startDate, DateTime? endDate)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            if (startDate == null || endDate == null)
            {
                TempData["Error"] = "Please select both start and end dates.";
                return RedirectToAction(nameof(Report));
            }

            var subscriptions = _context.Subscriptions
                .Where(s => s.Startdate >= startDate && s.Startdate <= endDate)
                .Include(s => s.Member)
                .Include(s => s.Plan)
                .ToList();

            decimal totalPrice = subscriptions.Sum(s => s.Plan?.Price ?? 0);
            decimal totalProfit = totalPrice * 0.07m;

            ViewBag.TotalPrice = totalPrice;
            ViewBag.TotalProfit = totalProfit;

            ViewBag.StartDate = startDate?.ToShortDateString();
            ViewBag.EndDate = endDate?.ToShortDateString();

            // ====== NEW: data for charts ======

            var revenueByDate = subscriptions
                .GroupBy(s => s.Startdate.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    total = g.Sum(x => x.Plan?.Price ?? 0)
                })
                .OrderBy(x => x.date)
                .ToList();

            var subsByPlan = subscriptions
                .GroupBy(s => s.Plan.Planname)
                .Select(g => new
                {
                    plan = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .ToList();

            ViewBag.RevenueByDate = JsonSerializer.Serialize(revenueByDate);
            ViewBag.SubsByPlan = JsonSerializer.Serialize(subsByPlan);

            return View(subscriptions);
        }






        // GET: Admins
        public IActionResult Index()
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            ViewBag.MembersCount = _context.Members.Count();
            ViewBag.TotalRevenue = _context.Membershipplans.Sum(x => x.Price);
            ViewBag.subs = _context.Subscriptions.Where(x => x.Status == true).Count();
            ViewBag.tCount = _context.Trainers.Count();
            ViewBag.TestimonialsCount = _context.Testimonials.Where(t => t.Approvalstatus == "Pending").Count();
            ViewBag.WorkoutCount = _context.Workoutplans.Count();


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveTestimonial(long testimonialId)
        {
            var testimonial = _context.Testimonials.FirstOrDefault(t => t.Testimonialid == testimonialId);
            if (testimonial != null)
            {
                testimonial.Approvalstatus = "Approved";
                testimonial.Approvedbyadminid = HttpContext.Session.GetInt32("AdminId");
                _context.SaveChanges();
            }
            return RedirectToAction("ManageTestimonials");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectTestimonial(long testimonialId)
        {
            var testimonial = _context.Testimonials.FirstOrDefault(t => t.Testimonialid == testimonialId);
            if (testimonial != null)
            {
                _context.Testimonials.Remove(testimonial);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageTestimonials");
        }


        public IActionResult ManageTestimonials()
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            var pendingTestimonials = _context.Testimonials
        .Where(t => t.Approvalstatus == "Pending")
        .Include(t => t.Member) // Ensure Member information is included
        .ToList();
            return View(pendingTestimonials);
        }




        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------







        // GET: Admins/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            var admins = _context.Admins.FirstOrDefault(a => a.Adminid == id);
            var login = _context.Userlogins.FirstOrDefault(u => u.Adminid == id);
            if(admins == null || login == null)
            {
                return NotFound();
               
            }

            var viewModel = new DetailsAdminWithUserLogin
            {
                admin = admins,
                userlogin = login
            };

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.Adminid == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

     

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            if (id == null)
            {
                return NotFound();
            }
            var login = await _context.Userlogins.FirstOrDefaultAsync(u => u.Adminid == id);
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            var vm = new MyGymSystem.Join.DetailsAdminWithUserLogin
            {
                admin = admin,
                userlogin = login ?? new Userlogin()
            };
         
            return View(vm);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind( "Adminid,Firstname,Lastname,Email,Phonenumber,Dateofbirth,Imagepath,ImageFilee", Prefix = "admin")] Admin admin, [FromForm(Name = "userlogin.Username")] string? Username,
    [FromForm(Name = "userlogin.Passwordhash")] string? Password)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            if (id != admin.Adminid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {



                    if (admin.ImageFilee != null)
                    {
                        string wwwrootpath = _webHostEnvironment.WebRootPath;

                        //Encrypt for the image name ex:fswe21212w_pizza.png
                        string FileName = Guid.NewGuid().ToString() + "_" + admin.ImageFilee.FileName;

                        string path = Path.Combine(wwwrootpath + "/Images/", FileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {

                            await admin.ImageFilee.CopyToAsync(fileStream);
                        }
                        admin.Imagepath = FileName;
                    }

                    _context.Update(admin);
                    if (admin.ImageFilee == null)
                    {
                        _context.Entry(admin).Property(a => a.Imagepath).IsModified = false;
                    }
                    await _context.SaveChangesAsync();

                    var userLogin = _context.Userlogins.FirstOrDefault(u => u.Adminid == admin.Adminid );

                    if (userLogin != null) {
                        userLogin.Username = Username?? userLogin.Username;
                        userLogin.Passwordhash = Password?? userLogin.Passwordhash;
                        _context.Update(userLogin);
                    }
                    await _context.SaveChangesAsync(); // <-- you were missing this after updating userLogin



                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Adminid))
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
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.Adminid == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(long id)
        {
            return _context.Admins.Any(e => e.Adminid == id);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------



        // GET: Members/Details/5
        public async Task<IActionResult> MemberDetails(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Memberid == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }



        // GET: Members/Edit/5
        public async Task<IActionResult> EditMember(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }






        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(long id, [Bind("Memberid,Firstname,Lastname,Email,Phonenumber,Dateofbirth,Fitnessgoal,Imagepath,ImageFile")] Member member)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            if (id != member.Memberid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (member.ImageFile != null)
                    {

                        string wwwrootpath = _webHostEnvironment.WebRootPath;

                        //Encrypt for the image name ex:fswe21212w_pizza.png
                        string FileName = Guid.NewGuid().ToString() + "_" + member.ImageFile.FileName;

                        string path = Path.Combine(wwwrootpath + "/Images/", FileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {

                            await member.ImageFile.CopyToAsync(fileStream);
                        }
                        member.Imagepath = FileName;
                    }




                        _context.Update(member);
                        await _context.SaveChangesAsync();
                   


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!aMemberExists(member.Memberid))
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
            return View(member);
        }



        private bool aMemberExists(long id)
        {
            return (_context.Members?.Any(e => e.Memberid == id)).GetValueOrDefault();
        }







        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------

        public async Task<IActionResult> DetailTrainer(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");


            if (id == null || _context.Trainers == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.Trainerid == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }



        // GET: Trainers/Edit/5
        public async Task<IActionResult> EditTrainer(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            if (id == null || _context.Trainers == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainer(long id, [Bind("Trainerid,Firstname,Lastname,Email,Phonenumber,Specialization,Imagepath,ImageFilet")] Trainer trainer)
        {
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            if (id != trainer.Trainerid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (trainer.ImageFilet != null)
                    {

                        string wwwrootpath = _webHostEnvironment.WebRootPath;

                        //Encrypt for the image name ex:fswe21212w_pizza.png
                        string FileName = Guid.NewGuid().ToString() + "_" + trainer.ImageFilet.FileName;

                        string path = Path.Combine(wwwrootpath + "/Images/", FileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {

                            await trainer.ImageFilet.CopyToAsync(fileStream);
                        }
                        trainer.Imagepath = FileName;

                    }



                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Trainerid))
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
            return View(trainer);
        }



        private bool TrainerExists(long id)
        {
            return (_context.Trainers?.Any(e => e.Trainerid == id)).GetValueOrDefault();
        }










    }
}
