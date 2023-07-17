namespace CRM.Application;

public static class Constants
{
    public static class Policies
    {
        public static class Company
        {
            public static class Commands
            {
                public const string Create = "Company.Commands.Create";
                public const string Update = "Company.Commands.Update";
                public const string Delete = "Company.Commands.Delete";
            }

            public static class Queries
            {
                public const string Create = "Company.Queries.Create";
                public const string View = "Company.Queries.View";
                public const string Update = "Company.Queries.Update";
                public const string Delete = "Company.Queries.Delete";
            }
        }
    }

    public static class Permissions
    {
        public static class Company
        {
            public const string Update = "Company.Update";
            public const string Create = "Company.Create";
            public const string Delete = "Company.Delete";
            public const string View = "Company.View";
        }
    }
}