using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;

namespace SMIJobXml.BL
{
    public class RedisCacheService : RedisCache
    {
        private volatile ConnectionMultiplexer _connection;
        private IDatabase _cache;

        private readonly RedisCacheOptions _options;

        private readonly SemaphoreSlim _connectionLock = new(initialCount: 1, maxCount: 1);

        public RedisCacheService(
            IOptions<RedisCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;
        }

        public async Task<List<string>> GetKeysAsync(string pattern, CancellationToken token = default)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            token.ThrowIfCancellationRequested();
            await ConnectAsync(token);

            var keys = new List<string>();
            foreach (var item in _options.ConfigurationOptions.EndPoints)
            {
                var server = _connection.GetServer(item);
                var data = server.Keys(pattern: $"{_options.InstanceName}{pattern}").Select(x => x.ToString());

                foreach (var element in data)
                {
                    var key = element;
                    if (element.StartsWith(_options.InstanceName))
                        key = element.Substring(_options.InstanceName.Length, element.Length - _options.InstanceName.Length);

                    keys.Add(key);
                }
            }
            return keys;
        }

        public async Task KeyExistAsync(string pattern, CancellationToken token = default)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            token.ThrowIfCancellationRequested();

            // if (!key.StartsWith(_options.InstanceName))
            //     key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);
        }

        public async Task<Dictionary<string, string>> HashGetAsync(string key, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            var data = await _cache.HashGetAllAsync(key);
            return data.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        }

        public async Task<string> HashGetAsync(string key, string hashField, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            var data = await _cache.HashGetAsync(key, hashField);
            if (data.ToString() == null)
                return default;

            return data.ToString();
        }

        public async Task<List<T>> HashGetAsync<T>(string key, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            var data = await _cache.HashGetAllAsync(key);
            return data.Select(x => JsonConvert.DeserializeObject<T>(x.Value.ToString())).ToList();
        }

        public async Task<List<T>> HashGetAsync<T>(string key, List<string> hashFields, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            var data = await _cache.HashGetAsync(key, hashFields.Select(x => RedisValue.Unbox(x)).ToArray());
            var items = new List<T>();
            foreach (var item in data)
            {
                var element = item.ToString();
                if (element != null)
                    items.Add(JsonConvert.DeserializeObject<T>(element));
            }
            return items;
        }

        public async Task<List<string>> HashGetAsync(string key, List<string> hashFields, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            var data = await _cache.HashGetAsync(key, hashFields.Select(x => RedisValue.Unbox(x)).ToArray());
            var items = new List<string>();
            foreach (var item in data)
            {
                var element = item.ToString();
                if (element != null)
                    items.Add(element);
            }
            return items;
        }

        public async Task<T> HashGetAsync<T>(string key, string hashField, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync();

            var data = await _cache.HashGetAsync(key, hashField);
            if (data.ToString() == null)
                return default;

            return JsonConvert.DeserializeObject<T>(data.ToString());
        }

        public async Task<List<string>> GetHashFieldAsync(string key, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync();
            var result = new List<string>();
            var response = _cache.HashScan(key);
            foreach (var i in response)
            {
                result.Add(i.Name.ToString());
            }
            return result;
        }

        public async Task HashSetAsync<T>(string key, Dictionary<string, T> data, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            await _cache.HashSetAsync(key, data.Select(x => new HashEntry(x.Key, JsonConvert.SerializeObject(x.Value))).ToArray());

            if (options != null && options.AbsoluteExpirationRelativeToNow.HasValue)
                await _cache.KeyExpireAsync(key, options.AbsoluteExpirationRelativeToNow.Value);
        }

        public async Task HashSetAsync(string key, Dictionary<string, string> data, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            await _cache.HashSetAsync(key, data.Select(x => new HashEntry(x.Key, x.Value)).ToArray());

            if (options != null && options.AbsoluteExpirationRelativeToNow.HasValue)
                await _cache.KeyExpireAsync(key, options.AbsoluteExpirationRelativeToNow.Value);
        }

        public async Task HashSetAsync(string key, string hashField, string data, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync();

            await _cache.HashSetAsync(key, hashField, data);

            if (options != null && options.AbsoluteExpirationRelativeToNow.HasValue)
                await _cache.KeyExpireAsync(key, options.AbsoluteExpirationRelativeToNow.Value);
        }

        public async Task HashDeleteAsync(string key, string hashField, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync();

            await _cache.HashDeleteAsync(key, hashField);
        }

        public async Task HashDeleteAsync(string key, string[] hashFields, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync();

            await _cache.HashDeleteAsync(key, hashFields.Select(x => RedisValue.Unbox(x)).ToArray());
        }

        public async Task<long> PushAsync(string key, string value, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            return await _cache.ListRightPushAsync(key, value);
        }

        public async Task<long> PushAsync(string key, string[] values, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync();

            return await _cache.ListRightPushAsync(key, values.Select(x => RedisValue.Unbox(x)).ToArray());
        }

        public async Task<string> PopAsync(string key, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            return await _cache.ListLeftPopAsync(key);
        }

        public async Task<T> ScriptEvaluateAsync<T>(string script, object parameters)
        {
            await ConnectAsync();
            var result = await _cache.ScriptEvaluateAsync(LuaScript.Prepare(script), parameters);
            return (T)Convert.ChangeType(result.ToString(), typeof(T));
        }

        private async Task ConnectAsync(CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            if (_cache != null)
            {
                return;
            }

            await _connectionLock.WaitAsync(token).ConfigureAwait(false);
            try
            {
                if (_cache == null)
                {
                    if (_options.ConfigurationOptions != null)
                    {
                        _connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions).ConfigureAwait(false);
                    }
                    else
                    {
                        _connection = await ConnectionMultiplexer.ConnectAsync(_options.Configuration).ConfigureAwait(false);
                    }

                    _cache = _connection.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task<T> QueryCacheKeyAsync<T>(string cache, Func<Task<T>> query, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            var bytes = await GetAsync(cache);
            if (bytes != null)
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));

            var data = await query.Invoke();
            await SetAsync(cache, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)), options);
            return data;
        }

        public async Task<T> QueryHashKeyAsync<T>(string cache, string key, Func<Task<T>> query, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            var data = await HashGetAsync<T>(cache, key);
            if (data != null)
                return data;

            var item = await query.Invoke();
            await HashSetAsync(cache, key, JsonConvert.SerializeObject(item));
            return item;
        }

        public async Task<List<T>> QueryHashKeysAsync<T>(string cache, List<string> keys, Func<Task<List<T>>> query, Func<List<T>, Dictionary<string, T>> parser, DistributedCacheEntryOptions options = null, CancellationToken token = default)
        {
            var data = await HashGetAsync<T>(cache, keys);
            if (data != null && data.Count == keys.Count)
                return data;

            var item = await query.Invoke();
            await HashSetAsync(cache, parser.Invoke(item));
            return item;
        }

        public async Task<string> StringGetAsync(string key, CommandFlags commandFlags = CommandFlags.None, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync();

            var data = await _cache.StringGetAsync(key);
            if (data.ToString() == null)
                return default;

            return data.ToString();
        }

        public async Task StringSetAsync(string key, string data, TimeSpan? expiry, CommandFlags commandFlags = CommandFlags.None, CancellationToken token = default)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            token.ThrowIfCancellationRequested();

            if (!key.StartsWith(_options.InstanceName))
                key = $"{_options.InstanceName}{key}";

            await ConnectAsync(token);

            await _cache.StringSetAsync(key, data, expiry);

        }
    }
}
