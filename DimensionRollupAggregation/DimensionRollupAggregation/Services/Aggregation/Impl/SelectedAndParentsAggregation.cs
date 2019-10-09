using DimensionRollupAggregation.Entities;
using DimensionRollupAggregation.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public sealed class SelectedAndParentsAggregation<T> : BaseAggregation<T> where T : IMember
    {
        public SelectedAndParentsAggregation(List<int> selectedMembers, List<T> members) : base(selectedMembers, members)
        {
        }
        protected override void PopulateDisplayMembers()
        {
            List<T> parents = new List<T>();
            MetaDataManager metaDataManager = new MetaDataManager();
            if (typeof(T).Equals(typeof(ExtendedHierarchyMember)))
            {
                foreach (int selectedMember in SelectedMembers.Distinct())
                {
                    T member = Members.FirstOrDefault(x => x.Id == selectedMember);
                    if (member != null)
                    {
                        parents.Add(member);
                        List<int> ancestors = metaDataManager.GetAncestors((member as ExtendedHierarchyMember).Lineage);
                        List<ExtendedHierarchyMember> members = (Members as List<ExtendedHierarchyMember>).Where(x => ancestors.Contains(x.Id) && x.MemberType == MemberType.Rollup).ToList();
                        if (members != null && members.Count > 0)
                        {
                            parents.AddRange(members as List<T>);
                        }
                    }
                }
            }
            //else if (typeof(T).Equals(typeof(TimeHierarchy)))
            //{                
            //    foreach (int selectedMember in SelectedMembers.Distinct())
            //    {
            //        T member = Members.FirstOrDefault(x => x.Id == selectedMember);
            //        if (member != null)
            //        {
            //            parents.Add(member);
            //            List<int> ancestors = metaDataManager.GetAncestors((member as TimeHierarchy).Lineage);
            //            List<TimeHierarchy> members = (Members as List<TimeHierarchy>).Where(x => ancestors.Contains(x.Id) && x.Level < 4).ToList();
            //            if (members != null && members.Count > 0)
            //            {
            //                parents.AddRange(members as List<T>);
            //            }
            //        }
            //    }
            //}
            DisplayMembers = parents.Distinct();
        }
    }
}
