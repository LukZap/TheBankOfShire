using SharedInterface;
using System;
using System.ServiceModel;
using System.Threading;

namespace CustomerTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var channelFactory = new ChannelFactory<ICustomerInterface>(
                new BasicHttpBinding { SendTimeout = TimeSpan.FromMinutes(1) },
                new EndpointAddress(Constants.FullBankAddress)))
            {
                ManualResetEvent[] endOfWorkEvents =
                    {new ManualResetEvent(false), new ManualResetEvent(false), new ManualResetEvent(false)};

                var historyPrintLock = new object();

                // Customer 1
                new Thread(() =>
                {
                    var customer = channelFactory.CreateChannel();

                    Thread.Sleep(TimeSpan.FromSeconds(10));

                    var accountId = customer.OpenAccount("Henrietta", "Baggins", 100.0f);
                    if (accountId == null)
                    {
                        throw new Exception("Failed to open account");
                    }

                    customer.Deposit(accountId.Value, 500.0f);

                    Thread.Sleep(TimeSpan.FromSeconds(10));

                    customer.Deposit(accountId.Value, 500.0f);
                    customer.Deposit(accountId.Value, 1000.0f);

                    if (2000.0f != customer.Withdraw(accountId.Value, 2000.0f))
                    {
                        throw new Exception("Can't withdraw a valid amount");
                    }

                    lock (historyPrintLock)
                    {
                        Console.WriteLine("=== Customer 1 ===");
                        Console.Write(customer.GetHistory(accountId.Value));
                    }

                    if (!customer.CloseAccount(accountId.Value))
                    {
                        throw new Exception("Failed to close account");
                    }

                    endOfWorkEvents[0].Set();
                }).Start();

                // Customer 2
                new Thread(() =>
                {
                    var customer = channelFactory.CreateChannel();

                    var accountId = customer.OpenAccount("Barbara", "Tuk", 50.0f);
                    if (accountId == null)
                    {
                        throw new Exception("Failed to open account");
                    }

                    if (customer.OpenAccount("Barbara", "Tuk", 500.0f) != null)
                    {
                        throw new Exception("Opened account for the same name twice!");
                    }

                    if (50.0f != customer.Withdraw(accountId.Value, 2000.0f))
                    {
                        throw new Exception("Can only borrow up to debit limit only");
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(10));

                    if (customer.CloseAccount(accountId.Value))
                    {
                        throw new Exception("Can't close the account with outstanding debt");
                    }

                    customer.Deposit(accountId.Value, 100.0f);
                    if (customer.CloseAccount(accountId.Value))
                    {
                        throw new Exception("Can't close the account before clearing all funds");
                    }

                    if (50.0f != customer.Withdraw(accountId.Value, 50.0f))
                    {
                        throw new Exception("Can't withdraw a valid amount");
                    }

                    lock (historyPrintLock)
                    {
                        Console.WriteLine("=== Customer 2 ===");
                        Console.Write(customer.GetHistory(accountId.Value));
                    }

                    if (!customer.CloseAccount(accountId.Value))
                    {
                        throw new Exception("Failed to close account");
                    }

                    endOfWorkEvents[1].Set();
                }).Start();


                // Customer 3
                new Thread(() =>
                {
                    var customer = channelFactory.CreateChannel();

                    var accountId = customer.OpenAccount("Gandalf", "Grey", 10000.0f);
                    if (accountId == null)
                    {
                        throw new Exception("Failed to open account");
                    }

                    var toProcess = 200;
                    var resetEvent = new ManualResetEvent(false);

                    for (var i = 0; i < 100; i++)
                    {
                        ThreadPool.QueueUserWorkItem(stateInfo =>
                        {
                            if (customer.Withdraw(accountId.Value, 10.0f) != 10.0f)
                            {
                                throw new Exception("Can't withdraw a valid amount!");
                            }

                            if (Interlocked.Decrement(ref toProcess) == 0)
                            {
                                resetEvent.Set();
                            }
                        });
                    }

                    for (var i = 0; i < 100; i++)
                    {
                        ThreadPool.QueueUserWorkItem(stateInfo =>
                        {
                            customer.Deposit(accountId.Value, 10.0f);
                            if (Interlocked.Decrement(ref toProcess) == 0)
                            {
                                resetEvent.Set();
                            }
                        });
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(10));

                    resetEvent.WaitOne();

                    lock (historyPrintLock)
                    {
                        Console.WriteLine("=== Customer 3 ===");
                        Console.Write(customer.GetHistory(accountId.Value));
                    }

                    if (!customer.CloseAccount(accountId.Value))
                    {
                        throw new Exception("Failed to close account");
                    }

                    endOfWorkEvents[2].Set();
                }).Start();

                WaitHandle.WaitAll(endOfWorkEvents);
            }

            Console.ReadKey();
        }
    }
}
