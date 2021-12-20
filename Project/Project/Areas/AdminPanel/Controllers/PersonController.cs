using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DAL;
using Project.Models;

namespace Project.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class PersonController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public PersonController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Person> person = _context.Persons.Where(bg => bg.IsDeleted == false)
               .Include(bg => bg.Competition).ToList();
            return View(person);
            
        }
        public async Task<IActionResult> Detail(int? id)
        {
            Person person = await _context.Persons.Include(p => p.Competition).FirstOrDefaultAsync(p => p.Id == id);
            return View(person);
        }

         public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person)
        {
            if (!ModelState.IsValid) return NotFound();
            bool isExist = _context.Persons.Where(p => p.IsDeleted == false).Any(p => p.Name.ToLower() == person.Name.ToLower());
            if (isExist)
            {
                ModelState.AddModelError("Name", "bu addan var");
                return View();
            }
            person.Name = person.Name;
            person.Surname = person.Surname;
            person.Age = person.Age;
            person.Email = person.Email;
            person.IsDeleted = false;
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



           

            //await _context.Persons.AddAsync(newEvent);
            //await _context.SaveChangesAsync();

            //Person newPerson = new Person();
            ////Competition newCompetition = new Competition();
            //if (!ModelState.IsValid)
            //{
            //    ModelState.AddModelError("", "Error");
            //    return View();
            //}

            //if (person.Name == null)
            //{
            //    ModelState.AddModelError("Name", "Name cannot be empty");
            //    return View();
            //}
            //if (person.Surname == null)
            //{
            //    ModelState.AddModelError("Surname", "Surname cannot be empty");
            //    return View();
            //}

            //if (person.Age == null)
            //{
            //    ModelState.AddModelError("Age", "Age cannot be empty");
            //    return View();
            //}
            //if (person.Email == null)
            //{
            //    ModelState.AddModelError("Email", "Email cannot be empty");
            //    return View();
            //}









            //newPerson.Name = newPerson.Name;
            //newPerson.Surname = newPerson.Surname;
            //newPerson.Age = newPerson.Age;
            //newPerson.Email = newPerson.Email;
            //await _context.AddAsync(newPerson);
            //await _context.SaveChangesAsync();


            ////newCompetition.Description = person.Competition.Description;


            ////newPerson.CompetitionId = newCompetition.Id;

            //await _context.AddAsync(/*newPerson*/person);
            //await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));

        }
        public IActionResult Update(int? id)
        {
            

            Person blogs = _context.Persons.Include(bg => bg.Competition).FirstOrDefault(bg => bg.Id == id);
            return View(blogs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Person person)
        {
           
            if (id == null) return NotFound();


            Person personOld = await _context.Persons.Include(c => c.Competition).FirstOrDefaultAsync(c => c.Id == id);
            Person isExist = _context.Persons.Where(cr => cr.IsDeleted == false).FirstOrDefault(cr => cr.Id == id);
            bool exist = _context.Persons.Where(cr => cr.IsDeleted == false).Any(cr => cr.Name == person.Name);

            if (exist)
            {
                if (isExist.Name != person.Name)
                {
                    ModelState.AddModelError("Name", "This name already has. Please write another name");
                    return View(personOld);
                }
            }

            if (person == null) return Content("Null");
           
            personOld.Surname = person.Surname;
            personOld.Age = person.Age;
            personOld.Email = person.Email;
            personOld.Competition.Description = person.Competition.Description;
            personOld.Name = person.Name;

      

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Person person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound();
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            Person person = _context.Persons.FirstOrDefault(c => c.Id == id);
            if (person == null) return NotFound();

            if (!person.IsDeleted)
            {
                person.IsDeleted = true;
                person.DeletedTime = DateTime.Now;
            }
            else
                person.IsDeleted = false;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        

       
        }
    }
}
