using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGymSystem.Context;
using MyGymSystem.Join;
using MyGymSystem.Models;
using NuGet.DependencyResolver;

namespace MyGymSystem.Controllers
{
    public class TrainersController : Controller
    {
        private readonly GymDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TrainersController(GymDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private void FillLayoutDataAdmin()
        {
           
            ViewData["name"] = HttpContext.Session.GetString("AdminName");
            ViewData["ad"] = HttpContext.Session.GetInt32("AdminId");

            var AdminId = HttpContext.Session.GetInt32("AdminId");
            if (AdminId != null)
            {
                var admin = _context.Admins.FirstOrDefault(m => m.Adminid == AdminId);
                if (admin != null)
                {
                    ViewData["AdminImage"] = admin.Imagepath;
                }
            }
        }

        private void FillLayoutData()
        {
            ViewData["name"] = HttpContext.Session.GetString("TName");
            ViewData["Tid"] = HttpContext.Session.GetInt32("TId");

            var tId = HttpContext.Session.GetInt32("TId");
            if (tId != null)
            {
                var Trainer = _context.Trainers.FirstOrDefault(m => m.Trainerid == tId);
                if (Trainer != null)
                {
                    ViewData["Timage"] = Trainer.Imagepath;
                }
            }
          
            
        }




        // ✅ helper: get current trainer id (change to your real logic)
        private long? GetTrainerId()
        {
            // example: session
            // return HttpContext.Session.GetInt32("TNId");
            return HttpContext.Session.GetInt32("TId");
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // ✅ Change this to how you store trainer id (session/userlogin)
            var trainerId =GetTrainerId();
            if (trainerId == null)
                return RedirectToAction("Login", "Authentication");
            FillLayoutData();
            var myMembersCount = await _context.MemberTrainer
                .CountAsync(mt => mt.TrainerId == trainerId.Value && mt.IsActive);

            var totalPlans = await _context.Workoutplans
                .CountAsync(w => w.Trainerid == trainerId.Value);

            var activePlans = await _context.Workoutplans
                .CountAsync(w => w.Trainerid == trainerId.Value && (w.Status ));

            var recentPlans = await _context.Workoutplans
                .Where(w => w.Trainerid == trainerId.Value)
                .OrderByDescending(w => w.Workoutplanid)
                .Take(5)
                .Select(w => new TrainerDashboardVM.RecentPlanRow
                {
                    Workoutplanid = w.Workoutplanid,
                    Memberid = w.Memberid,
                    MemberName = w.Member.Firstname + " " + w.Member.Lastname,
                    Planname = w.Planname,
                    Startdate = w.Startdate,
                    Status = w.Status
                })
                .ToListAsync();

            var vm = new TrainerDashboardVM
            {
                MyMembersCount = myMembersCount,
                TotalPlans = totalPlans,
                ActivePlans = activePlans,
                RecentPlans = recentPlans
            };



            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> MemberPlans(long memberId)
        {
            var trainerId = GetTrainerId();
            if (trainerId == null) return RedirectToAction("Login", "Authentication");

            FillLayoutData();


            var isMyMember = await _context.MemberTrainer.AnyAsync(mt =>
                mt.TrainerId == trainerId.Value && mt.MemberId == memberId && mt.IsActive);

            if (!isMyMember) return Forbid();

            var plans = await _context.Workoutplans
                .Where(w => w.Memberid == memberId && w.Trainerid == trainerId.Value)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.MemberId = memberId;
            return View(plans);
        }

        // 1) List my members
        [HttpGet]
        public async Task<IActionResult> MyMembers()
        {
            var trainerId = GetTrainerId();
            if (trainerId == null) return RedirectToAction("Login", "Authentication");

            FillLayoutData();

            // get active members who chose this trainer
            var members = await _context.MemberTrainer
                .Where(mt => mt.TrainerId == trainerId.Value && mt.IsActive)
                .Select(mt => mt.Member)
                .AsNoTracking()
                .ToListAsync();

            return View(members);
        }

        // 2) Create workout plan page
        [HttpGet]
        public async Task<IActionResult> CreateWorkoutPlan(long memberId)
        {
            var trainerId = GetTrainerId();
            if (trainerId == null) return RedirectToAction("Login", "Authentication");

            // security: member must be assigned to this trainer
            var isMyMember = await _context.MemberTrainer.AnyAsync(mt =>
                mt.TrainerId == trainerId.Value && mt.MemberId == memberId && mt.IsActive);

            if (!isMyMember) return Forbid();

            var model = new Workoutplan
            {
                Memberid = memberId,
                Trainerid = trainerId.Value
            };

            return View(model);
        }

        // 3) Create workout plan POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkoutPlan(Workoutplan model)
        {
            var trainerId = GetTrainerId();
            if (trainerId == null) return RedirectToAction("Login", "Authentication");

            FillLayoutData();

            // enforce correct trainer id (don’t trust posted data)
            model.Trainerid = trainerId.Value;

            // security: member must be assigned to this trainer
            var isMyMember = await _context.MemberTrainer.AnyAsync(mt =>
                mt.TrainerId == trainerId.Value && mt.MemberId == model.Memberid && mt.IsActive);

            if (!isMyMember) return Forbid();

            if (!ModelState.IsValid) return View(model);

            _context.Workoutplans.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyMembers));
        }

        // 4) Edit workout plan
        [HttpGet]
        public async Task<IActionResult> EditWorkoutPlan(long id)
        {

            FillLayoutData();

            var trainerId = GetTrainerId();
            if (trainerId == null) return RedirectToAction("Login", "Authentication");

            var plan = await _context.Workoutplans.FirstOrDefaultAsync(w => w.Workoutplanid == id);
            if (plan == null) return NotFound();

            // security: trainer can edit only his plans
            if (plan.Trainerid != trainerId.Value) return Forbid();

            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkoutPlan(long id, Workoutplan model)
        {
            FillLayoutData();

            var trainerId = GetTrainerId();
            if (trainerId == null) return RedirectToAction("Login", "Authentication");

            if (id != model.Workoutplanid) return BadRequest();

            var plan = await _context.Workoutplans.FirstOrDefaultAsync(w => w.Workoutplanid == id);
            if (plan == null) return NotFound();

            if (plan.Trainerid != trainerId.Value) return Forbid();

            if (!ModelState.IsValid) return View(model);

            // update allowed fields only (safer)
            plan.Planname = model.Planname;
            plan.Plandescription = model.Plandescription;
            plan.Startdate = model.Startdate;
            plan.Enddate = model.Enddate;
            plan.Routinedetails = model.Routinedetails;
            plan.Schedule = model.Schedule;
            plan.Goal = model.Goal;
            plan.Status = model.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyMembers));
        }

        // 5) Delete workout plan
        [HttpGet]
        public async Task<IActionResult> DeleteWorkoutPlan(long id)
        {
            FillLayoutData();

            var trainerId = GetTrainerId();
            if (trainerId == null) return RedirectToAction("Login", "Authentication");

            var plan = await _context.Workoutplans
                .Include(w => w.Member)
                .FirstOrDefaultAsync(w => w.Workoutplanid == id);

            if (plan == null) return NotFound();
            if (plan.Trainerid != trainerId.Value) return Forbid();

            return View(plan);
        }

        [HttpPost, ActionName("DeleteWorkoutPlan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkoutPlanConfirmed(long id)
        {
            FillLayoutData();

            var trainerId = GetTrainerId();
            if (trainerId == null) return RedirectToAction("Login", "Authentication");

            var plan = await _context.Workoutplans.FirstOrDefaultAsync(w => w.Workoutplanid == id);
            if (plan == null) return NotFound();
            if (plan.Trainerid != trainerId.Value) return Forbid();

            _context.Workoutplans.Remove(plan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyMembers));
        }
    







    // GET: Trainers
    public async Task<IActionResult> Index()
        {
            FillLayoutDataAdmin();
            


            return View(await _context.Trainers.ToListAsync());
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("TName");
            ViewData["Tid"] = HttpContext.Session.GetInt32("TId");


            if (id == null)
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

        // GET: Trainers/Create
        public IActionResult Create()
        {
            FillLayoutDataAdmin();

            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Trainerid,Firstname,Lastname,Email,Phonenumber,Specialization,Imagepath,ImageFilet")] Trainer trainer,string Username,string Password)
        {
            FillLayoutDataAdmin();

            if (ModelState.IsValid)
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
               await _context.AddAsync(trainer);
                await _context.SaveChangesAsync();

                Userlogin userlogin = new Userlogin
                    {Trainerid=trainer.Trainerid,
                        Username = Username,
                        Passwordhash = Password,
                        Roleid = 2
                    };

                await _context.Userlogins.AddAsync(userlogin);
                await _context.SaveChangesAsync();

                
              
                
                return RedirectToAction("Index","Admins");
            }
            return View(trainer);
        }

        [HttpGet]
        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            FillLayoutData();

            if (id == null)
            {
                return NotFound();
            }

            var trainers = await _context.Trainers.FirstOrDefaultAsync(a => a.Trainerid == id);
            if (trainers == null )
            {
                return NotFound();

            }
            var login = await _context.Userlogins.FirstOrDefaultAsync(u => u.Trainerid == id);


            // Pass to view (for display / editing)
            ViewBag.Username = login?.Username;
            ViewBag.Password = login?.Passwordhash; // (later you should hash, but this matches your code now)






            return View(trainers);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    long id,
    [Bind("Trainerid,Firstname,Lastname,Email,Phonenumber,Specialization,Imagepath,ImageFilet")] Trainer trainer,
    string? Username,
    string? Password)
        {
            FillLayoutData();

            if (id != trainer.Trainerid) return NotFound();
            if (!ModelState.IsValid) return View(trainer);

            // ✅ Load tracked entity (IMPORTANT)
            var dbTrainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Trainerid == id);
            if (dbTrainer == null) return NotFound();

            // ✅ Update scalar fields
            dbTrainer.Firstname = trainer.Firstname;
            dbTrainer.Lastname = trainer.Lastname;
            dbTrainer.Email = trainer.Email;
            dbTrainer.Phonenumber = trainer.Phonenumber;
            dbTrainer.Specialization = trainer.Specialization;

            // ✅ Image update only if a new file is chosen
            if (trainer.ImageFilet != null && trainer.ImageFilet.Length > 0)
            {
                string wwwrootpath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid() + "_" + Path.GetFileName(trainer.ImageFilet.FileName);
                string path = Path.Combine(wwwrootpath, "Images", fileName);

                using var fileStream = new FileStream(path, FileMode.Create);
                await trainer.ImageFilet.CopyToAsync(fileStream);

                dbTrainer.Imagepath = fileName; // ✅ SAVE TO TRACKED ENTITY
            }
            // else: keep dbTrainer.Imagepath as-is

            // ✅ Update login info (tracked automatically if loaded)
            var userlogin = await _context.Userlogins.FirstOrDefaultAsync(u => u.Trainerid == id);
            if (userlogin != null)
            {
                if (!string.IsNullOrWhiteSpace(Username))
                    userlogin.Username = Username;

                if (!string.IsNullOrWhiteSpace(Password))
                    userlogin.Passwordhash = Password; // hash if you use hashing
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = id });
        }


        // GET: Trainers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            FillLayoutDataAdmin();


            if (id == null)
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

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            FillLayoutDataAdmin();

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(long id)
        {
            return _context.Trainers.Any(e => e.Trainerid == id);
        }
    }
}
