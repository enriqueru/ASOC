using ASOC.Domain;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASOC.WebUI.ViewModels
{
    public class CurrentStatusViewModel: CURRENT_STATUS
    {
        public SelectList statusList { get; set; }

        public string searchString { get; set; }
        public string currentFilter { get; set; }
        public Nullable<DateTime> firstDate { get; set; }
        public Nullable<DateTime> secondDate { get; set; }
        public IPagedList<CURRENT_STATUS> CurList { get; set; }

        // не хорошо
        public IEnumerable<string> Costs { get; set; }
        public void GetCosts (ref CURRENT_STATUS cs)
        {

            if (Costs != null)
            {
                foreach (var item in Costs)
                {
                    cs.STATUS_COSTS.Add(new Domain.STATUS_COSTS
                    {
                        ID_CURRENT = ID,
                        COSTS = item
                });
                }
                

            }
        }
        public string reasonCur { get; set; }
    }
}