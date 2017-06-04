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
    }
}