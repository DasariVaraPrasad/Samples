using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public interface IDimensionAggregationVisitor
    {
        void Visit(DimensionWholePartHierarchy dimensionWholePartHierarchy);

        void Visit(DimensionMember dimensionMember);
    }

    
}
