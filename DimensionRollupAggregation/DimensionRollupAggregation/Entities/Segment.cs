using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DimensionRollupAggregation.Entities
{
    [KnownType(typeof(FinanceSegment))]
    [DataContract]
    public class Segment
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }
        [DataMember(Order = 1)]
        public string Name { get; set; }
        [DataMember(Order = 1)]
        public int DimensionId { get; set; }
    }
    
}
