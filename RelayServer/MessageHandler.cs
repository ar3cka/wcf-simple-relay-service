using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace RelayServer
{
	public class MessageHandler
	{
		private readonly Func<IRequestChannel> m_createRequestChannel;
		private readonly RequestContext m_requestContext;
		private IRequestChannel m_requestChannel;
		private Message m_replyMessage;

		public MessageHandler(RequestContext requestContext, Func<IRequestChannel> createRequestChannel)
		{
			m_requestContext = requestContext;
			m_createRequestChannel = createRequestChannel;
		}
	

		public void Handle()
		{
			m_requestChannel = m_createRequestChannel();
			m_requestChannel.Open();
			Console.WriteLine("ThreadID: {0}. Begin forward request.", Thread.CurrentThread.ManagedThreadId);
			m_requestChannel.BeginRequest(m_requestContext.RequestMessage, OnForwardRequestCompleted, null);
		}

		private void OnForwardRequestCompleted(IAsyncResult state)
		{
			try
			{
				Console.WriteLine("ThreadID: {0}. Sending reply back.", Thread.CurrentThread.ManagedThreadId);
				m_replyMessage = m_requestChannel.EndRequest(state);
				m_requestContext.BeginReply(m_replyMessage, OnReplyCompleted, null);
			}
			catch (CommunicationException)
			{
				m_requestContext.Close();
				m_requestChannel.Abort();
			}
			finally
			{
				m_requestChannel.Close();
			}
		}

		private void OnReplyCompleted(IAsyncResult state)
		{
			Console.WriteLine("ThreadID: {0}. Reply sended.", Thread.CurrentThread.ManagedThreadId);
			m_replyMessage.Close();
			m_requestContext.EndReply(state);
			m_requestContext.Close();
		}
	}
}