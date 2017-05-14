using System;
using FProj.Repository.Base;
using FProj.Data;
using System.Collections.Generic;
using System.Linq;
using FProj.Api;

namespace FProj.Repository
{
    public class FilmRepository : BaseRepository, IFetch<FilmApi>, ICrud<FilmApi>
    {
        public FilmRepository(FProjContext context) : base(context)
        {
        }

        public FilmApi Default()
        {
            return new FilmApi();
        }

        public FilmApi Create(FilmApi model)
        {
            var filmData = ApiToData.FilmApiToData(model);

            _dbContext.Film.Add(filmData);
            _dbContext.SaveChanges();

            return DataToApi.FilmToApi(filmData);
        }

        public void Delete(int Id)
        {
            var film = _dbContext.Film.FirstOrDefault(x => x.Id == Id);

            if (film == null) return;

            film.IsDeleted = true;
            _dbContext.SaveChanges();
        }

        public List<FilmApi> GetAll(bool IsDeleted = false)
        {
            var filmData = _dbContext.Film.Where(x => x.IsDeleted == IsDeleted).ToList();
            var filmApi = filmData.Select(x => DataToApi.FilmToApi(x));
            return filmApi.ToList();
        }

        public FilmApi GetById(int Id)
        {
            var filmData = _dbContext.Film.FirstOrDefault(x => x.Id == Id);

            if (filmData == null) return null;

            return DataToApi.FilmToApi(filmData);
        }

        public FilmApi Update(FilmApi model)
        {
            var original = _dbContext.Film.FirstOrDefault(x => x.Id == model.Id);
            if (original == null) return null;

            var newData = ApiToData.FilmApiToData(model);

            _dbContext.Entry(original).CurrentValues.SetValues(newData);
            _dbContext.SaveChanges();

            return DataToApi.FilmToApi(newData);
        }

        public ResponsePage<FilmApi> GetPage(int page, int size = 5, UserApi user = null)
        {

            var filmApi = user == null
                ? _dbContext.Film.OrderByDescending(x => x.DateCreated).Where(x => !x.IsDeleted)
                : _dbContext.Film.OrderByDescending(x => x.DateCreated)
                    .Where(x => !x.IsDeleted && x.UserIdCreator == user.Id);

            var data = filmApi
                .Skip((page - 1) * size)
                .Take(size)
                .ToList()
                .Select(DataToApi.FilmToApi)
                .ToList();

            int count = filmApi.Count() % size == 0
                ? filmApi.Count() / size
                : filmApi.Count() / size + 1;

            return new ResponsePage<FilmApi>()
            {
                Data = data,
                Count = count
            };
        }
    }
}
