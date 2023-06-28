using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Practice.code
{
    public class Functions
    {
        public static string GetTaskTypeColor(Practice.Models.TaskType type)
        {
            string typeColor = "";
            switch(type)
            {
                case Models.TaskType.Bug:
                    typeColor = "#d9534f";
                    break;
                case Models.TaskType.Epic:
                case Models.TaskType.Story:
                case Models.TaskType.Feature:
                    typeColor = "#5cb85c";
                    break;
                default:
                    break;
            }
            return typeColor;
        }

        public static string GetTaskPriorityColor(Practice.Models.PriorityType priority)
        {
            string priorityColor = "";
            switch(priority)
            {
                case Models.PriorityType.High:
                    priorityColor = "#d9534f";
                    break;
                case Models.PriorityType.Medium:
                    priorityColor = "#f0ad4e";
                    break;
                case Models.PriorityType.Low:
                    priorityColor = "#5bc0de";
                    break;
            }
            return priorityColor;
        }
    }
}