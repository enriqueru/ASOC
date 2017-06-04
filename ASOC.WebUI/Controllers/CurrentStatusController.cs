using ASOC.Domain;
using ASOC.WebUI.Infrastructure.Interfaces;
using ASOC.WebUI.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASOC.WebUI.Controllers
{
    public class CurrentStatusController : Controller
    {
        IRepository<CURRENT_STATUS> currentStatusRepository;

        public CurrentStatusController(IRepository<CURRENT_STATUS> currentStatusRepositoryParam)
        {
            currentStatusRepository = currentStatusRepositoryParam;
        }

        // GET: Index
        public ActionResult Index(int? page, CurrentStatusViewModel modelData)
        {
            if (modelData.searchString != null)
            {
                page = 1;
            }
            else
            {
                modelData.searchString = modelData.currentFilter;
            }

            modelData.currentFilter = modelData.searchString;

            IEnumerable<CURRENT_STATUS> CurLog = currentStatusRepository.GetAllList();

            if (!String.IsNullOrEmpty(modelData.searchString))
            {
                decimal searchDigit;
                bool isInt = Decimal.TryParse(modelData.searchString, out searchDigit);

                if (isInt)
                {
                    CurLog = CurLog.Where(s => s.COMPONENT.Equals(searchDigit)).
                        OrderBy(s => s.DATE_STATUS);
                }
                else
                {
                    CurLog = CurLog.Where(s => s.STATUS.NAME.Contains(modelData.searchString)).
                        OrderBy(s => s.DATE_STATUS);
                }
            }

            if (modelData.firstDate != null)
                CurLog = CurLog.Where(x => x.DATE_STATUS >= modelData.firstDate);
            if (modelData.secondDate != null)
                CurLog = CurLog.Where(x => x.DATE_STATUS <= modelData.secondDate);

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            CurrentStatusViewModel model = new CurrentStatusViewModel()
            {
                CurList = CurLog.ToPagedList(pageNumber, pageSize),
                currentFilter = modelData.currentFilter,
                searchString = modelData.searchString,
                firstDate = modelData.firstDate,
                secondDate = modelData.secondDate
            };
            return View(model);
        }

        // GET: Delete
        public ActionResult Delete(int? id)
        {


            CURRENT_STATUS CurStatus = currentStatusRepository.GetAllList().FirstOrDefault(x => x.ID.Equals(Convert.ToDecimal(id)));

            if (CurStatus == null)
            {
                return HttpNotFound();
            }
            return View(CurStatus);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            currentStatusRepository.Delete(id);
            currentStatusRepository.Save();
            return RedirectToAction("Index");
        }
    }

}
