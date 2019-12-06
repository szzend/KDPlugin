using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.K3.MFG.SUB.App.ServicePlugIn;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace Z.K3.MFG.SUB.ServicePlugIn
{
    [Kingdee.BOS.Util.HotUpdate]
    [Description("移除行拆分后携带的委外供应商")]
    public class RemoveSupplierIdPlugIn : BaseSUBOperationPlugIn
    {

        /// <summary>
        /// 操作开始前功能处理
        /// </summary>
        /// <param name="e"></param>
        public override void BeginOperationTransaction(BeginOperationTransactionArgs e)
        {
            base.BeginOperationTransaction(e);
            UpdateSupplierId(e.DataEntitys.ToList<DynamicObject>());
        }
    private void UpdateSupplierId(List<DynamicObject> subDataList)
        {
            if (subDataList == null || subDataList.Count <= 0)
                return;
            foreach(DynamicObject subData in subDataList)
            {
                
                DynamicObjectCollection entryDatas = subData["TreeEntity"] as DynamicObjectCollection;
                var datas = entryDatas.Where(d => d["CreateType"].ToString() == "6" && d["PurOrderId"].ToString()=="0").ToList<DynamicObject>();
                foreach(var d in datas)
                {
                    d["SupplierId_Id"] = 0;
                    d["SupplierId"] = null;
                }

                //DynamicObjectCollection entryDatas=subData.GetDynamicValue<DynamicObjectCollection>(CONST_SUB_SUBREQORDER.CONST_FTreeEntity.)
            }
        }


    }
}
