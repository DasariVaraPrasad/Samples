using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities
{
    public class ExtendedHierarchyMember : HierarchyMember
    {
        public int Level { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public string Operator { get; set; }
        public string Lineage { get; set; }
    }
}
