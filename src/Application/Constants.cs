namespace CRM.Application;

public static class Constants
{
    public static class Claims
    {
        public const string ClaimType = "authorization";
        public const string CreateCompany = "company.create";
        public const string UpdateCompany = "company.update";
        public const string DeleteCompany = "company.delete";
        public const string ViewCompany = "company.view";
        public const string ViewAnyCompany = "company.view.any";
        public const string DeleteAnyCompany = "company.delete.any";
        public const string UpdateAnyCompany = "company.update.any";
        public static class Company
        {
            public static class Any
            {
                public static class Manager
                {
                    public static class Any
                    {
                        public static class Set
                        {
                            public const string Any = "company.any.manager.any.set.any";
                            public const string Self = "company.any.manager.any.set.self";
                        }
                    }
                    public static class None
                    {
                        public static class Set
                        {
                            public const string Self = "company.any.manager.none.set.self";
                        }
                    }
                }
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
        public const string CreateCompany = "CreateCompany";
        public const string UpdateOwnCompany = "UpdateOwnCompany";
        public const string DeleteOwnCompany = "DeleteOwnCompany";
        public const string ViewOwnCompany = "ViewOwnCompany";
        public const string ViewAnyCompany = "ViewAnyCompany";
        public const string DeleteAnyCompany = "DeleteAnyCompany";
        public const string UpdateAnyCompany = "UpdateAnyCompany";
        public static class Company
        {
            public static class Any
            {
                public static class Manager
                {
                    public static class Any
                    {
                        public static class Set
                        {
                            public const string Any = "company.any.manager.any.set.any";
                            public const string Self = "company.any.manager.any.set.self";
                        }
                    }
                    public static class None
                    {
                        public static class Set
                        {
                            public const string Self = "company.any.manager.none.set.self";
                        }
                    }
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