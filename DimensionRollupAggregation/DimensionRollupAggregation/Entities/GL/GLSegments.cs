using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities.GL
{
    [Serializable]
    public class GLDataMembers
    {
        private Dictionary<int, int> segmentValues;

        public GLDataMembers()
        {
            segmentValues = new Dictionary<int, int>();
        }

        public void AddOrUpdate(int index, int id)
        {
            if (!segmentValues.ContainsKey(index))
            {
                segmentValues.Add(index, id);
            }
            else
            {
                segmentValues[index] = id;
            }
        }

        public void Remove(int index)
        {
            if (segmentValues.ContainsKey(index))
                segmentValues.Remove(index);
        }

        public int Get(int index)
        {
            if (segmentValues.ContainsKey(index))
                return segmentValues[index];
            else
                return 0;
        }

        public virtual IEnumerable<KeyValuePair<int, int>> SegmentValues => segmentValues;
    }
}
