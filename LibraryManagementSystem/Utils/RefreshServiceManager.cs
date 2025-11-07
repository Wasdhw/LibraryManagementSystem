using System;

namespace LibraryManagementSystem.Utils
{
    /// <summary>
    /// Singleton manager for the auto-refresh service
    /// </summary>
    public static class RefreshServiceManager
    {
        private static AutoRefreshService _instance;
        private static readonly object _lock = new object();

        public static AutoRefreshService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AutoRefreshService();
                        }
                    }
                }
                return _instance;
            }
        }

        public static void Start()
        {
            Instance.Start();
        }

        public static void Stop()
        {
            if (_instance != null)
            {
                _instance.Stop();
            }
        }

        public static void RegisterRefresh(string key, Action callback)
        {
            Instance.RegisterRefresh(key, callback);
        }

        public static void UnregisterRefresh(string key)
        {
            if (_instance != null)
            {
                Instance.UnregisterRefresh(key);
            }
        }

        public static void Dispose()
        {
            if (_instance != null)
            {
                _instance.Dispose();
                _instance = null;
            }
        }
    }
}

