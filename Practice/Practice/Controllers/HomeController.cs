using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Practice.code.MailService;
using Practice.Models;

namespace Practice.Controllers
{
    public class HomeController : Controller
    {
        private readonly PracticeEntities _Data = new PracticeEntities();

        [HttpGet]
        public ActionResult Index()
        {
            ProjectsViewModel pvm = new ProjectsViewModel();
            pvm.Projects.AddRange(_Data.Projects.Select(x => new ProjectViewModel() { Id = x.Id, Name = x.Name, Description = x.Description, CreatedDate = x.CreatedDate, ExpectedExpirationDate = x.ExpectedExpirationDate }));
            return View(pvm);
        }

        [HttpGet]
        public ActionResult CreateProject()
        {
            return View(new CreateProjectViewModel());
        }

        [HttpPost]
        public ActionResult CreateProject(CreateProjectViewModel model)
        {
            if (model.ExpectedExpirationDate < model.CreatedDate)
                ModelState.AddModelError("", "Expiration date cannot be fewer than creation date");
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            Project project = new Project()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedDate = model.CreatedDate,
                ExpectedExpirationDate = model.ExpectedExpirationDate
            };

            var addedProject = _Data.Projects.Add(project);
            _Data.SaveChanges();

            return RedirectToAction("Details", new { id = addedProject.Id });
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var project = _Data.Projects.FirstOrDefault(x => x.Id == id);
            ProjectViewModel pvm = new ProjectViewModel()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedDate = project.CreatedDate,
                ExpectedExpirationDate = project.ExpectedExpirationDate,
                ProjectWorkersViewModel = new ProjectWorkersViewModel()
                {
                    Id = project.Id,
                    WorkersOnProject = _Data.Workers.Where(x => x.ProjectId == project.Id).Select(x => new WorkerViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Surname = x.Surname,
                        Age = x.Age,
                        Job = (Models.WorkerJob)x.JobId
                    }).ToList()
                },
                ProjectTasksViewModel = new ProjectTasksViewModel()
                {
                    ProjectId = project.Id,
                    Tasks = _Data.Tasks.Where(t => t.ProjectId == project.Id)
                        .Select(t => new TaskViewModel()
                        {
                            Id= t.Id,
                            Name = t.Name,
                            Description = t.Description,
                            Type = (Models.TaskType)t.TaskType.Id,
                            Priority = (Models.PriorityType)t.PriorityId,
                            CreatedDate = t.CreationDate,
                            ExpirationDate = t.ExpirationDate,
                            Worker = t.Worker.Name + " " + t.Worker.Surname,
                            ProjectId = project.Id
                        }).ToList()
                }
            };

            return View(pvm);
        }

        private List<SelectListItem> GetWorkersForAddingWorkers()
        {
            return new SelectList(_Data.Workers.Where(x => x.ProjectId == null).Select(a => new { Id = a.Id, Name = a.Name + " " + a.Surname }), "Id", "Name").ToList();
        }

        private List<SelectListItem> GetWorkersForAddingTask(int id)
        {
            return new SelectList(_Data.Workers.Where(x => x.ProjectId == id).Select(a => new { Id = a.Id, Name = a.Name + " " + a.Surname }), "Id", "Name").ToList();
        }

        [HttpGet]
        public ActionResult AddWorkerToProject(int id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            AddWorkerToProjectViewModel vm = new AddWorkerToProjectViewModel() { ProjectId = id, ProjectName = _Data.Projects.FirstOrDefault(x => x.Id == id).Name };
            vm.Workers = GetWorkersForAddingWorkers();

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddWorkerToProject(AddWorkerToProjectViewModel model)
        {
            _Data.Workers.FirstOrDefault(x => x.Id == model.WorkerId).ProjectId = model.ProjectId;
            _Data.SaveChanges();

            return RedirectToAction("Details", new { id = model.ProjectId });
        }

        [HttpGet]
        public ActionResult AddTaskToProject(int id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            AddTaskToProjectViewModel vm = new AddTaskToProjectViewModel()
            {
                ProjectId = id,
                ProjectName = _Data.Projects.FirstOrDefault(x => x.Id == id).Name,
                Watchers = GetWorkersForAddingTask(id),
                Workers = GetWorkersForAddingTask(id)
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddTaskToProject(AddTaskToProjectViewModel model)
        {
            if (model.ExpirationDate < model.CreationDate)
                ModelState.AddModelError("", "Expiration date cannot be fewer than creation date");

            if (!ModelState.IsValid)
                return View(model);

            Task task = new Task()
            {
                Name = model.Name,
                Description = model.Description,
                CreationDate = model.CreationDate,
                ExpirationDate = model.ExpirationDate,
                ProjectId = model.ProjectId,
                TaskTypeId = (int)model.Type,
                PriorityId = (int)model.Priority,
                WatcherId = model.WatcherId,
                WorkerId = model.WorkerId
            };
            _Data.Tasks.Add(task);
            _Data.SaveChanges();

            var taskAttachments = new List<MailAttachment>();
            if(model.Files != null)
            {
                foreach (var file in model.Files)
                {
                    if(file != null)
                    {
                        if (string.IsNullOrEmpty(file.FileName))
                            throw new ArgumentException("Attachment name cannot be empty");
                        var attachment = new Attachement()
                        {
                            FileName = file.FileName
                        };
                        var stream = file.InputStream as MemoryStream;
                        if(stream == null)
                        {
                            stream = new MemoryStream();
                            file.InputStream.CopyTo(stream);
                        }
                        attachment.FileContent = stream.ToArray();
                        _Data.Attachements.Add(attachment);
                        _Data.AttachmentTasks.Add(new AttachmentTask() { TaskId = task.Id, AttachmentId = attachment.Id });
                        _Data.SaveChanges();
                        taskAttachments.Add(new MailAttachment() { Content = attachment.FileContent, Name = string.IsNullOrEmpty(attachment.FileName) ? "defaukt" : attachment.FileName });
                    }
                }
            }

            return RedirectToAction("Details", new { id = model.ProjectId });
        }

        [HttpGet]
        public ActionResult TaskDetails(int id)
        {
            ViewBag.ReturnUrl = null;

            var task = _Data.Tasks.FirstOrDefault(t => t.Id == id);
            TaskDetailsViewModel vm = new TaskDetailsViewModel()
            {
                TaskId = id,
                ProjectId = task.ProjectId,
                Name = task.Name,
                ProjectName = _Data.Projects.FirstOrDefault(p => p.Id == task.ProjectId).Name,
                Description = task.Description,
                CreationDate = task.CreationDate,
                ExpirationDate = task.ExpirationDate,
                Type = (Models.TaskType)task.TaskTypeId,
                Priority = (PriorityType)task.PriorityId,
                Attachments = new TaskAttachmentsViewModel()
                {
                    TaskId = id
                }
            };

            vm.Attachments.Attachments = (from record in _Data.AttachmentTasks
                                          where record.TaskId == id
                                          join attach in _Data.Attachements
                                          on record.AttachmentId equals attach.Id
                                          select new TaskAttachment()
                                          {
                                              Id = attach.Id,
                                              Name = attach.FileName
                                          }).ToList();

            if(task.WatcherId != null)
            {
                var watcher = _Data.Workers.FirstOrDefault(w => w.Id == task.WatcherId);
                vm.Watcher = watcher.Name + " " + watcher.Surname;
            }
            if(task.WorkerId != null)
            {
                var worker = _Data.Workers.FirstOrDefault(w => w.Id == task.WorkerId);
                vm.Worker = worker.Name + " " + worker.Surname;
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult GetAttachment(int attachmentId)
        {
            var file = _Data.Attachements.FirstOrDefault(a => a.Id == attachmentId);
            if (file == null)
                return null;
            return File(file.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
        }

        [HttpPost]
        public PartialViewResult RemoveAttachment(int taskId, int attachmentId)
        {
            var task = _Data.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                var attachment = _Data.Attachements.FirstOrDefault(a => a.Id == attachmentId);
                if (attachment != null)
                {
                    using(var dbTransaction = _Data.Database.BeginTransaction())
                    {
                        try
                        {
                            var related = _Data.AttachmentTasks.FirstOrDefault(at => at.AttachmentId == attachment.Id && at.TaskId == task.Id);
                            if (related != null)
                                _Data.Entry(related).State = System.Data.Entity.EntityState.Deleted;
                            _Data.SaveChanges();
                            _Data.Entry(attachment).State = System.Data.Entity.EntityState.Deleted;
                            _Data.SaveChanges();
                            dbTransaction.Commit();
                        }
                        catch(Exception e)
                        {
                            dbTransaction.Rollback();
                            ModelState.AddModelError("", "Error while updating records in database");
                        }
                    }
                }
            }

            var attachments = (from record in _Data.AttachmentTasks
                               where record.TaskId == task.Id
                               join attach in _Data.Attachements
                               on record.AttachmentId equals attach.Id
                               select new TaskAttachment()
                               {
                                   Id = attach.Id,
                                   Name = attach.FileName
                               }).ToList();
            var ret = new TaskAttachmentsViewModel() { TaskId = task.Id, Attachments = attachments };

            return PartialView("_AttachmentsList", ret);
        }

        [HttpPost]
        public PartialViewResult AddAttachments(int taskId)
        {
            var task = _Data.Tasks.FirstOrDefault(t => t.Id == taskId);
            if(task != null)
            {
                if(Request.Files != null && Request.Files.Count > 0)
                {
                    var fls = Request.Files;
                    bool hasEmpty = false;

                    for(int i = 0; i < fls.Count; i++)
                    {
                        if (fls[i].ContentLength == 0)
                            hasEmpty = true;
                    }

                    if (hasEmpty)
                        ModelState.AddModelError("", $"Attachments cannot be empty");
                    if (ModelState.IsValid)
                    {
                        var taskAttachments = new List<MailAttachment>();
                        using(var dbTransaction = _Data.Database.BeginTransaction())
                        {
                            try
                            {
                                for(int i = 0; i < fls.Count; i++)
                                {
                                    var file = fls[i];
                                    if(file != null)
                                    {
                                        var stream = file.InputStream as MemoryStream;
                                        if(stream == null)
                                        {
                                            stream = new MemoryStream();
                                            file.InputStream.CopyTo(stream);
                                        }
                                        var attachment = new Attachement()
                                        {
                                            FileName = Path.GetFileName(file.FileName),
                                            FileContent = stream.ToArray()
                                        };
                                        _Data.Attachements.Add(attachment);
                                        _Data.SaveChanges();
                                        _Data.AttachmentTasks.Add(new AttachmentTask() { AttachmentId = attachment.Id, TaskId = task.Id });
                                        _Data.SaveChanges();
                                        taskAttachments.Add(new MailAttachment() { Content = attachment.FileContent, Name = string.IsNullOrEmpty(attachment.FileName) ? "default" : attachment.FileName });
                                    }
                                }

                                dbTransaction.Commit();
                            }
                            catch (Exception e) 
                            {
                                dbTransaction.Rollback();
                                ModelState.AddModelError("", "Error while updating records in database");
                            }
                        }
                    }
                }
            }
            var attachments = (from record in _Data.AttachmentTasks
                               where record.TaskId == task.Id
                               join attach in _Data.Attachements
                               on record.AttachmentId equals attach.Id
                               select new TaskAttachment()
                               {
                                   Id = attach.Id,
                                   Name = attach.FileName
                               }).ToList();
            var ret = new TaskAttachmentsViewModel() { TaskId = task.Id, Attachments = attachments, Show = true };

            return PartialView("_AttachmentsPanel", ret);
        }
    }
}