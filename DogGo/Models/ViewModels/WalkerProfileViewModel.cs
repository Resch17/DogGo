using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models.ViewModels
{
    public class WalkerProfileViewModel
    {
        public Walker Walker { get; set; }
        public List<Walk> Walks { get; set; }
        public string TotalWalkDuration(List<Walk> walks)
        {
            List<int> durations = walks.Select(w => w.Duration).ToList();
            int total = durations.Sum();
            int minutes = total / 60;
            int hours = (minutes - minutes % 60) / 60;
            return $"{hours}hr {minutes}min";
        }
    }
}
