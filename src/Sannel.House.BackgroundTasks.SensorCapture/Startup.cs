using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sannel.House.Configuration;
using Sannel.House.Sensor;
using Sannel.House.SensorCapture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	internal class Startup
	{
		private static Startup current = null;
		private static Config Config { get; }
		private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

		internal ServiceProvider Provider
		{
			get;
			private set;
		}

		internal static async Task<Startup> GetStartupAsync()
		{
			await semaphoreSlim.WaitAsync();
			try
			{
				if (current != null)
				{
					return current;
				}

				current = new Startup();
				await current.InitAsync();
				return current;
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		private Startup()
		{

		}

		internal async Task InitAsync()
		{
			var serviceCollection = new ServiceCollection();
			var config = new Config();

			serviceCollection.AddLogging();
			serviceCollection.AddDbContext<DataContext>(options => options.UseSqlite("Data Source=SensorEntries.db"));
			serviceCollection.AddSingleton<TCPSensorPacketListener>();
			serviceCollection.AddSingleton<ReadingsManager>();
			serviceCollection.AddSingleton(config);

			Provider = serviceCollection.BuildServiceProvider();

#if DEBUG
			uint port = Defaults.Development.SENSOR_BROADCAST_PORT;
#else
			uint port = Defaults.Production.SENSOR_BROADCAST_PORT;
#endif

			using (var con = new ConfigurationConnection())
			{
				if(await con.ConnectAsync() == Windows.ApplicationModel.AppService.AppServiceConnectionStatus.Success)
				{
					var result = await con.GetConfiguration("SensorCaptureAppSecret",
															"ServerApiUrl", 
															"ServerUsername", 
															"ServerPassword", 
															"SensorsPort");
					Uri.TryCreate(result["ServerApiUrl"] as string, UriKind.Absolute, out var tmp);
					config.ServiceApiUrl = tmp;
					config.AppSecret = result["SensorCaptureAppSecret"] as string;
					config.Username = result["ServerUsername"] as string;
					config.Password = result["ServerPassword"] as string ?? "";
					config.Port = (uint)(result["SensorsPort"] as int? ?? (int)port);
				}
			}
			AppCenter.Start(config.AppSecret, typeof(Analytics));
			Analytics.TrackEvent("Background Task Started");
		}
	}
}
