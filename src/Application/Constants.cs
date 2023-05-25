namespace CRM.Application;

public static class Constants
{
    public static class Claims
    {
        public const string ClaimType = "authorization";

        public static class Company
        {
            public const string Create = "Company.Create";
            public static class Any
            {
                public const string View = "Company.Any.View";
                public const string Delete = "Company.Any.Delete";
                public const string Update = "Company.Any.Update";
                public const string SetManagerFromAnyToAny = "Company.Any.SetManagerFromAnyToAny";
                public const string SetManagerFromAnyToSelf = "Company.Any.SetManagerFromAnyToSelf";
                public const string SetManagerFromNoneToAny = "Company.Any.SetManagerFromNoneToAny";
                public const string SetManagerFromNoneToSelf = "Company.Any.SetManagerFromNoneToSelf";
                public const string SetManagerFromAnyToNone = "Company.Any.SetManagerFromAnyToNone";
                public const string SetManagerFromSelfToAny = "Company.Any.SetManagerFromSelfToAny";
                public const string SetManagerFromSelfToNone = "Company.Any.SetManagerFromSelfToNone";
            }

            public static class WhereUserIsManager
            {
                public const string View = "Company.WhereUserIsManager.View";
                public const string Delete = "Company.WhereUserIsManager.Delete";
                public const string Update = "Company.WhereUserIsManager.Update";
                public const string SetManagerFromSelfToAny = "Company.WhereUserIsManager.SetManagerFromSelfToAny";
            }
        }
    }

    public static class Policies
    {
        public static class Company
        {
            public const string Update = "Company.Update";
            public const string Create = "Company.Create";
            public const string Delete = "Company.Delete";
            public const string View = "Company.View";
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
            public const string SetManager = "Company.SetManager";
        }
    }

    public static class Access
    {
        public static class Company
        {
            public const string Create = "Company.Create";
            public const string SetManagerToAny = "Company.SetManagerToAny";
            public const string SetManagerToNone = "Company.SetManagerToNone";
            public const string SetManagerToSelf = "Company.SetManagerToSelf";
            public const string SetManagerFromAny = "Company.SetManagerFromAny";
            public const string SetManagerFromNone = "Company.SetManagerFromNone";
            public const string SetManagerFromSelf = "Company.SetManagerFromSelf";
            public const string SetNewCompanyManager = "Company.SetNewCompanyManager";
            public const string SetExistingCompanyManager = "Company.SetExistingCompanyManager";

            public static class Any
            {
                public const string View = "Company.Any.View";
                public const string Delete = "Company.Any.Delete";
                public const string Update = "Company.Any.Update";
                public const string SetManagerFromNoneToSelf = "Company.SetManagerFromNoneToSelf";
                public const string SetManagerFromNoneToAny = "Company.SetManagerFromNoneToAny";
                public const string SetManagerFromSelfToNone = "Company.SetManagerFromSelfToNone";
                public const string SetManagerFromAnyToNone = "Company.SetManagerFromAnyToNone";
                public const string SetManagerFromAnyToSelf = "Company.SetManagerFromAnyToSelf";
                public const string SetManagerFromSelfToAny = "Company.SetManagerFromSelfToAny";
                public const string SetManagerFromAnyToAny = "Company.SetManagerFromAnyToAny";
            }

            public static class WhereUserIsManager
            {
                public const string View = "Company.WhereUserIsManager.View";
                public const string Delete = "Company.WhereUserIsManager.Delete";
                public const string Update = "Company.WhereUserIsManager.Update";
            }
        }
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Tester = "Tester";
    }
}