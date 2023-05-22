namespace CRM.Application;

public static class Constants
{
    public static class Claims
    {
        public const string ClaimType = "authorization";

        public static class Company
        {
            public const string Create = "Company.Create";

            public static class New
            {
                public const string SetManagerToAny = "Company.New.SetManagerToAny";
                public const string SetManagerToSelf = "Company.New.SetManagerToSelf";
            }

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
            public const string SetManager = "Company.SetManager";
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
            public const string SetManagerToOrFromNone = "Company.SetManagerToOrFromNone";
            public const string SetManagerToAny = "Company.SetManagerToAny";
            public const string SetManagerToSelf = "Company.SetManagerToSelf";

            public static class New
            {
                public const string SetManager = "Company.New.SetManager";
                public const string SetManagerToAny = "Company.New.SetManagerToAny";
                public const string SetManagerToSelf = "Company.New.SetManagerToSelf";
            }

            public static class Old
            {
                public const string SetManagerFromAny = "Company.SetManagerFromAny";
                public const string SetManagerToSelf = "Company.SetManagerToSelf";

                public static class Any
                {
                    public const string View = "Company.Any.View";
                    public const string Delete = "Company.Any.Delete";
                    public const string Update = "Company.Any.Update";
                    public const string SetManager = "Company.Any.SetManager";
                }

                public static class WhereUserIsManager
                {
                    public const string View = "Company.WhereUserIsManager.View";
                    public const string Delete = "Company.WhereUserIsManager.Delete";
                    public const string Update = "Company.WhereUserIsManager.Update";
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