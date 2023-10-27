using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Data;
using WebUI.Models;

namespace WebUI.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin, Admin Editor, Moderator")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;
        public TagController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var tags = _context.Tags.ToList();
            return View(tags);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tag tag)
        {
            var tagName = _context.Tags.FirstOrDefault(x => x.TagName == tag.TagName);
            if(tagName != null)
            {
                ModelState.AddModelError("Error", "Tag name is exist!");
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    tag.CreatedDate = DateTime.Now;
                    _context.Tags.Add(tag);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index), nameof(Tag));
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpGet]
        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            Tag tag = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null) return NotFound();
            var tag = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpPost]
        public IActionResult Update(Tag tag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(tag);
                }

                tag.UpdatedDate = DateTime.Now;
                _context.Tags.Update(tag);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Errors = ex.Message;
                throw;
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var tag = _context.Tags.FirstOrDefault(y => y.Id == id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpPost]
        public IActionResult Delete(Tag tag)
        {
            _context.Tags.Remove(tag);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
