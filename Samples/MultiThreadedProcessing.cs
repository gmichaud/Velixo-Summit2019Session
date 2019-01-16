using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Common.Parser;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;


namespace Velixo.Summit2019Samples
{
    /* 
     * This extension demonstrates the new parallel processing features available in 2018R1+
     * To keep code simple i'm extending a parallel-friendly processing screen and simply adding the necessary options

     * web.config contains the following keys, related to parallel processing:
     *  ParallelProcessingDisabled      (default: TRUE)
     *  ParallelProcessingMaxThreads    (default: Environment.ProcessorCount);
     *  ParallelProcessingBatchSize     (default: 10);
     *  
     *  AsyncNumbering also needs to be turned on. To summarize, the following changes need to be applied:
     *  <add key="ParallelProcessingDisabled" value="false" />
     *  <add key="AsyncNumbering" value= "true" />
     *  
     *  Only specific processing delegates support parallel processing. This one works:
     *  public virtual void SetProcessDelegate<Graph>(ProcessItemDelegate<Graph> handler, FinallyProcessDelegate<Graph> handlerFinally)
    */
    public class INIntegrityCheckExt : PXGraphExtension<INIntegrityCheck>
    {
        protected virtual void INSiteFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            //Only thing that needs to be setup beside that are web.config settings (see notes above)
            Base.INItemList.ParallelProcessingOptions = settings => 
            {
                settings.IsEnabled = true;
                settings.BatchSize = 5;
            };
        }
    }
}
