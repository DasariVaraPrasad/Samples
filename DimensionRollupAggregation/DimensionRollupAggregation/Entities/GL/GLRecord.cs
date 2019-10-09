using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities.GL
{
    [Serializable]
    public class GLRecord
    {
        public int ScenarioId { get; }
        public int ReportingId { get; }
        public GLDataMembers GLMembers { get; }
        public GLRecordAmounts Values { get; }
        public GLRecord (int scenarioId, int reportingId, GLDataMembers glMembers, GLRecordAmounts gLRecordAmounts )
        {
            ScenarioId = scenarioId;
            ReportingId = reportingId;
            GLMembers = glMembers;
            Values = gLRecordAmounts;
        }
        public string UniqueKey
        {
            get
            {
                if (GLMembers != null)
                {
                    return string.Format("{0}{1}{2}", this.ScenarioId, this.ReportingId, string.Concat(GLMembers.SegmentValues.OrderBy(o => o.Key).Select(s => s.Value.ToString())));
                }
                return string.Empty;
            }
        }
    }
}
