using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DogGo.Controllers
{
    public class WalkersController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalkRepository _walkRepo;
        private readonly IDogRepository _dogRepo;
        private readonly IOwnerRepository _ownerRepo;

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalkersController(IWalkerRepository walkerRepository, IWalkRepository walkRepository, IDogRepository dogRepository, IOwnerRepository ownerRepository)
        {
            _walkerRepo = walkerRepository;
            _walkRepo = walkRepository;
            _dogRepo = dogRepository;
            _ownerRepo = ownerRepository;
        }

        // GET: WalkersController
        public ActionResult Index()
        {
            List<Walker> walkers = new List<Walker>();
            try
            {
                int userId = GetCurrentUserId();
                Owner currentUser = _ownerRepo.GetOwnerById(GetCurrentUserId());
                walkers = _walkerRepo.GetWalkersInNeighborhood(currentUser.NeighborhoodId);
                return View(walkers);

            }
            catch
            {
                walkers = _walkerRepo.GetAllWalkers();
                return View(walkers);
            }
        } 

        // GET: WalkersController/Details/5
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Walk> walks = _walkRepo.GetWalksByWalkerId(id);

            if (walker == null)
            {
                return NotFound();
            }

            WalkerProfileViewModel vm = new WalkerProfileViewModel()
            {
                Walker = walker,
                Walks = walks
            };
            return View(vm);
        }

        // GET: WalkersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WalkersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/CreateWalks
        public ActionResult CreateWalks(int id)
        {
            int ownerId = GetCurrentUserId();
            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);
            Walker walker = _walkerRepo.GetWalkerById(id);
            CreateWalkFormViewModel vm = new CreateWalkFormViewModel()
            {
                Dogs = dogs,
                Walker = walker,
                Walk = new Walk()
                {
                    WalkerId = walker.Id,
                    Duration = 0,
                }
            };
            return View(vm);
        }

       // POST: WalkersController/CreateWalks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWalks(CreateWalkFormViewModel vm)
        {
            try
            {
                foreach (int dogId in vm.DogIds)
                {
                    Walk newWalk = new Walk()
                    {
                        DogId = dogId,
                        WalkerId = vm.Walk.WalkerId,
                        Duration = vm.Walk.Duration,
                        Date = vm.Walk.Date,
                        WalkStatusId = 1
                    };
                    _walkRepo.AddWalk(newWalk);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(vm);
            }
        }


        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
