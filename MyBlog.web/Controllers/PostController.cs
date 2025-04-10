using Microsoft.AspNetCore.Mvc;
using MyBlog.core.Services;
using MyBlog.domain.Models;
using MyBlog.domain.ViewModels;
using MyBlog.infrastructure.Helpers;
using MyBlog.infrastructure.Repositories;
using System.Linq.Expressions;

namespace MyBlog.web.Controllers
{
    public class PostController : Controller
    {
        private readonly PostServices _PostServices;
        private readonly CategoryServices _CategoryServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string[] _AlloerdExtenions = [".jpg",".png",".gif",".jpeg"];
        private readonly IWebHostEnvironment _webHost;

        public PostController(PostServices postServices, CategoryServices categoryServices, IWebHostEnvironment webHost,IUnitOfWork unitOfWork)
        {
            _PostServices = postServices;
            _CategoryServices = categoryServices;
            _webHost = webHost;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Posts(int? categoryId)
        {
            ViewBag.Categories = await _CategoryServices.GetCategories();
            if (categoryId.HasValue)
            {
                var postsData = await _PostServices.GetAllPostsAsync(p => p.CategoryId == categoryId,
                    new Expression<Func<Post, object>>[] { p => p.Category} 
                    );
                return View(postsData);
            }

            // commit    => ok 
            // rollback  => error 


            //var posts = await _PostServices.GetAllPostsAsync();
            var posts = await _PostServices.GetAllIPostsAsync();
            return View(posts);


        }
        public async Task<IActionResult> AllPosts()
        {
            var posts = await _PostServices.GetAllPostsAsync();
            return View("AllPosts",posts);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //ViewBag.Categories = _CategoryServices.GetCategoriesWithSelectListItem() ;// selectListItem
            var postViewModel = new PostViewModel();
            postViewModel.Categories = await _CategoryServices.GetCategoriesWithSelectListItem();
            return View(postViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(/*Post post*/ PostViewModel postViewModel)
        {
            if(ModelState.IsValid)
            {
                var fileExtention = Path.GetExtension(postViewModel.MyImage.FileName).ToLower();
                bool isAllowed = _AlloerdExtenions.Contains(fileExtention);
                if(! isAllowed)
                {

                    ModelState.AddModelError("MyImage", "Extention not allowed");
                    postViewModel.Categories = await _CategoryServices.GetCategoriesWithSelectListItem();
                    return View(postViewModel);
                }

                // upload img????
                //postViewModel.Post.Image = await uploadFile(postViewModel.MyImage,"Posts");
                postViewModel.Post.Image = await UploadFileHelper.UploadFile(postViewModel.MyImage, _webHost.WebRootPath, "Posts");
                await _PostServices.AddPostAsync(postViewModel.Post);
                return RedirectToAction("Posts", "Post");
            }
            postViewModel.Categories = await _CategoryServices.GetCategoriesWithSelectListItem();
            return View(postViewModel);


            //if(!ModelState.IsValid)
            //{
            //    return View(post);
            //}
            //await _PostServices.AddPostAsync(post);
            //var posts = await _PostServices.GetAllPostsAsync();
            //return View(nameof(AllPosts),posts);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Post post)
        {
            await _PostServices.AddPostAsync(post);
            var posts = await _PostServices.GetAllPostsAsync();
            return View(nameof(AllPosts), posts);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var post = await _PostServices.GetPostAsync(id);
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Post post)
        {
            await _PostServices.UpdatePostAsync(post);
            var posts = await _PostServices.GetAllPostsAsync();
            return View(nameof(AllPosts), posts);
        }

        [HttpPost]
        public async Task<IActionResult> EditWithTransaction(Post post)
        {
            // start transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _PostServices.UpdatePostAsync(post);
                await _unitOfWork.ComplteAsync();


                await _unitOfWork.CommitTransactionAsync();
                var posts = await _PostServices.GetAllPostsAsync();
                return View(nameof(AllPosts), posts);

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                // log error
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            var post = await _PostServices.GetPostAsync(id);
            if(post == null)
            {
                //return View("~/Views/Shared/_404.cshtml");
                return View("_404");
            }
            return View(post);
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var post  = await _PostServices.GetPostAsync(id);
            if (post is null)
            {
                return View("_404");
            }
            return View(post);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            if(ModelState.IsValid)
            {
                // get item data
                var PostData = await _PostServices.GetPostAsync(id); // Images/Posts/img.jpg
                // delete image
                string filePath = Path.Combine(_webHost.WebRootPath, PostData.Image);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                await _PostServices.DeletePostAsync(id);
                //var posts = await _PostServices.GetAllPostsAsync();
                //return View(nameof(AllPosts), posts);
                return RedirectToAction("Posts", "Post");

            }

            var post = await _PostServices.GetPostAsync(id);
            return View(post);
        }
        private async Task<string> uploadFile(IFormFile file , string folderName)
        {
            var extention = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + extention; // unique name
            var wwwroot = _webHost.WebRootPath;
            var ImagePath = Path.Combine(wwwroot, "Images/" + folderName);

            if(! Directory.Exists(ImagePath))
                Directory.CreateDirectory(ImagePath);

            var filePath = Path.Combine(ImagePath, fileName);

            try
            {
                await using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return "Error uploading file : " + ex;
            }

            return "Images/"+folderName+"/"+fileName ;
        }

    }
}
