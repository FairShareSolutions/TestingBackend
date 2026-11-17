using Amazon.Textract;
using FairShareSolutions_PYMT.Model.IRepository;
using FairShareSolutions_PYMT.Model.IRepository.IAnonymousGroupRepository;
using FairShareSolutions_PYMT.Model.IRepository.IDashboardRepository;
using FairShareSolutions_PYMT.Model.Repository;
using FairShareSolutions_PYMT.Model.Repository.AnonymousGroupRepository;
using FairShareSolutions_PYMT.Model.Repository.DashboardRepository;
using FairShareSolutions_PYMT.Repository.IRepository;
using FairShareSolutions_PYMT.Repository.Repository;
using FairShareSolutions_PYMT.Services.IServices;
using FairShareSolutions_PYMT.Services.Services;

namespace FairShareSolutions_PYMT.API.CustomDependencyInjection
{
	public static class DependencyInjection
	{
		public static void CustomDIService(this IServiceCollection services)
		{

			services.AddScoped<IAccountService, AccountServices>();
			services.AddScoped<IAccountRepository, AccountRepository>();
			services.AddScoped<ITokenRepository, TokenRepository>();
			services.AddScoped<IGroupDetailRepository, GroupDetailRepository>();
			services.AddScoped<IAnonymousGroupServices, AnonymousGroupServices>();
			services.AddScoped<IGroupDetailService, GroupDetailService>();
			//services.AddScoped<IPaymentWalletService, PaymentWalletService>();    	//--> Commented - 27 - 10 - 2026
			//services.AddScoped<IPaymentWalletRepository, PaymentWalletRepository>();    	//--> Commented - 27 - 10 - 2026
			services.AddScoped<IAnonymousGroupRepository, AnonymousGroupRepository>();
		//	services.AddScoped<IPaymentIntegrationAPIs, PaymentIntegrationAPIs>();
			services.AddScoped<IAwsTextractService, AwsTextractService>();
			services.AddAWSService<IAmazonTextract>();
			services.AddScoped<HttpClient>();

			services.AddScoped<IUsioService, UsioService>();
			services.AddScoped<IUsioRepository, UsioRepository>();
			services.AddScoped<IGiftCardService, GiftCardService>();
			services.AddScoped<IGiftCardRepository, GiftCardRepository>();
			services.AddScoped<ICurrentUserService, CurrentUserService>();
			services.AddScoped<IAnalyticsRepository,AnalyticsRepository>();
			services.AddScoped<IAnalyticsService, AnalyticsService>();
		}
	}
}
