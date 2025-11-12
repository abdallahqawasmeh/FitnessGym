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

        // GET: Members
        public async Task<IActionResult> Index()
        {
            return View(await _context.Members.ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");
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
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Memberid,Firstname,Lastname,Email,Phonenumber,Dateofbirth,Fitnessgoal,Imagepath,ImageFile")] Member member, string Username, string Password)
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");

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
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");


            if (id == null)
            {
                return NotFound();
            }

         
       
        var login = await _context.Userlogins.FirstOrDefaultAsync(u => u.Adminid == id);
        var Member = await _context.Members.FindAsync(id);
        if (Member == null)
            {
                    return NotFound();
               }
                var vm = new MyGymSystem.Join.DetailsMemberWithUserLogin
                {
                    member = Member,
                    userlogin = login ?? new Userlogin()
                };

            return View(vm);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Memberid,Firstname,Lastname,Email,Phonenumber,Dateofbirth,Fitnessgoal,Imagepath,ImageFile")] Member member, string Username, string Password)
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");

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



                           var userlogin = await _context.Userlogins.FirstOrDefaultAsync(u => u.Memberid == member.Memberid);
                    if (userlogin != null)
                    {
                        userlogin.Username = Username;
                        userlogin.Passwordhash = Password;
                        _context.Update(userlogin);
                        await _context.SaveChangesAsync();
                    }
                


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Memberid))
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

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            ViewData["name"] = HttpContext.Session.GetString("MName");
            ViewData["memid"] = HttpContext.Session.GetInt32("MNId");

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
