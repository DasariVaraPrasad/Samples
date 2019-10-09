using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DimensionRollupAggregation.Entities
{
    [DataContract]
    public enum SegmentType
    {
        [EnumMember]
        Other = 0,
        [EnumMember]
        Account = 1,
        [EnumMember]
        Legal = 2,
        [EnumMember]
        IC = 3,
        [EnumMember]
        System = 4,
        [EnumMember]
        Flow = 5
    }
}
