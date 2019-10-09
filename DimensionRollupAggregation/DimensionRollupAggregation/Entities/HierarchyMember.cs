using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DimensionRollupAggregation.Entities
{
    [KnownType(typeof(BaseMember))]
    [DataContract]
    public class HierarchyMember : BaseMember
    {
        [DataMember(Order = 5)]
        public int ParentId { get; set; }
        /// <summary>
        /// Parent represents Label of the member parent
        /// </summary>
        [DataMember(Order = 6)]
        public string Parent { get; set; }
        [DataMember(Order = 7)]
        public MemberType MemberType { get; set; }
        [DataMember(Order = 8)]
        public string RollupOperator { get; set; }
        [DataMember(Order = 9)]
        public string Formula { get; set; }
        [DataMember(Order = 10)]
        public string FormulaText { get; set; }
    }
}
