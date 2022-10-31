using EntityFrameworkProject.Data;
using EntityFrameworkProject.Helpers;
using EntityFrameworkProject.Models;
using EntityFrameworkProject.ViewModels.BlogVM;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Blog> blog = await _context.Blogs.Where(m => !m.IsDeleted)
                .ToListAsync();

            List<BlogVM> mapDatas = GetMapDatas(blog);

            return View(mapDatas);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return BadRequest();

            Blog dbBlog = await GetByIdAsync((int)id);

            if (dbBlog == null) return NotFound();

            BlogVM blogVM = new BlogVM
            {
                Id = dbBlog.Id,
                Title = dbBlog.Title,
                Description = dbBlog.Desc,
                Photo = dbBlog.Image,
            };

            return View(blogVM);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Blog blog = await _context.Blogs
                .Where(m => !m.IsDeleted && m.Id == id)
                .FirstOrDefaultAsync();

            if (blog == null) return NotFound();

            blog.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            Blog dbBlog = await GetByIdAsync((int)id);

            if (dbBlog == null) return NotFound();

            BlogUpdateVM blogUpdateVM = new BlogUpdateVM
            {
                Id = dbBlog.Id,
                Title = dbBlog.Title,
                Description = dbBlog.Desc,
                Photo = dbBlog.Image,
                
            };

            return View(blogUpdateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogUpdateVM blogUpdateVM)
        {
            if (!ModelState.IsValid) return View(blogUpdateVM);

            Blog dbBlog = await GetByIdAsync(id);

            if (blogUpdateVM.Image != null)
            {
                    if (!blogUpdateVM.Image.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View(blogUpdateVM);
                    }

                    if (!blogUpdateVM.Image.CheckFileSize(500))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View(blogUpdateVM);
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "img", dbBlog.Image);
                    Helper.DeleteFile(path);
                
                    string fileName = Guid.NewGuid().ToString() + "_" + blogUpdateVM.Image.FileName;

                    string pathh = Helper.GetFilePath(_env.WebRootPath, "img", fileName);

                    await Helper.SaveFile(pathh, blogUpdateVM.Image);

                    Blog dbImages = new Blog
                    {
                         Image = fileName
                    };

                    _context.Blogs.Update(dbImages);

            }
                    dbBlog.Title = blogUpdateVM.Title;
                    dbBlog.Desc = blogUpdateVM.Description;

                    await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Create() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCreateVM blogCreateVM)
        {
            if (!ModelState.IsValid) return View(blogCreateVM);

            if (blogCreateVM.Image != null)
            {
                if (!blogCreateVM.Image.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image type");
                    return View(blogCreateVM);
                }

                if (!blogCreateVM.Image.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image size");
                    return View(blogCreateVM);
                }

                string fileName = Guid.NewGuid().ToString() + "_" + blogCreateVM.Image.FileName;

                string pathh = Helper.GetFilePath(_env.WebRootPath, "img", fileName);

                await Helper.SaveFile(pathh, blogCreateVM.Image);

                Blog blog = new Blog
                {
                    Image = fileName,
                    Title = blogCreateVM.Title,
                    Desc = blogCreateVM.Description,
                    Date = DateTime.Now
                };

                await _context.Blogs.AddAsync(blog);

            }
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        private List<BlogVM> GetMapDatas(List<Blog> blogs)
        {
            List<BlogVM> blogVM = new List<BlogVM>();

            foreach (var blog in blogs)
            {
                BlogVM newBlog = new BlogVM
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    Description = blog.Desc,
                    Photo = blog.Image,
                    
                };

                blogVM.Add(newBlog);
            }

            return blogVM;
        }

        

        private async Task<Blog> GetByIdAsync(int id)
        {
            return await _context.Blogs.Where(m => !m.IsDeleted &&  m.Id == id).FirstOrDefaultAsync();
        }
    }
}
