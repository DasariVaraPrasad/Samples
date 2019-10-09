using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DimensionRollupAggregation.Entities
{
    [DataContract]
    public class BaseMember : IMember
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }
        [DataMember(Order = 2)]
        public string Code { get; set; }
        [DataMember(Order = 3)]
        public string Name { get; set; }
        [DataMember(Order = 4)]
        public string Label
        {
            get
            {
                var name = string.IsNullOrEmpty(this.Name) ? "" : " - " + this.Name;
                return string.IsNullOrEmpty(this.Code) ? this.Name : string.Format("{0}{1}", this.Code, name);
            }
            set { }
        }
    }
}
