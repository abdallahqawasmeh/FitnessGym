using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGymSystem.Context;
using MyGymSystem.Models;

namespace MyGymSystem.Controllers
{
  //  [Authorize(Roles = "Admin")]
    public class ContentPagesController : Controller
    {
        private readonly GymDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ContentPagesController(GymDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ Keep this aligned with how YOU store AdminId
        private long? GetAdminId()
        {
            return HttpContext.Session.GetInt32("ANId");
        }

        // ✅ If you already have your own FillLayoutData(), replace this call with yours
        private void FillLayoutData()
        {
            // optional: ViewData["name"] = HttpContext.Session.GetString("AName");
        }

        // =========================
        // ✅ Shared helper: Save image and return saved filename
        // =========================
        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var wwwroot = _env.WebRootPath;
            var folder = Path.Combine(wwwroot, "Images");
            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(folder, fileName);

            await using var fs = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(fs);

            return fileName;
        }

        // =====================================================
        // 1) HOME (includes hero + 3 slider images)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> EditHome()
        {
            FillLayoutData();

            var home = await _context.Homepages.FirstOrDefaultAsync();
            if (home == null) return NotFound("Homepage row not found. Seed one row first.");

            return View(home);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHome(Homepage model)
        {
            FillLayoutData();
            if (!ModelState.IsValid) return View(model);

            var db = await _context.Homepages.FirstOrDefaultAsync();
            if (db == null) return NotFound("Homepage row not found. Seed one row first.");

            // ✅ update text fields
            db.Welcomemessage = model.Welcomemessage;
            db.Featuredcontent = model.Featuredcontent;

            db.HeroTitle = model.HeroTitle;
            db.HeroSubtitle = model.HeroSubtitle;
            db.HeroButtonText = model.HeroButtonText;
            db.HeroButtonUrl = model.HeroButtonUrl;

            db.SliderCaption1 = model.SliderCaption1;
            db.SliderCaption2 = model.SliderCaption2;
            db.SliderCaption3 = model.SliderCaption3;

            // ✅ audit
            db.Updatedbyadminid = GetAdminId();
            db.Lastupdated = DateTime.UtcNow;

            // ✅ IMPORTANT: update image path ONLY if a new file was uploaded (otherwise keep old)
            if (model.Sliderimage != null && model.Sliderimage.Length > 0)
                db.Sliderimagepath = await SaveImageAsync(model.Sliderimage);

            if (model.SliderImage1 != null && model.SliderImage1.Length > 0)
                db.Sliderimagepath1 = await SaveImageAsync(model.SliderImage1);

            if (model.SliderImage2 != null && model.SliderImage2.Length > 0)
                db.Sliderimagepath2 = await SaveImageAsync(model.SliderImage2);

            if (model.SliderImage3 != null && model.SliderImage3.Length > 0)
                db.Sliderimagepath3 = await SaveImageAsync(model.SliderImage3);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Home page content updated successfully.";
            return RedirectToAction(nameof(EditHome));
        }

        // =====================================================
        // 2) ABOUT
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> EditAbout()
        {
            FillLayoutData();

            var about = await _context.Aboutuspages.FirstOrDefaultAsync();
            if (about == null) return NotFound("Aboutus row not found. Seed one row first.");

            return View(about);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAbout(Aboutuspage model)
        {
            FillLayoutData();
            if (!ModelState.IsValid) return View(model);

            var db = await _context.Aboutuspages.FirstOrDefaultAsync();
            if (db == null) return NotFound("Aboutus row not found. Seed one row first.");

            // ✅ update fields
            db.PageTitle = model.PageTitle;
            db.IntroText = model.IntroText;

            db.Missionstatement = model.Missionstatement;
            db.Visionstatement = model.Visionstatement;
            db.Teamdescription = model.Teamdescription;

            db.Value1Title = model.Value1Title;
            db.Value1Text = model.Value1Text;
            db.Value2Title = model.Value2Title;
            db.Value2Text = model.Value2Text;
            db.Value3Title = model.Value3Title;
            db.Value3Text = model.Value3Text;

            // ✅ audit
            db.Updatedbyadminid = GetAdminId();
            db.Lastupdated = DateTime.UtcNow;

            // ✅ image update only if new file chosen
            if (model.ImageFiler != null && model.ImageFiler.Length > 0)
                db.Imagepath = await SaveImageAsync(model.ImageFiler);

            await _context.SaveChangesAsync();

            TempData["Success"] = "About page content updated successfully.";
            return RedirectToAction(nameof(EditAbout));
        }

        // =====================================================
        // 3) CONTACT
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> EditContact()
        {
            FillLayoutData();

            var contact = await _context.Contactuspages.FirstOrDefaultAsync();
            if (contact == null) return NotFound("Contact row not found. Seed one row first.");

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContact(Contactuspage model)
        {
            FillLayoutData();
            if (!ModelState.IsValid) return View(model);

            var db = await _context.Contactuspages.FirstOrDefaultAsync();
            if (db == null) return NotFound("Contact row not found. Seed one row first.");

            // ✅ update fields
            db.PageTitle = model.PageTitle;
            db.IntroText = model.IntroText;
            db.WorkingHours = model.WorkingHours;

            db.Address = model.Address;
            db.Phonenumber = model.Phonenumber;
            db.Email = model.Email;
            db.Mapembedcode = model.Mapembedcode;
            db.Contenet = model.Contenet;

            // ✅ audit
            db.Updatedbyadminid = GetAdminId();
            db.Lastupdated = DateTime.UtcNow;

            // ✅ image update only if new file chosen
            if (model.picFile != null && model.picFile.Length > 0)
                db.Picpath = await SaveImageAsync(model.picFile);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Contact page content updated successfully.";
            return RedirectToAction(nameof(EditContact));
        }

        // =====================================================
        // 4) PLANS PAGE (Membershipplanspage)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> EditPlansPage()
        {
            FillLayoutData();

            var page = await _context.Membershipplanspages.FirstOrDefaultAsync();
            if (page == null) return NotFound("Membershipplanspage row not found. Seed one row first.");

            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlansPage(Membershipplanspage model)
        {
            FillLayoutData();
            if (!ModelState.IsValid) return View(model);

            var db = await _context.Membershipplanspages.FirstOrDefaultAsync();
            if (db == null) return NotFound("Membershipplanspage row not found. Seed one row first.");

            db.PageTitle = model.PageTitle;
            db.IntroText = model.IntroText;

            db.Updatedbyadminid = GetAdminId();
            db.Lastupdated = DateTime.UtcNow;

            // ✅ image update only if new file chosen
            if (model.Bannerimage != null && model.Bannerimage.Length > 0)
                db.Bannerimagepath = await SaveImageAsync(model.Bannerimage);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Plans page content updated successfully.";
            return RedirectToAction(nameof(EditPlansPage));
        }

        // =====================================================
        // 5) TRAINERS PAGE (Trainerspage)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> EditTrainersPage()
        {
            FillLayoutData();

            var page = await _context.Trainerspages.FirstOrDefaultAsync();
            if (page == null) return NotFound("Trainerspage row not found. Seed one row first.");

            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainersPage(Trainerspage model)
        {
            FillLayoutData();
            if (!ModelState.IsValid) return View(model);

            var db = await _context.Trainerspages.FirstOrDefaultAsync();
            if (db == null) return NotFound("Trainerspage row not found. Seed one row first.");

            db.PageTitle = model.PageTitle;
            db.IntroText = model.IntroText;

            db.Updatedbyadminid = GetAdminId();
            db.Lastupdated = DateTime.UtcNow;

            // ✅ image update only if new file chosen
            if (model.Bannerimage != null && model.Bannerimage.Length > 0)
                db.Bannerimagepath = await SaveImageAsync(model.Bannerimage);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Trainers page content updated successfully.";
            return RedirectToAction(nameof(EditTrainersPage));
        }

        // =====================================================
        // 6) TESTIMONIALS PAGE (Testimonialspage)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> EditTestimonialsPage()
        {
            FillLayoutData();

            var page = await _context.Testimonialspages.FirstOrDefaultAsync();
            if (page == null) return NotFound("Testimonialspage row not found. Seed one row first.");

            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTestimonialsPage(Testimonialspage model)
        {
            FillLayoutData();
            if (!ModelState.IsValid) return View(model);

            var db = await _context.Testimonialspages.FirstOrDefaultAsync();
            if (db == null) return NotFound("Testimonialspage row not found. Seed one row first.");

            db.PageTitle = model.PageTitle;
            db.IntroText = model.IntroText;

            db.Updatedbyadminid = GetAdminId();
            db.Lastupdated = DateTime.UtcNow;

            // ✅ image update only if new file chosen
            if (model.Bannerimage != null && model.Bannerimage.Length > 0)
                db.Bannerimagepath = await SaveImageAsync(model.Bannerimage);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Testimonials page content updated successfully.";
            return RedirectToAction(nameof(EditTestimonialsPage));
        }
    }
}

