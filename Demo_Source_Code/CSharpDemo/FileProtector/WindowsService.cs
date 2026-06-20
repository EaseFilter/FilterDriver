using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FileProtector
{
    partial class WindowsService : ServiceBase
    {
        ProtectorForm protector = new ProtectorForm();
        public WindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            protector.toolStripButton_StartFilter_Click(null, null);
        }

        protected override void OnStop()
        {
            protector.toolStripButton_Stop_Click(null, null);
        }
    }
}
