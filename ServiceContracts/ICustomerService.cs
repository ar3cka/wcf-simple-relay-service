using System.ServiceModel;

namespace ServiceContracts
{
	[ServiceContract(Namespace = "http://schemas.sitels.ru/services/")]
	public interface ICustomerService
	{
		[OperationContract]
		void UpdateCustomerName(int customerId, string customerName);
	}
}