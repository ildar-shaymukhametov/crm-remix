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
        public const string SetManagerToSelfFromNone = "company.manager.set.selfFromNone";
        public const string SetManagerToAnyFromNone = "company.manager.set.anyFromNone";
        public const string SetManagerToNoneFromSelf = "company.manager.set.noneFromSelf";
        public const string SetManagerToAnyFromSelf = "company.manager.set.anyFromSelf";
        public const string SetManagerToSelfFromAny = "company.manager.set.selfFromAny";
        public const string SetManagerToNoneFromAny = "company.manager.set.noneFromAny";
        public const string SetManagerToAnyFromAny = "company.manager.set.anyFromAny";
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
        public const string SetManagerToSelfFromNone = "SetManagerToSelfFromNone";
        public const string SetManagerToAnyFromNone = "SetManagerToAnyFromNone";
        public const string SetManagerToNoneFromSelf = "SetManagerToNoneFromSelf";
        public const string SetManagerToAnyFromSelf = "SetManagerToAnyFromSelf";
        public const string SetManagerToSelfFromAny = "SetManagerToSelfFromAny";
        public const string SetManagerToNoneFromAny = "SetManagerToNoneFromAny";
        public const string SetManagerToAnyFromAny = "SetManagerToAnyFromAny";
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Tester = "Tester";
    }
}