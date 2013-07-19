using System.ServiceModel;

namespace ServiceContracts
{
	[ServiceContract(Namespace = "http://schemas.sitels.ru/services/")]
    public interface ICalculatorService
	{
		[OperationContract]
		int Add(int a, int b);
	}
}
