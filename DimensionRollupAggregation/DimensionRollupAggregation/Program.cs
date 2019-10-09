using DimensionRollupAggregation.Entities;
using DimensionRollupAggregation.Entities.GL;
using DimensionRollupAggregation.Managers;
using DimensionRollupAggregation.Services;
using DimensionRollupAggregation.Services.Factory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation
{
    class Program
    {
        static void Main(string[] args)
        {
            // Preparation of Meta Data
            MetaDataManager metaDataManager = new MetaDataManager();
            List<Segment> segments = metaDataManager.GetSegments();

            Dictionary<Segment, HierarchyMetaData> metaData = new Dictionary<Segment, HierarchyMetaData>();
            foreach (Segment segment in segments)
            {
                metaData.Add(segment, new HierarchyMetaData(metaDataManager.GetHierarchyMembers(segment.Id)));
            }

            //List<TimeHierarchy> timeHierarchies = metaDataManager.GetTimeHierarchies();
            //Utility.PopulateLRValues(timeHierarchies);

            List<DimensionSelection> selections = new List<DimensionSelection>()
            {
                new DimensionSelection(){ SegmentID = 1, SelectedMembers = new List<int>(){ 1 }, Option = ExtendedMemberOption.SelectedAndAllChildern },
                new DimensionSelection(){ SegmentID = 2, SelectedMembers = new List<int>(){ 1 }, Option = ExtendedMemberOption.SelectedAndAllChildern }
                //,new DimensionSelection(){ SegmentID = 3, SelectedMembers = new List<int>(){ 1 }, Option = ExtendedMemberOption.SelectedAndAllChildern }
                //,new DimensionSelection(){ SegmentID = 4, SelectedMembers = new List<int>(){ 1 }, Option = ExtendedMemberOption.SelectedAndAllChildern }
            };

            selections = metaDataManager.ApplyDefaultSegmentMemberIDs(segments, selections);

            List<GLRecord> lstGLRecord = metaDataManager.GetSourceData();
            Dictionary<DimensionSelection, IAggregation<ExtendedHierarchyMember>> selectionAggregations = new Dictionary<DimensionSelection, IAggregation<ExtendedHierarchyMember>>();
            foreach (DimensionSelection selection in selections)
            {
                AggregationFactory<ExtendedHierarchyMember> aggregationFactory = new AggregationFactory<ExtendedHierarchyMember>(selection.SelectedMembers, metaData[segments.FirstOrDefault(x => x.Id == selection.SegmentID)].Members.ToList());
                IAggregation<ExtendedHierarchyMember> aggregation = aggregationFactory.Create(selection.Option);
                lstGLRecord.RemoveAll(x => !aggregation.RelevantLeafIDs.Contains(x.GLMembers.SegmentValues.FirstOrDefault(z => z.Key == selection.SegmentID).Value));
                selectionAggregations.Add(selection, aggregation);
            }

            foreach (KeyValuePair<DimensionSelection, IAggregation<ExtendedHierarchyMember>> selectionAggregation in selectionAggregations)
            {

                DimensionAggregationFactory<ExtendedHierarchyMember, HierarchyMemberAncestor> dimensionAggregationFactory =
                    new DimensionAggregationFactory<ExtendedHierarchyMember, HierarchyMemberAncestor>(selectionAggregation.Key.SegmentID, lstGLRecord, selectionAggregation.Value, metaData[segments.FirstOrDefault(x => x.Id == selectionAggregation.Key.SegmentID)].MemberAncestors.ToList());

                switch (selectionAggregation.Key.Option)
                {
                    case ExtendedMemberOption.Leaves:
                    default:
                        break;
                    case ExtendedMemberOption.Selected:
                        if (lstGLRecord.Select(x => x.GLMembers.SegmentValues.FirstOrDefault(z => z.Key == selectionAggregation.Key.SegmentID).Value).Except(selectionAggregation.Value.DisplayMembers.Select(x => x.Id)).Count() > 0)
                        {
                            foreach (ExtendedHierarchyMember extendedHierarchyMember in selectionAggregation.Value.DisplayMembers)
                            {
                                DimensionMember dimensionMember = new DimensionMember(extendedHierarchyMember);
                                dimensionMember.Accept(dimensionAggregationFactory.Create(CompositionType.Part));
                            }
                        }
                        break;
                    case ExtendedMemberOption.Children:
                    case ExtendedMemberOption.AllChildren:
                    case ExtendedMemberOption.SelectedAndChildren:
                    case ExtendedMemberOption.SelectedAndAllChildern:
                    case ExtendedMemberOption.SelectedAndLeaves:
                    case ExtendedMemberOption.SelectedAndParents:
                        int maxDepth = selectionAggregation.Value.DisplayMembers.Where(x => x.MemberType == MemberType.Rollup).Max(z => z.Level);
                        int minDepth = selectionAggregation.Value.DisplayMembers.Where(x => x.MemberType == MemberType.Rollup).Min(z => z.Level);
                        for (int level = maxDepth; level >= minDepth; level--)
                        {
                            foreach (ExtendedHierarchyMember extendedHierarchyMember in selectionAggregation.Value.DisplayMembers.Where(x => x.MemberType == MemberType.Rollup && x.Level == level))
                            {
                                if (!selectionAggregation.Value.DisplayMembers.Any(x => x.MemberType == MemberType.Rollup && ( x.ParentId == extendedHierarchyMember.Id)))
                                {
                                    DimensionMember dimensionMember = new DimensionMember(extendedHierarchyMember);
                                    dimensionMember.Accept(dimensionAggregationFactory.Create(CompositionType.Part));
                                }
                                else
                                {
                                    DimensionWholePartHierarchy dimensionWholePartHierarchy = new DimensionWholePartHierarchy(selectionAggregation.Value.DisplayMembers.Where(x => x.ParentId == extendedHierarchyMember.Id || x.Id == extendedHierarchyMember.Id).ToList());
                                    dimensionWholePartHierarchy.Accept(dimensionAggregationFactory.Create(CompositionType.Composite));
                                }
                            }
                        }
                        break;
                }
                lstGLRecord.RemoveAll(x => selectionAggregation.Value.PostProcessDeleteMemberIDs.Contains(x.GLMembers.SegmentValues.FirstOrDefault(z => z.Key == selectionAggregation.Key.SegmentID).Value));
            }

            Utility.Export(lstGLRecord, segments, metaData);
        }
    }
}
