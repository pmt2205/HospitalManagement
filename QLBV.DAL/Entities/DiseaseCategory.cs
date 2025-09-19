using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBV.DAL.Entities
{
    public class DiseaseCategory
    {
        public int DiseaseCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DepartmentId { get; set; }

    }
}
