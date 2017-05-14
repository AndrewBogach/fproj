using FProj.Api;
using FProj.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace FProj.Web.Controllers
{
    public class FilmController : Controller
    {
        // GET: Film
        public ActionResult Index()
        {
            var data = UnitOfWork.Instance.FilmRepository.GetPage(1);

            return View(data);
        }

        [HttpGet]
        public ActionResult GetPage(PageRequest request)
        {
            var data = request == null ? UnitOfWork.Instance.FilmRepository.GetPage(1) : UnitOfWork.Instance.FilmRepository.GetPage(request.PageNumber, request.CountPerPage);

            return PartialView("FilmList", data.Data);
        }

        public ActionResult Details(int Id) => View(UnitOfWork.Instance.FilmRepository.GetById(Id));

        [Authorize]
        [HttpPost]
        public ActionResult UploadPoster(int Id, HttpPostedFileBase file, bool IsPoster = true)
        {
            string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string localPath = Path.Combine(Server.MapPath($"~{WebConfigurationManager.AppSettings["ImageFolder"]}"), uniqueName);

            file.SaveAs(localPath);
            var path = UnitOfWork.Instance.ImageRepository.AddPoster(new Api.ImageApi() { Path = uniqueName }, Id);
            return Json(new { Ok = true, Path = WebConfigurationManager.AppSettings["ImageFolder"] + path });
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Actors = UnitOfWork.Instance.ActorRepository.GetAll();
            return View(UnitOfWork.Instance.FilmRepository.Default());
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(FilmApi model, HttpPostedFileBase file, IEnumerable<HttpPostedFileBase> frames)
        {
            if (file == null)
            {
                ModelState.AddModelError("poster", "hello");
            }

            if (!ModelState.IsValid) return View(model);

            model.DateCreated = DateTime.Now;
            model.User = UnitOfWork.Instance.UserRepository.GetUserByEmail(User.Identity.Name);

            var film = UnitOfWork.Instance.FilmRepository.Create(model);
            if (file != null)
                UploadPoster(film.Id, file);

            if (frames != null)
            {
                var filesApi = new List<ImageApi>();
                foreach (var frame in frames)
                {
                    string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(frame.FileName);
                    string localPath = Path.Combine(Server.MapPath($"~{WebConfigurationManager.AppSettings["ImageFolder"]}"), uniqueName);

                    frame.SaveAs(localPath);
                    filesApi.Add(new ImageApi() { Path = uniqueName });
                }
                UnitOfWork.Instance.ImageRepository.AddPictures(filesApi, film.Id);
            }

            return RedirectToAction("Details", new { Id = film.Id });
        }

        [Authorize]
        public ActionResult MyFilms()
        {
            var user = UnitOfWork.Instance.UserRepository.GetUserByEmail(User.Identity.Name);
            var data = UnitOfWork.Instance.FilmRepository.GetPage(1, user: user);

            return View("MyFilmList", data);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetMyPage(PageRequest request)
        {
            var user = UnitOfWork.Instance.UserRepository.GetUserByEmail(User.Identity.Name);
            var data = request == null ? UnitOfWork.Instance.FilmRepository.GetPage(1, user: user) : UnitOfWork.Instance.FilmRepository.GetPage(request.PageNumber, request.CountPerPage, user);

            return PartialView("FilmList", data.Data);
        }
    }
}