using System;

namespace LibraryManagementSystem.Utils
{
    public static class SessionManager
    {
        private static int _currentUserId = 0;
        private static string _currentUserName = "";
        private static string _currentUserRole = "";

        public static int CurrentUserId
        {
            get { return _currentUserId; }
            set { _currentUserId = value; }
        }

        public static string CurrentUserName
        {
            get { return _currentUserName; }
            set { _currentUserName = value; }
        }

        public static string CurrentUserRole
        {
            get { return _currentUserRole; }
            set { _currentUserRole = value; }
        }

        public static void SetUserSession(int userId, string userName, string role)
        {
            _currentUserId = userId;
            _currentUserName = userName;
            _currentUserRole = role;
        }

        public static void ClearSession()
        {
            _currentUserId = 0;
            _currentUserName = "";
            _currentUserRole = "";
        }

        public static bool IsLoggedIn()
        {
            return _currentUserId > 0;
        }

        public static bool IsAdmin()
        {
            return _currentUserRole.ToLower() == "admin";
        }

        public static bool IsStudent()
        {
            return _currentUserRole.ToLower() == "student";
        }
    }
}
