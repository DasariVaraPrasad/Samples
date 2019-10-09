using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public sealed class LeavesAggregation<T> : BaseAggregation<T> where T : IMember
    {
        public LeavesAggregation(List<int> selectedMembers, List<T> members) : base(selectedMembers, members)
        {

        }

        protected override void PopulateDisplayMembers()
        {
            List<T> leaves = new List<T>();
            if (typeof(T).Equals(typeof(ExtendedHierarchyMember)))
            {
                foreach (int selectedMember in SelectedMembers.Distinct())
                {
                    if (Members.FirstOrDefault(x => x.Id == selectedMember) is ExtendedHierarchyMember member)
                    {
                        if (member != null)
                        {
                            List<ExtendedHierarchyMember> members = (Members as List<ExtendedHierarchyMember>).Where(x => x.Left >= member.Left && x.Right <= member.Right && x.MemberType == MemberType.Member).ToList();
                            if (members != null && members.Count > 0)
                            {
                                leaves.AddRange(members as List<T>);
                            }
                        }
                    }
                }
            }
            else if (typeof(T).Equals(typeof(TimeHierarchy)))
            {
                foreach (int selectedMember in SelectedMembers.Distinct())
                {
                    if (Members.FirstOrDefault(x => x.Id == selectedMember) is TimeHierarchy member)
                    {
                        if (member != null)
                        {
                            List<TimeHierarchy> members = (Members as List<TimeHierarchy>).Where(x => x.Left >= member.Left && x.Right <= member.Right && x.Level == 4).ToList();
                            if (members != null && members.Count > 0)
                            {
                                leaves.AddRange(members as List<T>);
                            }
                        }
                    }
                }
            }
            DisplayMembers = leaves.Distinct();
        }
    }
}
