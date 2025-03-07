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
            var posts = await _PostServices.GetAllPostsAsync();
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
    }
}
