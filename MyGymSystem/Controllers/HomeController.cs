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

        private void FillLayoutData()
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");

            var memId = HttpContext.Session.GetInt32("MNId");
            if (memId != null)
            {
                var member = _context.Members.FirstOrDefault(m => m.Memberid == memId);
                if (member != null)
                {
                    ViewData["memImage"] = member.Imagepath;
                }
            }
        }


        [HttpGet]
        public IActionResult SubmitTestimonial()
		{
            FillLayoutData();

            return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SubmitTestimonial(Testimonial testimonial)
        {
            FillLayoutData();

            if (ModelState.IsValid)
			{
				var memberId = HttpContext.Session.GetInt32("MNId"); // Assuming the member ID is stored in claims


				testimonial.Submitteddate = DateTime.Now;
				testimonial.Approvalstatus = "Pending"; // Default status is "Pending"
				testimonial.Memberid = memberId;
				_context.Testimonials.Add(testimonial);
				_context.SaveChanges();

				return RedirectToAction("TestimonialSubmitted"); // Or wherever you want to redirect after submission
			}
			return View(testimonial);
		}


        [HttpGet]
        public IActionResult TestimonialSubmitted()
        {

            return View();
        }



        [HttpGet]

		public IActionResult Index()
        {
            FillLayoutData();


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
		
        
        [HttpGet]

		public IActionResult Trainers()
        {

            FillLayoutData();

            var trainers = _context.Trainers.ToList();
			return View(trainers);
		}


		[HttpGet]

		public IActionResult Contact()
        {
            FillLayoutData();

            return View();
        }
		
        
        
        
        // GET: Aboutuspages
		[HttpGet]

		public async Task<IActionResult> AboutUs()
        {
            FillLayoutData();

            var aa = _context.Aboutuspages
                     .Include(a => a.Updatedbyadmin)
                     .OrderByDescending(a => a.Lastupdated)
                     .Take(1); // Ensure only one row is shown

            return View(await aa.ToListAsync());
         
        }


		[HttpGet]

		public IActionResult Plans()
        {
            FillLayoutData();


            var plans = _context.Membershipplans.ToList();


            return View(plans);
        }

        [HttpGet]
        public IActionResult Testimonials()
        {
            FillLayoutData();
                var approvedTestimonials = _context.Testimonials
             .Where(t => t.Approvalstatus == "Approved").Include(t => t.Member) // Load member details if needed
             .ToList();

            // Pass testimonials to the view
            ViewBag.Testimonials = approvedTestimonials;
           
            return View();

        } 
        
        
        
        
        // POST: Testimonials/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Testimonials([Bind("Testimonialid,Memberid,Feedbacktext,Approvalstatus,Submitteddate,Approvedbyadminid")] Testimonial testimonial)
        {
            FillLayoutData();

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
