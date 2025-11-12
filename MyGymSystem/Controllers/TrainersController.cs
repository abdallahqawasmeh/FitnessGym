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

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            ViewData["name"] = HttpContext.Session.GetString("TName");
            ViewData["Tid"] = HttpContext.Session.GetInt32("TId");


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
            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Trainerid,Firstname,Lastname,Email,Phonenumber,Specialization,Imagepath,ImageFilet")] Trainer trainer,string Username,string Password)
        {
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

                    Userlogin userlogin = new Userlogin
                    {

                        Username = Username,
                        Passwordhash = Password,
                        Roleid = 2
                    };


                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                
              
                
                return RedirectToAction("Home");
            }
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("TName");
            ViewData["Tid"] = HttpContext.Session.GetInt32("TId");
           
            if (id == null)
            {
                return NotFound();
            }

            var trainers = await _context.Trainers.FirstOrDefaultAsync(a => a.Trainerid == id);
            var login = await _context.Userlogins.FirstOrDefaultAsync(u => u.Trainerid == id);
            if (trainers == null || login == null)
            {
                return NotFound();

            }

            var viewModel = new DetailsTrainerWithUserLogin
            {
                trainer = trainers,
                userlogin = login
            };


           

           
            return View(viewModel);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Trainerid,Firstname,Lastname,Email,Phonenumber,Specialization,Imagepath,ImageFilet"
        ,Prefix ="trainer")] Trainer trainer, [FromForm(Name = "userlogin.Username")] string? Username
        ,[FromForm(Name = "userlogin.Passwordhash")] string? Password)
        {
            ViewData["name"] = HttpContext.Session.GetString("TName");
            ViewData["Tid"] = HttpContext.Session.GetInt32("TId");


            if (id != trainer.Trainerid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(trainer.ImageFilet != null)
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
                    if (trainer.ImageFilet == null)
                    {
                        _context.Entry(trainer).Property(a => a.Imagepath).IsModified = false;
                    }
                    await _context.SaveChangesAsync();

                    var userlogin = _context.Userlogins.FirstOrDefault(u => u.Trainerid == trainer.Trainerid);
                    if (userlogin != null)
                    {
                        userlogin.Username = Username ?? userlogin.Username;
                        userlogin.Passwordhash = Password ?? userlogin.Passwordhash;
                        _context.Update(userlogin);
                        await _context.SaveChangesAsync();
                    }

                    
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

        // GET: Trainers/Delete/5
        public async Task<IActionResult> Delete(long? id)
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

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
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
