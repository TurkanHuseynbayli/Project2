using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DAL;
using Project.Extensions;
using Project.Helpers;
using Project.Models;

namespace Project.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class CompetitionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CompetitionController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Competitions.Where(c => c.IsDeleted == false).ToList());
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Competition competition)
        {
            if (competition.Photos == null)
            {
                return View();
            }
           
            foreach (IFormFile photo in competition.Photos)
            {

                if (!photo.IsImage())
                {
                    ModelState.AddModelError("Photo", $"{photo.FileName}-not image type");
                    return View();
                }
                if (!photo.MaxSize(200))
                {
                    ModelState.AddModelError("Photo", $"{photo.FileName}-max length 200kb");
                    return View();
                }
                Competition newCompetition = new Competition();
                string path = Path.Combine("img");
                newCompetition.Image = await photo.SaveImageAsync(_env.WebRootPath, path);
                newCompetition.Name = competition.Name;
                newCompetition.Description = competition.Description;
                await _context.Competitions.AddAsync(newCompetition);


                //Slider newSlider = new Slider();
                //newSlider.Image = await photo.SaveImageAsync(_env.WebRootPath, "img",);
                //await _context.Sliders.AddAsync(newSlider);
            }

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
            //if (competition.Photos == null)
            //{
            //    return View();
            //}


            //foreach (IFormFile photo in competition.Photos)
            //{

            //    if (!photo.IsImage())
            //    {
            //        ModelState.AddModelError("Photo", $"{photo.FileName}-not image type");
            //        return View();
            //    }
            //    if (!photo.MaxSize(200))
            //    {
            //        ModelState.AddModelError("Photo", $"{photo.FileName}-max length 200kb");
            //        return View();
            //    }
            //    Competition newCompetition = new Competition();
            //    string path = Path.Combine("img");
            //    newCompetition.Image = await photo.SaveImageAsync(_env.WebRootPath, path);
            //    newCompetition.Name = competition.Name;
            //    newCompetition.Description = competition.Description;

            //    await _context.Competitions.AddAsync(newCompetition);
            //    if (!ModelState.IsValid) return NotFound();
            //    bool isExist = _context.Competitions.Where(c => c.IsDeleted == false).Any(c => c.Name.ToLower() == competition.Name.ToLower());
            //    if (isExist)
            //    {
            //        ModelState.AddModelError("Name", "bu addan var");
            //        return View();
            //    }

            //    competition.IsDeleted = false;
            //    await _context.Competitions.AddAsync(competition);



            //}
            //await _context.SaveChangesAsync();


            //return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return NotFound();
            Competition competition = _context.Competitions.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Id == id);
            if (competition == null) return NotFound();
            return View(competition);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Competition competition = _context.Competitions.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Id == id);
            if (competition == null) return NotFound();
            return View(competition);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            Competition competition = _context.Competitions.Where(c => c.IsDeleted == false).Include(c => c.Persons).FirstOrDefault(c => c.Id == id);
            if (competition == null) return NotFound();

            bool isDeleted = Helper.DeleteImage(_env.WebRootPath, "img", competition.Image);
            if (!isDeleted)
            {
                ModelState.AddModelError(" ", "Some problem exists");
            }
                        competition.IsDeleted = true;
            competition.DeletedTime = DateTime.Now;
            //foreach (Person p in competition.Persons)
            //{
            //    p.IsDeleted = true;
            //}
            _context.Competitions.Remove(competition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Competition competition = _context.Competitions.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Id == id);
            if (competition == null) return NotFound();
            return View(competition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> Update(int? id, Competition competition)
        {

          

            if (id == null) return NotFound();
            if (competition == null) return NotFound();
            Competition comp= await _context.Competitions.FindAsync(id);

            if (!competition.Photo.IsImage())
            {
                ModelState.AddModelError("Photos", $"{competition.Photo.FileName} - not image type");
                return View(comp);
            }
            if (!competition.Photo.MaxSize(200))
            {
                ModelState.AddModelError("Photos", $"{competition.Photo.FileName} - Max image length must be less than 200kb");
                return View(comp);
            }


            string folder = Path.Combine("img");
            string fileName = await competition.Photo.SaveImageAsync(_env.WebRootPath, folder);
            if (fileName == null)
            {
                return Content("Error");
            }

           
           
            Competition isExist = _context.Competitions.Where(c => c.IsDeleted == false).FirstOrDefault(c => c.Name.ToLower() == competition.Name.ToLower());
            if (isExist != null)
            {
                if (isExist != comp)
                {
                    ModelState.AddModelError("Name", "Artiq bu adda category movcuddur");
                    return View();
                }
            }
            comp.Image = fileName;
            comp.Name = comp.Name;
            comp.Description = comp.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


    }
}
