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
        public const string UpdateCompany = "UpdateCompany";
        public const string CreateCompany = "CreateCompany";
        public const string DeleteCompany = "DeleteCompany";
        public const string GetCompany = "GetCompany";
    }

    public static class Permissions
    {
        public const string UpdateCompany = "UpdateCompany";
        public const string CreateCompany = "CreateCompany";
        public const string DeleteCompany = "DeleteCompany";
        public const string ViewCompany = "ViewCompany";
    }

    public static class Access
    {
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

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Tester = "Tester";
    }
}