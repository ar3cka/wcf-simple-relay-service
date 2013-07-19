using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using ServiceContracts;

namespace ServiceServer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Starting ICalculatorService host...");
			var calculator = CreateHost(typeof (CalculatorService), typeof (ICalculatorService), "http://localhost:8989/services/calculator");
			var customer = CreateHost(typeof (CustomerService), typeof (ICustomerService), "http://localhost:9898/services/customer");
		
			var hostOpened = false;
			try
			{
				calculator.Open();
				customer.Open();
				hostOpened = true;
				Console.WriteLine("CalculatorService started.");
				Console.WriteLine("Press any key to stop services...");
				Console.WriteLine();
				Console.ReadKey();
			}
			finally 
			{
				if (hostOpened)
				{
					calculator.Close();
					customer.Close();
				}
			}
		}

		private static ServiceHost CreateHost(Type hostType, Type serviceType, string uri)
		{
			var host = new ServiceHost(hostType);
			host.AddServiceEndpoint(serviceType, new BasicHttpBinding(BasicHttpSecurityMode.None), uri);

			var metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>() ?? new ServiceMetadataBehavior();
			metadataBehavior.HttpGetEnabled = true;
			metadataBehavior.HttpGetUrl = new Uri(uri);
			host.Description.Behaviors.Add(metadataBehavior);

			var serviceThrottlingBehavior = host.Description.Behaviors.Find<ServiceThrottlingBehavior>() ?? new ServiceThrottlingBehavior();
			serviceThrottlingBehavior.MaxConcurrentCalls = 100;
			serviceThrottlingBehavior.MaxConcurrentInstances = 100;
			serviceThrottlingBehavior.MaxConcurrentSessions = 100;
			host.Description.Behaviors.Add(serviceThrottlingBehavior);
			return host;
		}
	}
}
