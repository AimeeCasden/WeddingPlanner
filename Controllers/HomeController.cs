using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.SetInt32("Id", 100);
            HttpContext.Session.SetString("Email", "");
            return View();
        }

        [Route("login")]
        [HttpGet]

        public IActionResult Login()
        {
            return View();
        }

        [Route("RegGuest")]
        [HttpPost]

        public IActionResult RegGuest(Guest NewGuestFromForm)
        {
            if (ModelState.IsValid)
            {
                Guest GuestToAdd = new Guest()
                {
                    FirstName = NewGuestFromForm.FirstName,
                    LastName = NewGuestFromForm.LastName,
                    Email = NewGuestFromForm.Email,
                    Password = NewGuestFromForm.Password
                };
                dbContext.Guest.Add(GuestToAdd);
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("Id", GuestToAdd.GuestId);
                HttpContext.Session.SetString("Email", GuestToAdd.Email);

                return RedirectToAction("Welcome");
            }
            else
            {
                return View("Index");
            }
        }
        [Route("Welcome")]
        [HttpGet]
        public IActionResult Welcome()
        {
            ViewBag.GuestId = HttpContext.Session.GetInt32("Id");
            // if statement used for RSVP
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                return RedirectToAction("Index");
            }
            // this collects all the data needed to display the newly created wedding in the table on the Welcome page.
            List<Wedding> AllWeddings = dbContext.Wedding
            .Include(g => g.AttendingGuests)
            .ThenInclude(w => w.Person).ToList();



            ViewBag.EveryWedding = AllWeddings;
            HttpContext.Session.GetString("Email");
            return View("Welcome");
        }

        [Route("GuestLogin")]
        [HttpPost]
        public IActionResult GuestLogin(GuestLogin NewLoginFromForm)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var GuestToLogin = dbContext.Guest.FirstOrDefault(g => g.Email == NewLoginFromForm.Email);
                // If no user exists with provided email
                if (GuestToLogin == null || GuestToLogin.Password != NewLoginFromForm.Password)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                // TAKE WHOEVER IS LOGGED IN AND PUT IT IN SESSION (use for rsvp and view)
                HttpContext.Session.SetInt32("Id", GuestToLogin.GuestId);
                HttpContext.Session.SetString("Email", GuestToLogin.Email);
                return RedirectToAction("Welcome");
            }
            return View("Login");
        }

        [Route("AddWedding")]
        [HttpGet]
        public IActionResult AddWedding(Wedding newWedding, RSVP firstAttendee)
        {

            if (ModelState.IsValid)
            {
                //  every time a new wedding is created the planner (int) is assigned to the person in session//
                newWedding.GuestId = (int)HttpContext.Session.GetInt32("Id");
                dbContext.Wedding.Add(newWedding);
                dbContext.SaveChanges();

                // ADDING PLANNER TO GUEST LIST//
                Guest WeddingPlanner = dbContext.Guest.FirstOrDefault(w => w.GuestId == HttpContext.Session.GetInt32("Id"));
                // CREATED A NEW RSVP WHERE THE WEDDING ID IS THE WEDDING JUST CREATED. AND THE FIRSTATTENDEE GUESTID IS THE ID OF THE WEDDING PLANNER//
                firstAttendee.GuestId = WeddingPlanner.GuestId;
                firstAttendee.WeddingId = newWedding.WeddingId;
                dbContext.RSVP.Add(firstAttendee);
                dbContext.SaveChanges();
                return RedirectToAction("Welcome");
            }
            else
            {
                return View("AddWedding");
            }
        }
        [Route("CreateWedding")]
        [HttpPost]

        public IActionResult CreateWedding(Wedding CreateWedding)
        {
            if (ModelState.IsValid)
            {
                // creates a new record in the wedding table
                Guest RetrievedGuest = dbContext.Guest.FirstOrDefault(g => g.GuestId == HttpContext.Session.GetInt32("Id"));
                CreateWedding.Planner = RetrievedGuest;
                CreateWedding.GuestId = RetrievedGuest.GuestId;
                dbContext.Wedding.Add(CreateWedding);
                dbContext.SaveChanges();
                return RedirectToAction("Welcome");

            }
            else
            {
                return View("AddWedding");
            }
        }

        [Route("ViewWedding/{id}")]
        [HttpGet]

        public IActionResult ViewWedding(int id)
        {
            // RETRIVEVING THE USER IN SESSION
            IEnumerable<RSVP> Attending = dbContext.RSVP.Where(g => g.WeddingId == id)
            .Include(g=>g.Person).ToList();
            ViewBag.Guests = Attending;
            ViewBag.PersonInSession = HttpContext.Session.GetInt32("Id");
            Wedding CreateWedding = dbContext.Wedding.FirstOrDefault(w => w.WeddingId == id);
            ViewBag.ViewWedding = CreateWedding;
            return View("ViewWedding");
        }

        [Route("RSVP/{WeddingIdRSVP}")]
        [HttpGet]
        public IActionResult RSVP(int WeddingIdRSVP)
        {
            System.Console.WriteLine("////////////////////////////////");
            System.Console.WriteLine(WeddingIdRSVP);
            System.Console.WriteLine(HttpContext.Session.GetInt32("Id"));
            // grab the wedding
            // grab the person(guest)
            // create a new RSVP (record of the 2 things combined)
            // Add both ID's to the RSVP (wedding and person ID)
            // add the person and the wedding to RSVP
            // add rsvp to dbcontext
            // savechanges to dbcontext
            Wedding WeddingToAttend = dbContext.Wedding.FirstOrDefault(w => w.WeddingId == WeddingIdRSVP);
            Guest WeddingAttendee = dbContext.Guest.FirstOrDefault(w => w.GuestId == HttpContext.Session.GetInt32("Id"));
            RSVP newRSVP = new RSVP();
            newRSVP.WeddingId = WeddingIdRSVP;
            newRSVP.GuestId = (int)HttpContext.Session.GetInt32("Id");
            newRSVP.Wedding = WeddingToAttend;
            newRSVP.Person = WeddingAttendee;
            dbContext.RSVP.Add(newRSVP);
            dbContext.SaveChanges();

            return RedirectToAction("Welcome");
        }

        [Route("unRSVP/{WeddingId}")]
        [HttpGet]
        public IActionResult unRSVP(int WeddingId)
        {

            // IE because this handles LITERALLY ALL THE DATA U EVER WANT 
            IEnumerable<RSVP> GuestList = dbContext.RSVP.Where(r => r.WeddingId == WeddingId);
            RSVP flaker = GuestList.SingleOrDefault(a => a.GuestId == HttpContext.Session.GetInt32("Id"));
            dbContext.RSVP.Remove(flaker);
            // Finally, .SaveChanges() will remove the corresponding row representing this User from DB 
            dbContext.SaveChanges();
            return RedirectToAction("Welcome");
        }


        [Route("clear")]
        [HttpGet]
        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        [Route("delete/{WeddingId}")]
        [HttpGet]

        public IActionResult Delete(int WeddingId)
        {
            Wedding Goodbye = dbContext.Wedding.FirstOrDefault(r => r.WeddingId == WeddingId);
            dbContext.Wedding.Remove(Goodbye);
            // Finally, .SaveChanges() will remove the corresponding row representing this User from DB 
            dbContext.SaveChanges();
            return RedirectToAction("Welcome");
        }


    }


}

