using Microsoft.AspNetCore.Mvc;
using MyGymSystem.Context;
using MyGymSystem.Models;

namespace MyGymSystem.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly GymDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthenticationController(GymDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Logout()
        {
            // Clear the session to log out the user
            HttpContext.Session.Clear();

            // Redirect the user to the login page
            return RedirectToAction("Login", "Authentication");
        }

        [HttpGet]

        public IActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Memberid,Firstname,Lastname,Email,Phonenumber,Dateofbirth,Fitnessgoal,Imagepath,ImageFile")] Member member, string Username, string password)
        {
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


                Userlogin login = new Userlogin();
                {
                    login.Username = Username;
                    login.Passwordhash = password;
                    login.Memberid = member.Memberid;
                    login.Roleid = 1;
                };






                _context.Add(login);
                await _context.SaveChangesAsync();


                return RedirectToAction("Login");

            }
            return View(member);
        }



        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Login([Bind("Username, Passwordhash")] Userlogin userLogin)
        {
            var authUser = _context.Userlogins.Where(x => x.Username == userLogin.Username && x.Passwordhash == userLogin.Passwordhash).SingleOrDefault();

            if (authUser != null)
            {
                switch (authUser.Roleid)
                {
                    case 3:
                        if (authUser.Adminid != null)
                        {
                            HttpContext.Session.SetString("AdminName", authUser.Username);
                            HttpContext.Session.SetInt32("AdminId", (int)authUser.Adminid);

                        }
                        else
                        {
                            throw new Exception("Admin ID is null for a user with Admin role.");
                        }

                        return RedirectToAction("Index", "Admins");

                    //Auth is Admin Controller

                    case 2:
                        if (authUser.Trainerid != null)
                        {
                            HttpContext.Session.SetString("TName", authUser.Username);
                            HttpContext.Session.SetInt32("TId", (int)authUser.Trainerid);
                           
                           
                        }
                        else
                        {
                            throw new Exception("Trainer ID is null for a user with Trainer role.");
                        }
                        return RedirectToAction("Home", "Trainers");


                    case 1:
                        if (authUser.Memberid != null)
                        {
                            HttpContext.Session.SetString("MName", authUser.Username);
                            HttpContext.Session.SetInt32("MNId", (int)authUser.Memberid);

                        }

                        else
                        {
                            throw new Exception("Member ID is null for a user with Member role.");
                        }

                        return RedirectToAction("Index", "Home");


                }
            }



            return View();
        }
    }
}
