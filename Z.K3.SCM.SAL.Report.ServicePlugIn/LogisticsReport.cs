using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.Contracts.Report;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Report;
using System.ComponentModel;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Util;
using Kingdee.BOS.Core.Report.PlugIn.Args;

namespace Z.K3.SCM.SAL.Report.ServicePlugIn
{
    [Kingdee.BOS.Util.HotUpdate]
    [Description("出货报关资料报表")]
    public class LogisticsReport : SysReportBaseService
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // 简单账表类型：基本简单账表
            ReportProperty.ReportType = ReportType.REPORTTYPE_NORMAL;
            // 标识报表支持分组汇总
            ReportProperty.IsGroupSummary = true;
            // 标识报表的列通过插件定义
            ReportProperty.IsUIDesignerColumns = true;
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FMATERIALID", "FMATNUMBER");
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FCUSTOMERID", "FCUSTNUMBER");
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FSALESMANID", "FSALERNUMBER");
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FSETTLECURRID", "FSETTLECURRNAME");
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FBOMID", "FBOMNUMBER");
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FUNITID", "FUNITNAME");
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FPRICEUNITID", "FPRICEUNITNAME");
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FRCURRENCYID", "FRCURRENCYNAME");
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FRUNITID", "FRUNITNAME");

        }

        /// <summary>
        /// 向报表临时表，插入报表数据
        /// </summary>
        /// <param name="filter">过滤信息</param>
        /// <param name="tableName">临时表名</param>
        public override void BuilderReportSqlAndTempTable(IRptParams filter, string tableName)
        {
            base.BuilderReportSqlAndTempTable(filter, tableName);
            //默认排序字段：需要从filter中取用户设置的排序字段
            //KSQL_SEQ: ROW_NUMBER() OVER(ORDER BY  {0} ) FIDENTITYID
            if (filter.FilterParameter.SortString.IsNullOrEmptyOrWhiteSpace())
            {
                KSQL_SEQ = string.Format(KSQL_SEQ, " t1.FBILLNO desc");
            }
            else
            {
                KSQL_SEQ = string.Format(KSQL_SEQ, filter.FilterParameter.SortString);
            }
            string sql = string.Format(
                @"/*dialect*/select  *, 
                {0}
                INTO {1}
                FROM (select e.*,lgt.FBILLNO FRBILLNO,lgt.FSEQ FRSEQ, lgt.FRDATE,lgt.FCONTRACTNO,lgt.FTRACKINGNO,lgt.FDECLARATIONNO,lgt.FPRTNAME,
                lgt.FRUNITID,lgt.FRUNITNAME,lgt.FRQTY,lgt.FRCURRENCYNAME,lgt.FRPRICE,lgt.FRAMOUNT,lgt.FRTAMOUNT,lgt.FRCUSTNAME,
                lgt.FINSURE,lgt.FBANKNO,lgt.FBANKNAME,lgt.FTRMODE,lgt.FTRTYPE,lgt.FREMARK,lgt.FDELIVERYNOTICE,lgt.FRCURRENCYID,lgt.FRCURRSYSMBOL
                from V_QNWB_OUTSTOCKENTITY e
                full join V_QNWB_LOGISTICS lgt on e.FSID=lgt.FDELIVERYNOTICE and e.FSTABLENAME='T_SAL_DELIVERYNOTICEENTRY') t1
                WHERE   1 = 1 ", KSQL_SEQ, tableName);

            //添加条件过滤
            string conditionStr = filter.FilterParameter.FilterString;
            if (!conditionStr.IsNullOrEmptyOrWhiteSpace())
            {
                sql += string.Format("AND {0}", conditionStr);
            }

            DBUtils.Execute(Context, sql);
        }
        public void OnFormatRowConditions(ReportFormatConditionArgs args)
        {

        }
        public void FormatCellValue(FormatCellValueArgs args)
        {
            var h = args.Header;
        }
        /// <summary>
        /// 设置报表标题
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override ReportTitles GetReportTitles(IRptParams filter)
        {
            var result = base.GetReportTitles(filter);
            if (result == null)
            {
                result = new ReportTitles();
            }
            return result;
        }
        public override ReportHeader GetReportHeadersForDesigner(IRptParams filter)
        {
            return base.GetReportHeadersForDesigner(filter);
        }
        /// <summary>
        /// 设置报表合计列
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override List<SummaryField> GetSummaryColumnInfo(IRptParams filter)
        {
            var result = base.GetSummaryColumnInfo(filter);
            result.Add(new SummaryField("FREALQTY", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FPRICEUNITQTY", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FAMOUNT", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FAMOUNT_LC", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FALLAMOUNT", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FALLAMOUNT_LC", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FDISCOUNT", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FCOSTAMOUNT", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FCOSTAMOUNT_LC", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FRQTY", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FRAMOUNT", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FRTAMOUNT", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            return result;
        }
    }
}
