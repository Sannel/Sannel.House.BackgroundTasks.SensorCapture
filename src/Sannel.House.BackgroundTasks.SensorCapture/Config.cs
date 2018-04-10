using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	internal class Config
	{
		internal Uri ServiceApiUrl { get; set; }

		internal string AppSecret { get; set; }
		internal string Username { get; set; }
		internal string Password { get; set; }
		internal uint Port { get; set; }
	}
}
