using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public interface IDimensionMember
    {
        void Accept(IDimensionAggregationVisitor aggregationVisitor);
    }
}
