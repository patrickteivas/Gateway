using ProxyKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiGateway
{
    /// <summary>
    ///     Represents a round robing collection of hosts.
    /// </summary>
    public class ServerPool : IServerPool
    {
        private readonly HashSet<UpstreamHost> _hosts = new HashSet<UpstreamHost>();
        private UpstreamHost[] _distribution;
        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private long _position = -1;

        /// <summary>
        ///     Initializes a new instance of <see cref="RoundRobin"/>
        /// </summary>
        /// <param name="hosts"></param>
        public ServerPool(params UpstreamHost[] hosts)
        {
            foreach (var host in hosts)
            {
                _hosts.Add(host);
            }
            BuildDistribution();
        }

        public void Add(UpstreamHost upstreamHost)
        {
            _hosts.Add(upstreamHost);
            BuildDistribution();
        }

        public void Remove(UpstreamHost upstreamHost)
        {
            _hosts.Remove(upstreamHost);
            BuildDistribution();
        }

        /// <summary>
        ///     Gets the next upstream host
        /// </summary>
        /// <returns>An upstream host instance.</returns>
        public UpstreamHost Next()
        {
            _lockSlim.EnterReadLock();

            UpstreamHost upstreamHost = null;

            if (_distribution.Length == 1)
            {
                var singleHost = _distribution[0];
            }
            else if (_distribution.Length > 1)
            {
                var position = Interlocked.Increment(ref _position);
                var mod = position % _distribution.Length;
                upstreamHost = _distribution[mod];
            }

            _lockSlim.ExitReadLock();

            return upstreamHost;
        }

        private void BuildDistribution()
        {
            _lockSlim.EnterWriteLock();
            var upstreamHosts = new List<UpstreamHost>();
            foreach (var upstreamHost in _hosts)
            {
                for (var i = 0; i < upstreamHost.Weight; i++)
                {
                    upstreamHosts.Add(upstreamHost);
                }
            }
            _distribution = upstreamHosts.ToArray();
            _lockSlim.ExitWriteLock();
        }

        public IEnumerator<UpstreamHost> GetEnumerator()
        {
            return _hosts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => _hosts.GetEnumerator();
    }

    public interface IServerPool : IEnumerable<UpstreamHost>
    {
        void Add(UpstreamHost upstreamHost);
        void Remove(UpstreamHost upstreamHost);
        UpstreamHost Next();
    }
}
