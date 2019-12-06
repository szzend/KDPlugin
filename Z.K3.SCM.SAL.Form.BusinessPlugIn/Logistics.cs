using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingdee.BOS;
using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Core.SqlBuilder;
using Kingdee.BOS.Core.Metadata;
using System.ComponentModel;
using Kingdee.BOS.ServiceHelper;

namespace Z.K3.SCM.SAL.Form.BusinessPlugIn
{
    [Kingdee.BOS.Util.HotUpdate]
    [Description("出货报关资料表单插件")]
    public class Logistics : AbstractBillPlugIn
    {
        /// <summary>
        /// 初始化，对其他界面传来的参数进行处理，对控件某些属性进行处理
        /// 这里不宜对数据DataModel进行处理
        /// </summary>
        /// <param name="e"></param>
        public override void OnInitialize(InitializeEventArgs e)
        {

        }

        /// <summary>
        /// 新建单据加载数据完成之后，需要处理的功能
        /// </summary>
        /// <param name="e"></param>
        public override void AfterCreateNewData(EventArgs e)
        {

        }

        /// <summary>
        /// 修改，查看单据加载已有数据之后，需要处理的功能
        /// </summary>
        /// <param name="e"></param>
        public override void AfterLoadData(EventArgs e)
        {   /*
            int count= this.Model.GetEntryRowCount("FEntity");
            for(int i=0; i< count; i++) {
                if (this.Model.GetValue("FDeliveryNotice",i) is DynamicObject notice)
                {
                    DynamicObject cust_no = notice["CustId"] as DynamicObject;
                    this.Model.SetValue("FCustNo", cust_no["Number"].ToString(),i);
                    DynamicObject mat_no = notice["MaterialID"] as DynamicObject;
                    this.Model.SetValue("FMatNo", mat_no["Number"].ToString(),i);
                    DynamicObject currency = notice["CurrencyId"] as DynamicObject;
                    this.Model.SetItemValueByNumber("FOrderCurrency", currency["Number"].ToString(), i);
                    this.Model.SetValue("FOrderPrice", notice["TaxPrice"],i);

                }

            }
            */


        }

        /// <summary>
        /// 数据加载之后，需要处理的功能，这里主要对界面样式进行处理，尽量不要对Datamodel进行处理
        /// </summary>
        /// <param name="e"></param>
        public override void AfterBindData(EventArgs e)
        {

        }

        /// <summary>
        /// 在根据编码检索数据之前调用；
        /// 通过重载本事件，可以设置必要的过滤条件，以限定检索范围；
        /// 还可以控制当前过滤是否启用组织隔离，数据状态隔离
        /// </summary>
        /// <param name="e"></param>
        public override void BeforeSetItemValueByNumber(BeforeSetItemValueByNumberArgs e)
        {
            switch (e.BaseDataField.Key.ToUpperInvariant())
            {
                //case "FXXX":通过字段的Key[大写]来区分不同的基础资料
                //e.Filter = "FXXX= AND fxxy=";过滤的字段使用对应基础资料的字段的Key，支持ksql语法
                //break;
                case "":
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 显示基础资料列表之前调用
        /// 通过重载本事件，可以设置必要的过滤条件，以限定检索范围；
        /// </summary>
        /// <param name="e"></param>
        public override void BeforeF7Select(BeforeF7SelectEventArgs e)
        {
            switch (e.FieldKey.ToUpperInvariant())
            {
                //case "FXXX":通过字段的Key[大写]来区分不同的基础资料
                //    e.ListFilterParameter.Filter = "FXXX= AND fxxy=";过滤的字段使用对应基础资料的字段的Key，支持ksql语法
                //break;
                case "":
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 界面数据发生变化之前，需要处理的功能
        /// </summary>
        /// <param name="e"></param>
        public override void BeforeUpdateValue(BeforeUpdateValueEventArgs e)
        {
            switch (e.Key.ToUpperInvariant())
            {
                //case "FXXX":通过字段的Key[大写]来区分不同的控件的数据变化功能，如果要阻止该次变化，可以用e.Cancel = true;
                //    e.Cancel = true;
                //    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 界面数据发生变化之后，需要处理的功能
        /// </summary>
        /// <param name="e"></param>
        public override void DataChanged(DataChangedEventArgs e)
        {
            switch (e.Field.Key.ToUpperInvariant())
            {
                case "FDELIVERYNOTICE":
                    if (this.Model.GetValue("FDeliveryNotice", e.Row) is DynamicObject notice)
                    {
                        DynamicObject cust_no = notice["CustId"] as DynamicObject;
                        this.Model.SetValue("FCustNo", cust_no["Number"].ToString(), e.Row);
                        DynamicObject mat_no = notice["MaterialID"] as DynamicObject;
                        this.Model.SetValue("FMatNo", mat_no["Number"].ToString(), e.Row);
                        DynamicObject currency = notice["CurrencyId"] as DynamicObject;

                        this.Model.SetItemValueByNumber("FOrderCurrency", currency["Number"].ToString(), e.Row);
                        this.Model.SetItemValueByNumber("FCurrencyId", currency["Number"].ToString(), e.Row);
                        this.Model.SetValue("FOrderPrice", notice["TaxPrice"], e.Row);
                        DynamicObjectCollection material = DBServiceHelper.ExecuteDynamicObject(this.Context, String.Format("select  FBASEUNITID from t_bd_materialbase where FMATERIALID={0}", mat_no["Id"]));
                        this.Model.SetItemValueByID("FUnitID", material[0]["FBASEUNITID"], e.Row);
                    }
                    else
                    {
                        this.Model.SetValue("FCustNo", null, e.Row);
                        this.Model.SetValue("FMatNo", null, e.Row);
                        this.Model.SetValue("FOrderCurrency", null, e.Row);
                        this.Model.SetValue("FCurrencyId", null, e.Row);
                        this.Model.SetValue("FOrderPrice", null, e.Row);
                        this.Model.SetValue("FUnitID", null, e.Row);

                    }
                    break;
                case "FPRICE":
                case "FQTY":
                    decimal price, qty;
                    Decimal.TryParse(this.Model.GetValue("FPRICE", e.Row).ToString(), out price);
                    Decimal.TryParse(this.Model.GetValue("FQTY", e.Row).ToString(), out qty);
                    this.Model.SetValue("FAmount", price * qty, e.Row);
                    break;
                case "FCurrencyId":
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// 单据持有事件发生前需要完成的功能
        /// </summary>
        /// <param name="e"></param>
        public override void BeforeDoOperation(BeforeDoOperationEventArgs e)
        {
            switch (e.Operation.FormOperation.Operation.ToUpperInvariant())
            {
                //case "SAVE": 表单定义的事件都可以在这里执行，需要通过事件的代码[大写]区分不同事件
                //break;
                case "":
                    break;
                default:
                    break;
            }


        }

        /// <summary>
        /// 单据持有事件发生后需要完成的功能
        /// </summary>
        /// <param name="e"></param>
        public override void AfterDoOperation(AfterDoOperationEventArgs e)
        {
            switch (e.Operation.Operation.ToUpperInvariant())
            {
                //case "SAVE": 表单定义的事件都可以在这里执行，需要通过事件的代码[大写]区分不同事件
                //break;
                case "":
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// queryservice取数方案，通过业务对象来获取数据，推荐使用
        /// </summary>
        /// <returns></returns>
        public DynamicObjectCollection GetQueryDatas()
        {
            QueryBuilderParemeter paramCatalog = new QueryBuilderParemeter()
            {
                FormId = "",//取数的业务对象
                FilterClauseWihtKey = "",//过滤条件，通过业务对象的字段Key拼装过滤条件
                SelectItems = SelectorItemInfo.CreateItems("", "", ""),//要筛选的字段【业务对象的字段Key】，可以多个，如果要取主键，使用主键名
            };

            DynamicObjectCollection dyDatas = Kingdee.BOS.ServiceHelper.QueryServiceHelper.GetDynamicObjectCollection(this.Context, paramCatalog);
            return dyDatas;
        }
    }


}
