using iText.Commons.Actions.Contexts;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MyGymSystem.Context;
using MyGymSystem.Join;
using MyGymSystem.Models;
using System.Diagnostics;
using MailKit.Net.Smtp;
using iText.Layout.Element;
using iText.Layout.Properties;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using iText.Layout;

namespace MyGymSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GymDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, GymDbContext context,IConfiguration configuration)
        {
            _logger = logger;

            _context = context;

            _configuration = configuration;
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
            FillLayoutData();


            return View();
        }





        [HttpGet]
        public async Task<IActionResult> Index()
        {
            FillLayoutData();

            var home = await _context.Homepages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Homepageid == 1);

            var about = await _context.Aboutuspages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Aboutuspageid == 1);

            var contact = await _context.Contactuspages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Contactuspageid == 1);

            var plans = await _context.Membershipplans
                .AsNoTracking()
                .Where(p => p.Status == true)
                .ToListAsync();

            var trainers = await _context.Trainers
                .AsNoTracking()
                .ToListAsync();

            var approvedTestimonials = await _context.Testimonials
                .Where(t => t.Approvalstatus == "Approved")
                .Include(t => t.Member)
                .AsNoTracking()
                .ToListAsync();

            var vm = new MyGymSystem.Join.HomePageVM
            {
                Home = home,
                About = about,
                Contact = contact,
                Plans = plans,
                Trainers = trainers,
                Testimonials = approvedTestimonials
            };

            return View(vm);
        }









        //      [HttpGet]

        //public IActionResult Index()
        //      {
        //          FillLayoutData();


        //          var membershipplans = _context.Membershipplans.ToList();
        //          var trainers = _context.Trainers.ToList();

        //          var home = _context.Homepages.ToList();
        //          var about = _context.Aboutuspages.ToList();
        //          var contact = _context.Contactuspages.ToList();
        //          var final = Tuple.Create<IEnumerable<Membershipplan>, IEnumerable<Trainer>, IEnumerable<Homepage>, IEnumerable<Aboutuspage>, IEnumerable<Contactuspage>>(membershipplans, trainers, home, about, contact);

        //          var approvedTestimonials = _context.Testimonials
        //           .Where(t => t.Approvalstatus == "Approved").Include(t => t.Member) // Load member details if needed
        //           .ToList();

        //          // Pass testimonials to the view
        //          ViewBag.Testimonials = approvedTestimonials;
        //          var contactusPage = _context.Contactuspages.FirstOrDefault(o => o.Contactuspageid == 1);

        //          if (contactusPage != null)
        //          {
        //              ViewData["Address"] = contactusPage.Address;
        //              ViewData["Email"] = contactusPage.Email;
        //              ViewData["Phone"] = contactusPage.Phonenumber;
        //              ViewData["Content"] = contactusPage.Contenet;
        //          }
        //          else
        //          {
        //              // Handle the case where no record is found
        //              ViewData["Address"] = "Address not available";
        //              ViewData["Email"] = "Email not available";
        //              ViewData["Phone"] = "Phone number not available";
        //              ViewData["Content"] = "Content not available";
        //          }

        //          return View(final);
        //      }





        public async Task<IActionResult> MyPrivateWorkout()
         {

             FillLayoutData();

          var memberId = HttpContext.Session.GetInt32("MNId");
           if (memberId == null) return RedirectToAction("Login", "Authentication");

           //  Get active trainer for this member
           var trainer = await _context.MemberTrainer
               .Where(mt => mt.MemberId == memberId.Value && mt.IsActive)
               .Select(mt => mt.Trainer)
               .AsNoTracking()
               .FirstOrDefaultAsync();

           //  Get ALL workout plans for this member (created by trainer or any)
           // If you want ONLY plans from his active trainer, add: && w.Trainerid == trainer.Trainerid
           var plans = await _context.Workoutplans
               .Where(w => w.Memberid == memberId.Value)
               .OrderByDescending(w => w.Workoutplanid)
               .AsNoTracking()
               .ToListAsync();

           var vm = new MemberPrivatePlanVM
           {
               Trainer = trainer,
               Plans = plans
           };

           return View(vm);
         }





        [HttpGet]
        public async Task<IActionResult> Trainers()
        {
            FillLayoutData();

            var page = await _context.Trainerspages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Trainerspageid == 1);

            var trainers = await _context.Trainers
                .AsNoTracking()
                .ToListAsync();

            var vm = new TrainersVM
            {
                Page = page,
                Trainers = trainers
            };

            return View(vm);
        }



        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            FillLayoutData();

            var page = await _context.Contactuspages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Contactuspageid == 1);

            var vm = new ContactVM { Page = page };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactVM vm)
        {
            FillLayoutData();

            // reload CMS row again (because POST only receives form fields)
            vm.Page = await _context.Contactuspages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Contactuspageid == 1);

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                // Send the email to your website inbox
                await SendContactEmailAsync(vm);

                TempData["ContactSuccess"] = "Your message has been sent successfully. We’ll get back to you soon!";
                return RedirectToAction(nameof(Contact));
            }
            catch
            {
                TempData["ContactError"] = "Something went wrong while sending your message. Please try again.";
                return View(vm);
            }
        }
        private async Task SendContactEmailAsync(ContactVM vm)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var websiteInbox = emailSettings["SenderEmail"]; // or use a separate key like "InboxEmail"
            if (string.IsNullOrWhiteSpace(websiteInbox))
                throw new Exception("Website inbox email is not configured.");

            var message = new MimeMessage();

            // From: your website sender account (MUST be your authenticated sender)
            message.From.Add(new MailboxAddress("MyGymSystem Website", emailSettings["SenderEmail"]));

            // To: website inbox (same or different)
            message.To.Add(MailboxAddress.Parse(websiteInbox));

            // Reply-To: the user who filled the form
            if (!string.IsNullOrWhiteSpace(vm.Form.Email))
                message.ReplyTo.Add(new MailboxAddress(vm.Form.Name ?? "Website Visitor", vm.Form.Email));

            message.Subject = $"[Contact Us] {vm.Form.Subject}";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
            <h2>New Contact Message</h2>
            <p><strong>Name:</strong> {System.Net.WebUtility.HtmlEncode(vm.Form.Name)}</p>
            <p><strong>Email:</strong> {System.Net.WebUtility.HtmlEncode(vm.Form.Email)}</p>
            <p><strong>Subject:</strong> {System.Net.WebUtility.HtmlEncode(vm.Form.Subject)}</p>
            <hr />
            <p><strong>Message:</strong></p>
            <p style='white-space:pre-wrap'>{System.Net.WebUtility.HtmlEncode(vm.Form.Message)}</p>
        ",
                TextBody = $@"
New Contact Message

Name: {vm.Form.Name}
Email: {vm.Form.Email}
Subject: {vm.Form.Subject}

Message:
{vm.Form.Message}
        "
            };

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                emailSettings["SmtpServer"],
                int.Parse(emailSettings["SmtpPort"]!),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                emailSettings["SmtpUsername"],
                emailSettings["SmtpPassword"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }


        // GET: Aboutuspages

        [HttpGet]
        public async Task<IActionResult> AboutUs()
        {
            FillLayoutData();

            var page = await _context.Aboutuspages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Aboutuspageid == 1);

            var vm = new AboutUsVM { Page = page };
            return View(vm);
        }



        [HttpGet]
        public async Task<IActionResult> Plans()
        {
            FillLayoutData();

            var page = await _context.Membershipplanspages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Membershipplanspageid == 1);

            var plans = await _context.Membershipplans
                .AsNoTracking()
                .Where(p => p.Status == true)
                .ToListAsync();

            var vm = new PlansVM
            {
                Page = page,
                Plans = plans
            };

            return View(vm);
        }


        //[HttpGet]
        //public IActionResult Testimonials()
        //{
        //    FillLayoutData();
        //        var approvedTestimonials = _context.Testimonials
        //     .Where(t => t.Approvalstatus == "Approved").Include(t => t.Member) // Load member details if needed
        //     .ToList();

        //    // Pass testimonials to the view
        //    ViewBag.Testimonials = approvedTestimonials;
           
        //    return View();

        //}

        [HttpGet]
        public async Task<IActionResult> Testimonials()
        {
            FillLayoutData();

            var page = await _context.Testimonialspages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Testimonialspageid == 1);

            var approvedTestimonials = await _context.Testimonials
                .Where(t => t.Approvalstatus == "Approved")
                .Include(t => t.Member)
                .AsNoTracking()
                .ToListAsync();

            var vm = new TestimonialsVM
            {
                Page = page,
                Testimonials = approvedTestimonials,
                Form = new Testimonial { Memberid = HttpContext.Session.GetInt32("MNId") }
            };

            return View(vm);
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
