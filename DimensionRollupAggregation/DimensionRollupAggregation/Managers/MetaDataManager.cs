using DimensionRollupAggregation.Entities;
using DimensionRollupAggregation.Entities.GL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DimensionRollupAggregation.Managers
{
    public class MetaDataManager
    {
        public List<ExtendedHierarchyMember> GetHierarchyMembers(int SegmentId)
        {
            List<ExtendedHierarchyMember> hierarchyMembers = new List<ExtendedHierarchyMember>();
            DataTable dataTable = GetHierarchyData(SegmentId);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                hierarchyMembers.Add(new ExtendedHierarchyMember()
                {
                    Id = Convert.ToInt32(dataRow["IDX"]),
                    Code = Convert.ToString(dataRow["CODE"]),
                    Name = Convert.ToString(dataRow["NAME"]),
                    ParentId = Convert.ToInt32(dataRow["PARENT_IDX"]),
                    MemberType = (MemberType)Convert.ToInt32(dataRow["TYPE"]),
                    Level = Convert.ToInt32(dataRow["LEVEL_VAL"]),
                    Left = Convert.ToInt32(dataRow["LEFT_VAL"]),
                    Right = Convert.ToInt32(dataRow["RIGHT_VAL"]),
                    Operator = Convert.ToString(dataRow["ROLLUP_OPERATOR"]),
                    Lineage = Convert.ToString(dataRow["LINEAGE"])
                });
            }
            return hierarchyMembers;
        }
        private DataTable GetHierarchyData(int SegmentId)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("IDX");
            dataTable.Columns.Add("CODE");
            dataTable.Columns.Add("NAME");
            dataTable.Columns.Add("PARENT_IDX");
            dataTable.Columns.Add("TYPE");
            dataTable.Columns.Add("LEVEL_VAL");
            dataTable.Columns.Add("LEFT_VAL");
            dataTable.Columns.Add("RIGHT_VAL");
            dataTable.Columns.Add("ROLLUP_OPERATOR");
            dataTable.Columns.Add("LINEAGE");

            switch (SegmentId)
            {
                case 1:
                    dataTable.Rows.Add(1, string.Empty, "Account Main", 0, 3, 1, 1, 18, "+", "/");
                    dataTable.Rows.Add(101, "Consolidation Data", string.Empty, 1, 3, 2, 2, 17, "+", "/1/");
                    dataTable.Rows.Add(102, "NetIncome", string.Empty, 101, 3, 3, 3, 16, "+", "/1/101/");
                    dataTable.Rows.Add(103, "Revenue", string.Empty, 102, 3, 4, 4, 9, "+", "/1/101/102/");
                    dataTable.Rows.Add(104, "Expense", string.Empty, 102, 3, 4, 10, 15, "-", "/1/101/102/");
                    dataTable.Rows.Add(105, "28Revenue", string.Empty, 103, 4, 5, 5, 6, "+", "/1/101/102/103/");
                    dataTable.Rows.Add(106, "15Revenue", string.Empty, 103, 4, 5, 7, 8, "+", "/1/101/102/103/");
                    dataTable.Rows.Add(107, "28Expense", string.Empty, 104, 4, 5, 11, 12, "+", "/1/101/102/104/");
                    dataTable.Rows.Add(108, "15Expense", string.Empty, 104, 4, 4, 13, 14, "+", "/1/101/102/104/");
                    break;
                case 2:
                    dataTable.Rows.Add(1, string.Empty, "Company Main", 0, 3, 1, 1, 20, "+", "/");
                    dataTable.Rows.Add(100, "Host United States", string.Empty, 1, 3, 2, 2, 5, "+", "/1/");
                    dataTable.Rows.Add(101, "Host India", string.Empty, 1, 3, 2, 6, 19, "+", "/1/");
                    dataTable.Rows.Add(102, "Host Redwood City", string.Empty, 100, 4, 3, 3, 4, "+", "/1/100/");
                    dataTable.Rows.Add(103, "Host South India", string.Empty, 101, 3, 3, 7, 12, "+", "/1/101/");
                    dataTable.Rows.Add(104, "Host North India", string.Empty, 101, 3, 3, 13, 18, "+", "/1/101/");
                    dataTable.Rows.Add(105, "Host Hyderabad", string.Empty, 103, 4, 4, 8, 9, "+", "/1/101/103/");
                    dataTable.Rows.Add(106, "Host Chennai", string.Empty, 103, 4, 4, 10, 11, "+", "/1/101/103/");
                    dataTable.Rows.Add(107, "Host Delhi", string.Empty, 104, 4, 4, 14, 15, "+", "/1/101/104/");
                    dataTable.Rows.Add(108, "Host Mumbai", string.Empty, 104, 4, 4, 16, 17, "+", "/1/101/104/");
                    break;
                case 3:
                    dataTable.Rows.Add(1, string.Empty, "IC Segment Main", 0, 3, 1, 1, 10000, "+", "/");
                    dataTable.Rows.Add(10, "IC Default", string.Empty, 1, 4, 2, 1, 1, "+", "/1/");
                    dataTable.Rows.Add(90, string.Empty, "IC SubAccounts", 1, 3, 2, 1, 1, "+", "/1/");
                    dataTable.Rows.Add(102, "Host Redwood City", string.Empty, 90, 4, 3, 3, 4, "+", "/1/90/");
                    dataTable.Rows.Add(105, "Host Hyderabad", string.Empty, 90, 4, 3, 8, 9, "+", "/1/90/");
                    dataTable.Rows.Add(106, "Host Chennai", string.Empty, 90, 4, 3, 10, 11, "+", "/1/90/");
                    dataTable.Rows.Add(107, "Host Delhi", string.Empty, 90, 4, 3, 14, 15, "+", "/1/90/");
                    dataTable.Rows.Add(108, "Host Mumbai", string.Empty, 90, 4, 3, 16, 17, "+", "/1/90/");
                    break;
                case 4:
                    dataTable.Rows.Add(1, string.Empty, "Department Main", 0, 3, 1, 1, 16, "+", "/");
                    dataTable.Rows.Add(101, "Administration", string.Empty, 1, 3, 2, 2, 5, "+", "/1/");
                    dataTable.Rows.Add(102, "Product Development", string.Empty, 1, 3, 2, 6, 15, "+", "/1/");
                    dataTable.Rows.Add(103, "G & A", string.Empty, 101, 4, 3, 3, 4, "+", "/1/101/");
                    dataTable.Rows.Add(104, "Development", string.Empty, 102, 4, 3, 7, 8, "+", "/1/102/");
                    dataTable.Rows.Add(105, "Quality Assurance", string.Empty, 102, 4, 3, 9, 10, "+", "/1/102/");
                    dataTable.Rows.Add(106, "Operations", string.Empty, 102, 4, 3, 11, 12, "+", "/1/102/");
                    dataTable.Rows.Add(107, "Customer Focus", string.Empty, 102, 4, 3, 13, 14, "+", "/1/102/");
                    break;
                default: break;
            }            
            return dataTable;
        }
        public List<Segment> GetSegments()
        {
            int[] ids = { 1, 2, 3, 4 };
            int[] dimIds = { 100, 102, 104, 103 };
            string[] names = { "Account", "Company", "ICSegment", "Department" };
            SegmentType[] types = { SegmentType.Account, SegmentType.Legal, SegmentType.IC, SegmentType.Other};
            List<Segment> segments = new List<Segment>();
            for (int i = 0; i < 4; i++)
            {
                FinanceSegment segment = new FinanceSegment();
                segment.DimensionId = dimIds[i];
                segment.Id = ids[i];
                segment.Name = names[i];
                segment.Type = types[i];
                segments.Add(segment);
            }
            return segments;
        }
        public List<int> GetAncestors(string lineage)
        {
            return lineage.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Reverse().Select(int.Parse).ToList();
        }               
        public List<GLRecord> GetSourceData()
        {
            List<GLRecord> sourceData = new List<GLRecord>();

            GLDataMembers objGLDataMembers = new GLDataMembers();
            objGLDataMembers.AddOrUpdate(1, 105);
            objGLDataMembers.AddOrUpdate(2, 105);
            objGLDataMembers.AddOrUpdate(3, 10);
            objGLDataMembers.AddOrUpdate(4, 103);

            GLRecordAmounts gLRecordAmounts = new GLRecordAmounts();
            gLRecordAmounts.Add(385, new GLRecordAmount(2000, 2000, 2000));
            gLRecordAmounts.Add(386, new GLRecordAmount(2500, 4500, 4500));
            gLRecordAmounts.Add(387, new GLRecordAmount(3000, 7500, 7500));
            gLRecordAmounts.Add(388, new GLRecordAmount(3500, 3500, 11000));
            gLRecordAmounts.Add(389, new GLRecordAmount(4000, 7500, 15000));
            gLRecordAmounts.Add(390, new GLRecordAmount(4500, 12000, 19500));
            gLRecordAmounts.Add(391, new GLRecordAmount(5000, 5000, 24500));
            gLRecordAmounts.Add(392, new GLRecordAmount(5500, 10500, 30000));
            gLRecordAmounts.Add(393, new GLRecordAmount(6000, 16500, 36000));
            gLRecordAmounts.Add(394, new GLRecordAmount(6500, 6500, 42500));
            gLRecordAmounts.Add(395, new GLRecordAmount(7000, 13500, 49500));
            gLRecordAmounts.Add(396, new GLRecordAmount(7500, 21000, 57000));


            sourceData.Add(new GLRecord(1, 10,objGLDataMembers, gLRecordAmounts));

            objGLDataMembers = new GLDataMembers();
            objGLDataMembers.AddOrUpdate(1, 106);
            objGLDataMembers.AddOrUpdate(2, 107);
            objGLDataMembers.AddOrUpdate(3, 10);
            objGLDataMembers.AddOrUpdate(4, 104);

            gLRecordAmounts = new GLRecordAmounts();
            gLRecordAmounts.Add(385, new GLRecordAmount(3000, 3000, 3000));
            gLRecordAmounts.Add(386, new GLRecordAmount(3500, 6500, 6500));
            gLRecordAmounts.Add(387, new GLRecordAmount(4000, 10500, 10500));
            gLRecordAmounts.Add(388, new GLRecordAmount(4500, 4500, 15000));
            gLRecordAmounts.Add(389, new GLRecordAmount(5000, 9500, 20000));
            gLRecordAmounts.Add(390, new GLRecordAmount(5500, 15000, 25500));
            gLRecordAmounts.Add(391, new GLRecordAmount(6000, 6000, 31500));
            gLRecordAmounts.Add(392, new GLRecordAmount(6500, 12500, 38000));
            gLRecordAmounts.Add(393, new GLRecordAmount(7000, 19500, 45000));
            gLRecordAmounts.Add(394, new GLRecordAmount(7500, 7500, 52500));
            gLRecordAmounts.Add(395, new GLRecordAmount(8000, 15500, 60500));
            gLRecordAmounts.Add(396, new GLRecordAmount(8500, 24000, 69000));

            sourceData.Add(new GLRecord(1, 10, objGLDataMembers, gLRecordAmounts));

            objGLDataMembers = new GLDataMembers();
            objGLDataMembers.AddOrUpdate(1, 107);
            objGLDataMembers.AddOrUpdate(2, 105);
            objGLDataMembers.AddOrUpdate(3, 10);
            objGLDataMembers.AddOrUpdate(4, 105);

            gLRecordAmounts = new GLRecordAmounts();
            gLRecordAmounts.Add(385, new GLRecordAmount(1000, 1000, 1000));
            gLRecordAmounts.Add(386, new GLRecordAmount(1500, 2500, 2500));
            gLRecordAmounts.Add(387, new GLRecordAmount(2000, 4500, 4500));
            gLRecordAmounts.Add(388, new GLRecordAmount(2500, 2500, 7000));
            gLRecordAmounts.Add(389, new GLRecordAmount(3000, 5500, 10000));
            gLRecordAmounts.Add(390, new GLRecordAmount(3500, 9000, 13500));
            gLRecordAmounts.Add(391, new GLRecordAmount(4000, 4000, 17500));
            gLRecordAmounts.Add(392, new GLRecordAmount(4500, 8500, 22000));
            gLRecordAmounts.Add(393, new GLRecordAmount(5000, 13500, 27000));
            gLRecordAmounts.Add(394, new GLRecordAmount(5500, 5500, 32500));
            gLRecordAmounts.Add(395, new GLRecordAmount(6000, 11500, 38500));
            gLRecordAmounts.Add(396, new GLRecordAmount(6500, 18000, 45000));

            sourceData.Add(new GLRecord(1, 10, objGLDataMembers, gLRecordAmounts));

            objGLDataMembers = new GLDataMembers();
            objGLDataMembers.AddOrUpdate(1, 108);
            objGLDataMembers.AddOrUpdate(2, 107);
            objGLDataMembers.AddOrUpdate(3, 10);
            objGLDataMembers.AddOrUpdate(4, 104);

            gLRecordAmounts = new GLRecordAmounts();
            gLRecordAmounts.Add(385, new GLRecordAmount(3500, 3500, 3500));
            gLRecordAmounts.Add(386, new GLRecordAmount(4000, 7500, 7500));
            gLRecordAmounts.Add(387, new GLRecordAmount(4500, 12000, 12000));
            gLRecordAmounts.Add(388, new GLRecordAmount(5000, 5000, 17000));
            gLRecordAmounts.Add(389, new GLRecordAmount(5500, 10500, 22500));
            gLRecordAmounts.Add(390, new GLRecordAmount(6000, 16500, 28500));
            gLRecordAmounts.Add(391, new GLRecordAmount(6500, 6500, 35000));
            gLRecordAmounts.Add(392, new GLRecordAmount(7000, 13500, 42000));
            gLRecordAmounts.Add(393, new GLRecordAmount(7500, 21000, 49500));
            gLRecordAmounts.Add(394, new GLRecordAmount(8000, 8000, 57500));
            gLRecordAmounts.Add(395, new GLRecordAmount(8500, 16500, 66000));
            gLRecordAmounts.Add(396, new GLRecordAmount(9000, 25500, 70000));

            sourceData.Add(new GLRecord(1, 10, objGLDataMembers, gLRecordAmounts));

            return sourceData;
        }
        public List<DimensionSelection> ApplyDefaultSegmentMemberIDs(List<Segment> segments, List<DimensionSelection> selections)
        {
            Dictionary<int, int> keyValuePairs = GetDefaultSegmentIDs();
            foreach (Segment segment in segments)
            {
                if(!selections.Any(x => x.SegmentID == segment.Id))
                {
                    selections.Add(new DimensionSelection()
                    {
                        SegmentID = segment.Id,
                        Option = ExtendedMemberOption.Selected,
                        SelectedMembers = new List<int> { keyValuePairs[segment.Id]}
                    });
                }
            }
            return selections;
        }
        private Dictionary<int, int> GetDefaultSegmentIDs()
        {
            Dictionary<int, int> keyValuePairs = new Dictionary<int, int>();
            keyValuePairs.Add(1, 1);
            keyValuePairs.Add(2, 1);
            keyValuePairs.Add(3, 1);
            keyValuePairs.Add(4, 1);
            return keyValuePairs;
        }

        public List<TimeHierarchy> GetTimeHierarchies()
        {
            return new List<TimeHierarchy>()
            {
                new TimeHierarchy(){ Id = 1, Name="All Time", ParentId = 0},
                new TimeHierarchy(){ Id = 2017, Name = "2017", ParentId = 1, TimeType = TimeFormat.Year},
                new TimeHierarchy(){ Id = 5086, Name = "Q1 2017", ParentId = 2017, TimeType = TimeFormat.Quarter},
                new TimeHierarchy(){ Id = 5087, Name = "Q2 2017", ParentId = 2017, TimeType = TimeFormat.Quarter},
                new TimeHierarchy(){ Id = 5088, Name = "Q3 2017", ParentId = 2017, TimeType = TimeFormat.Quarter},
                new TimeHierarchy(){ Id = 5089, Name = "Q4 2017", ParentId = 2017, TimeType = TimeFormat.Quarter},
                new TimeHierarchy(){ Id = 385, Name = "Jan-17", ParentId = 5086, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 386, Name = "Feb-17", ParentId = 5086, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 387, Name = "Mar-17", ParentId = 5086, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 388, Name = "Apr-17", ParentId = 5087, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 389, Name = "May-17", ParentId = 5087, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 390, Name = "Jun-17", ParentId = 5087, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 391, Name = "Jul-17", ParentId = 5088, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 392, Name = "Aug-17", ParentId = 5088, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 393, Name = "Sep-17", ParentId = 5088, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 394, Name = "Oct-17", ParentId = 5089, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 395, Name = "Nov-17", ParentId = 5089, TimeType = TimeFormat.Month},
                new TimeHierarchy(){ Id = 396, Name = "Dec-17", ParentId = 5089, TimeType = TimeFormat.Month}
            };
        }
    }
}
