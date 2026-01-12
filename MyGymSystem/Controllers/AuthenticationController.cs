using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGymSystem.Context;
using MyGymSystem.Models;
using MyGymSystem.Models.ViewModels;
using System.Linq;

namespace MyGymSystem.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly GymDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PasswordHasher<Userlogin> _hasher = new();

        public AuthenticationController(GymDbContext context, IWebHostEnvironment webHostEnvironment,PasswordHasher<Userlogin> passwordHasher)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _hasher = passwordHasher;
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
            return View(new SignUpVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpVM vm)
        {
            // 1) DataAnnotations validation
            if (!ModelState.IsValid)
                return View(vm);

            // 2) Uniqueness checks
            bool usernameExists = await _context.Userlogins.AnyAsync(x => x.Username == vm.Username);
            if (usernameExists)
            {
                ModelState.AddModelError(nameof(vm.Username), "Username is already taken.");
                return View(vm);
            }

            bool emailExists = await _context.Members.AnyAsync(x => x.Email == vm.Email);
            if (emailExists)
            {
                ModelState.AddModelError(nameof(vm.Email), "Email is already registered.");
                return View(vm);
            }

            // 3) Optional image save with validation
            string? savedFileName = null;
            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(vm.ImageFile.FileName).ToLowerInvariant();

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError(nameof(vm.ImageFile), "Only JPG, PNG, WEBP are allowed.");
                    return View(vm);
                }

                if (vm.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(vm.ImageFile), "Image must be 2MB or less.");
                    return View(vm);
                }

                Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "Images"));

                savedFileName = $"{Guid.NewGuid()}_{Path.GetFileName(vm.ImageFile.FileName)}";
                var savePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", savedFileName);

                using var fs = new FileStream(savePath, FileMode.Create);
                await vm.ImageFile.CopyToAsync(fs);
            }

            // 4) Save Member + Userlogin in one transaction (safe)
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var member = new Member
                {
                    Firstname = vm.Firstname,
                    Lastname = vm.Lastname,
                    Email = vm.Email,
                    Phonenumber = vm.Phonenumber,
                    Dateofbirth = vm.Dateofbirth,
                    Fitnessgoal = vm.Fitnessgoal,
                    Imagepath = savedFileName
                };

                _context.Members.Add(member);
                await _context.SaveChangesAsync(); // Memberid generated

                var login = new Userlogin
                {
                    Username = vm.Username,
                    Memberid = member.Memberid,
                    Roleid = 1
                };

                // ✅ Hash password (DON'T store plain text)
                login.Passwordhash = _hasher.HashPassword(login, vm.Password);

                _context.Userlogins.Add(login);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return RedirectToAction("Login");
            }
            catch
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", "Something went wrong while creating your account. Please try again.");
                return View(vm);
            }
        }
    






    // [HttpGet]

    //public IActionResult SignUp()
    //{

    //    return View();
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> SignUp([Bind("Memberid,Firstname,Lastname,Email,Phonenumber,Dateofbirth,Fitnessgoal,Imagepath,ImageFile")] Member member, string Username, string password)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        if (member.ImageFile != null)
    //        {
    //            // 7 preparing the image path before adding the object
    //            //GETTING W3ROOOT PATH
    //            string wwwrootpath = _webHostEnvironment.WebRootPath;

    //            //Encrypt for the image name ex:fswe21212w_pizza.png
    //            string FileName = Guid.NewGuid().ToString() + "_" + member.ImageFile.FileName;

    //            string path = Path.Combine(wwwrootpath + "/Images/", FileName);

    //            using (var fileStream = new FileStream(path, FileMode.Create))
    //            {
    //                await member.ImageFile.CopyToAsync(fileStream);
    //            }


    //            member.Imagepath = FileName;
    //        }

    //        _context.Add(member);
    //        await _context.SaveChangesAsync();


    //        Userlogin login = new Userlogin();
    //        {
    //            login.Username = Username;
    //            login.Passwordhash = password;
    //            login.Memberid = member.Memberid;
    //            login.Roleid = 1;
    //        };






    //        _context.Add(login);
    //        await _context.SaveChangesAsync();


    //        return RedirectToAction("Login");

    //    }
    //    return View(member);
    //}


         [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM userLogin)
        {
            if (!ModelState.IsValid)
                return View(userLogin);

            var authUser = _context.Userlogins
                .SingleOrDefault(x => x.Username == userLogin.Username);

            if (authUser != null)
            {
                //var hasher = new PasswordHasher<Userlogin>();
                //var verify = hasher.VerifyHashedPassword(authUser, authUser.Passwordhash, userLogin.Password);

                //if (verify != PasswordVerificationResult.Failed)
                //{
                //    // ✅ SAME SWITCH LOGIC AS YOU HAVE
                    switch (authUser.Roleid)
                    {
                        case 3:
                            if (authUser.Adminid != null)
                            {
                                HttpContext.Session.SetString("AdminName", authUser.Username);
                                HttpContext.Session.SetInt32("AdminId", (int)authUser.Adminid);
                            }
                            else throw new Exception("Admin ID is null for a user with Admin role.");

                            return RedirectToAction("Index", "Admins");

                        case 2:
                            if (authUser.Trainerid != null)
                            {
                                HttpContext.Session.SetString("TName", authUser.Username);
                                HttpContext.Session.SetInt32("TId", (int)authUser.Trainerid);
                            }
                            else throw new Exception("Trainer ID is null for a user with Trainer role.");

                            return RedirectToAction("Dashboard", "Trainers");

                        case 1:
                            if (authUser.Memberid != null)
                            {
                                HttpContext.Session.SetString("MName", authUser.Username);
                                HttpContext.Session.SetInt32("MNId", (int)authUser.Memberid);
                            }
                            else throw new Exception("Member ID is null for a user with Member role.");

                            return RedirectToAction("Index", "Home");

                    }
                }
        //    }

          
            return View(userLogin);
        }

        //[HttpPost]
        //public IActionResult Login([Bind("Username, Passwordhash")] Userlogin userLogin)
        //{
        //    var authUser = _context.Userlogins.Where(x => x.Username == userLogin.Username && x.Passwordhash == userLogin.Passwordhash).SingleOrDefault();

        //    if (authUser != null)
        //    {
        //        switch (authUser.Roleid)
        //        {
        //            case 3:
        //                if (authUser.Adminid != null)
        //                {
        //                    HttpContext.Session.SetString("AdminName", authUser.Username);
        //                    HttpContext.Session.SetInt32("AdminId", (int)authUser.Adminid);

        //                }
        //                else
        //                {
        //                    throw new Exception("Admin ID is null for a user with Admin role.");
        //                }

        //                return RedirectToAction("Index", "Admins");

        //            //Auth is Admin Controller

        //            case 2:
        //                if (authUser.Trainerid != null)
        //                {
        //                    HttpContext.Session.SetString("TName", authUser.Username);
        //                    HttpContext.Session.SetInt32("TId", (int)authUser.Trainerid);


        //                }
        //                else
        //                {
        //                    throw new Exception("Trainer ID is null for a user with Trainer role.");
        //                }
        //                return RedirectToAction("Dashboard", "Trainers");


        //            case 1:
        //                if (authUser.Memberid != null)
        //                {
        //                    HttpContext.Session.SetString("MName", authUser.Username);
        //                    HttpContext.Session.SetInt32("MNId", (int)authUser.Memberid);

        //                }

        //                else
        //                {
        //                    throw new Exception("Member ID is null for a user with Member role.");
        //                }

        //                return RedirectToAction("Index", "Home");


        //        }
        //    }



        //    return View();
        //}
    }
}
