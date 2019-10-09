using DimensionRollupAggregation.Entities;
using DimensionRollupAggregation.Entities.GL;
using DimensionRollupAggregation.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Services
{
    public abstract class BaseDimensionAggregationVisitor<T1, T2> : MustInitialize<IAggregation<T1>>, IDimensionAggregationVisitor 
        where T1 : ExtendedHierarchyMember where T2 : HierarchyMemberAncestor
    {
        protected IAggregation<T1> aggregation { get; private set; }
        protected IEnumerable<T2> memberAncestors { get; private set; }
        protected List<GLRecord> data { get; private set; }
        protected int SegmentID { get; private set; }
        public BaseDimensionAggregationVisitor(int segmentId, List<GLRecord> data, IAggregation<T1> aggregation, IEnumerable<T2> memberAncestors) : base(aggregation)
        {
            this.SegmentID = segmentId;
            this.data = data;
            this.aggregation = aggregation;
            this.memberAncestors = memberAncestors;
        }

        public virtual void Visit(DimensionWholePartHierarchy dimensionWholePartHierarchy)
        {
            int depth = dimensionWholePartHierarchy.Members.Min(z => z.Level);
            ExtendedHierarchyMember rootExtendedHierarchyMember = dimensionWholePartHierarchy.Members.FirstOrDefault(x => x.Level == depth);
            List<ExtendedHierarchyMember> childExtendedHierarchyMembers = dimensionWholePartHierarchy.Members.Where(x => x.Level == depth + 1).ToList();

            List<GLRecord> gLRecords = data.Where(x => childExtendedHierarchyMembers.Select(z => z.Id).Contains(x.GLMembers.Get(SegmentID))).ToList();

            Dictionary<int, int> childrenOperators = new Dictionary<int, int>();
            childExtendedHierarchyMembers.ForEach(x => { childrenOperators.Add(x.Id, Utility.GetMemberOperator(x.Operator)); });

            if (gLRecords != null && gLRecords.Count > 0)
            {
                foreach (GLRecord gLRecord in gLRecords)
                {
                    string newUniqueKey = Utility.GetGLUniqueKey(gLRecord, VisitorType.Segment, rootExtendedHierarchyMember.Id, SegmentID);
                    GLRecord objGLRecord = data.FirstOrDefault(x => x.UniqueKey == newUniqueKey);
                    if (objGLRecord == null)
                    {
                        objGLRecord = gLRecord.CloneObjectSerializable<GLRecord>();
                        foreach (KeyValuePair<int, GLRecordAmount> gLRecordAmount in objGLRecord.Values.Amounts)
                        {
                            gLRecordAmount.Value.Mtd = 0;
                            gLRecordAmount.Value.Qtd = 0;
                            gLRecordAmount.Value.Ytd = 0;
                        }
                        objGLRecord.GLMembers.AddOrUpdate(SegmentID, rootExtendedHierarchyMember.Id);
                        data.Add(objGLRecord);
                    }
                    int Operator = childrenOperators[gLRecord.GLMembers.Get(SegmentID)];
                    foreach (KeyValuePair<int, GLRecordAmount> cell in gLRecord.Values.Amounts)
                    {
                        GLRecordAmount existingRecordAmount = objGLRecord.Values.Amounts.FirstOrDefault(x => x.Key == cell.Key).Value;
                        GLRecordAmount gLRecordAmount = new GLRecordAmount(existingRecordAmount.Mtd + (cell.Value.Mtd * Operator),
                                                                           existingRecordAmount.Qtd + (cell.Value.Qtd * Operator),
                                                                           existingRecordAmount.Ytd + (cell.Value.Ytd * Operator));
                        objGLRecord.Values.AddOrUpdate(cell.Key, null, (updated) => { return gLRecordAmount; });
                    }
                }
            }
        }

        public virtual void Visit(DimensionMember dimensionMember)
        {
            List<T2> ancestors = memberAncestors.Where(x => x.AncestorId == dimensionMember.Member.Id).ToList();
            List<GLRecord> gLRecords = data.Where(x => ancestors.Select(z => z.Id).Contains(x.GLMembers.Get(SegmentID))).ToList();
            if (gLRecords != null && gLRecords.Count > 0)
            {       
                foreach (GLRecord gLRecord in gLRecords)
                {
                    string newUniqueKey = Utility.GetGLUniqueKey(gLRecord, VisitorType.Segment, dimensionMember.Member.Id, SegmentID);
                    GLRecord objGLRecord = data.FirstOrDefault(x => x.UniqueKey == newUniqueKey);
                    if(objGLRecord == null)
                    {
                        objGLRecord = gLRecord.CloneObjectSerializable<GLRecord>();
                        foreach(KeyValuePair<int, GLRecordAmount> gLRecordAmount in objGLRecord.Values.Amounts)
                        {
                            gLRecordAmount.Value.Mtd = 0;
                            gLRecordAmount.Value.Qtd = 0;
                            gLRecordAmount.Value.Ytd = 0;
                        }
                        objGLRecord.GLMembers.AddOrUpdate(SegmentID, dimensionMember.Member.Id);
                        data.Add(objGLRecord);
                    }
                    T2 ancestor = ancestors.FirstOrDefault(x => x.Id == gLRecord.GLMembers.Get(SegmentID));
                    foreach (KeyValuePair<int, GLRecordAmount> cell in gLRecord.Values.Amounts)
                    {
                        GLRecordAmount existingRecordAmount = objGLRecord.Values.Amounts.FirstOrDefault(x => x.Key == cell.Key).Value;
                        GLRecordAmount gLRecordAmount = new GLRecordAmount(existingRecordAmount.Mtd + (cell.Value.Mtd * ancestor.Operator), 
                                                                           existingRecordAmount.Qtd + (cell.Value.Qtd * ancestor.Operator),
                                                                           existingRecordAmount.Ytd + (cell.Value.Ytd * ancestor.Operator));
                        objGLRecord.Values.AddOrUpdate(cell.Key, null, (updated) => { return gLRecordAmount; });
                    }
                }
            }
        }
    }
}
