using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace LibraryManagementSystem.Utils
{
    /// <summary>
    /// Service that automatically refreshes DataGridViews when database or blob storage changes are detected
    /// </summary>
    public class AutoRefreshService : IDisposable
    {
        private System.Timers.Timer _refreshTimer;
        private Dictionary<string, DateTime> _lastCheckTimes;
        private Dictionary<string, Action> _refreshCallbacks;
        private bool _isRunning;
        private readonly object _lock = new object();

        // Refresh interval in milliseconds (default: 5 seconds)
        public int RefreshInterval { get; set; } = 5000;

        public AutoRefreshService()
        {
            _lastCheckTimes = new Dictionary<string, DateTime>();
            _refreshCallbacks = new Dictionary<string, Action>();
        }

        /// <summary>
        /// Register a DataGridView refresh callback for a specific table/view
        /// </summary>
        /// <param name="key">Unique identifier (e.g., "books", "users", "issued_books")</param>
        /// <param name="refreshCallback">Method to call when refresh is needed</param>
        public void RegisterRefresh(string key, Action refreshCallback)
        {
            lock (_lock)
            {
                _refreshCallbacks[key] = refreshCallback;
                if (!_lastCheckTimes.ContainsKey(key))
                {
                    _lastCheckTimes[key] = DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// Unregister a refresh callback
        /// </summary>
        public void UnregisterRefresh(string key)
        {
            lock (_lock)
            {
                _refreshCallbacks.Remove(key);
                _lastCheckTimes.Remove(key);
            }
        }

        /// <summary>
        /// Start the auto-refresh service
        /// </summary>
        public void Start()
        {
            if (_isRunning) return;

            _refreshTimer = new System.Timers.Timer(RefreshInterval);
            _refreshTimer.Elapsed += OnTimerElapsed;
            _refreshTimer.AutoReset = true;
            _refreshTimer.Start();
            _isRunning = true;
        }

        /// <summary>
        /// Stop the auto-refresh service
        /// </summary>
        public void Stop()
        {
            if (!_isRunning) return;

            if (_refreshTimer != null)
            {
                _refreshTimer.Stop();
                _refreshTimer.Elapsed -= OnTimerElapsed;
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }
            _isRunning = false;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                CheckForUpdates();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Auto-refresh error: {ex.Message}");
            }
        }

        private void CheckForUpdates()
        {
            lock (_lock)
            {
                bool booksChanged = false;
                bool usersChanged = false;
                bool issuedBooksChanged = false;
                bool returnedBooksChanged = false;

                // Check books table
                if (_refreshCallbacks.ContainsKey("books") || _refreshCallbacks.ContainsKey("dashboard"))
                {
                    booksChanged = HasBooksChanged();
                    if (booksChanged && _refreshCallbacks.ContainsKey("books"))
                    {
                        InvokeRefresh("books");
                    }
                }

                // Check users table
                if (_refreshCallbacks.ContainsKey("users") || _refreshCallbacks.ContainsKey("dashboard"))
                {
                    usersChanged = HasUsersChanged();
                    if (usersChanged && _refreshCallbacks.ContainsKey("users"))
                    {
                        InvokeRefresh("users");
                    }
                }

                // Check issued books
                if (_refreshCallbacks.ContainsKey("issued_books") || _refreshCallbacks.ContainsKey("dashboard"))
                {
                    issuedBooksChanged = HasIssuedBooksChanged();
                    if (issuedBooksChanged && _refreshCallbacks.ContainsKey("issued_books"))
                    {
                        InvokeRefresh("issued_books");
                    }
                }

                // Check returned books
                if (_refreshCallbacks.ContainsKey("returned_books") || _refreshCallbacks.ContainsKey("dashboard"))
                {
                    returnedBooksChanged = HasReturnedBooksChanged();
                    if (returnedBooksChanged && _refreshCallbacks.ContainsKey("returned_books"))
                    {
                        InvokeRefresh("returned_books");
                    }
                }

                // Check dashboard (refresh if any related data changed)
                if (_refreshCallbacks.ContainsKey("dashboard"))
                {
                    if (booksChanged || usersChanged || issuedBooksChanged || returnedBooksChanged)
                    {
                        InvokeRefresh("dashboard");
                    }
                }

                // Check blob storage for book covers (async check)
                if (_refreshCallbacks.ContainsKey("book_covers"))
                {
                    try
                    {
                        var hasBlobUpdates = BlobCovers.HasNewOrUpdatedCoversAsync().Result;
                        if (hasBlobUpdates)
                        {
                            InvokeRefresh("book_covers");
                        }
                    }
                    catch
                    {
                        // If blob check fails, skip this cycle
                    }
                }
            }
        }

        private bool HasBooksChanged()
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = "SELECT MAX(CASE WHEN date_update > date_insert OR date_insert IS NULL THEN date_update ELSE date_insert END) as last_change FROM books WHERE date_delete IS NULL";
                    using (var cmd = new SqlCommand(query, con))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            var lastChange = Convert.ToDateTime(result);
                            var key = "books";
                            if (!_lastCheckTimes.ContainsKey(key) || _lastCheckTimes[key] < lastChange)
                            {
                                _lastCheckTimes[key] = lastChange;
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                // If check fails, don't refresh
            }
            return false;
        }

        private bool HasUsersChanged()
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = "SELECT MAX(CASE WHEN date_update > date_register OR date_register IS NULL THEN date_update ELSE date_register END) as last_change FROM users WHERE date_delete IS NULL";
                    using (var cmd = new SqlCommand(query, con))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            var lastChange = Convert.ToDateTime(result);
                            var key = "users";
                            if (!_lastCheckTimes.ContainsKey(key) || _lastCheckTimes[key] < lastChange)
                            {
                                _lastCheckTimes[key] = lastChange;
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                // If check fails, don't refresh
            }
            return false;
        }

        private bool HasIssuedBooksChanged()
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = "SELECT MAX(CASE WHEN date_update > date_issue OR date_issue IS NULL THEN date_update ELSE date_issue END) as last_change FROM issue_books WHERE date_delete IS NULL";
                    using (var cmd = new SqlCommand(query, con))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            var lastChange = Convert.ToDateTime(result);
                            var key = "issued_books";
                            if (!_lastCheckTimes.ContainsKey(key) || _lastCheckTimes[key] < lastChange)
                            {
                                _lastCheckTimes[key] = lastChange;
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                // If check fails, don't refresh
            }
            return false;
        }

        private bool HasReturnedBooksChanged()
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = "SELECT MAX(CASE WHEN date_update > date_return OR date_return IS NULL THEN date_update ELSE date_return END) as last_change FROM issue_books WHERE date_return IS NOT NULL AND date_delete IS NULL";
                    using (var cmd = new SqlCommand(query, con))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            var lastChange = Convert.ToDateTime(result);
                            var key = "returned_books";
                            if (!_lastCheckTimes.ContainsKey(key) || _lastCheckTimes[key] < lastChange)
                            {
                                _lastCheckTimes[key] = lastChange;
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                // If check fails, don't refresh
            }
            return false;
        }

        private void InvokeRefresh(string key)
        {
            if (_refreshCallbacks.ContainsKey(key))
            {
                try
                {
                    var callback = _refreshCallbacks[key];
                    // Invoke on UI thread if needed
                    if (Application.OpenForms.Count > 0)
                    {
                        var mainForm = Application.OpenForms[0];
                        if (mainForm.InvokeRequired)
                        {
                            mainForm.Invoke(new Action(() => callback()));
                        }
                        else
                        {
                            callback();
                        }
                    }
                    else
                    {
                        callback();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error invoking refresh for {key}: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            Stop();
            lock (_lock)
            {
                _refreshCallbacks.Clear();
                _lastCheckTimes.Clear();
            }
        }
    }
}

