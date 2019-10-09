using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities.GL
{
    [Serializable]
    public class GLRecordAmounts
    {
        private SortedDictionary<int, GLRecordAmount> values;

        public GLRecordAmounts()
        {
            values = new SortedDictionary<int, GLRecordAmount>();
        }

        public virtual void Add(int timeId, GLRecordAmount value)
        {
            values.Add(timeId, value);
        }

        public virtual void Remove(int timeId)
        {
            values.Remove(timeId);
        }

        public virtual GLRecordAmount Get(int timeId)
        {
            GLRecordAmount value;
            if (values.TryGetValue(timeId, out value))
            {
                return value;
            }

            return null;
        }

        public virtual GLRecordAmount AddOrUpdate(int timeId, Func<int, GLRecordAmount> adder, Func<GLRecordAmount, GLRecordAmount> updater)
        {
            if (!values.ContainsKey(timeId))
            {
                values[timeId] = adder(timeId);
            }
            else
            {
                values[timeId] = updater(values[timeId]);
            }

            return values[timeId];
        }


        public virtual IEnumerable<KeyValuePair<int, GLRecordAmount>> Amounts => values;
    }
}
