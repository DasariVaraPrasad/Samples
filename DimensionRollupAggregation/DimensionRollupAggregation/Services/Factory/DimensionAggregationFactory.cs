using DimensionRollupAggregation.Entities;
using DimensionRollupAggregation.Entities.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services.Factory
{
    public class DimensionAggregationFactory<T1, T2>
        where T1 : ExtendedHierarchyMember where T2 : HierarchyMemberAncestor
    {
        private int SegmentID;
        private List<GLRecord> Data;
        private IAggregation<T1> aggregation;
        private List<T2> MemberAncestors;
        private List<int> selectedMembers;
        public DimensionAggregationFactory(int segmentId, List<GLRecord> data, IAggregation<T1> aggregation, List<T2> memberAncestors)
        {
            this.SegmentID = segmentId;
            this.Data = data;
            this.aggregation = aggregation;
            this.MemberAncestors = memberAncestors;
        }

        public IDimensionAggregationVisitor Create(CompositionType compositionType)
        {
            switch (compositionType)
            {
                case CompositionType.Part:
                    return new PartDimensionAggregationVisitor<T1, T2>(SegmentID, Data, aggregation, MemberAncestors);
                case CompositionType.Composite:
                    return new CompositeDimensionAggregationVisitor<T1, T2>(SegmentID, Data, aggregation, MemberAncestors);
                default: return null;
            }
        }
    }
}
