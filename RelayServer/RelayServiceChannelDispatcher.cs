using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Threading;


namespace RelayServer
{
	internal sealed class RelayServiceChannelDispatcher : ChannelDispatcherBase
	{
		private readonly Uri m_routingUri;
		private readonly Binding m_binding;
		private readonly IChannelListener<IReplyChannel> m_listener;
		private IReplyChannel m_replyChannel;
		private ServiceHostBase m_host;
		private IChannelFactory<IRequestChannel> m_requestChannelFactory;


		public RelayServiceChannelDispatcher(Uri routingUri, Binding binding, IChannelListener<IReplyChannel> listener)
		{
			m_routingUri = routingUri;
			m_binding = binding;
			m_listener = listener;
		}

		protected override void Attach(ServiceHostBase host)
		{
			m_host = host;
		}

		protected override void OnAbort()
		{
			m_listener.Abort();
		}

		protected override void OnClose(TimeSpan timeout)
		{
			m_listener.Close(timeout);
		}

		protected override void OnEndClose(IAsyncResult result)
		{
			m_listener.EndClose(result);
		}

		protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return m_listener.BeginClose(timeout, callback, state);
		}

		protected override void OnOpen(TimeSpan timeout)
		{
			m_listener.Open(timeout);
		}

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return m_listener.BeginOpen(timeout, callback, state);
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			m_listener.EndOpen(result);
		}

		protected override void OnOpened()
		{
			base.OnOpened();
			ThreadPool.QueueUserWorkItem(AcceptCallback, m_listener);
		}

		protected override void OnClosed()
		{
			base.OnClosed();
			m_replyChannel.Close();
			m_requestChannelFactory.Close();
		}

		private void AcceptCallback(object state)
		{
			var listener = (IChannelListener<IReplyChannel>)state;
			m_replyChannel = listener.AcceptChannel(TimeSpan.MaxValue);
			m_requestChannelFactory = m_binding.BuildChannelFactory<IRequestChannel>();
			m_requestChannelFactory.Open();

			if (m_replyChannel != null)
			{
				m_replyChannel.Open();
				StartProcessingRequests(m_replyChannel);
			}
			else
			{
				listener.Close();
			}
		}

		private void StartProcessingRequests(IReplyChannel channel)
		{
			while (true)
			{
				var requestContext = channel.ReceiveRequest(TimeSpan.MaxValue);
				Console.WriteLine("Thread ID: {0}. Message {1} received.", Thread.CurrentThread.ManagedThreadId, requestContext.RequestMessage.Headers.Action);

				if (requestContext == null) break;

				new MessageHandler(
					requestContext,
					() => m_requestChannelFactory.CreateChannel(new EndpointAddress(m_routingUri))).Handle();
			}
		}

		protected override TimeSpan DefaultCloseTimeout
		{
			get { return TimeSpan.FromMinutes(1); }
		}

		protected override TimeSpan DefaultOpenTimeout
		{
			get { return TimeSpan.FromMinutes(1); }
		}

		public override ServiceHostBase Host
		{
			get { return m_host; }
		}

		public override IChannelListener Listener
		{
			get { return m_listener; }
		}
	}
}