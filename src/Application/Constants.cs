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
                public const string Delete = "Company.Any.Delete";
                public const string Update = "Company.Any.Update";
                public static class Manager
                {
                    public const string SetFromAnyToAny = "Company.Any.Manager.SetFromAnyToAny";
                    public const string SetFromAnyToSelf = "Company.Any.Manager.SetFromAnyToSelf";
                    public const string SetFromNoneToAny = "Company.Any.Manager.SetFromNoneToAny";
                    public const string SetFromNoneToSelf = "Company.Any.Manager.SetFromNoneToSelf";
                    public const string SetFromAnyToNone = "Company.Any.Manager.SetFromAnyToNone";
                    public const string SetFromSelfToAny = "Company.Any.Manager.SetFromSelfToAny";
                    public const string SetFromSelfToNone = "Company.Any.Manager.SetFromSelfToNone";
                }
                public static class Other
                {
                    public const string View = "Company.Any.Other.View";
                    public const string Update = "Company.Any.Other.Update";
                }
            }

            public static class WhereUserIsManager
            {
                public const string Delete = "Company.WhereUserIsManager.Delete";
                public const string Update = "Company.WhereUserIsManager.Update";
                public static class Other
                {
                    public const string View = "Company.WhereUserIsManager.Other.View";
                    public const string Update = "Company.WhereUserIsManager.Other.Update";
                }
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
        }
    }

    public static class Access
    {
        public static class Company
        {
            public const string Create = "Company.Create";
            public const string SetManagerToAny = "Company.SetManagerToAny";
            public const string SetManagerToSelf = "Company.SetManagerToSelf";
            public const string SetManagerFromAny = "Company.SetManagerFromAny";
            public const string SetManagerFromNone = "Company.SetManagerFromNone";
            public const string SetManagerFromSelf = "Company.SetManagerFromSelf";
            public const string SetNewCompanyManager = "Company.SetNewCompanyManager";
            public const string SetExistingCompanyManager = "Company.SetExistingCompanyManager";

            public static class Any
            {
                public const string Delete = "Company.Any.Delete";
                public const string Update = "Company.Any.Update";

                public static class Manager
                {
                    public const string SetFromAnyToAny = "Company.Any.Manager.SetFromAnyToAny";
                    public const string SetFromAnyToSelf = "Company.Any.Manager.SetFromAnyToSelf";
                    public const string SetFromNoneToAny = "Company.Any.Manager.SetFromNoneToAny";
                    public const string SetFromNoneToSelf = "Company.Any.Manager.SetFromNoneToSelf";
                    public const string SetFromAnyToNone = "Company.Any.Manager.SetFromAnyToNone";
                    public const string SetFromSelfToAny = "Company.Any.Manager.SetFromSelfToAny";
                    public const string SetFromSelfToNone = "Company.Any.Manager.SetFromSelfToNone";
                }
                public static class Other
                {
                    public const string View = "Company.Any.Other.View";
                    public const string Update = "Company.Any.Other.Update";
                }
            }

            public static class WhereUserIsManager
            {
                public const string Delete = "Company.WhereUserIsManager.Delete";
                public const string Update = "Company.WhereUserIsManager.Update";

                public static class Other
                {
                    public const string View = "Company.WhereUserIsManager.Other.View";
                    public const string Update = "Company.WhereUserIsManager.Other.Update";
                }
                public static class Manager
                {
                    public const string SetFromAnyToAny = "Company.Any.Manager.SetFromAnyToAny";
                    public const string SetFromAnyToSelf = "Company.Any.Manager.SetFromAnyToSelf";
                    public const string SetFromAnyToNone = "Company.Any.Manager.SetFromAnyToNone";
                    public const string SetFromSelfToAny = "Company.Any.Manager.SetFromSelfToAny";
                    public const string SetFromSelfToNone = "Company.Any.Manager.SetFromSelfToNone";
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