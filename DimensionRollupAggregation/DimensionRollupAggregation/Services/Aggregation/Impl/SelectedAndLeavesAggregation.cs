using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public sealed class SelectedAndLeavesAggregation<T> : BaseAggregation<T> where T : IMember
    {
        public SelectedAndLeavesAggregation(List<int> selectedMembers, List<T> members) : base(selectedMembers, members)
        {
        }

        protected override void PopulateDisplayMembers()
        {
            List<T> leaves = new List<T>();
            if (typeof(T).Equals(typeof(ExtendedHierarchyMember)))
            {
                foreach (int selectedMember in SelectedMembers.Distinct())
                {
                    T member = Members.FirstOrDefault(x => x.Id == selectedMember);
                    if (member != null)
                    {
                        leaves.Add(member);
                        List<ExtendedHierarchyMember> members = (Members as List<ExtendedHierarchyMember>).Where(x => x.Left >= (member as ExtendedHierarchyMember).Left && x.Right <= (member as ExtendedHierarchyMember).Right && x.MemberType == MemberType.Member).ToList();
                        if (members != null && members.Count > 0)
                        {
                            leaves.AddRange(members as List<T>);
                        }
                    }
                }
            }
            else if (typeof(T).Equals(typeof(TimeHierarchy)))
            {
                foreach (int selectedMember in SelectedMembers.Distinct())
                {
                    T member = Members.FirstOrDefault(x => x.Id == selectedMember);
                    if (member != null)
                    {
                        leaves.Add(member);
                        List<TimeHierarchy> members = (Members as List<TimeHierarchy>).Where(x => x.Left >= (member as TimeHierarchy).Left && x.Right <= (member as TimeHierarchy).Right && x.Level == 4).ToList();
                        if (members != null && members.Count > 0)
                        {
                            leaves.AddRange(members as List<T>);
                        }
                    }
                }
            }
            DisplayMembers = leaves.Distinct();
        }
    }
}
