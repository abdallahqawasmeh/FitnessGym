using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MyGymSystem.Context;
using MyGymSystem.Join;
using MyGymSystem.Models;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace MyGymSystem.Controllers
{
    public class MembersController : Controller
    {
        private readonly GymDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MembersController(GymDbContext context, IWebHostEnvironment webHostEnvironment)
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





        // GET: Members
        public async Task<IActionResult> Index()
        {
            FillLayoutDataAdmin();

            return View(await _context.Members.ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            FillLayoutData();
            var Member = _context.Members.FirstOrDefault(a => a.Memberid == id);
            var login = _context.Userlogins.FirstOrDefault(u => u.Memberid == id);
            if (Member == null || login == null)
            {
                return NotFound();

            }

            var viewModel = new DetailsMemberWithUserLogin
            {
                member = Member,
                userlogin = login
            };
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Memberid == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            FillLayoutDataAdmin();
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Memberid,Firstname,Lastname,Email,Phonenumber,Dateofbirth,Fitnessgoal,Imagepath,ImageFile")] Member member, string Username, string Password)
        {
            FillLayoutDataAdmin();

            if (ModelState.IsValid)
            {


                if (member.ImageFile != null)
                {
                    // 7 preparing the image path before adding the object
                    //GETTING W3ROOOT PATH
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






                _context.Add(member);
                await _context.SaveChangesAsync();

                Userlogin userlogin = new Userlogin()
                {
                    Memberid = member.Memberid,
                    Username = Username,
                    Passwordhash = Password,
                    Roleid = 1

                };
                await _context.Userlogins.AddAsync(userlogin);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            FillLayoutData();
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                          .Include(m => m.Invoices) // Include invoices for the user
                          .FirstOrDefaultAsync(m => m.Memberid == id.Value);
            if (member == null)
            {
                return NotFound();
            }

            // Get user login for this member
            var login = await _context.Userlogins
                                      .FirstOrDefaultAsync(u => u.Memberid == id);

            // Pass to view (for display / editing)
            ViewBag.Username = login?.Username;
            ViewBag.Password = login?.Passwordhash; // (later you should hash, but this matches your code now)

            return View(member);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    long id,
    [Bind("Memberid,Firstname,Lastname,Email,Phonenumber,Dateofbirth,Fitnessgoal,Imagepath,ImageFile")] Member member,
    string Username,
    string Password)
        {
            FillLayoutData();

            if (id != member.Memberid)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // 🔹 repopulate ViewBag so Account section doesn’t disappear
                var loginInvalid = await _context.Userlogins
                                                 .FirstOrDefaultAsync(u => u.Memberid == member.Memberid);

                ViewBag.Username = !string.IsNullOrWhiteSpace(Username)
                    ? Username
                    : loginInvalid?.Username;

                ViewBag.Password = !string.IsNullOrWhiteSpace(Password)
                    ? Password
                    : loginInvalid?.Passwordhash;

                return View(member);
            }

            var existingMember = await _context.Members.FindAsync(id);
            if (existingMember == null)
            {
                return NotFound();
            }

            try
            {
                // ---- Update scalar fields ----
                existingMember.Firstname = member.Firstname ?? existingMember.Firstname;
                existingMember.Lastname = member.Lastname ?? existingMember.Lastname;
                existingMember.Email = member.Email ?? existingMember.Email;
                existingMember.Phonenumber = member.Phonenumber ?? existingMember.Phonenumber;
                existingMember.Dateofbirth = member.Dateofbirth ?? existingMember.Dateofbirth;
                existingMember.Fitnessgoal = member.Fitnessgoal ?? existingMember.Fitnessgoal;

                // ---- Image upload ----
                if (member.ImageFile != null)
                {
                    string wwwrootpath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + member.ImageFile.FileName;
                    string path = Path.Combine(wwwrootpath, "Images", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await member.ImageFile.CopyToAsync(fileStream);
                    }

                    existingMember.Imagepath = fileName;
                }

                await _context.SaveChangesAsync();

                // ---- Update login info ----
                var userlogin = await _context.Userlogins
                                              .FirstOrDefaultAsync(u => u.Memberid == existingMember.Memberid);

                if (userlogin != null)
                {
                    if (!string.IsNullOrWhiteSpace(Username))
                        userlogin.Username = Username;

                    if (!string.IsNullOrWhiteSpace(Password))
                        userlogin.Passwordhash = Password;

                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(member.Memberid))
                    return NotFound();
                else
                    throw;
            }

            // 🔹 reload GET Edit → updated image + account fields
            return RedirectToAction(nameof(Edit), new { id = existingMember.Memberid });
        }




        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            FillLayoutDataAdmin();


            if (id == null)
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

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            FillLayoutDataAdmin();

            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(long id)
        {
            return _context.Members.Any(e => e.Memberid == id);
        }
    }
}
