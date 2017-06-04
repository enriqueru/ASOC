using ASOC.Domain;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASOC.WebUI.ViewModels
{
    public class ComponentViewModel: COMPONENT
    {
        public SelectList listStatus { get; set; }

        public decimal currentCoast { get; set; }
        public decimal oldCoast { get; set; }
        public string currentStatus { get; set; }
        public string searchString { get; set; }
        public string currentFilter { get; set; }
        public SelectList typeList { get; set; }
        public SelectList statusList { get; set; }
        public IPagedList<ComponentViewModel> componentList { get; set; }
        public SelectList listType { get; set; }
        public SelectList listModel { get; set; }
    }
}