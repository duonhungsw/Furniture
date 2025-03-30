using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furniture.Core.Dtos.Order
{
    public class MonthlyRevenueViewModel
    {
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; }
        public List<string> Labels { get; set; }
        public List<double> Revenues { get; set; } 
        public int SelectedYear { get; set; }
    }

}
