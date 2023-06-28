using Practice.code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Practice.Models
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpectedExpirationDate { get; set; }
        public ProjectWorkersViewModel ProjectWorkersViewModel { get; set; }
        public ProjectTasksViewModel ProjectTasksViewModel { get; set; }
    }

    public class ProjectsViewModel
    {
        public List<ProjectViewModel> Projects { get; set; } = new List<ProjectViewModel>();
    }

    public class CreateProjectViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Project Creation Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Project Expected Expiration Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime ExpectedExpirationDate { get; set; } = DateTime.Now;
    }

    public enum TaskType
    {
        [EnumDisplayName("Bug")]
        Bug = 1,
        [EnumDisplayName("Feature")]
        Feature = 2,
        [EnumDisplayName("Story")]
        Story = 3,
        [EnumDisplayName("Epic")]
        Epic = 4
    }

    public enum PriorityType
    {
        [EnumDisplayName("Low")]
        Low = 1,
        [EnumDisplayName("Medium")]
        Medium = 2,
        [EnumDisplayName("High")]
        High = 3
    }

    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskType Type { get; set; }
        public PriorityType Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Worker { get; set; }
        public int ProjectId { get; set; }
    }

    public class ProjectWorkersViewModel
    {
        public int Id { get; set; }
        public List<WorkerViewModel> WorkersOnProject { get; set; } = new List<WorkerViewModel>();
    }

    public class AddWorkerToProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<SelectListItem> Workers { get; set; }
        public int WorkerId { get; set; }
    }

    public class ProjectTasksViewModel
    {
        public int ProjectId { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
    }

    public class AddTaskToProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskType Type { get; set; }
        public PriorityType Priority { get; set; }
        [Display(Name = "Task Creation Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Task Expiration Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; } = DateTime.Now;
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
        public List<SelectListItem> Watchers { get; set; }
        public int WatcherId { get; set; }
        public List<SelectListItem> Workers { get; set; }
        public int WorkerId { get; set; }
    }

    public class TaskAttachment
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TaskAttachmentsViewModel
    {
        public int TaskId { get; set; }
        public bool Show { get; set; }
        public IEnumerable<TaskAttachment> Attachments { get; set; }
    }

    public class TaskDetailsViewModel
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public TaskType Type { get; set; }
        public PriorityType Priority { get; set; }
        public string Watcher { get; set; }
        public string Worker { get; set; }
        public TaskAttachmentsViewModel Attachments { get; set; }
    } 
}