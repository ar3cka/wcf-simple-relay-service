using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace RelayServer
{
	public sealed class RelayServiceHost : ServiceHostBase
	{
		private readonly Uri m_listeningUri;
		private readonly Uri m_routingUri;

		public RelayServiceHost(string listeningUri, string routingUri)
		{
			m_listeningUri = new Uri(listeningUri);
			m_routingUri = new Uri(routingUri);
			InitializeDescription(new UriSchemeKeyedCollection());
		}

		protected override ServiceDescription CreateDescription(out IDictionary<string, ContractDescription> implementedContracts)
		{
			implementedContracts = null;
			return null;
		}

		protected override void InitializeRuntime()
		{
			var binding = new BasicHttpBinding();
			var listener = binding.BuildChannelListener<IReplyChannel>(m_listeningUri, new BindingParameterCollection());
			ChannelDispatchers.Add(new RelayServiceChannelDispatcher(m_routingUri, binding, listener));
			
		}

		protected override void ApplyConfiguration()
		{
		}
	}


}