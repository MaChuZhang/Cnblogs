using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Umeng.Socialize
{
    public partial class UMShareAPI
    {
        public partial class InitThread : global::Com.Umeng.Socialize.Common.QueuedWork.UMAsyncTask
        {
            protected override Java.Lang.Object DoInBackground()
            {
                return DoInBackgrounds();
            }
        }
    }
}
