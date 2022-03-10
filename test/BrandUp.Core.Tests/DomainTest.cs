using BrandUp.Example.Queries;
using BrandUp.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace BrandUp
{
    public class DomainTest
    {
        [Fact]
        public async void ReadAsync()
        {
            #region Prepare

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDomain(options =>
            {
                options.AddQuery<UserByPhoneQueryHandler>();
            })
                .AddValidator<ComponentModelValidator>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            using var scope = serviceProvider.CreateAsyncScope();

            var domain = scope.ServiceProvider.GetRequiredService<IDomain>();

            #endregion

            var userByPhoneResult = await domain.ReadAsync(new UserByPhoneQuery { Phone = "89232229022" });

            Assert.True(userByPhoneResult.IsSuccess);
            Assert.NotNull(userByPhoneResult.Data);
            Assert.Single(userByPhoneResult.Data);
            Assert.Equal("89232229022", userByPhoneResult.Data.Single().Phone);
        }

        [Fact]
        public async void SendAsync_NotResult()
        {
            #region Prepare

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDomain(options =>
                {
                    options.AddCommand<Example.Commands.VisitUserCommandHandler>();
                })
                .AddValidator<ComponentModelValidator>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            using var scope = serviceProvider.CreateAsyncScope();

            var domain = scope.ServiceProvider.GetRequiredService<IDomain>();

            #endregion

            var joinUserResult = await domain.SendAsync(new Example.Commands.VisitUserCommand { Phone = "+79231145449" });

            Assert.True(joinUserResult.IsSuccess);
        }

        [Fact]
        public async void SendAsync_WithResult()
        {
            #region Prepare

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDomain(options =>
                {
                    options.AddCommand<Example.Commands.JoinUserCommandHandler>();
                })
                .AddValidator<ComponentModelValidator>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            using var scope = serviceProvider.CreateAsyncScope();

            var domain = scope.ServiceProvider.GetRequiredService<IDomain>();

            #endregion

            var joinUserResult = await domain.SendAsync(new Example.Commands.JoinUserCommand { Phone = "+79231145449" });

            Assert.True(joinUserResult.IsSuccess);
            Assert.NotNull(joinUserResult.Data.User);
            Assert.Equal("+79231145449", joinUserResult.Data.User.Phone);
        }

        [Fact]
        public async void SendAsync_CommandInvalid()
        {
            #region Prepare

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDomain(options =>
            {
                options.AddCommand<Example.Commands.JoinUserCommandHandler>();
            })
                .AddValidator<ComponentModelValidator>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            using var scope = serviceProvider.CreateAsyncScope();

            var domain = scope.ServiceProvider.GetRequiredService<IDomain>();

            #endregion

            var joinUserResult = await domain.SendAsync(new Example.Commands.JoinUserCommand());

            Assert.False(joinUserResult.IsSuccess);
        }
    }
}