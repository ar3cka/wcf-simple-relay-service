using System;
using System.ServiceModel;
using System.Threading;
using ServiceContracts;

namespace ServiceServer
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false)]
	public class CustomerService : ICustomerService
	{
		readonly Random m_random = new Random();
		public void UpdateCustomerName(int customerId, string customerName)
		{
			Thread.Sleep(TimeSpan.FromSeconds(m_random.Next(0, 5)));
			Console.WriteLine("ThreadID: {0}. ICustomerService.Update operation called.", Thread.CurrentThread.ManagedThreadId);
		}
	}
}