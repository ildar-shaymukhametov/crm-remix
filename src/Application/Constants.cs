namespace CRM.Application;

public static class Constants
{
    public static class Claims
    {
        public const string ClaimType = "authorization";

        public static class Company
        {
            public const string Create = $"{nameof(Company)}.{nameof(Create)}";
            public static class Any
            {
                public const string Delete = $"{nameof(Company)}.{nameof(Any)}.{nameof(Delete)}";

                public static class Manager
                {
                    public const string View = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(View)}";
                    public const string SetFromAnyToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToAny)}";
                    public const string SetFromAnyToSelf = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToSelf)}";
                    public const string SetFromNoneToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromNoneToAny)}";
                    public const string SetFromNoneToSelf = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromNoneToSelf)}";
                    public const string SetFromAnyToNone = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToNone)}";
                    public const string SetFromSelfToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromSelfToAny)}";
                    public const string SetFromSelfToNone = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromSelfToNone)}";
                }
                public static class Other
                {
                    public const string View = $"{nameof(Company)}.{nameof(Any)}.{nameof(Other)}.{nameof(View)}";
                    public const string Update = $"{nameof(Company)}.{nameof(Any)}.{nameof(Other)}.{nameof(Update)}";
                }
            }

            public static class WhereUserIsManager
            {
                public const string Delete = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Delete)}";

                public static class Other
                {
                    public const string View = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Other)}.{nameof(View)}";
                    public const string Update = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Other)}.{nameof(Update)}";
                }
                public static class Manager
                {
                    public const string View = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(View)}";
                    public const string SetFromSelfToAny = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(SetFromSelfToAny)}";
                    public const string SetFromSelfToNone = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(SetFromSelfToNone)}";
                }
            }
        }
    }

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
                public const string Create = "Company.Commands.Create";
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

    public static class Access
    {
        public static class Company
        {
            public const string Create = $"{nameof(Company)}.{nameof(Create)}";

            public static class Any
            {
                public const string Delete = $"{nameof(Company)}.{nameof(Any)}.{nameof(Delete)}";

                public static class Manager
                {
                    public const string View = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(View)}";
                    public const string SetFromAnyToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToAny)}";
                    public const string SetFromAnyToSelf = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToSelf)}";
                    public const string SetFromNoneToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromNoneToAny)}";
                    public const string SetFromNoneToSelf = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromNoneToSelf)}";
                    public const string SetFromAnyToNone = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromAnyToNone)}";
                    public const string SetFromSelfToAny = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromSelfToAny)}";
                    public const string SetFromSelfToNone = $"{nameof(Company)}.{nameof(Any)}.{nameof(Manager)}.{nameof(SetFromSelfToNone)}";
                }
                public static class Other
                {
                    public const string View = $"{nameof(Company)}.{nameof(Any)}.{nameof(Other)}.{nameof(View)}";
                    public const string Update = $"{nameof(Company)}.{nameof(Any)}.{nameof(Other)}.{nameof(Update)}";
                }
            }

            public static class WhereUserIsManager
            {
                public const string Delete = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Delete)}";

                public static class Other
                {
                    public const string View = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Other)}.{nameof(View)}";
                    public const string Update = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Other)}.{nameof(Update)}";
                }
                public static class Manager
                {
                    public const string View = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(View)}";
                    public const string SetFromSelfToAny = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(SetFromSelfToAny)}";
                    public const string SetFromSelfToNone = $"{nameof(Company)}.{nameof(WhereUserIsManager)}.{nameof(Manager)}.{nameof(SetFromSelfToNone)}";
                }
            }
        }
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Tester = "Tester";
    }
}