using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WebUI.Data;
using WebUI.Helper;
using WebUI.Models;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext  _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(ILogger<HomeController> logger, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index(int pg = 1)
        {
            const int pageSize = 9;
            if(pg < 1)
            {
                pg = 1;
            }

            int articleCount = _context.Articles.Count();

            var pager = new Pager(articleCount, pg, pageSize);

            int artSkip = (pg - 1) * pageSize;

            var articles = _context.Articles
                .Include(x => x.User)
                .Include(x => x.Category)
                .OrderByDescending(x => x.Id)
                .Skip(artSkip)
                .Take(pager.PageSize)
                .Where(x => x.IsDeleted == false && x.IsActive == true)
                .ToList();

            var firstArticle = _context.Articles
                .Include(x => x.Category).Include(x => x.User)
                .OrderByDescending(x => x.Id)
                .Where(x => x.IsDeleted == false && x.IsActive == true)
                .FirstOrDefault();
            
            HomeVM homeVM = new()
            {
                Articles = articles,
                FirstSlot = firstArticle
            };

            ViewBag.Pager = pager;

            return View(homeVM);
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUser = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            
            if (id == null) return NotFound();
            var article = _context.Articles.Include(x => x.Category).Include(x => x.User).Include(x => x.ArticleTag).ThenInclude(x =>x.Tag).SingleOrDefault(x => x.Id == id);

            var cookie = _httpContextAccessor.HttpContext.Request.Cookies[$"Views"];
            string[] findCookie = { "" };

            if (cookie != null)
            {
                findCookie = cookie.Split('-').ToArray();
            }

            if (!findCookie.Contains(article.Id.ToString()))
            {
                Response.Cookies.Append($"Views", $"{cookie}-{article.Id}",
                    new CookieOptions
                    {
                        Secure = true,
                        HttpOnly = true,
                        Expires = DateTime.Now.AddYears(1),
                    });

                article.ViewCount += 1;
                _context.Articles.Update(article);
                await _context.SaveChangesAsync();
            }


            //article.ViewCount += 1;
            //_context.Articles.Update(article);
            //await _context.SaveChangesAsync();



            if (article == null) return NotFound();
            var popArt = _context.Articles.OrderByDescending(x => x.ViewCount).Take(3).ToList();
            var comments = _context.ArticleComments.Include(x =>x.User).Where(x => x.ArticleId == article.Id).ToList();
            var nextArticle = _context.Articles.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Id < id);
            var prevArticle = _context.Articles.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Id > id);
            var similarArticle = _context.Articles.Include(x => x.Category).OrderByDescending(x => x.Id).Where(x => x.CategoryId == article.CategoryId && x.Id != article.Id).Take(2).ToList();
            DetailVM detailVM = new()
            {
                Article = article,
                PopularArticle = popArt,
                ArticleComments = comments,
                NextArticle = nextArticle,
                PrevArticle = prevArticle, 
                SimilarArticle = similarArticle
            };
            return View(detailVM);
        }

        [HttpPost]  
        public async Task<IActionResult> AddComment(ArticleComment articleComment, int articleId) 
        {
            articleComment.PublishDate = DateTime.Now;
            articleComment.UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _context.ArticleComments.AddAsync(articleComment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", "Home", new {Id = articleId});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId, int articleId)
        {
            var artCom = _context.ArticleComments.FirstOrDefault(x => x.Id == commentId);
            _context.ArticleComments.Remove(artCom);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", "Home", new { Id = articleId });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}