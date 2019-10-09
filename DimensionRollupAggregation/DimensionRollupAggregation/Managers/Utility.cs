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
    public static class Utility
    {
        public static T CloneObjectSerializable<T>(this T obj) where T : GLRecord 
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            ms.Position = 0;
            object result = bf.Deserialize(ms);
            ms.Close();
            return (T)result;
        }
        public static string GetGLUniqueKey(GLRecord objGLRecord, VisitorType visitorType, int newMemberId, int segmentId = 0)
        {
            switch (visitorType)
            {
                case VisitorType.Segment:
                    Dictionary<int, int> SegmentValues = objGLRecord.GLMembers.SegmentValues.ToDictionary(x => x.Key, x => x.Value);
                    if (SegmentValues != null && segmentId != 0)
                    {
                        if (SegmentValues.ContainsKey(segmentId))
                        {
                            SegmentValues[segmentId] = newMemberId;
                        }
                        return string.Format("{0}{1}{2}", objGLRecord.ScenarioId, objGLRecord.ReportingId, string.Concat(SegmentValues.OrderBy(o => o.Key).Select(s => s.Value.ToString())));
                    }
                    return objGLRecord.UniqueKey;
                default: return objGLRecord.UniqueKey;
            }
        }
        public static int GetMemberOperator(string Operator)
        {
            switch (Operator)
            {
                case "+": return 1;
                case "-": return -1;
                case "~": return 0;
                default: return 1;
            }
        }

        public static void PopulateLRValues(List<TimeHierarchy> timeHierarchies)
        {
            int lrvalue = 1;
            Dictionary<int, List<int>> data = new Dictionary<int, List<int>>();
            foreach (TimeHierarchy timeHierarchy in timeHierarchies)
            {
                if (!data.ContainsKey(timeHierarchy.Id))
                {
                    data.Add(timeHierarchy.Id, new List<int>());
                }
                if (data.ContainsKey(timeHierarchy.ParentId))
                {
                    List<int> childs = (List<int>)data[timeHierarchy.ParentId];
                    childs.Add(timeHierarchy.Id);
                    data[timeHierarchy.ParentId] = childs;
                }
            }
            CalLRValues(1, (List<int>)data[1], lrvalue, data, timeHierarchies, timeHierarchies.FirstOrDefault(x => x.Id == 1));
        }

        private static void CalLRValues(int id, List<int> child, int lrvalue, Dictionary<int, List<int>> data, List<TimeHierarchy> timeHierarchies, TimeHierarchy timeHierarchy)
        {
            int lvalue = lrvalue++;
            for (int i = 0; i < child.Count; i++)
            {
                CalLRValues(child[i], (List<int>)data[child[i]], lrvalue, data, timeHierarchies, timeHierarchies.FirstOrDefault(x => x.Id == child[i]));
            }
            int rvalue = lrvalue++;

            timeHierarchy.Left = lvalue;
            timeHierarchy.Right = rvalue;
        }

        public static void Export(List<GLRecord> lstGLRecord, List<Segment> segments, Dictionary<Segment, HierarchyMetaData> metaData)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Account");
            dataTable.Columns.Add("Company");
            dataTable.Columns.Add("Department");
            dataTable.Columns.Add("IC Segment");
            dataTable.Columns.Add("Measure");
            dataTable.Columns.Add("Jan-17");
            dataTable.Columns.Add("Feb-17");
            dataTable.Columns.Add("Mar-17");
            dataTable.Columns.Add("Apr-17");
            dataTable.Columns.Add("May-17");
            dataTable.Columns.Add("Jun-17");
            dataTable.Columns.Add("Jul-17");
            dataTable.Columns.Add("Aug-17");
            dataTable.Columns.Add("Sep-17");
            dataTable.Columns.Add("Oct-17");
            dataTable.Columns.Add("Nov-17");
            dataTable.Columns.Add("Dec-17");

            foreach(GLRecord gLRecord in lstGLRecord)
            {
                for (int i = 1; i <= 3; i++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["Account"] = metaData[segments.FirstOrDefault(x => x.Id == 1)].Members.FirstOrDefault(x => x.Id == gLRecord.GLMembers.Get(1)).Label;
                    dataRow["Company"] = metaData[segments.FirstOrDefault(x => x.Id == 2)].Members.FirstOrDefault(x => x.Id == gLRecord.GLMembers.Get(2)).Label;
                    dataRow["IC Segment"] = metaData[segments.FirstOrDefault(x => x.Id == 3)].Members.FirstOrDefault(x => x.Id == gLRecord.GLMembers.Get(3)).Label;
                    dataRow["Department"] = metaData[segments.FirstOrDefault(x => x.Id == 4)].Members.FirstOrDefault(x => x.Id == gLRecord.GLMembers.Get(4)).Label;
                    dataRow["Measure"] = GetMeasureName(i);
                    dataRow["Jan-17"] = GetValue(gLRecord.Values, i, 385);
                    dataRow["Feb-17"] = GetValue(gLRecord.Values, i, 386);
                    dataRow["Mar-17"] = GetValue(gLRecord.Values, i, 387);
                    dataRow["Apr-17"] = GetValue(gLRecord.Values, i, 388);
                    dataRow["May-17"] = GetValue(gLRecord.Values, i, 389);
                    dataRow["Jun-17"] = GetValue(gLRecord.Values, i, 390);
                    dataRow["Jul-17"] = GetValue(gLRecord.Values, i, 391);
                    dataRow["Aug-17"] = GetValue(gLRecord.Values, i, 392);
                    dataRow["Sep-17"] = GetValue(gLRecord.Values, i, 393);
                    dataRow["Oct-17"] = GetValue(gLRecord.Values, i, 394);
                    dataRow["Nov-17"] = GetValue(gLRecord.Values, i, 395);
                    dataRow["Dec-17"] = GetValue(gLRecord.Values, i, 396);

                    dataTable.Rows.Add(dataRow);
                }                
            }

            var lines = new List<string>();

            string[] columnNames = dataTable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();

            var header = string.Join(",", columnNames);
            lines.Add(header);

            var valueLines = dataTable.AsEnumerable()
                               .Select(row => string.Join(",", row.ItemArray));
            lines.AddRange(valueLines);

            File.WriteAllLines(@"D:\output.csv", lines);
        }

        private static string GetMeasureName(int i)
        {
            switch (i) { case 1: return "MTD"; case 2: return "QTD"; case 3: return "YTD"; default: return string.Empty; }
        }

        private static decimal GetValue(GLRecordAmounts glRecordAmounts, int i, int monthID)
        {
            switch (i) { case 1: return glRecordAmounts.Get(monthID).Mtd; case 2: return glRecordAmounts.Get(monthID).Qtd; case 3: return glRecordAmounts.Get(monthID).Ytd; default: return 0; }
        }
    
    }
}
