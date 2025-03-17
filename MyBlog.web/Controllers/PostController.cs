using Microsoft.AspNetCore.Mvc;
using MyBlog.core.Services;
using MyBlog.domain.Models;

namespace MyBlog.web.Controllers
{
    public class PostController : Controller
    {
        private readonly PostServices _PostServices;

        public PostController(PostServices postServices)
        {
            _PostServices = postServices;
        }

        public async Task<IActionResult> Posts()
        {
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

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            if(!ModelState.IsValid)
            {
                return View(post);
            }
            await _PostServices.AddPostAsync(post);
            var posts = await _PostServices.GetAllPostsAsync();
            return View(nameof(AllPosts),posts);
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
                await _PostServices.DeletePostAsync(id);
                var posts = await _PostServices.GetAllPostsAsync();
                return View(nameof(AllPosts), posts);

            }

            var post = await _PostServices.GetPostAsync(id);
            return View(post);
        }


    }
}
