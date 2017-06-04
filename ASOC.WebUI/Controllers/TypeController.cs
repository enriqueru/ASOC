using ASOC.Domain;
using ASOC.WebUI.Infrastructure.Interfaces;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASOC.WebUI.Controllers
{
    public class TypeController : Controller
    {
        IRepository<TYPE> typeRepository;

        public TypeController(IRepository<TYPE> typeRepositoryParam)
        {
            typeRepository = typeRepositoryParam;
        }
              
        // GET: Index                  
        public ActionResult Index(int? page, string currentFilter, string searchString)
        {
            if (ModelState.IsValid)
            {
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                var types = typeRepository.GetAllList();

                if (!String.IsNullOrEmpty(searchString))
                {
                    types = types.Where(s => s.NAME.Contains(searchString)).OrderBy(s => s.NAME);
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(types.ToPagedList(pageNumber, pageSize));
            }
            Entities db = new Entities();
            return View(db.TYPE.ToList());
        }     

        // GET: Delete
        public ActionResult Delete(int? id)
        {
           
            if (id == null)
            {
                return HttpNotFound();
            }

            TYPE type = typeRepository.GetAllList().FirstOrDefault(x => x.ID.Equals(Convert.ToDecimal(id)));

            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);   
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {            
            typeRepository.Delete(id);
            typeRepository.Save();
            return RedirectToAction("Index");         
        }

        // Get: Edit
        public ActionResult Edit(int? id)
        {          
            if (id == null)
            {
                return HttpNotFound();
            }
            TYPE type = typeRepository.GetAllList().First(x => x.ID.Equals(Convert.ToDecimal(id)));
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
            
        }

        // POST: Edit              
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TYPE type)
        {           
            if (ModelState.IsValid)
            {
                typeRepository.Update(type);
                typeRepository.Save();
                return RedirectToAction("Index");
            }
            return View(type);           
        }

        // Get: Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TYPE type)
        {            
            if (ModelState.IsValid)
            {
                typeRepository.Create(type);
                typeRepository.Save();
                return RedirectToAction("Index");
            }
            return View(type);         
        }
    }
}