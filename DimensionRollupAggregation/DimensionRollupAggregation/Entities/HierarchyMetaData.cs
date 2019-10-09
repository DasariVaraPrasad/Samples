using DimensionRollupAggregation.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities
{
    public class HierarchyMetaData
    {
        private MetaDataManager metaDataManager;
        private List<ExtendedHierarchyMember> members;
        private List<HierarchyMemberAncestor> memberAncestors;
        private Dictionary<int, int> memberOperators;
        public IEnumerable<ExtendedHierarchyMember> Members { get { return members; } }        
        public IEnumerable<HierarchyMemberAncestor> MemberAncestors { get { return memberAncestors; } }
        public IReadOnlyDictionary<int, int> MemberOperators { get { return memberOperators; } }

        public HierarchyMetaData(List<ExtendedHierarchyMember> members)
        {
            this.metaDataManager = new MetaDataManager();
            this.members = members;
            this.memberAncestors = new List<HierarchyMemberAncestor>();
            this.memberOperators = new Dictionary<int, int>();
            members.ForEach(x => memberOperators.Add(x.Id, Utility.GetMemberOperator(x.Operator)));
            PopulateAncestors(members);
        }        
        private void PopulateAncestors(List<ExtendedHierarchyMember> members)
        {
            foreach(ExtendedHierarchyMember member in members.Where(x => x.MemberType == MemberType.Member))
            {
                int cumulativeOperator = 1;
                HierarchyMemberAncestor hierarchyMemberAncestor = new HierarchyMemberAncestor() { Id = member.Id, AncestorId = member.Id, Level = member.Level, Operator = 1 };
                memberAncestors.Add(hierarchyMemberAncestor);                
                List<int> ancestors = metaDataManager.GetAncestors(member.Lineage);
                int levelCounter = hierarchyMemberAncestor.Level;
                int previousMemberID = member.Id;
                foreach (int ancestor in ancestors)
                {
                    levelCounter -= 1;
                    cumulativeOperator *= memberOperators[previousMemberID];
                    memberAncestors.Add(new HierarchyMemberAncestor() { Id = member.Id, AncestorId = ancestor, Level = levelCounter, Operator = cumulativeOperator });                    
                    previousMemberID = ancestor;
                }
            }
        }
    }
}
