using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollupAggregator
{
    public class SegmentMember
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int ParentId { get; set; }
        public int MemberType { get; set; }
        public int LevelVal { get; set; }
        public int LeftVal { get; set; }
        public int RightVal { get; set; }
        public string RollupOperator { get; set; }
        public string Lineage { get; set; }
        public List<SignValue> SignValues { get; set; }
    }
    public class GlData
    {
        public int AccountId { get; set; }
        public int CompanyId { get; set; }
        public int MonthId { get; set; }
        public int ReportingId { get; set; }
        public double YTDAmount { get; set; }
    }
    public class SignValue
    {
        public int IntLevel { get; set; }
        public int Sign { get; set; }
    }
    public class RollupAggregator
    {
        static string SQLConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
        static Dictionary<int, string> idRollup = new Dictionary<int, string>();
        static Dictionary<string, List<SignValue>> lineageSignValues = new Dictionary<string, List<SignValue>>();
        static void Main(string[] args)
        {
            DateTime startTime ;
            DateTime endTime ;
            List<GlData> glDataMembers = new List<GlData>();
            List<GlData> aggrMembers = new List<GlData>();
            Console.WriteLine("Populating segment members Start Time : " + DateTime.Now);
            startTime = DateTime.Now;
            List<SegmentMember> segmentMembers = PopulateSegmentMember();
            Console.WriteLine("Populating segment members End Time : " + DateTime.Now);
            endTime = DateTime.Now;
            Console.WriteLine("Populating segment members time in MS : " + Convert.ToInt32(endTime.Subtract(startTime).TotalMilliseconds));
            
            Console.WriteLine("Populating sign at each level Start Time : " + DateTime.Now);
            startTime = DateTime.Now;
            segmentMembers = PopulateLevelSigns(segmentMembers);
            endTime = DateTime.Now;
            Console.WriteLine("Populating sign at each level End Time : " + DateTime.Now);
            Console.WriteLine("Populating sign at each level time in MS : " + Convert.ToInt32(endTime.Subtract(startTime).TotalMilliseconds));
            
            Console.WriteLine("Reading fact gl data Start Time : " + DateTime.Now);
            startTime = DateTime.Now;
            glDataMembers = ReadData(1, 1131, 10,5075);
            endTime = DateTime.Now;
            Console.WriteLine("Reading fact gl data End Time : " + DateTime.Now);
            Console.WriteLine("Reading fact gl data time in MS : " + Convert.ToInt32(endTime.Subtract(startTime).TotalMilliseconds));

            Console.WriteLine("Calculating Rollup Data Start Time : " + DateTime.Now);
            startTime = DateTime.Now;
            aggrMembers = CalculateRollupData(1, 1131, 10, 5075, glDataMembers, segmentMembers);
            endTime = DateTime.Now;
            Console.WriteLine("Calculating Rollup Data End Time : " + DateTime.Now);
            Console.WriteLine("Calculating Rollup Data time in MS : " + Convert.ToInt32(endTime.Subtract(startTime).TotalMilliseconds));
        }
        static List<SegmentMember> PopulateSegmentMember()
        {
            List<SegmentMember> segmentMembers = new List<SegmentMember>();
            string strQuery = "SELECT IDX,LABEL,PARENT_IDX,LEVEL_VAL,TYPE,LEFT_VAL,RIGHT_VAL,ROLLUP_OPERATOR,LINEAGE FROM DIM_SEG1 WHERE ROLLUP_ID = 1  ";
            using (SqlConnection con = new SqlConnection(SQLConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = strQuery;
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    con.Open();
                    using (var irdr = cmd.ExecuteReader())
                    {
                        if (irdr.HasRows)
                        {
                            while (irdr.Read())
                            {
                                var segmentMember = new SegmentMember()
                                {
                                    Id = Convert.ToInt32(irdr["IDX"]),
                                    Label = Convert.ToString(irdr["LABEL"]),
                                    ParentId = Convert.ToInt32(irdr["PARENT_IDX"]),
                                    LeftVal = Convert.ToInt32(irdr["LEFT_VAL"]),
                                    RightVal = Convert.ToInt32(irdr["RIGHT_VAL"]),
                                    LevelVal = Convert.ToInt32(irdr["LEVEL_VAL"]),
                                    MemberType = Convert.ToInt32(irdr["TYPE"]),
                                    RollupOperator = Convert.ToString(irdr["ROLLUP_OPERATOR"]),
                                    Lineage = Convert.ToString(irdr["LINEAGE"])
                                };
                                if (Convert.ToInt32(irdr["TYPE"]) == 3)
                                {
                                    idRollup.Add(Convert.ToInt32(irdr["IDX"]), Convert.ToString(irdr["ROLLUP_OPERATOR"]));
                                }
                                segmentMembers.Add(segmentMember);
                            }
                        }
                    }
                }
            }
            return segmentMembers;
        }
        static List<SegmentMember> PopulateLevelSigns(List<SegmentMember> segMembers)
        {
            var lineageIndexVal = new Dictionary<int, int>();
            int lineIndex;
            string lineage = "";
            int segLevelVal;
            List<string> lineageVal;
            int signVal = 1;
            List<SignValue> signValues;
            SegmentMember tempSegmember = new SegmentMember();
            foreach (SegmentMember segmember in segMembers.Where(x => x.MemberType == 4))
            {
                signValues = new List<SignValue>();
                lineage = segmember.Lineage;
                if (lineageSignValues.ContainsKey(lineage))
                {
                    signValues = lineageSignValues[lineage];
                }
                else
                {
                    lineageIndexVal = new Dictionary<int, int>();
                    lineageVal = lineage.Split('/').ToList();
                    lineIndex = 1;
                    // Populate Lineage Index
                    foreach (string linVal in lineageVal)
                    {
                        if (linVal.Trim() != "")
                        {
                            lineageIndexVal.Add(Convert.ToInt32(lineIndex), Convert.ToInt32(linVal));
                            lineIndex++;
                        }
                    }
                    segLevelVal = segmember.LevelVal - 1;
                    signVal = 1;
                    // populate sign at each level
                    while (segLevelVal >= 1)
                    {
                        signVal = signVal * Convert.ToInt32(ReturnSingValueByRollupOperator(idRollup[lineageIndexVal[segLevelVal]]));
                        var signValue = new SignValue()
                        {
                            IntLevel = Convert.ToInt32(segLevelVal),
                            Sign = Convert.ToInt32(signVal)
                        };
                        signValues.Add(signValue);
                        segLevelVal--;
                    }
                    lineageSignValues.Add(lineage,signValues);
                }
                segmember.SignValues = signValues;
            }
            return segMembers;
        }
        static int ReturnSingValueByRollupOperator(string rollupOperator)
        {
            int returnVal = 1;
            switch (rollupOperator)
            {
                case "+":
                    returnVal = 1;
                    break;
                case "-":
                    returnVal = -1;
                    break;
                case "~":
                    returnVal = 0;
                    break;
                default:
                    returnVal = 1;
                    break;
            }
            return returnVal;
        }
        static List<GlData> ReadData(int accountRollupId,int companyId,int reportingId, int yearId)
        {
            List<GlData> glDataMembers = new List<GlData>();
            var _params = new List<SqlParameter>();
            _params.Add(new SqlParameter("@accountRollupId", accountRollupId));
            _params.Add(new SqlParameter("@companyId", companyId));
            _params.Add(new SqlParameter("@reportingId", reportingId));
            _params.Add(new SqlParameter("@yearId", yearId));
            SqlParameter[] paramList = _params != null ? _params.ToArray() : null;
            string strQuery = @"SELECT 
                    SEG1_ID, SEG2_ID , TIME_ID , REPORTING_ID , SUM(YTD) YTD 
	        FROM 
                FACT_GL FGL,  	
	            (
			            SELECT 
			            DS.REF_IDX
			            FROM DIM_SEG2 DS,
					            (	SELECT LEFT_VAL, RIGHT_VAL 
						            FROM DIM_SEG2 
						            WHERE IDX = @companyId
					            ) DS_LVL
			            WHERE
				            DS.LEFT_VAL >= DS_LVL.LEFT_VAL AND DS.RIGHT_VAL <= DS_LVL.RIGHT_VAL AND 
				            DS.TYPE = 4	AND ROLLUP_ID = 1
	            )SEG2_IDS ,
	            (
		            SELECT LEVEL2_ID FROM DIM_REPORTING WHERE LEVEL1_ID IN (@reportingId) OR LEVEL2_ID IN (@reportingId)
	            ) REP_IDS , 
	            (
			            SELECT 
			            DS.REF_IDX
			            FROM DIM_SEG1 DS,
					            (	SELECT LEFT_VAL, RIGHT_VAL 
						            FROM DIM_SEG1 
						            WHERE IDX = @accountRollupId
					            ) DS_LVL
			            WHERE
				            DS.LEFT_VAL >= DS_LVL.LEFT_VAL AND DS.RIGHT_VAL <= DS_LVL.RIGHT_VAL AND 
				            DS.TYPE = 4	AND ROLLUP_ID = 1
	            )SEG1_IDS ,
	            (
		            SELECT MONTH_ID FROM DIM_TIME WHERE YEAR_ID IN (@yearId) OR QTR_ID IN (@yearId) OR MONTH_ID IN (@yearId)
	            ) TIME_IDS
            WHERE 
	            FGL.SCENARIO_ID = 1
	            AND FGL.SEG2_ID = SEG2_IDS.REF_IDX 
	            AND FGL.REPORTING_ID = REP_IDS.LEVEL2_ID 
	            AND FGL.SEG1_ID = SEG1_IDS.REF_IDX 
	            AND FGL.TIME_ID = TIME_IDS.MONTH_ID 
            GROUP BY 
	            SEG1_ID, SEG2_ID , TIME_ID , REPORTING_ID ";
            using (SqlConnection con = new SqlConnection(SQLConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = strQuery;
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddRange(paramList);
                    con.Open();
                    using (var irdr = cmd.ExecuteReader())
                    {
                        if (irdr.HasRows)
                        {
                            while (irdr.Read())
                            {
                                var glDataMember = new GlData()
                                {
                                    AccountId = Convert.ToInt32(irdr["SEG1_ID"]),
                                    CompanyId = Convert.ToInt32(irdr["SEG2_ID"]),
                                    MonthId = Convert.ToInt32(irdr["TIME_ID"]),
                                    ReportingId = Convert.ToInt32(irdr["REPORTING_ID"]),
                                    YTDAmount = Convert.ToDouble(irdr["YTD"])
                                };
                                glDataMembers.Add(glDataMember);
                            }
                        }
                    }
                }
            }
            return glDataMembers;
        }
        static List<int> GetLeafMembers(int parentId,string dimensionName)
        {
            List<int> leafMemberIds = new List<int>();
            var _params = new List<SqlParameter>();
            _params.Add(new SqlParameter("@parentId", parentId));            
            SqlParameter[] paramList = _params != null ? _params.ToArray() : null;
            string strQuery = "";
            if (dimensionName == "Company")
            {
                strQuery = @"
			            SELECT 
			                DS.REF_IDX LEAF_ID
			            FROM DIM_SEG2 DS,
					        (SELECT LEFT_VAL, RIGHT_VAL FROM DIM_SEG2 WHERE IDX = @parentId) DS_LVL
			            WHERE
				            DS.LEFT_VAL >= DS_LVL.LEFT_VAL AND DS.RIGHT_VAL <= DS_LVL.RIGHT_VAL AND 
				            DS.TYPE = 4	AND ROLLUP_ID = 1
	                    ";
            }
            else if (dimensionName == "time")
            {
                strQuery = @"SELECT MONTH_ID LEAF_ID FROM DIM_TIME WHERE YEAR_ID IN (@parentId) OR QTR_ID IN (@parentId) OR MONTH_ID IN (@parentId)";
            }
            else if (dimensionName == "reporting")
            {
                strQuery = @"SELECT LEVEL2_ID LEAF_ID FROM DIM_REPORTING WHERE LEVEL1_ID IN (@parentId) OR LEVEL2_ID IN (@parentId)";
            }
            using (SqlConnection con = new SqlConnection(SQLConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = strQuery;
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddRange(paramList);
                    con.Open();
                    using (var irdr = cmd.ExecuteReader())
                    {
                        if (irdr.HasRows)
                        {
                            while (irdr.Read())
                            {
                                leafMemberIds.Add(Convert.ToInt32(irdr["LEAF_ID"]));
                            }
                        }
                    }
                }
            }
            return leafMemberIds;
        }
        static List<GlData> CalculateRollupData(int accountRollupId, int companyId, int reportingId, int yearId, List<GlData> leafDataMembers, List<SegmentMember> segmentMembers)
        {
            double aggrValue;
            double currMemValue;
            int signatLevel;
            List<int> leafCompanyids = new List<int>();
            List<int> leafMonthids = new List<int>();
            List<int> leafRepportingids = new List<int>();
            List<GlData> glDataMembers = new List<GlData>();
            GlData currGlDaaMember = new GlData();
            // Get Current segment member of rollup
            SegmentMember currSegMember = segmentMembers.Find(t => t.Id == accountRollupId);
            //Get account Rollup Level val
            int accountRollupLevelVal = currSegMember.LevelVal;
            //Get all leaf members of account under the rollup
            int accountRollupLeftVal = currSegMember.LeftVal;
            int accountRollupRightVal = currSegMember.RightVal;
            List<SegmentMember> leafMembers = segmentMembers.FindAll(t => t.LeftVal >= accountRollupLeftVal && t.RightVal <= accountRollupRightVal && t.MemberType == 4);
            Console.WriteLine("Populating Company Leaf members Start Time : " + DateTime.Now);
            // Get all leaf members of company
            
            leafCompanyids = GetLeafMembers(companyId, "Company");
            Console.WriteLine("Populating Company Leaf members End Time : " + DateTime.Now);
            // Get all month ids based on year
            Console.WriteLine("Populating Time Leaf members Start Time : " + DateTime.Now);
            leafMonthids = GetLeafMembers(yearId, "time");
            Console.WriteLine("Populating Time Leaf members End Time : " + DateTime.Now);
            // Get all reporting ids based on rollup
            Console.WriteLine("Populating Time Reporting members Start Time : " + DateTime.Now);
            leafRepportingids = GetLeafMembers(reportingId, "reporting");
            Console.WriteLine("Populating Time Reporting members End Time : " + DateTime.Now);
            foreach (int reportingid in leafRepportingids)
            {
                Console.WriteLine("Aggregating data at reporting id: " + reportingid + " Start Time : " + DateTime.Now);
                foreach (int compId in leafCompanyids)
                {
                    foreach (int monthId in leafMonthids)
                    {
                        aggrValue = 0.0;
                        signatLevel = 1;
                        // loop through all leaf account members and aggregate values
                        foreach (SegmentMember segMemb in leafMembers)
                        {            
                            currMemValue = 0.0;
                            currGlDaaMember = new GlData();
                            currGlDaaMember = leafDataMembers.Find(t => t.ReportingId == reportingid && t.CompanyId == compId && t.MonthId == monthId && t.AccountId == segMemb.Id);
                            if (currGlDaaMember != null)
                            {
                                signatLevel = segMemb.SignValues.Find(t => t.IntLevel == accountRollupLevelVal).Sign;
                                currMemValue = currGlDaaMember.YTDAmount;
                                aggrValue = aggrValue + (currMemValue * signatLevel);
                            }                            
                        }
                        if (aggrValue != 0.0)
                        { 
                            var glDataMember = new GlData()
                            {
                                AccountId = Convert.ToInt32(accountRollupId),
                                CompanyId = Convert.ToInt32(compId),
                                MonthId = Convert.ToInt32(monthId),
                                ReportingId = Convert.ToInt32(reportingid),
                                YTDAmount = Convert.ToDouble(aggrValue)
                            };
                            glDataMembers.Add(glDataMember);
                        }
                    }
                }
                Console.WriteLine("Aggregating data at reporting id: " + reportingid + " End Time : " + DateTime.Now);
                Console.WriteLine("Aggregating data count : " + glDataMembers.Count());
            }
            return glDataMembers;
        }
    }
}
