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
            TimeSpan t = TimeSpan.FromSeconds(total);
            return string.Format("{0:D2} hours {1:D2} min", t.Hours, t.Minutes);
        }
    }
}
