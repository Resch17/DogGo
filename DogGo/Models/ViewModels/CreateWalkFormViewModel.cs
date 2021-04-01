using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models.ViewModels
{
    public class CreateWalkFormViewModel
    {
        public Walker Walker { get; set; }
        public List<Dog> Dogs { get; set; }
        public Walk Walk { get; set; }
        public List<int> DogIds { get; set; }
    }
}
