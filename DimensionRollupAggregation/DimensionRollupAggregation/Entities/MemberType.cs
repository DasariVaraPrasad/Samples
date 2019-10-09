using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DimensionRollupAggregation.Entities
{
    [DataContract]
    public enum MemberType
    {
        //Root = 1,
        [EnumMember]
        Formula = 2,
        [EnumMember]
        Rollup = 3,
        [EnumMember]
        Member = 4
    }
}
