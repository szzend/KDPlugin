using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.Contracts.Report;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Report;
using System.ComponentModel;
using System.Data;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Util;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.App;
using Kingdee.BOS;

namespace Z.K3.SCM.SAL.Report.ServicePlugIn
{
    [Kingdee.BOS.Util.HotUpdate]
    [Description("销售排名分页报表插件")]
    public class SaleRankingReport : SysReportBaseService
    {
        private string[] ColumnTableNamesOld;
        private string RankingOnNew = "";

        private Dictionary<String, String> filters;
        public override void Initialize()
        {
            base.Initialize();
            // 简单账表类型：分页账表
            ReportProperty.ReportType = ReportType.REPORTTYPE_MOVE;
            // 标识报表支持分组汇总
            ReportProperty.IsGroupSummary = true;
            // 标识报表的列通过插件定义
            ReportProperty.IsUIDesignerColumns = false;
            // 报表主键字段名：默认为FIDENTITYID，可以修改
            //this.ReportProperty.IdentityFieldName = "FIDENTITYID";
            //
            // 设置精度控制
            List<DecimalControlField> list = new List<DecimalControlField>();
            // 数量
            list.Add(new DecimalControlField
            {
                ByDecimalControlFieldName = "FSalQty",
                DecimalControlFieldName = "FUnitPrecision"
            });
            // 单价
            list.Add(new DecimalControlField
            {
                ByDecimalControlFieldName = "FPrice",
                DecimalControlFieldName = "FPRICEDIGITS"
            });
            // 金额
            list.Add(new DecimalControlField
            {
                ByDecimalControlFieldName = "FAmount",
                DecimalControlFieldName = "FAMOUNTDIGITS"
            });
            this.ReportProperty.DecimalControlFieldList = list;

        }

        /// <summary>
        /// 动态够造列
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override ReportHeader GetReportHeaders(IRptParams filter)
        {
            Kingdee.BOS.Core.List.ListHeader ranking, mnumber, mname, model, group, cty, saler, custnumber, custname, dep;
            ReportHeader header = new ReportHeader();

            ranking = header.AddChild("FIDENTITYID", new LocaleValue("排名"), 0);
            ranking.Width = 40;
            switch (RankingOnNew)
            {
                case "COUNTRYMATERIAL":
                    mnumber = header.AddChild("FMNUMBER", new LocaleValue("物料编码"), 1);
                    mnumber.Width = 160;
                    mname = header.AddChild("FMNAME", new LocaleValue("物料名称"), 2);
                    mname.Width = 140;
                    model = header.AddChild("FModel", new LocaleValue("规格描述"), 3);
                    model.Width = 200;
                    group = header.AddChild("FGROUPNAME", new LocaleValue("物料分组"), 4);
                    group.Width = 140;
                    cty = header.AddChild("FCOUNTRYNAME", new LocaleValue("客户国家"), 5);
                    cty.Width = 60;
                    break;
                case "SALERCUSTCOUNTRY":
                    saler = header.AddChild("FSALERNAME", new LocaleValue("业务员"), 1);
                    saler.Width = 60;
                    custnumber = header.AddChild("FCUSTNUMBER", new LocaleValue("客户代码"), 2);
                    custnumber.Width = 60;
                    custname = header.AddChild("FCUSTNAME", new LocaleValue("客户名称"), 3);
                    custname.Width = 120;
                    cty = header.AddChild("FCOUNTRYNAME", new LocaleValue("客户国家"), 4);
                    cty.Width = 60;
                    break;
                case "MATERIALGROUP":
                    group = header.AddChild("FGROUPNAME", new LocaleValue("物料分组"), 1);
                    group.Width = 140;
                    break;
                case "BD_CUSTOMER":
                    custnumber = header.AddChild("FCUSTNUMBER", new LocaleValue("客户代码"), 1);
                    custnumber.Width = 60;
                    custname = header.AddChild("FCUSTNAME", new LocaleValue("客户名称"), 2);
                    custname.Width = 120;
                    cty = header.AddChild("FCOUNTRYNAME", new LocaleValue("客户国家"), 3);
                    cty.Width = 60;
                    break;
                case "SALER":
                    saler = header.AddChild("FSALERNAME", new LocaleValue("业务员"), 1);
                    saler.Width = 60;
                    break;
                case "SALEDEPARTMENT":
                    dep = header.AddChild("FDEPNAME", new LocaleValue("销售部门"), 1);
                    dep.Width = 60;
                    break;
                case "CUSTMATERIALGROUP":
                    custnumber = header.AddChild("FCUSTNUMBER", new LocaleValue("客户代码"), 1);
                    custnumber.Width = 60;
                    custname = header.AddChild("FCUSTNAME", new LocaleValue("客户名称"), 2);
                    custname.Width = 120;
                    group = header.AddChild("FGROUPNAME", new LocaleValue("物料分组"), 3);
                    group.Width = 140;
                    break;

                case "SALERCUSTMATERIALGROUP":
                    saler = header.AddChild("FSALERNAME", new LocaleValue("业务员"), 1);
                    saler.Width = 60;
                    custnumber = header.AddChild("FCUSTNUMBER", new LocaleValue("客户代码"), 2);
                    custnumber.Width = 60;
                    custname = header.AddChild("FCUSTNAME", new LocaleValue("客户名称"), 3);
                    custname.Width = 120;
                    group = header.AddChild("FGROUPNAME", new LocaleValue("物料分组"), 4);
                    group.Width = 140;
                    break;
                case "BD_MATERIAL":
                    mnumber = header.AddChild("FMNUMBER", new LocaleValue("物料编码"), 1);
                    mnumber.Width = 160;
                    mname = header.AddChild("FMNAME", new LocaleValue("物料名称"), 2);
                    mname.Width = 140;
                    model = header.AddChild("FModel", new LocaleValue("规格描述"), 3);
                    model.Width = 200;
                    group = header.AddChild("FGROUPNAME", new LocaleValue("物料分组"), 4);
                    group.Width = 140;
                    break;
                case "SALERMATERIAL":
                    saler = header.AddChild("FSALERNAME", new LocaleValue("业务员"), 1);
                    saler.Width = 60;
                    mnumber = header.AddChild("FMNUMBER", new LocaleValue("物料编码"), 2);
                    mnumber.Width = 160;
                    mname = header.AddChild("FMNAME", new LocaleValue("物料名称"), 3);
                    mname.Width = 140;
                    model = header.AddChild("FModel", new LocaleValue("规格描述"), 4);
                    model.Width = 200;
                    group = header.AddChild("FGROUPNAME", new LocaleValue("物料分组"), 5);
                    group.Width = 140;
                    break;
                case "SALEDEPMATERIAL":
                    dep = header.AddChild("FDEPNAME", new LocaleValue("销售部门"), 1);
                    dep.Width = 60;
                    mnumber = header.AddChild("FMNUMBER", new LocaleValue("物料编码"), 2);
                    mnumber.Width = 160;
                    mname = header.AddChild("FMNAME", new LocaleValue("物料名称"), 3);
                    mname.Width = 140;
                    model = header.AddChild("FModel", new LocaleValue("规格描述"), 4);
                    model.Width = 200;
                    group = header.AddChild("FGROUPNAME", new LocaleValue("物料分组"), 5);
                    group.Width = 140;
                    break;
                case "CUSTMATERIAL":
                    custnumber = header.AddChild("FCUSTNUMBER", new LocaleValue("客户代码"), 1);
                    custnumber.Width = 60;
                    custname = header.AddChild("FCUSTNAME", new LocaleValue("客户名称"), 2);
                    custname.Width = 120;
                    mnumber = header.AddChild("FMNUMBER", new LocaleValue("物料编码"), 3);
                    mnumber.Width = 160;
                    mname = header.AddChild("FMNAME", new LocaleValue("物料名称"), 4);
                    mname.Width = 140;
                    model = header.AddChild("FModel", new LocaleValue("规格描述"), 5);
                    model.Width = 200;
                    group = header.AddChild("FGROUPNAME", new LocaleValue("物料分组"), 6);
                    group.Width = 140;
                    break;
                case "CUSTOMERSALERMATERIAL":
                    custnumber = header.AddChild("FCUSTNUMBER", new LocaleValue("客户代码"), 1);
                    custnumber.Width = 60;
                    custname = header.AddChild("FCUSTNAME", new LocaleValue("客户名称"), 2);
                    custname.Width = 120;
                    saler = header.AddChild("FSALERNAME", new LocaleValue("业务员"), 3);
                    saler.Width = 60;
                    mnumber = header.AddChild("FMNUMBER", new LocaleValue("物料编码"), 4);
                    mnumber.Width = 160;
                    mname = header.AddChild("FMNAME", new LocaleValue("物料名称"), 5);
                    mname.Width = 140;
                    model = header.AddChild("FModel", new LocaleValue("规格描述"), 6);
                    model.Width = 200;
                    group = header.AddChild("FGROUPNAME", new LocaleValue("物料分组"), 7);
                    group.Width = 140;
                    break;
            }

            var qty = header.AddChild("FSalQty", new LocaleValue("销售数量"), SqlStorageType.SqlDecimal);
            qty.ColIndex = header.GetChildCount() - 1;
            qty.Width = 80;
            var amt = header.AddChild("FAmount", new LocaleValue("销售金额"), SqlStorageType.SqlDecimal);
            //amt.ColIndex = 22;
            amt.Width = 90;
            var price = header.AddChild("FPrice", new LocaleValue("平均单价"), SqlStorageType.SqlDecimal);
            //price.ColIndex = 23;
            price.Width = 60;
            return header;
        }

        /// <summary>
        /// 设置报表头
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override ReportTitles GetReportTitles(IRptParams filter)
        {
            ReportTitles titles = new ReportTitles();

            if (CacheDataList != null && CacheDataList.ContainsKey(filter.CurrentPosition))
            {
                var dr = CacheDataList[filter.CurrentPosition];
                if (dr != null)
                {
                    titles.AddTitle("F_QNWB_SalOrg", Convert.ToString(dr["FOrgName"]));
                }
            }
            titles.AddTitle("F_QNWB_DateRange", String.Format("{0}至{1}", filters["StartDate"].Substring(0, 10), filters["EndDate"].Substring(0, 10)));
            titles.AddTitle("F_QNWB_Currency", "人民币");
            return titles;


        }

        /// <summary>
        /// 构造取数Sql，取数据填充到临时表：tableName
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="tableName"></param>
        public override void BuilderReportSqlAndTempTable(IRptParams filter, string tableName)
        {
            IDBService service = ServiceHelper.GetService<IDBService>();
            ColumnTableNamesOld = service.CreateTemporaryTableName(base.Context, 1);
            string tb1 = ColumnTableNamesOld[0];//第一次查询使用的临时表
            //string tb2 = ColumnTableNamesNew[0];
            //父类执行得到原数据源
            //base.BuilderReportSqlAndTempTable(filter, tb1);
            DynamicObject customFilter = filter.FilterParameter.CustomFilter;
            get_params(customFilter, out filters);
            string sqlStr = "exec {0} '{1}','{2}','{3}'";
            switch (filters["DataSource"])
            {
                case "Sal_SaleOrder":
                    sqlStr = String.Format(sqlStr, "[dbo].[z_get_sal_orders]", filters["StartDate"], filters["EndDate"], tb1);
                    break;
                case "Sal_OUtStock":
                    sqlStr = String.Format(sqlStr, "[dbo].[z_get_sal_outstock]", filters["StartDate"], filters["EndDate"], tb1);
                    break;
                case "AR_ReceiveBill":
                    sqlStr = String.Format(sqlStr, "[dbo].[z_get_sal_receivable]", filters["StartDate"], filters["EndDate"], tb1);
                    break;
            }
            DBUtils.Execute(this.Context, sqlStr);

            string sqlStr2 = String.Format("({0}) T", buildSqlstr(tb1));
            RankingOnNew = ((customFilter["RankingOnNew"] != null) ? customFilter["RankingOnNew"].ToString() : "");
            //StringBuilder stringBuilder = new StringBuilder();
            string sql = @"/*dialect*/select IDENTITY(INT,1,1) FIDENTITYID,{0},SUM(isnull(T.FBASEUNITQTY,0)) FSalQty,SUM(isnull(T.FAMOUNT_LC,0)) FAmount,
                                CASE WHEN SUM(T.FBASEUNITQTY) = 0  THEN 0 ELSE SUM(T.FAMOUNT_LC)/SUM(T.FBASEUNITQTY) END  FPrice,
                                2 as  FPRICEDIGITS,2 as FAMOUNTDIGITS,0 as FUnitPrecision 
                                into {1} from {2}   group by {3}  order by FAmount desc
                          ";
            string fields = "";
            switch (RankingOnNew)
            {

                case "COUNTRYMATERIAL":
                    fields = "T.FMNUMBER,T.FMNAME,T.FModel,T.FGROUPNAME,T.FCOUNTRYNAME";
                    break;
                case "SALERCUSTCOUNTRY":
                    fields = "T.FSALERNUMBER,T.FSALERNAME,T.FCUSTNUMBER,T.FCUSTNAME,T.FCOUNTRYNAME";
                    break;
                case "MATERIALGROUP":
                    fields = "T.FGROUPNUMBER,T.FGROUPNAME";
                    break;
                case "BD_CUSTOMER":
                    fields = "T.FCUSTNUMBER,T.FCUSTNAME,T.FCOUNTRYNAME";
                    break;
                case "SALER":
                    fields = "T.FSALERNUMBER,T.FSALERNAME";
                    break;
                case "SALEDEPARTMENT":
                    fields = "T.FDEPNAME";
                    break;
                case "CUSTMATERIALGROUP":
                    fields = "T.FCUSTNUMBER,T.FCUSTNAME,FGROUPNAME";
                    break;

                case "SALERCUSTMATERIALGROUP":
                    fields = "T.FSALERNAME,T.FCUSTNUMBER,T.FCUSTNAME,FGROUPNAME";
                    break;
                case "BD_MATERIAL":
                    fields = "T.FMNUMBER,T.FMNAME,T.FModel,T.FGROUPNAME";
                    break;
                case "SALERMATERIAL":
                    fields = "T.FSALERNAME,T.FMNUMBER,T.FMNAME,T.FModel,T.FGROUPNAME";
                    break;
                case "SALEDEPMATERIAL":
                    fields = "T.FDEPNAME,T.FMNUMBER,T.FMNAME,T.FModel,T.FGROUPNAME";
                    break;
                case "CUSTMATERIAL":
                    fields = "T.FCUSTNUMBER,T.FCUSTNAME,T.FMNUMBER,T.FMNAME,T.FModel,T.FGROUPNAME";
                    break;
                case "CUSTOMERSALERMATERIAL":
                    fields = "T.FCUSTNUMBER,T.FCUSTNAME,T.FSALERNAME,T.FMNUMBER,T.FMNAME,T.FModel,T.FGROUPNAME";
                    break;

            }
            DBUtils.Execute(this.Context, String.Format(sql, fields, tableName, sqlStr2, fields));


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override DataTable GetList(IRptParams filter)
        {
            DynamicObject customFilter = filter.FilterParameter.CustomFilter;
            var OrgID = customFilter["SaleOrgList"].ToString();
            var dt = new DataTable();
            dt.Columns.Add("FOrgID");
            dt.Columns.Add("FOrgName");
            var sql = String.Format("select FORGID,FNAME from t_org_organizations_l where FLOCALEID=2052 and FORGID in ('{0}');", OrgID);
            using (var rd = DBUtils.ExecuteReader(Context, sql))
            {
                while (rd.Read())
                {
                    var row = dt.NewRow();
                    row["FOrgID"] = rd["FORGID"];
                    row["FOrgName"] = rd["FNAME"];
                    dt.Rows.Add(row);
                }

                rd.Close();
            }

            return dt;

        }


        /// <summary>
        /// 设置汇总行，只有显示财务信息时才需要汇总
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override List<SummaryField> GetSummaryColumnInfo(IRptParams filter)
        {
            List<SummaryField> summaryList = new List<SummaryField>
            {
                new SummaryField("FSalQty", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM),
                new SummaryField("FAmount", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM)
            };
            return summaryList;
        }

        public override void CloseReport()
        {
            IDBService service = ServiceHelper.GetService<IDBService>();
            if (!ColumnTableNamesOld.IsNullOrEmptyOrWhiteSpace())
            {
                service.DeleteTemporaryTableName(base.Context, ColumnTableNamesOld);
            }
            base.CloseReport();
        }

        private void get_params(DynamicObject customFilter, out Dictionary<string, string> filters)
        {
            filters = new Dictionary<string, string>();
            //var FSaleOrgList = customFilter["SaleOrgList"].ToString();
            var FStartDate = customFilter["StartDate"].ToString();
            filters.Add("StartDate", FStartDate);
            var FEndDate = customFilter["EndDate"].ToString();
            filters.Add("EndDate", FEndDate);
            DynamicObject FSaleDeptFrom = customFilter["SaleDeptFrom"] as DynamicObject;
            if (FSaleDeptFrom != null) { filters.Add("SaleDeptFrom", FSaleDeptFrom["Number"].ToString()); }
            DynamicObject FSaleDeptTo = customFilter["SaleDeptTo"] as DynamicObject;
            if (FSaleDeptTo != null) { filters.Add("SaleDepTo", FSaleDeptTo["SaleDepTo"].ToString()); }
            DynamicObject FSalerFrom = customFilter["SalerFrom"] as DynamicObject;
            if (FSalerFrom != null) { filters.Add("SalerFrom", FSalerFrom["SalerFrom"].ToString()); }
            DynamicObject FSalerTo = customFilter["SalerTo"] as DynamicObject;
            if (FSalerTo != null) { filters.Add("SalerTo", FSalerTo["SalerTo"].ToString()); }
            DynamicObject FMaterialFrom = customFilter["MaterialFrom"] as DynamicObject;
            if (FMaterialFrom != null) { filters.Add("MaterialFrom", FMaterialFrom["MaterialFrom"].ToString()); }
            DynamicObject FMaterialTo = customFilter["MaterialTo"] as DynamicObject;
            if (FMaterialTo != null) { filters.Add("MaterialTo", FMaterialTo["MaterialTo"].ToString()); }
            DynamicObject FCustFrom = customFilter["CustFrom"] as DynamicObject;
            if (FCustFrom != null) { filters.Add("CustFrom", FCustFrom["CustFrom"].ToString()); }
            DynamicObject FCustTo = customFilter["CustTo"] as DynamicObject;
            if (FCustTo != null) { filters.Add("CustTo", FCustTo["CustTo"].ToString()); }
            string FDataSource = customFilter["DataSource"].ToString();
            filters.Add("DataSource", FDataSource);
            string FDocumentStatus = customFilter["DocumentStatus"].ToString();
            filters.Add("DocumentStatus", FDocumentStatus);
            string FIsIncludeGift = Convert.ToBoolean(customFilter["IsIncludeGift"]) ? "1" : "0";
            filters.Add("igift", FIsIncludeGift);

        }
        private string buildSqlstr(string tableFrom)
        {
            string selectStr = @"select *   from {0} where 1=1 {1}";
            StringBuilder whereStr = new StringBuilder();
            if (filters.ContainsKey("SaleDeptFrom")) { whereStr.Append(String.Format("and FDATE>='{0}' ", filters["SaleDeptFrom"])); }
            if (filters.ContainsKey("SaleDeptTo")) { whereStr.Append(String.Format("and FDATE<='{0}' ", filters["SaleDeptTo"])); }
            if (filters.ContainsKey("SalerFrom")) { whereStr.Append(String.Format("and FSALERNUMBER>='{0}' ", filters["SalerFrom"])); }
            if (filters.ContainsKey("SalerTo")) { whereStr.Append(String.Format("and FSALERNUMBER<='{0}' ", filters["SalerTo"])); }
            if (filters.ContainsKey("MaterialFrom")) { whereStr.Append(String.Format("and FMNUMBER>='{0}' ", filters["MaterialFrom"])); }
            if (filters.ContainsKey("MaterialTo")) { whereStr.Append(String.Format("and FMNUMBER<='{0}' ", filters["MaterialTo"])); }
            if (filters.ContainsKey("CustFrom")) { whereStr.Append(String.Format("and FCUSTNUMBER>='{0}' ", filters["CustFrom"])); }
            if (filters.ContainsKey("CustTo")) { whereStr.Append(String.Format("and FCUSTNUMBER<='{0}' ", filters["CustTo"])); }
            if (filters["DocumentStatus"] == "A") { whereStr.Append("and FDOCUMENTSTATUS='C' "); }
            else if (filters["DocumentStatus"] == "B") { whereStr.Append("and FDOCUMENTSTATUS<>'C' "); }
            if (filters["igift"] == "0") { whereStr.Append("and FISFREE='0' "); }
            return String.Format(selectStr, tableFrom, whereStr.ToString());
        }


    }
}
