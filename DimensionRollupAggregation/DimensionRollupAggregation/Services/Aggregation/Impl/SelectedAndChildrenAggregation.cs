using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public sealed class SelectedAndChildrenAggregation<T> : BaseAggregation<T> where T : IMember
    {
        public SelectedAndChildrenAggregation(List<int> selectedMembers, List<T> members) : base(selectedMembers, members)
        {
        }

        protected override void PopulateDisplayMembers()
        {
            List<T> selectedAndChildren = new List<T>();
            if (typeof(T).Equals(typeof(ExtendedHierarchyMember)))
            {
                foreach (int selectedMember in SelectedMembers.Distinct())
                {
                    if (Members.FirstOrDefault(x => x.Id == selectedMember) is ExtendedHierarchyMember member)
                    {
                        if (member != null)
                        {
                            List<ExtendedHierarchyMember> members = (Members as List<ExtendedHierarchyMember>).Where(x => x.Id == member.Id || x.ParentId == member.Id).ToList();
                            if (members != null && members.Count > 0)
                            {
                                selectedAndChildren.AddRange(members as List<T>);
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
                            List<TimeHierarchy> members = (Members as List<TimeHierarchy>).Where(x => x.Id == member.Id || x.ParentId == member.Id).ToList();
                            if (members != null && members.Count > 0)
                            {
                                selectedAndChildren.AddRange(members as List<T>);
                            }
                        }
                    }
                }
            }
            DisplayMembers = selectedAndChildren.Distinct();
        }
    }
}
