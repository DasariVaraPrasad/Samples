using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities
{
    public class TimeHierarchy : IMember
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public TimeFormat TimeType { get; set; }
        public int DataLockFlag { get; set; }
        public int Level { get { switch (TimeType) { case TimeFormat.Month: return 4; case TimeFormat.Quarter: return 3; case TimeFormat.Year: return 2; default: return 1; } } }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Operator { get { return 1; } }
    }
}
