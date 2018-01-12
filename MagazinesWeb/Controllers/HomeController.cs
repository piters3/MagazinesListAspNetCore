using MagazinesWeb.Data;
using MagazinesWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagazinesWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly MagazinesContext _context;

        public HomeController(MagazinesContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            ViewBag.MagazinesCount = _context.Magazines.Count();
            return View();
        }


        public JsonResult JsonTable()
        {
            //var data = _context.Magazines.Where(m => m.LP != null && m.Title == m.Title.Trim() && m.List == m.List.Replace(";", string.Empty));
            var data = _context.Magazines.Where(m => m.LP != null);

            return Json(new { data });
        }


        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Magazine> magazines = _context.Magazines.Where(m => m.ISSN == id).ToList();
            if (magazines.Count == 0)
            {
                //return Content("Item not found");
                return NotFound();
            }
            else
            {
                magazines.RemoveAt(0);
            }
            if (magazines == null)
            {
                return NotFound();
            }
            return View(magazines);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
