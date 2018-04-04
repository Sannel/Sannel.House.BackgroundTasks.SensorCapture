using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sannel.House.Sensor;
using Sannel.House.SensorCapture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	internal class Startup
	{
		private static Startup current = null;
		internal static Lazy<Startup> Instance = new Lazy<Startup>(() => current ?? (current = new Startup()));

		internal ServiceProvider Provider
		{
			get;
			private set;
		}

		internal Startup()
		{
			MobileCenter.Start("", typeof(Analytics));
			Analytics.TrackEvent("Background Task Started");

			var serviceCollection = new ServiceCollection();

			serviceCollection.AddLogging();
			serviceCollection.AddDbContext<DataContext>(options => options.UseSqlite("Data Source=SensorEntries.db"));
			serviceCollection.AddSingleton<Configuration.ConfigurationConnection>();
			serviceCollection.AddSingleton<TCPSensorPacketListener>();
			serviceCollection.AddSingleton<ReadingsManager>();

			Provider = serviceCollection.BuildServiceProvider();
		}

		internal async Task InitAsync()
		{

		}
	}
}
