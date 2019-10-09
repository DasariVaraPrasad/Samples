using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities
{
    public class HierarchyMemberAncestor
    {
        public int Id { get; set; }
        public int AncestorId { get; set; }
        public int Level { get; set; }
        public int Operator { get; set; }
    }
}
