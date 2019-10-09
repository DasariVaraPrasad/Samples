using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities
{
    public class Selection
    {
        public ExtendedMemberOption Option { get; set; }

        public List<int> SelectedMembers { get; set; }
    }
}
