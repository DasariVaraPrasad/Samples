using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public sealed class ChildrenAggregation<T> : BaseAggregation<T> where T : IMember
    {
        public ChildrenAggregation(List<int> selectedMembers, List<T> members) : base(selectedMembers, members)
        {
        }
        protected override void PopulateDisplayMembers()
        {
            if (typeof(T).Equals(typeof(ExtendedHierarchyMember)))
            {
                List<ExtendedHierarchyMember> members = (Members as List<ExtendedHierarchyMember>).Where(x => SelectedMembers.Distinct().Contains(x.ParentId)).ToList();
                DisplayMembers = members as List<T>;
            }
            else if (typeof(T).Equals(typeof(TimeHierarchy)))
            {
                List<TimeHierarchy> members = (Members as List<TimeHierarchy>).Where(x => SelectedMembers.Distinct().Contains(x.ParentId)).ToList();
                DisplayMembers = members as List<T>;
            }
        }
    }
}
