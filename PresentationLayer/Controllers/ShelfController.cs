using BuisnessLogicLayer.Services;
using BuisnessLogicLayer.ViewModels;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System;
using System.Diagnostics;

namespace PresentationLayer.Controllers
{
    public class ShelfController : Controller
    {
            private readonly IShelfService _shelfService;


            public ShelfController(IShelfService service)
            {
                _shelfService = service;
            }

            [HttpGet]
            public IActionResult Index(IEnumerable<int>? s = null, int? S_id = null)
            {
                if (s.Any() && s != null)
                {
                    IEnumerable<Shelf> shelfs = _shelfService.Shelfs.Where(row => s.Contains(row.Id)).ToList();
                    return View(shelfs);
                }
                else
                {
                    if (TempData["ErrorMessage"] != null)
                    {
                        ViewBag.ErrorMessage = TempData["ErrorMessage"];
                    }
                    else if (TempData["Delete1"] != null)
                    {
                        ViewBag.Delete1 = TempData["Delete1"];
                        ViewBag.Delete2 = TempData["Delete2"];

                    }
                    else if (S_id != null)//the delete is Done
                    {
                        ViewBag.Deleted = "Shelf is deleted successfully";
                    }
                }
                return View(_shelfService.Shelfs);
            }

            [HttpPost]
            public IActionResult Index(string shelfName)
            {
                bool exist = _shelfService.CheckName(shelfName);
                if (!string.IsNullOrEmpty(shelfName) && exist == false)
                {
                    _shelfService.Add(shelfName);
                    ViewBag.Added = shelfName + " is Added Successfully";
                }
                else if (!string.IsNullOrEmpty(shelfName) && exist == true)
                {
                    ViewBag.Exist = shelfName + " is Already Exist";
                }

                return View(_shelfService.Shelfs);
            }



            [HttpGet]
            public IActionResult EditShelf(int id)
            {
                if (TempData["Exist"] != null)
                {
                    ViewBag.Exist = TempData["Exist"];
                }
                if (id == null)
                {
                    return NotFound();
                }

                Shelf? shelf = _shelfService.Shelfs.SingleOrDefault(m => m.Id == id);

                if (shelf == null)
                {
                    return NotFound();
                }
                var newShelf = new EditShelfViewModel
                {
                    Id = shelf.Id,
                    ShelfName = shelf.ShelfName,

                };

                return View(newShelf);
            }

            [HttpPost]
            public IActionResult EditShelf(int id, EditShelfViewModel newShelf)
            {

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Failed to edit shelf");
                    return View("EditShelf", newShelf);
                }
                Shelf? shelf = _shelfService.Shelfs.SingleOrDefault(m => m.Id == id);
                if (shelf == null)
                {
                    return NotFound();
                }
                bool exist = _shelfService.CheckName(newShelf.ShelfName);
                    if (exist == true)
                    {
                        TempData["Exist"] = newShelf.ShelfName + " is Already Exist";
                        return RedirectToAction("EditShelf");
                    }
               
                shelf.Id = id;
                shelf.ShelfName = newShelf.ShelfName;
                

                _shelfService.Update(shelf);
                return RedirectToAction("Index");
            }
            [HttpGet]
            public IActionResult DeleteShelf(int id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                Shelf? shelf = _shelfService.Shelfs.SingleOrDefault(m => m.Id == id);

                if (shelf == null)
                {
                    return NotFound();
                }
                TempData["Delete1"] = "Are you sure you want to delete this shelf ? " + shelf.ShelfName;
                TempData["Delete2"] = "Be Aware That All Related Books Will Be Deleted Too";
                TempData["shelfId"] = shelf.Id;

            return RedirectToAction("Index");

            }

            [HttpPost, ActionName("DeleteShelf")]
            [ValidateAntiForgeryToken]
            public IActionResult DeleteConfirmed()
            {
            int shelfId;
            shelfId = (int)TempData["shelfId"];
            Shelf shelf = _shelfService.Shelfs.SingleOrDefault(m => m.Id == shelfId);
                _shelfService.Delete(shelf);
                _shelfService.DeleteRelatedBooks(shelf.Id);
                return RedirectToAction("Index", new { S_id = shelf.Id });
            }

            [HttpPost]
            public IActionResult SearchShelf(string shelfName)
            {
                IEnumerable<Shelf>? shelfs = _shelfService.GetShelfByName(shelfName);

                if (shelfs.Any())
                {
                    IEnumerable<int> ShelfIdList = _shelfService.GetShelfsID(shelfs);
                    return RedirectToAction("Index", new { s = ShelfIdList });
                }
                else
                {
                    TempData["ErrorMessage"] = "No Shelfs Found";
                    return RedirectToAction("Index");
                }
            }
           
    }
}

