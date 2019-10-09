using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DimensionRollupAggregation.Entities
{
    [KnownType(typeof(Segment))]
    [DataContract]
    public class FinanceSegment : Segment
    {
        [DataMember(Order = 2)]
        public SegmentType Type { get; set; }
    }
}
