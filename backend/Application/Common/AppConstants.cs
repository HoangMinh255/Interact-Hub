namespace InteractHub.Application.Common;

public static class AppConstants
{
    public static class Roles
    {
        public const string User = "User";
        public const string Admin = "Admin";
    }

    public static class Pagination
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
    }

    public static class Claims
    {
        public const string UserId = "uid";
        public const string Email = "email";
        public const string FullName = "full_name";
        public const string Role = "role";
    }

    public static class Content
    {
        public const int DefaultNameMaxLength = 100;
        public const int DefaultTextMaxLength = 1000;
    }
}