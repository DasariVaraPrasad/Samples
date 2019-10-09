using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities
{
    public enum ExtendedMemberOption 
    {
        Selected = MemberOption.Selected,
        Children = MemberOption.Children,
        AllChildren = MemberOption.AllChildren,
        SelectedAndChildren = MemberOption.SelectedAndChildren,
        SelectedAndAllChildern = MemberOption.SelectedAndAllChildern,
        Leaves = MemberOption.Leaves,
        SelectedAndLeaves = MemberOption.SelectedAndLeaves,
        SelectedAndParents = MemberOption.SelectedAndParents,
        Month,
        Quarter,
        Year
    }
}
