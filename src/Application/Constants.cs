namespace CRM.Application;

public static class Constants
{
    public static class Authorization
    {
        public static class Claims
        {
            public const string ClaimType = "authorization";
            public const string CreateCompany = "company.create";
            public const string UpdateCompany = "company.update";
            public const string DeleteCompany = "company.delete";
            public const string ViewCompany = "company.view";
        }

        public static class Policies
        {
            public const string UpdateCompany = "UpdateCompany";
        }
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
    }
}