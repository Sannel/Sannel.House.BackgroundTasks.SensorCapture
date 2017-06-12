using Sannel.House.SensorCapture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using System.IO;
using System.Diagnostics;
using Sannel.House.SensorCapture.Data.Models;

namespace Sannel.House.BackgroundTasks.SensorCapture
{
	internal class BroadcastListener : IDisposable
	{
		private DataContext context;
		private DatagramSocket listenerSocket;

		public BroadcastListener(DataContext context)
		{
			this.context = context;
			listenerSocket = new DatagramSocket();
			listenerSocket.MessageReceived += listenerSocket_MessageReceived;
		}

		public async void Begin()
		{
			await listenerSocket.BindServiceNameAsync("8172");
		}

		public void Dispose()
		{
			listenerSocket?.Dispose();
		}

		private void listenerSocket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{ 
			using(var stream = args.GetDataStream())
			{
				using(var s = stream.AsStreamForRead())
				{
					var bits = new byte[48];
					s.Read(bits, 0, bits.Length);

					var entry = new SensorEntry();
					entry.DeviceId = BitConverter.ToInt32(bits, 0);
					entry.SensorType = (SensorType)BitConverter.ToInt32(bits, 4);
					entry.Value = BitConverter.ToDouble(bits, 8);

					Debug.WriteLine(entry);
				}
			}
		}
	}
}
