using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using ServiceContracts;

namespace ServiceClient
{
	class Program
	{
		private const string s_calculatorServiceUri = "http://localhost:8989/services/calculator";
		private const string s_customerServiceUri = "http://localhost:9898/services/customer";

//		private static string s_calculatorServiceUri = "http://localhost:8080/relayservice/calculator";
//		private static string s_customerServiceUri = "http://localhost:8080/relayservice/customer";


		static void Main(string[] args)
		{
//			var factory1 = new ChannelFactory<ICalculatorService>(
//				new BasicHttpBinding(), 
//				new EndpointAddress("http://localhost:8080/relayservice/calculator"));

//			var factory2 = new ChannelFactory<ICustomerService>(
//				new BasicHttpBinding(),
//				new EndpointAddress("http://localhost:8080/relayservice/customer"));
			
			DoTest().Wait();
		}

		public async static Task DoTest()
		{
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			var t1 = CallCalculator();
			var t2 = CallCustomer();
			Task.WaitAll(t1, t2);
			stopwatch.Stop();
			Console.WriteLine("Process completed for {0}:", stopwatch.ElapsedMilliseconds);
		}

		private static Task CallCalculator()
		{
			var f = new ChannelFactory<CalculatorServices.ICalculatorService>(
				new BasicHttpBinding(),
				new EndpointAddress(s_calculatorServiceUri));

			var tasks = new List<Task>();
			for (int i = 0; i < 10; i++)
			{
				var service = f.CreateChannel();
				tasks.Add(service.AddAsync(i, i + 2)
					.ContinueWith((t) => Console.WriteLine(
						"ThreadID: {0}: ICalculatorService::Add operation call completed. Result {1}", 
						Thread.CurrentThread.ManagedThreadId,  t.Result)));
				
			}

			return Task.WhenAll(tasks);
		}

		private static Task CallCustomer()
		{
			var f = new ChannelFactory<CustomerServices.ICustomerService>(
				new BasicHttpBinding(),
				new EndpointAddress(s_customerServiceUri));

			var tasks = new List<Task>();
			for (int i = 0; i < 10; i++)
			{
				var service = f.CreateChannel();
				tasks.Add(service.UpdateCustomerNameAsync(i, "Name_" + i)
					.ContinueWith(t => Console.WriteLine("ThreadID: {0}: ICustomer::UpdateCustomerName", Thread.CurrentThread.ManagedThreadId)));
			}

			return Task.WhenAll(tasks);
		}

	}
}
