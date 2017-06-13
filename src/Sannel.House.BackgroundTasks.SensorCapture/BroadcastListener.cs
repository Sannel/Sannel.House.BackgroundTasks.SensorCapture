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
using Sannel.House.Sensor;

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
					var bits = new byte[80];
					s.Read(bits, 0, bits.Length);

					var packet = new SensorPacket();
					packet.Fill(bits);

					var entry = new SensorEntry();
					entry.DeviceId = packet.DeviceId;
					entry.SensorType = packet.SensorType;
					entry.LocalId = Guid.NewGuid();
					entry.CreatedDate = DateTime.UtcNow;
					entry.Value = packet.Values[0];
					entry.Value2 = packet.Values[1];
					entry.Value3 = packet.Values[2];
					entry.Value4 = packet.Values[3];
					entry.Value5 = packet.Values[4];
					entry.Value6 = packet.Values[5];
					entry.Value7 = packet.Values[6];
					entry.Value8 = packet.Values[7];
					entry.Value9 = packet.Values[8];

					context.SensorEntries.Add(entry);
					context.SaveChanges();

					Debug.WriteLine(entry);
				}
			}
		}
	}
}
