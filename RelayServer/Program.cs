using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RelayServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var service1 = new RelayServiceHost("http://localhost:8080/relayservice/calculator", "http://localhost:8989/services/calculator");
			service1.Open();

			var service2 = new RelayServiceHost("http://localhost:8080/relayservice/customer", "http://localhost:9898/services/customer");
			service2.Open();


			try
			{
				Console.WriteLine("Press any key to RelayService stop...");
				Console.ReadKey();

				service1.Close();
				service2.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		public static Message ForwardMessage(Message requestMessage)
		{
			var binding = CreateBinding();

			//Step2: Use the binding to build the channel factory.
			var factory = binding.BuildChannelFactory<IRequestChannel>(new BindingParameterCollection());
			
			//Open the channel factory.
			factory.Open();

			//Step3: Use the channel factory to create a channel.
			var channel = factory.CreateChannel(new EndpointAddress("http://localhost:8989/services/calculator"));
			channel.Open();
			
			//Send message.
			var replyMessage = channel.Request(requestMessage);
			Console.WriteLine("Reply message received");
			Console.WriteLine("Reply action: {0}", replyMessage.Headers.Action);
			
			//Do not forget to close the channel.
			channel.Close();
			//Do not forget to close the factory.
			factory.Close();
			
			return replyMessage;
		}

		private static CustomBinding CreateBinding()
		{
			var textEncoding = new TextMessageEncodingBindingElement();
			textEncoding.MessageVersion = MessageVersion.Soap11;
			
			var httpTransport = new HttpTransportBindingElement();
			var binding = new CustomBinding(new BindingElement[]
			{
				textEncoding, httpTransport
			});
			
			return binding;
		}
	}
}
