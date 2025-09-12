using Services;
using System.Collections.Generic;

namespace Services
{
    public class FavoritesService
    {
        private readonly Infrastructure.FavoritesRepository _repo;
        public FavoritesService(Infrastructure.FavoritesRepository repo) => _repo = repo;

        public int Count() => _repo.Count();
        public void Add(string ticker) => _repo.Add(ticker);
        public void Remove(string ticker) => _repo.Remove(ticker);
        public IEnumerable<string> All() => _repo.All();
    }
}
