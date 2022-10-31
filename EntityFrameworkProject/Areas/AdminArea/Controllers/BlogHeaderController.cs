using EntityFrameworkProject.Data;
using EntityFrameworkProject.Models;
using EntityFrameworkProject.ViewModels.BlogHeaderVM;
using EntityFrameworkProject.ViewModels.BlogVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class BlogHeaderController : Controller
    {
        private readonly AppDbContext _context;
    

        public BlogHeaderController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<BlogHeaders> blogHeader = await _context.BlogHeaders.Where(m => !m.IsDeleted)
                .ToListAsync();

            List<BlogHeaderVM> mapDatas = GetMapDatas(blogHeader);

            return View(mapDatas);
        }

        private List<BlogHeaderVM> GetMapDatas(List<BlogHeaders> blogHeaders)
        {
            List<BlogHeaderVM> blogHeaderVM = new List<BlogHeaderVM>();

            foreach (var blogHeader in blogHeaders)
            {
                BlogHeaderVM newHeaderBlog = new BlogHeaderVM
                {
                    Id = blogHeader.Id,
                    Title = blogHeader.Title,
                    Description = blogHeader.Description,
                };

                blogHeaderVM.Add(newHeaderBlog);
            }

            return blogHeaderVM;
        }
    }
}
