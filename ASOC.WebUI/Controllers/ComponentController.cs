using ASOC.Domain;
using ASOC.WebUI.Infrastructure.Interfaces;
using ASOC.WebUI.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASOC.WebUI.Controllers
{
    public class ComponentController : Controller
    {
        IRepository<COMPONENT> componentRepository;
        IGetList getList;
        IRepository<CURRENT_STATUS> statusRepository;

        public ComponentController(IRepository<COMPONENT> componentRepositoryParam, IGetList getListParam,
            IRepository<CURRENT_STATUS> statusRepositoryParam)
        {
            componentRepository = componentRepositoryParam;
            getList = getListParam;
            statusRepository = statusRepositoryParam;
        }




        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return HttpNotFound();
            }


            Entities db = new Entities();
            //COMPONENT comp = componentRepository.GetAllList().First(x => x.ID.Equals(id));
            COMPONENT comp = db.COMPONENT.Find(Convert.ToDecimal(id));


            var n = new ComponentViewModel()
            {
                ID_MODEL = comp.ID_MODEL,
                ID_TYPE = comp.ID_TYPE,
                ID_SERIES = comp.ID_SERIES,
                DATE_ADD = comp.DATE_ADD,
                PARTNUMBER = comp.PARTNUMBER,
                CURRENT_STATUS = comp.CURRENT_STATUS,
                MODEL = comp.MODEL,
                TYPE = comp.TYPE,
                currentStatus = comp.CURRENT_STATUS.Where(x => x.ID_COMPLECT.Equals(Convert.ToDecimal(id)))
                        .OrderByDescending(x => x.DATE_STATUS).FirstOrDefault().STATUS.NAME,
                currentCoast = comp.MODEL.PRICE.Where(x => x.ID_MODEL.Equals(Convert.ToDecimal(comp.ID_MODEL)))
                        .OrderByDescending(x => x.DATE_ADD).FirstOrDefault().COAST

            };

            return View(n);


        }



        // GET: Index                  
        public ActionResult Index(int? page, ComponentViewModel modelData)
        {
            if (modelData.searchString != null)
            {
                page = 1;
            }
            else
            {
                modelData.ID_TYPE = modelData.ID_TYPE;
            }

            modelData.currentFilter = modelData.searchString;

            var components = componentRepository.GetAllList();

            if (modelData.ID_TYPE != null)
            {
                components = components.Where(s => s.ID_TYPE.Equals(modelData.ID_TYPE));
            }           

            decimal searchDigit;
            bool isInt = Decimal.TryParse(modelData.searchString, out searchDigit);

            if (!String.IsNullOrEmpty(modelData.searchString))
            {
                if (!isInt)
                {                    
                    if (components.Where(m => m.TYPE.NAME.Contains(modelData.searchString)).Count() != 0)
                    {
                        components = components.Where(m => m.TYPE.NAME.Contains(modelData.searchString));
                    }
                    if(components.Where(m => m.MODEL.NAME.Contains(modelData.searchString)).Count() != 0)
                    {
                        components = components.Where(s => s.MODEL.NAME.Contains(modelData.searchString));
                    }                   
                }
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            List<ComponentViewModel> componentList = new List<ComponentViewModel>();

            foreach (COMPONENT item in components)
            {
                componentList.Add(new ComponentViewModel()
                {
                    ID = item.ID,                    
                    DATE_ADD = item.DATE_ADD,
                    ID_MODEL = item.ID_MODEL,
                    ID_SERIES = item.ID_SERIES,
                    ID_TYPE = item.ID_TYPE,                   
                    PARTNUMBER = item.PARTNUMBER,
                    currentStatus = item.CURRENT_STATUS.Where(x => x.ID_COMPLECT.Equals(item.ID))
                        .OrderByDescending(x => x.DATE_STATUS).FirstOrDefault().STATUS.NAME,
                    MODEL = item.MODEL,
                    TYPE = item.TYPE,
                    
                });
            }

            if (!String.IsNullOrEmpty(modelData.searchString))
            {
                if (isInt)
                {
                    componentList = componentList.FindAll(m => m.currentCoast.Equals(searchDigit));
                }
            }

            ComponentViewModel model = new ComponentViewModel
            {
                componentList = componentList.ToPagedList(pageNumber, pageSize),
                typeList = getList.getTypeSelectList(),
                searchString = modelData.searchString,
                currentFilter = modelData.currentFilter,
                statusList = getList.getStatusSelectList()
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult StatusChange(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            COMPONENT component = componentRepository.GetAllList().
                FirstOrDefault(x => x.ID.Equals(Convert.ToDecimal(id)));

            if (component == null)
            {
                return HttpNotFound();
            }

          
            decimal status = component.CURRENT_STATUS.Where(x => x.ID_COMPLECT.Equals(component.ID))
                .OrderByDescending(x => x.DATE_STATUS).FirstOrDefault().ID_STATUS;
            string statusName = component.CURRENT_STATUS.Where(x => x.ID_COMPLECT.Equals(component.ID))
                .OrderByDescending(x => x.DATE_STATUS).FirstOrDefault().STATUS.NAME;

            Entities db = new Entities();
            //var reason = db.STATUS_REASON.Where(x => x.ID_CURRENT).FirstOrDefault().REASON;


            CurrentStatusViewModel modelData = new CurrentStatusViewModel()
            {
                ID = component.ID,
                ID_COMPLECT = component.ID,
                ID_STATUS = status,
                statusList = getList.getStatusSelectList(),
                //reasonCur = Convert.ToString(reason)
            };
            


           
            return View(modelData);
        }

        [HttpPost]
        public ActionResult StatusChange(CurrentStatusViewModel modelData)
        {
            if (ModelState.IsValid)
            {
                CURRENT_STATUS status = new CURRENT_STATUS()
                {
                    ID_COMPLECT = modelData.ID_COMPLECT,
                    ID_STATUS = modelData.ID_STATUS,
                    DATE_STATUS = DateTime.Now
                };

                modelData.GetCosts(ref status);
                statusRepository.Create(status);
                statusRepository.Save(); 

                return RedirectToAction("Index");
            }
            else
            { return HttpNotFound(); }






        }

        // GET: Delete
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return HttpNotFound();
            }

            COMPONENT component = componentRepository.GetAllList().FirstOrDefault(x => x.ID.Equals(id));

            if (component == null)
            {
                return HttpNotFound();
            }
            return View(component);
        }

   

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            componentRepository.Delete(id);
            componentRepository.Save();
            return RedirectToAction("Index");
        }

        // Get: Edit
        public ActionResult Edit(int? id)
        {

            Entities db = new Entities();
            COMPONENT comp = db.COMPONENT.Find(id);

            var n = new ComponentViewModel()
            {
                listType = new SelectList(db.TYPE, "ID", "NAME"),
                listModel = new SelectList(db.MODEL, "ID", "NAME"),
                listStatus = new SelectList(db.STATUS, "ID", "NAME"),
                ID_MODEL = comp.ID_MODEL,
                ID_TYPE = comp.ID_TYPE,
                ID_SERIES = comp.ID_SERIES,
                DATE_ADD = comp.DATE_ADD,
                PARTNUMBER = comp.PARTNUMBER,
            };
            return View(n);


        }

        // POST: Edit              
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(COMPONENT component)
        {
            if (ModelState.IsValid)
            {
                componentRepository.Update(component);
                componentRepository.Save();
                return RedirectToAction("Index");
            }
            return View(component);
        }

      public ActionResult Create()
        {
            Entities db = new Entities();
            var n = new ComponentViewModel();
            n.listType = new SelectList(db.TYPE, "ID", "NAME");
            n.listModel = new SelectList(db.MODEL, "ID", "NAME");
            n.listStatus = new SelectList(db.STATUS, "ID", "NAME");
            return View(n);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComponentViewModel l)
        {
            Entities db = new Entities();

            //componentRepository.Create(l);

            db.Entry(new CURRENT_STATUS()
            {
                ID = l.ID,
                ID_COMPLECT = l.ID,
                ID_STATUS = Convert.ToDecimal(l.listStatus),
                DATE_STATUS = DateTime.Now
            }).State = EntityState.Added;

            db.Entry(
                new COMPONENT()
                {
                    ID = l.ID,
                    ID_MODEL = l.ID_MODEL,
                    ID_SERIES = l.ID_SERIES,
                    ID_TYPE = l.ID_TYPE,
                    PARTNUMBER = l.PARTNUMBER,
                    DATE_ADD = l.DATE_ADD
                }
                ).State = EntityState.Added;


            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}