using System;
using System.ServiceModel;
using System.Threading;
using ServiceContracts;

namespace ServiceServer
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false)]
	public class CalculatorService : ICalculatorService
	{
		readonly Random m_random = new Random();

		public int Add(int a, int b)
		{
			Thread.Sleep(TimeSpan.FromSeconds(m_random.Next(0, 5)));
			Console.WriteLine("ThreadID: {0}. ICalculatorService.Add operation called.", Thread.CurrentThread.ManagedThreadId);
			return a + b;
		}
	}
}