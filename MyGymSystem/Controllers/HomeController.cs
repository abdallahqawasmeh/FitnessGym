using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGymSystem.Context;
using MyGymSystem.Models;
using System.Diagnostics;

namespace MyGymSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GymDbContext _context;

        public HomeController(ILogger<HomeController> logger, GymDbContext context)
        {
            _logger = logger;

            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");




            var membershipplans = _context.Membershipplans.ToList();
            var trainers = _context.Trainers.ToList();

            var home = _context.Homepages.ToList();
            var about = _context.Aboutuspages.ToList();
            var contact = _context.Contactuspages.ToList();
            var final = Tuple.Create<IEnumerable<Membershipplan>, IEnumerable<Trainer>, IEnumerable<Homepage>, IEnumerable<Aboutuspage>, IEnumerable<Contactuspage>>(membershipplans, trainers, home, about, contact);

            var approvedTestimonials = _context.Testimonials
       .Where(t => t.Approvalstatus == "Approved").Include(t => t.Member) // Load member details if needed
       .ToList();

            // Pass testimonials to the view
            ViewBag.Testimonials = approvedTestimonials;
            var contactusPage = _context.Contactuspages.FirstOrDefault(o => o.Contactuspageid == 1);

            if (contactusPage != null)
            {
                ViewData["Address"] = contactusPage.Address;
                ViewData["Email"] = contactusPage.Email;
                ViewData["Phone"] = contactusPage.Phonenumber;
                ViewData["Content"] = contactusPage.Contenet;
            }
            else
            {
                // Handle the case where no record is found
                ViewData["Address"] = "Address not available";
                ViewData["Email"] = "Email not available";
                ViewData["Phone"] = "Phone number not available";
                ViewData["Content"] = "Content not available";
            }

            return View(final);
        }

        public IActionResult Trainers()
        {

            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");

            return View();
        }



            public IActionResult Contact()
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");

            return View();
        }
        // GET: Aboutuspages

        public async Task<IActionResult> AboutUs()
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");
            var aa = _context.Aboutuspages
                     .Include(a => a.Updatedbyadmin)
                     .OrderByDescending(a => a.Lastupdated)
                     .Take(1); // Ensure only one row is shown

            return View(await aa.ToListAsync());
         
        }



        public IActionResult Plans()
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");


            var plans = _context.Membershipplans.ToList();


            return View(plans);
        }


        public IActionResult Testimonials()
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");


            return View();

        }   // POST: Testimonials/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Testimonials([Bind("Testimonialid,Memberid,Feedbacktext,Approvalstatus,Submitteddate,Approvedbyadminid")] Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Approvedbyadminid"] = new SelectList(_context.Admins, "Adminid", "Adminid", testimonial.Approvedbyadminid);
            ViewData["Memberid"] = new SelectList(_context.Members, "Memberid", "Memberid", testimonial.Memberid);
            return View(testimonial);
        }









        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
