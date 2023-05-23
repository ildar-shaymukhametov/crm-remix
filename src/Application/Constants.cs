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

            public static class Old
            {
                public static class Any
                {
                    public const string View = "Company.Old.Any.View";
                    public const string Delete = "Company.Old.Any.Delete";
                    public const string Update = "Company.Old.Any.Update";
                    public const string SetManagerFromAnyToAny = "Company.Old.Any.SetManagerFromAnyToAny";
                    public const string SetManagerFromAnyToSelf = "Company.Old.Any.SetManagerFromAnyToSelf";
                    public const string SetManagerFromNoneToAny = "Company.Old.Any.SetManagerFromNoneToAny";
                    public const string SetManagerFromNoneToSelf = "Company.Old.Any.SetManagerFromNoneToSelf";
                    public const string SetManagerFromAnyToNone = "Company.Old.Any.SetManagerFromAnyToNone";
                    public const string SetManagerFromSelfToAny = "Company.Old.Any.SetManagerFromSelfToAny";
                    public const string SetManagerFromSelfToNone = "Company.Old.Any.SetManagerFromSelfToNone";
                }

                public static class WhereUserIsManager
                {
                    public const string View = "Company.Old.WhereUserIsManager.View";
                    public const string Delete = "Company.Old.WhereUserIsManager.Delete";
                    public const string Update = "Company.Old.WhereUserIsManager.Update";
                    public const string SetManagerFromSelfToAny = "Company.Old.WhereUserIsManager.SetManagerFromSelfToAny";
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
                public const string SetManager = "Company.Old.SetManager";
                public const string SetManagerFromAny = "Company.Old.SetManagerFromAny";

                public static class Any
                {
                    public const string View = "Company.Old.Any.View";
                    public const string Delete = "Company.Old.Any.Delete";
                    public const string Update = "Company.Old.Any.Update";
                    public const string SetManagerFromNoneToSelf = "Company.Old.SetManagerFromNoneToSelf";
                    public const string SetManagerFromNoneToAny = "Company.Old.SetManagerFromNoneToAny";
                    public const string SetManagerFromSelfToNone = "Company.Old.SetManagerFromSelfToNone";
                }

                public static class WhereUserIsManager
                {
                    public const string View = "Company.Old.WhereUserIsManager.View";
                    public const string Delete = "Company.Old.WhereUserIsManager.Delete";
                    public const string Update = "Company.Old.WhereUserIsManager.Update";
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