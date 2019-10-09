using DimensionRollupAggregation.Entities;
using DimensionRollupAggregation.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services.Factory
{
    public class AggregationFactory<T> where T : IMember
    {
        private List<T> members;
        private List<int> selectedMembers;
        public AggregationFactory(List<int> selectedMembers, List<T> members)
        {
            this.selectedMembers = selectedMembers;
            this.members = members;
        }

        public IAggregation<T> Create(ExtendedMemberOption memberOption)
        {
            switch (memberOption)
            {
                case ExtendedMemberOption.Selected:
                    return new SelectedAggregation<T>(selectedMembers, members);
                case ExtendedMemberOption.Children:
                    return new ChildrenAggregation<T>(selectedMembers, members);
                case ExtendedMemberOption.AllChildren:
                    return new AllChildrenAggregation<T>(selectedMembers, members);
                case ExtendedMemberOption.SelectedAndChildren:
                    return new SelectedAndChildrenAggregation<T>(selectedMembers, members);
                case ExtendedMemberOption.SelectedAndAllChildern:
                    return new SelectedAndAllChildernAggregation<T>(selectedMembers, members);
                case ExtendedMemberOption.Leaves:
                    return new LeavesAggregation<T>(selectedMembers, members);
                case ExtendedMemberOption.SelectedAndLeaves:
                    return new SelectedAndLeavesAggregation<T>(selectedMembers, members);
                case ExtendedMemberOption.SelectedAndParents:
                    return new SelectedAndParentsAggregation<T>(selectedMembers, members);
                default: return null;
            }
        }
    }
}
