using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Practice.code;

namespace Practice.Models
{
    public class WorkerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public WorkerJob Job { get; set; }
        public string ProjectName { get; set; }
    }

    public enum WorkerJob
    {
        [EnumDisplayName("Programmer")]
        Programmer = 1,
        [EnumDisplayName("Teamlead")]
        TeamLead = 2,
        [EnumDisplayName("QA")]
        QA = 3,
        [EnumDisplayName("Designer")]
        Designer = 4
    }

    public class WorkersViewModel
    {
        public List<WorkerViewModel> Workers { get; set; } = new List<WorkerViewModel>();
    }

    public class CreateWorkerViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public WorkerJob Job { get; set; }
    }
}