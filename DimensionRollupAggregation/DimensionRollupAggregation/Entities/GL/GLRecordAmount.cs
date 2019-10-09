using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Entities.GL
{
    [Serializable]
    public class GLRecordAmount
    {
        public GLRecordAmount(decimal mtd, decimal qtd, decimal ytd)
        {
            Mtd = mtd;
            Qtd = qtd;
            Ytd = ytd;
        }

        public decimal Mtd { get; set; }
        public decimal Qtd { get; set; }
        public decimal Ytd { get; set; }
    }
}
