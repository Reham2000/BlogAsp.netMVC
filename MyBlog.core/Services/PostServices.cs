using MyBlog.domain.Models;
using MyBlog.infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.core.Services
{
    public class PostServices
    {
        private readonly IRepository<Post> _postRepo;
        private readonly IPostRepo _IpostRepo;
        private readonly Action<string> _logAction;

        public PostServices(IRepository<Post> postRepo,IPostRepo IpostRepo)
        {
            _postRepo = postRepo;
            _logAction = message => Console.WriteLine($"LOG: {message}");
            _IpostRepo = IpostRepo;
        }


        public async Task<IEnumerable<Post>> GetAllIPostsAsync()
        {
            return await _IpostRepo.GetAllPosts();
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _postRepo.GetAllAsync();
        }
        public async Task<Post> GetPostAsync(int id)
        {
            return await _postRepo.GetByIdAsync(id);
        }
        public async Task AddPostAsync(Post post)
        {
             await _postRepo.AddAsync(post,_logAction);
        }
        public async Task UpdatePostAsync(Post post)
        {
            await _postRepo.UpdateAstnc(post, _logAction);
        }
        public async Task DeletePostAsync(int id)
        {
            await _postRepo.DeleteAsync(id, _logAction);
        }
    }
}
