using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Mandarin.Api.Transactions;
using Mandarin.Grpc;
using Mandarin.Grpc.Converters;
using Mandarin.Transactions.External;
using Moq;
using Xunit;

namespace Mandarin.Tests.Grpc
{
    public class TransactionsGrpcServiceTests
    {
        private static readonly DateTime Date = new(2021, 06, 01, 00, 00, 00, DateTimeKind.Utc);

        private readonly IMapper mapper;
        private readonly Mock<IBackgroundJobClient> backgroundJobClient = new();
        private readonly TransactionsGrpcService subject;

        protected TransactionsGrpcServiceTests()
        {
            this.mapper = new MapperConfiguration(c => c.AddProfile<MandarinGrpcMapperProfile>()).CreateMapper();
            this.subject = new TransactionsGrpcService(this.mapper, this.backgroundJobClient.Object);
        }

        private Task<Job> GivenHangfireCapturesJob()
        {
            var tcs = new TaskCompletionSource<Job>();
            this.backgroundJobClient.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Callback((Job job, IState _) => tcs.SetResult(job));

            return tcs.Task;
        }

        public class SynchronizeTransactions : TransactionsGrpcServiceTests
        {
            [Fact]
            public async Task ShouldEnqueueCorrectJobOnSynchronizeTransaction()
            {
                var jobTask = this.GivenHangfireCapturesJob();
                var request = new SynchronizeTransactionsRequest
                {
                    Start = Timestamp.FromDateTime(TransactionsGrpcServiceTests.Date),
                    End = Timestamp.FromDateTime(TransactionsGrpcServiceTests.Date.AddDays(1)),
                };
                await this.subject.SynchronizeTransactions(request, Mock.Of<ServerCallContext>());

                var job = await jobTask;
                job.Type.Should().Be<ITransactionSynchronizer>();
                job.Method.Name.Should().Be(nameof(ITransactionSynchronizer.LoadExternalTransactions));
                job.Args.Should().BeEquivalentTo(TransactionsGrpcServiceTests.Date, TransactionsGrpcServiceTests.Date.AddDays(1));
            }
        }
    }
}
