using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities
{
    public enum MemberOption
    {
        Selected,
        Children,
        AllChildren,
        SelectedAndChildren,
        SelectedAndAllChildern,
        Leaves,
        SelectedAndLeaves,
        SelectedAndParents
    }
}
