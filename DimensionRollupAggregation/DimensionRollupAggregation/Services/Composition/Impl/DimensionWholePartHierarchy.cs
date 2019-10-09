using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public class DimensionWholePartHierarchy : IDimensionMember
    {
        public IEnumerable<ExtendedHierarchyMember> Members { get; private set; }

        public DimensionWholePartHierarchy(List<ExtendedHierarchyMember> members)
        {
            this.Members = members;
        }

        public void Accept(IDimensionAggregationVisitor aggregationVisitor)
        {
            aggregationVisitor.Visit(this);
        }
    }
}
