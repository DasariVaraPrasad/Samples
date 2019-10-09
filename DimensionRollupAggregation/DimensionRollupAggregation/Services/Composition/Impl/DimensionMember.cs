using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public class DimensionMember : IDimensionMember
    {
        public ExtendedHierarchyMember Member { get; private set; }

        public DimensionMember(ExtendedHierarchyMember member)
        {
            this.Member = member;
        }

        public void Accept(IDimensionAggregationVisitor aggregationVisitor)
        {
            aggregationVisitor.Visit(this);
        }
    }
}
