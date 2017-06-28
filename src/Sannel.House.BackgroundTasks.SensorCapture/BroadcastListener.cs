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
					entry.Value2 = (double.IsNaN(packet.Values[1])) ? (double?)null : packet.Values[1];
					entry.Value3 = (double.IsNaN(packet.Values[2])) ? (double?)null : packet.Values[2];
					entry.Value4 = (double.IsNaN(packet.Values[3])) ? (double?)null : packet.Values[3];
					entry.Value5 = (double.IsNaN(packet.Values[4])) ? (double?)null : packet.Values[4];
					entry.Value6 = (double.IsNaN(packet.Values[5])) ? (double?)null : packet.Values[5];
					entry.Value7 = (double.IsNaN(packet.Values[6])) ? (double?)null : packet.Values[6];
					entry.Value8 = (double.IsNaN(packet.Values[7])) ? (double?)null : packet.Values[7];
					entry.Value9 = (double.IsNaN(packet.Values[8])) ? (double?)null : packet.Values[8];

					context.SensorEntries.Add(entry);
					context.SaveChanges();
				}
			}
		}
	}
}
 