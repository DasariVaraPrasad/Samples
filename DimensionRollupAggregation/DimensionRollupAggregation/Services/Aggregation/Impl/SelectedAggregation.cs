using DimensionRollupAggregation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public sealed class SelectedAggregation<T> : BaseAggregation<T> where T : IMember
    {
        public SelectedAggregation(List<int> selectedMembers, List<T> members) : base(selectedMembers,members)
        {
            IsCompositeType = true;
        }

        protected override void PopulateDisplayMembers()
        {
            DisplayMembers = Members.Where(x => SelectedMembers.Distinct().Contains(x.Id)).ToList();
        }
    }
}
