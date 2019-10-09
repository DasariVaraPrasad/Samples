using DimensionRollupAggregation.Entities;
using DimensionRollupAggregation.Entities.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public sealed class CompositeDimensionAggregationVisitor<T1, T2> : BaseDimensionAggregationVisitor<T1, T2>
        where T1 : ExtendedHierarchyMember where T2 : HierarchyMemberAncestor
    {
        public CompositeDimensionAggregationVisitor(int segmentId, List<GLRecord> data, IAggregation<T1> aggregation, IEnumerable<T2> memberAncestors) : base(segmentId, data, aggregation, memberAncestors)
        {

        }

        public override void Visit(DimensionMember dimensionMember)
        {

        }
    }
}

