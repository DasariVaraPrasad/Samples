using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public sealed class AllChildrenAggregation<T> : BaseAggregation<T> where T : IMember
    {
        public AllChildrenAggregation(List<int> selectedMembers, List<T> members) : base(selectedMembers, members)
        {
        }

        protected override void PopulateDisplayMembers()
        {
            List<T> allChildren = new List<T>();
            if (typeof(T).Equals(typeof(ExtendedHierarchyMember)))
            {
                foreach (int selectedMember in SelectedMembers.Distinct())
                {
                    if (Members.FirstOrDefault(x => x.Id == selectedMember) is ExtendedHierarchyMember member)
                    {
                        allChildren.AddRange((Members as List<ExtendedHierarchyMember>).Where(x => x.Left > member.Left && x.Right < member.Right) as List<T>);
                    }
                }
            }
            DisplayMembers = allChildren.Distinct();
        }
    }
}
