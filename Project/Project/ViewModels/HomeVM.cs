using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.ViewModels
{
    public class HomeVM
    {
        public List<Person> Persons { get; set; }
        public List<Competition> Competitions { get; set; }
    }
}
