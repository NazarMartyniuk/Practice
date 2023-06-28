using Practice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Practice.Controllers
{
    public class WorkerController : Controller
    {
        private readonly PracticeEntities _Data = new PracticeEntities();

        [HttpGet]
        public ActionResult Index()
        {
            WorkersViewModel wvm = new WorkersViewModel();
            wvm.Workers.AddRange(_Data.Workers.Select(x => new WorkerViewModel() { Name = x.Name, Surname = x.Surname, Age = x.Age, Job = (Models.WorkerJob)x.JobId, ProjectName = x.ProjectId != null ? x.Project.Name : "" }));

            return View(wvm);
        }

        [HttpGet]
        public ActionResult HireWorker()
        {
            return View(new CreateWorkerViewModel());
        }

        [HttpPost]
        public ActionResult HireWorker(CreateWorkerViewModel model) 
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            Worker worker = new Worker()
            {
                Name = model.Name,
                Surname = model.Surname,
                Age = model.Age,
                JobId = (int)model.Job,
            };

            _Data.Workers.Add(worker);
            _Data.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}