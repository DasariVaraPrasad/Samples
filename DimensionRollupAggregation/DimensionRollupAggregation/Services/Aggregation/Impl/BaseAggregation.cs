using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public abstract class BaseAggregation<T> : IAggregation<T> where T : IMember
    {
        public bool IsCompositeType { get; protected set; }
        protected List<T> Members { get; private set; }
        protected List<int> SelectedMembers { get; private set; }
        public IEnumerable<T> DisplayMembers { get; protected set; }
        public IEnumerable<int> RelevantLeafIDs { get; protected set; }
        public IEnumerable<int> PostProcessDeleteMemberIDs { get; protected set; }
        public BaseAggregation(List<int> selectedMembers, List<T> members)
        {
            Members = members;
            SelectedMembers = selectedMembers;
            DisplayMembers = new List<T>();
            PostProcessDeleteMemberIDs = new List<int>();
            Setup();
        }
        private void Setup()
        {
            PopulateDisplayMembers();
            PopulateRelevantLeafMembers();
            PopulatePostProcessDeleteMemberIDs();
        }
        protected abstract void PopulateDisplayMembers();
        protected virtual void PopulateRelevantLeafMembers()
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
            RelevantLeafIDs = leaves.Distinct().Select(x => x.Id);
        }
        protected virtual void PopulatePostProcessDeleteMemberIDs()
        {
            List<T> selectedAndAllChildren = new List<T>();
            if (typeof(T).Equals(typeof(ExtendedHierarchyMember)))
            {
                foreach (int selectedMember in SelectedMembers.Distinct())
                {
                    if (Members.FirstOrDefault(x => x.Id == selectedMember) is ExtendedHierarchyMember member)
                    {
                        if (member != null)
                        {
                            List<ExtendedHierarchyMember> members = (Members as List<ExtendedHierarchyMember>).Where(x => x.Left >= member.Left && x.Right <= member.Right).ToList();
                            if (members != null && members.Count > 0)
                            {
                                selectedAndAllChildren.AddRange(members as List<T>);
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
                            List<TimeHierarchy> members = (Members as List<TimeHierarchy>).Where(x => x.Left >= member.Left && x.Right <= member.Right).ToList();
                            if (members != null && members.Count > 0)
                            {
                                selectedAndAllChildren.AddRange(members as List<T>);
                            }
                        }
                    }
                }
            }
            PostProcessDeleteMemberIDs = selectedAndAllChildren.Distinct().Select(x => x.Id).Except(DisplayMembers.Select(z => z.Id));
        }      
    }
}
