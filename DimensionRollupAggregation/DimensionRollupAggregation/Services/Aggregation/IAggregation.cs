using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public interface IAggregation <T> where T : IMember
    {
        bool IsCompositeType { get; }
        IEnumerable<T> DisplayMembers { get; }
        IEnumerable<int> RelevantLeafIDs { get; }
        IEnumerable<int> PostProcessDeleteMemberIDs { get; }
    }
}
