using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Idera.SQLcompliance.Core;
using PluginAddInView;
using PluginCommon;
using Logger = TracerX.Logger;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class InstanceSynchronizationHelper
    {
        private readonly IPrincipal _prinicpal;
        private readonly SqlConnection _connection;
        private readonly int _productId;
        private readonly HostObject _dashboardHost;
        private readonly Logger _logger;
        private readonly string _productIdString;

        public InstanceSynchronizationHelper(IPrincipal prinicpal, SqlConnection connection, string productId, HostObject dashboardHost, Logger logger)
        {
            _prinicpal = prinicpal;
            _connection = connection;
            _productIdString = productId;
            int.TryParse(productId, out _productId);
            _dashboardHost = dashboardHost;
            _logger = logger;
        }

        public void SynchronizeAsync()
        {
            Task.Factory.StartNew(Synchronize);
        }

        public void Synchronize()
        {
            using (_logger.InfoCall("InstanceSynchronizationHelper.Synchronize"))
            {
                try
                {
                    using (_connection)
                    {
                        var instancesToSynch = GetInstancesToSynchronize();
                        if (instancesToSynch == null || instancesToSynch.Count == 0) return;

                        var result = SynchronizeInstances(instancesToSynch);
                        var instancesToRegister = GetNotAssociatedInstancesWithCurrentProduct(result);
                        AssociateInstancesWithCurrentProduct(instancesToRegister);
                        MarkInstancesAsSynchronized(instancesToSynch);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }
        }

        private void MarkInstancesAsSynchronized(IEnumerable<InstanceForSynchronization> instancesToSynch)
        {
            using (_logger.InfoCall("InstanceSynchronizationHelper.MarkInstancesAsSynchronized"))
            {
                try
                {
                    var query = QueryBuilder.Instance.MarkInstancesAsSynchronized();
                    QueryExecutor.Instance.MarkInstancesAsSynchronized(_connection, query, instancesToSynch.Select(i => i.SrvId));
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    throw;
                }
            }
        }

        private void AssociateInstancesWithCurrentProduct(List<RegisterProductInstance> instancesToRegister)
        {
            using (_logger.InfoCall("InstanceSynchronizationHelper.AssociateInstancesWithCurrentProduct"))
            {
                try
                {
                    if (instancesToRegister == null || instancesToRegister.Count == 0) return;
                    for (int i = 0; i < instancesToRegister.Count; i++)
                    {
                        instancesToRegister[i].Status = InstanceStatus.Managed;
                    }
                    _dashboardHost.RegisterProductInstances(_productIdString, new RegisterProductInstances(instancesToRegister), _prinicpal);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    throw;
                }
            }
        }

        private List<RegisterProductInstance> GetNotAssociatedInstancesWithCurrentProduct(IEnumerable<CreateInstanceResult> allInstances)
        {
            var result = allInstances.Where(NotAssociatedWithProduct()).Select(i => new RegisterProductInstance(i.Instance.Id)).ToList();
            return result;
        }

        private IEnumerable<ProductInstance> GetAlreadyRegisteredInstances()
        {
            using (_logger.InfoCall("InstanceSynchronizationHelper.GetAlreadyRegisteredInstances"))
            {
                try
                {
                    var productInstances = _dashboardHost.GetProductInstances(null, null, null, null, null, _prinicpal);
                    var result = productInstances == null ? null : productInstances.Where(BelongsToProduct()).ToList();
                    return result;
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    throw;
                }
            }
        }

        private Func<ProductInstance, bool> BelongsToProduct()
        {
            return i => i.Products.Any(p => _productId == p.Id);
        }

        private Func<CreateInstanceResult, bool> NotAssociatedWithProduct()
        {
            var alreadyRegisteredInstances = GetAlreadyRegisteredInstances();
            return i => alreadyRegisteredInstances.Any(ri => ri.Name.Equals(i.Instance.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        private IEnumerable<CreateInstanceResult> SynchronizeInstances(IEnumerable<InstanceForSynchronization> instancesToSynch)
        {
            using (_logger.InfoCall("InstanceSynchronizationHelper.SynchronizeInstances"))
            {
                try
                {
                    var result = _dashboardHost.SynchronizeInstances(_productIdString, new CreateInstances(instancesToSynch.Select(i => (CreateInstance)i)), _prinicpal).ToList();
                    return result;
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    throw;
                }
            }
        }

        private List<InstanceForSynchronization> GetInstancesToSynchronize()
        {
            using (_logger.InfoCall("InstanceSynchronizationHelper.GetInstancesToSynchronize"))
            {
                try
                {
                    var query = QueryBuilder.Instance.GetInstancesForSynchronization();
                    var instancesToSynch = QueryExecutor.Instance.GetInstancesForSynchronization(_connection, query);
                    return instancesToSynch;
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    throw;
                }
            }
        }
    }
}
