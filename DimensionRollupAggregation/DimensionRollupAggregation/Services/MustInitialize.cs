using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public abstract class MustInitialize<T> where T : class
    {
        public MustInitialize(T parameters)
        {

        }
    }
}
